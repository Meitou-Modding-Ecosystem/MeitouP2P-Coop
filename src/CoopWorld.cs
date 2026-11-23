using System.Globalization;
using System.Numerics;
using System.Text.Json;
using Meitou.Simulation;

namespace MeitouP2P.Coop;

internal sealed record Offer(Guid Peer, Guid Grant, CharacterId Actor) : SimCommand;
internal sealed record Observation(Guid Grant, string Name, string Role, long Tick, float X, float Y, float Z);

internal sealed class CoopWorld(ISessionApi session, IReadOnlyDictionary<string,string> labels, Action<string> log, StoatWalkTrial? trial) : ITickSystem
{
    // Only the simulation thread owns the assignment. Network handlers see an immutable observation.
    private Offer? assigned;
    private sealed record Publication(World World, Guid Peer, CharacterId Actor, Observation View);
    private sealed record Destination(Guid Peer, Guid Grant, Vector3 Point);
    private volatile Publication? latest;
    private Destination? pending;

    public string Reply(Guid peer, string request)
    {
        if (trial is { CanGrant: false }) return "Waiting for both worlds.";
        var publication = latest;
        if (publication?.Peer != peer) return "The host must offer a player character.";
        if (request == "view") return JsonSerializer.Serialize(publication.View);
        if (request == "poses" && trial != null) return trial.HostPose(publication.World, publication.Actor, publication.View.Grant);

        var fields = request.Split(' ');
        if (fields.Length != 5 || fields[0] != "move" || !Guid.TryParse(fields[1], out var grant)
            || grant != publication.View.Grant) return "Unrecognized order or stale grant.";
        if (!Coordinate(fields[2], out float x) || !Coordinate(fields[3], out float y) || !Coordinate(fields[4], out float z))
            return "Invalid destination.";
        var order = new Destination(peer, grant, new(x, y, z));
        return Interlocked.CompareExchange(ref pending, order, null) == null
            ? "Move received. Unpause the host to resolve it." : "An order is waiting for the host tick.";
    }

    private static bool Coordinate(string text, out float value) =>
        float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && float.IsFinite(value) && Math.Abs(value) <= 1_000_000;

    public void Inputs(World world, IReadOnlyList<SimCommand> commands)
    {
        var peer = session.HostedPeer;
        if (trial is { CanGrant: false })
        {
            if (assigned != null) log($"Invalidated Scout grant {assigned.Grant} at host tick {world.Tick}; trial is not ready.");
            assigned = null; latest = null; Interlocked.Exchange(ref pending, null); return;
        }
        if (trial is { Host: true } && assigned?.Peer != peer && peer is Guid joined && trial.ScoutIn(world) is { } scout)
        {
            assigned = new Offer(joined, Guid.NewGuid(), scout) { Tick = world.Tick };
            log($"Issued Scout grant {assigned.Grant} to peer {joined} at host tick {world.Tick}.");
        }
        foreach (var command in commands)
            if (command is Offer offer && offer.Peer == peer) assigned = offer;
        var order = Interlocked.Exchange(ref pending, null);
        if (assigned is not { } active || active.Peer != peer || !world.Characters.TryResolveNext(active.Actor, out int slot)
            || world.Characters.Cold(slot) is not { IsPlayer: true })
        {
            assigned = null;
            latest = null;
            return;
        }
        if (order != null && order.Peer == peer && order.Grant == active.Grant)
        {
            world.Defer(new MoveOrder([active.Actor], order.Point) { Tick = world.Tick + 1 });
            log($"Guest move submitted for actor {active.Actor} at host tick {world.Tick + 1}.");
        }
    }

    public void SlowWorld(World world)
    {
        if (assigned is not { } active || active.Peer != session.HostedPeer ||
            !world.Characters.TryResolveNext(active.Actor, out int slot) || world.Characters.Cold(slot) is not { IsPlayer: true } actor)
        {
            latest = null;
            return;
        }
        var point = world.Characters.Next[slot].Position;
        latest = new(world, active.Peer, active.Actor, new(active.Grant, actor.Name, labels.GetValueOrDefault(actor.RecordId, "Guest"),
            world.Tick + 1, point.X, point.Y, point.Z));
    }
}
