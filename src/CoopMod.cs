using System.Numerics;
using System.Text.Json;
using Meitou.Modding;
using Meitou.Simulation;

namespace MeitouP2P.Coop;

public sealed class CoopMod : IMod
{
    private ISessionApi session = null!;
    private volatile CoopWorld? world;
    private string route = "";
    private Task<string>? last;
    private Observation? observed;

    public void Initialize(ModContext context)
    {
        route = context.Id;
        session = context.Dependency<ISessionApi>("meitou.p2p") ?? throw new InvalidOperationException("Connector unavailable.");
        var labels = RoleLabels.Load(context);
        var trial = StoatWalkTrial.Open(context, session);
        context.AddSystem("Guest character", _ =>
        {
            trial?.ResetWorld();
            return world = new CoopWorld(session, labels, context.Log, trial);
        }, SystemAnchor.End);
        session.Register(route, trial == null ? 1 : 2, (peer, body) => world?.Reply(peer, body) ?? "No host world.");
        context.AddDebugCommand("Co-op Offer selected", c =>
        {
            if (session.HostedPeer is not Guid peer) return "A guest must join first.";
            if (c.Selected.Count != 1) return "Select one player character.";
            c.Enqueue(new Offer(peer, Guid.NewGuid(), c.Selected[0]) { Tick = c.Tick + 1 });
            return "Offer queued for the host tick.";
        });
        context.AddDebugCommand("Co-op Observe", _ => Begin(Observe));
        context.AddDebugCommand("Co-op Move to camera", c => Begin(() => Move(c.Target)));
        context.AddDebugCommand("Co-op Reply", _ => last == null ? "No request yet." : ConnectorMod.ReplyStatus(last));
    }

    private string Begin(Func<Task<string>> action)
    {
        if (last is { IsCompleted: false }) return "Waiting for the previous request.";
        last = action();
        return "See Co-op Reply.";
    }

    private async Task<string> Observe()
    {
        observed = null;
        string reply = await session.RequestAsync(route, "view");
        if (!reply.StartsWith('{')) return reply;
        observed = JsonSerializer.Deserialize<Observation>(reply) ?? throw new InvalidDataException("Empty observation.");
        return FormattableString.Invariant($"{observed.Name} [{observed.Role}] at {observed.X:F2}, {observed.Y:F2}, {observed.Z:F2} on host tick {observed.Tick}");
    }

    private Task<string> Move(Vector3 target)
    {
        if (observed == null) return Task.FromResult("Observe the offered character first.");
        string request = FormattableString.Invariant($"move {observed.Grant} {target.X:R} {target.Y:R} {target.Z:R}");
        return session.RequestAsync(route, request);
    }
}
