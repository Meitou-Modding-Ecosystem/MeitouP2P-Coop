using System.Reflection;
using HarmonyLib;
using Meitou.Game.Ui;
using Meitou.Modding;
using Meitou.Simulation;
using Meitou.Simulation.Editing;

namespace MeitouP2P.Coop;

internal sealed class StoatMode : IMultiplayerMode
{
    readonly ModContext context;
    readonly StoatWalkTrial trial;
    static StoatMode? active;
    object? host;
    World? before, preparedWorld;
    bool preparing, finishedCreation;
    Guid trialId;
    long loadStarted;
    public string Id => "meitou.p2p.coop";
    public string Name => "Co-op / Stoat Walk preview";
    public string? Unavailable { get; }
    public bool Prepared { get; private set; }
    internal StoatMode(ModContext context, StoatWalkTrial trial)
    {
        this.context = context;
        this.trial = trial;
        var assembly = typeof(IMod).Assembly;
        var editor = assembly.GetType("Meitou.Game.Editor.CharacterEditorHost", true)!;
        var open = AccessTools.DeclaredMethod(editor, "OpenPendingEditor", Type.EmptyTypes)
            ?? throw new MissingMethodException("CharacterEditorHost.OpenPendingEditor");
        _ = AccessTools.Field(editor,"PendingCreation") ?? throw new MissingFieldException("PendingCreation");
        _ = AccessTools.Field(editor,"_creationWarmOwed") ?? throw new MissingFieldException("_creationWarmOwed");
        context.Harmony.Patch(open, prefix: new HarmonyMethod(typeof(StoatMode), nameof(Creation)));
        active = this;
    }
    static object? Read(object owner, string name) => AccessTools.Field(owner.GetType(), name)?.GetValue(owner)
        ?? AccessTools.Property(owner.GetType(), name)?.GetValue(owner);
    static void Set(object owner, string name, object? value)
    {
        if (AccessTools.Field(owner.GetType(), name) is { } field) field.SetValue(owner, value);
        else if (AccessTools.Property(owner.GetType(), name) is { } property) property.SetValue(owner, value);
        else throw new MissingMemberException(owner.GetType().Name, name);
    }
    static World World(object host) => (World)Read(Read(host,"Session")!,"World")!;
    public void Prepare(bool hosting, Guid id, object gameHost)
    {
        host = gameHost;
        trialId = id;
        before = World(host);
        preparedWorld = null;
        Prepared = finishedCreation = false;
        preparing = true;
        loadStarted = Environment.TickCount64;
        trial.BeginRuntime(hosting);
        var options = Read(host,"GameOptions")!;
        Set(options,"Seed",17UL);
        Set(options,"SeedGiven",true);
        var player = Read(host,"Player")!;
        ((MyGuiScreenHost)Read(player,"FaithfulScreens")!).CloseAll();
        // Use the stock new-game request and its asynchronous scene loading pipeline.
        Set(Read(host,"NewGame")!,"NewGameRequested","3-Stoat Walk.mod");
        context.Log($"Preparing Stoat fixture; trial {id}; role {(hosting ? "host" : "guest")}; seed 17.");
    }
    static bool Creation(object __instance)
    {
        if (active is not { preparing: true, host: { } host } mode || Read(__instance,"PendingCreation") == null) return true;
        if (Read(host,"ActiveSceneLoad") != null || Read(host,"Warming") is true) return false;
        Set(__instance,"PendingCreation",null);
        Set(__instance,"_creationWarmOwed",true);
        var world = World(host);
        world.Commands.Enqueue(new FinishCreation { Tick = world.Tick });
        mode.finishedCreation = true;
        return false;
    }
    public void Update(object gameHost, bool playing)
    {
        if (host == null) return;
        var world = World(host);
        if (Prepared && !ReferenceEquals(preparedWorld,world)) throw new InvalidOperationException("World replaced. Start a fresh trial.");
        if (preparing)
        {
            if (Environment.TickCount64 - loadStarted > 300_000) throw new TimeoutException("Fixture loading took more than five minutes.");
            if (!ReferenceEquals(before,world) && finishedCreation && Read(host,"ActiveSceneLoad") == null && Read(host,"Warming") is not true)
            {
                var snapshot = (WorldSnapshot)Read(Read(host,"Session")!,"CurrentSnapshot")!;
                if (snapshot.Characters.Count(c => c.RecordId is "1-Stoat Walk.mod" or "2-Stoat Walk.mod") == 2)
                {
                    preparing = false;
                    Prepared = true;
                    preparedWorld = world;
                    Set(Read(host,"Player")!,"Follow",CharacterId.None);
                    context.Log($"Fixture ready; trial {trialId}; world tick {world.Tick}.");
                }
            }
        }
        trial.RuntimeFrame(playing && Prepared);
    }
    public void Stop()
    {
        trial.EndRuntime();
        if (host != null) Set(Read(host,"NewGame")!,"MainMenuRequested",true);
        host = null;
        preparing = Prepared = false;
    }
}
