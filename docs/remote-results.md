# Multiplayer-menu preview: local and remote results

Local test date: **22 November 2026**. Game: stock Windows Meitou **0.22.1**, build `9ee4074bbc6fd7fdeb26108a04afc9f90cb1dc01`. Packages: P2P and Co-op **0.2.0**, connector protocol **2**. The combined ZIP's `preview.json` records its exact source commit pair.

**First two-PC movement session reported on 23 November 2026:** approximately 30 minutes over Tailscale, followed by a successful fresh-session restart. The host log corroborates connection, world preparation, guest movement submissions and grant invalidation. This is a movement milestone, not a pass for shared world interactions or the complete remote acceptance checklist.

## Two-PC volunteer session — 23 November 2026

The host reported that, after the volunteer installed the preview and accepted the Tailscale device-sharing invitation, guest Ready followed by host Start trial was straightforward. Both players ran independently, entered and left buildings, and separated their characters during roughly 30 minutes of play.

Observed boundaries:

| Action | Reported outcome |
| --- | --- |
| Scout and Guide movement | Worked across the two PCs during sustained manual play. |
| Doors | Opening/closing a door did not consistently update its appearance on the other PC. |
| Carrying and dropping a corpse | The carried/dropped body was not visible to the other player as expected. |
| Trading | Trade UI opened locally; a guest-reported clothing transfer did not appear on the host. No synchronized inventory transaction is established. |
| Full disconnect and subsequent join | Host returned to the main menu. Hosting again, joining and starting launched a fresh session successfully. This does not establish same-world resume. |
| Setup | Tailscale account/device sharing and manual installation were a substantial usability barrier. Simplifying instructions and connection feedback is a follow-up; this result does not require replacing the transport. |

The retained host log contains **1,229 guest move submissions** across two trial IDs, fixture readiness for each trial, and fresh Scout grants. It records grant invalidation on both disconnects. Between the trials it explicitly records `Start a fresh host trial after a world replacement or new guest.` The lobby treats a new connection without the prepared trial ID as requiring a fresh trial; stopping Co-op requests the main menu. The unexpected menu transition needs clearer user-facing explanation. The host error log is empty; that alone does not establish absence of defects. Host-side `round trip 0 ms` entries are not a measured internet-latency result.

Evidence: participant report plus a locally retained host log archive. No remote-session screenshots/video were captured, and guest logs have not yet been reviewed. The full pan-away / old-position miss / destination-body reselection sequence, Guide-order refusal, independent selection retention, and same-world reconnect remain separately unverified remotely. The captures below are **local** evidence only.

Tested package: P2P `af345e1ec8f5273de2f5b9292f6af4212f96ea2c`, Co-op `4bb3336ff3631dd1bba4150c9dfd96646b6618a0`; combined ZIP SHA-256 `9848bcc09a13fb43a901321d015a35a5e98eb52776b78efde2dfba807283a970`. This results update does not rebuild or alter that tested ZIP. Raw logs remain private because they include machine and connection details.

## Local validation scope

The checks below used two visible clients on one Windows PC, first over loopback and then through a loopback fault proxy. They do not establish internet latency or stability on the volunteer's machine.

## What passed locally

| Check | Observation |
| --- | --- |
| Native menu | Multiplayer, Host, address copy/paste, Join, Ready and Start loaded the fixed fixture through the stock new-game path. Character creation was skipped. Buildings and foliage remained enabled. |
| Compatibility and failures | Exact build, mode revision and changed-file refusals; occupied port; second guest rejected as Session full without displacing the first; unreachable host; cancelled handshake. |
| Readiness | Ready alone did not start the world. Slow guest preparation blocked play. A pending click and request were discarded on disconnect. Movement before renewed Ready was refused. |
| Native walk | Guest body selection, plain ground clicks, simultaneous Guide movement, guest pan-away and clear-selection, old-location miss, destination body reselection, and a second Scout move all passed. Guide-only and mixed guest orders were refused. |
| Independent views | Host retained Guide selection and the same camera pivot while the guest panned and selected Scout. |
| Recovery | Rejoin required Ready again, produced a different grant, and moved Scout successfully. Pose polling continued with the HUD hidden. |
| World replacement | Starting a different game in the guest stopped its trial and invalidated the host grant. The lobby regression also rejects a fresh guest attempting to reuse the suspended trial; start a fresh host session. |
| UI styles and size | Meitou and Faithful gameplay styles; 860-pixel client and maximized 1920-pixel client; native close button and restored window size. |
| Existing baseline | Connector-only handshake/ping/leave, combined host-resolved movement, stale grants, reconnect and replacement-world checks passed. The earlier loopback replay remains unchanged. |
| Player package | Clean sibling builds produced the combined ZIP. Its setup and launcher ran with neither the SDK nor Python on the player PATH, and that extracted package passed the native walk. The corrected log collector captured both running clients' complete log snapshots and version metadata; archived bytes matched the source files. |
| Cache location follow-up | Stock Windows PowerShell setup/launcher checks verified a saved cache directory, a one-run override, older-profile fallback, spaces and trailing separators, and both legacy clients using the same cache. Launch interception verified child-only environment changes without opening game windows. This launcher follow-up does not change movement or network code. |

## Walk evidence

The host is on the left and guest on the right in each pair. These are original captures, not a reconstruction. [Recorded camera, position, selection and tick samples](remote-preview/stages.json) cover the full walk; only selected images are curated here.

Guest pans away while Guide remains selected on the host:

| Host | Guest |
| --- | --- |
| ![Host retains Guide and camera](remote-preview/03-host.png) | ![Guest pans both Wanderers off-screen](remote-preview/03-guest.png) |

The old position misses:

| Host | Guest |
| --- | --- |
| ![Host at the old-location check](remote-preview/19-host.png) | ![Guest has no selection after clicking Scout's old location](remote-preview/19-guest.png) |

Scout is selected at her destination, with Guide still selected on the host:

| Host | Guest |
| --- | --- |
| ![Host retains Guide selection](remote-preview/20-host.png) | ![Guest selects Scout at arrival](remote-preview/20-guest.png) |

Scout arrived at `(34778.9, 966.9, -70158.0)`. The host camera pivot stayed at `(34665, 969.2, -70142)`. The second guest click moved Scout again. The test counted exactly two accepted guest orders and no host order for the subsequent Guide-only or mixed selections. [Refusal capture](remote-preview/25-guest.png) · [Fresh-grant result](remote-preview/reconnect.json) · [Faithful lobby at a larger size](remote-preview/faithful-maximized.png)

## Reproduce

Build both sibling repositories and prepare two isolated profiles using the combined ZIP's setup helper. Launch each with `-TestControl` for developer automation. Complete Host / Join / Ready / Start through the menu, then run the connector's `tools/stoat-native-walk.py <run-folder> --connected`, where the folder contains `host` and `guest` profiles. The driver sends actual screen-coordinate clicks through Meitou's input path and retains both clients' captures and query transcripts.

For recovery testing, run `tools/loopback-proxy.py <run-folder>/proxy`, host on `127.0.0.1:42871`, and join `127.0.0.1:42872`. After the walk, run `tools/stoat-reconnect.py <run-folder>`. The proxy can hold replies and drop its own connections without changing system networking. It is a developer test tool and is excluded from the player ZIP.

Run `dotnet run --project tools/SessionChecks -c Release` for transport/lobby failure checks, and `python tools/smoke.py` for the released loader and simulation regressions. These require the developer SDK/build setup documented in the connector README. Player setup does not.

## Observed limitations

The isolated profile uses the stock static title because the animated title stalled during this local test. Several explicit background-window activations by the Windows automation helper were followed by a native `GLFW.PollEvents` access violation. The cause is unresolved. Keeping the guest foreground and using Meitou's built-in coordinate-click test channel allowed the checks above to complete. Manual window switching and the actual remote volunteer session still need broader stability testing.

Only Scout and Guide's poses and guest movement requests are shared. Doors, carried objects, NPCs, combat, inventory and campaign simulation remain independent. Loading another world requires a fresh host trial. Follow the separate [volunteer acceptance checklist](remote-playtest.md) and collect both PCs' logs before recording a complete remote acceptance pass.

The [gameplay-footage notice](../LICENSING.md#stoat-replay) applies to these captures; they are excluded from the CC0 dedication for original guides and fixtures.
