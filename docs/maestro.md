# Authoring a guest role with Maestro

Co-op supplies a v1 provider action on FCS Character records. Maestro writes the project dependency on `MeitouP2P-Coop.mod` and an entry in `coop-roles.json`. At runtime, Co-op reads enabled role packs in load order. A later label wins for the same template. The host assigns the actual character to the connection. [Descriptor](../pack/maestro.json) · [Maestro v1 reference](https://github.com/mattresspad/MeitouMaestro/blob/v0.1.0-playtest.1/DESCRIPTORS.md)

## Reproduce the tagged-core check

Use sibling checkouts named `MeitouP2P`, `MeitouP2P-Coop` and `MeitouMaestro`. From the connector checkout, build both mods against stock Windows 0.20.0 as described in the connector README. Set `MEITOU_0201_DIR` to the extracted 0.20.1 folder containing `Meitou.Data.dll`. Build the tagged core and a separate authoring probe with those readers.

```powershell
git clone --branch v0.1.0-playtest.1 --depth 1 https://github.com/mattresspad/MeitouMaestro.git ../MeitouMaestro
dotnet build ../MeitouMaestro/Core/Maestro.Core.csproj -c Release "-p:MeitouDir=$env:MEITOU_0201_DIR"
$core = (Resolve-Path ../MeitouMaestro/Core/bin/Release/net11.0/Maestro.Core.dll).Path
dotnet build tools/Probe/Probe.csproj -c Release "-p:MeitouDir=$env:MEITOU_0201_DIR" "-p:MaestroCore=$core" -o artifacts/maestro-author
python tools/smoke.py --maestro artifacts/maestro-author/Probe.dll
```

The authoring process reviews, saves and exports a Stoat Scout role. The smoke test checks the FCS dependency and JSON fields, installs the exported pack, and reads the authored label back through two 0.20.0 loader processes. The fixture uses original synthetic data. This check exercises Maestro's authoring core. The visible Stoat replay tests the 0.20.0 runtime. [Connector build](https://github.com/mattresspad/MeitouP2P#build-and-test-with-co-op)

## Handoff for Husbandry and Dialogue

Maestro's released Gear Conditions provider selects a `DialogueLine`, adds its FCS dependency and writes a `conditions` reference to a stable provider record. Its runtime interprets the condition on the conversation target. This is a working reference for a future Husbandry provider. [Gear Conditions descriptor](https://github.com/mattresspad/MeitouMaestro/blob/v0.1.0-playtest.1/Providers/Gate/maestro.json)

A candidate predicate is “the conversation target is an animal under the player's care.” Husbandry owns the definition of care and the source of that state. The Dialogue integration owns evaluation on the conversation **target**. Maestro's provider would attach the condition to an authored line. The provider authors should agree on the predicate and record ID before implementing it.

**Acceptance case.** Author and export one line once. Hold the speaker and other conditions constant. With a qualifying animal as the conversation target the line should be available. With a nonqualifying target the same exported line should be unavailable. Record both target identities, their care state, build versions and observed results.
