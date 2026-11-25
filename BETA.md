# Beta development

Develop on a working branch and open a PR targeting `beta`. All validation jobs must pass before the required `Development readiness` check succeeds. The current beta rules require a PR and successful CI, with zero approvals and no bypass needed. This setup does not change protections or bypass required CI. Main and release remain separate review and release gates.

## Automated validation

Builds both mods against Meitou 0.22.1 and runs the existing transport/lobby SessionChecks plus the connector/Co-op two-process smoke suite, including handshake, host movement, stale grants, reconnect and world replacement. The companion repository is pinned to `af345e1ec8f5273de2f5b9292f6af4212f96ea2c`; update that pin deliberately when testing a new dependency version.

The workflow runs on pushes and pull requests with a read-only token and pinned Actions. `GetCIEngine.ps1` downloads SHA-256-verified official managed build fixtures; no Kenshi installation, credentials or personal game data are required. To reproduce locally, install .NET 11, Python 3 and PowerShell 7, run that script with an external fixture directory, then run `CI.ps1` as shown in `.github/workflows/ci.yml`.

These are development checks for the documented baseline, not a production release certification or a test of unreleased sandbox engine hooks. No deployment, release publishing, Steam synchronization, GUI playtest or native game session runs in this workflow. Changes needing a newer engine must update the fixture/dependency pin and corresponding tests explicitly. Live rendering and real game save/load scenarios still require the documented manual trials.

## Paired multiplayer preview

This beta integrates the 0.2.0 multiplayer menu, private-network lobby and matching Stoat Walk mode. CI pairs the connector source `af345e1ec8f5273de2f5b9292f6af4212f96ea2c` with Co-op source `4bb3336ff3631dd1bba4150c9dfd96646b6618a0`; integration changes add CI and documentation around those runtime sources. Two-PC/Tailscale validation remains pending; local and hosted loopback checks do not establish remote-play success. See Co-op’s `docs/remote-playtest.md` for the volunteer procedure.
