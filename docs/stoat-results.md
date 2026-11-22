# Stoat Scout playtest

The recorded replays below used a launcher with `--no-objects` and `--no-foliage`. Buildings, map features and foliage were omitted while residents still spawned. They demonstrate the character movement and selection checks in that reduced scene. The launcher now loads buildings and foliage for hands-on play.

The complete native-input trial passed again on November 22, 2026 with buildings and foliage enabled in two fresh 0.22.1 windows. The host accepted exactly two Scout moves at ticks 215 and 894. The guest panned away, reselected her at arrival and moved her again while the host retained its camera and Guide selection. Guide-only and mixed-selection moves were refused. Both clients loaded the building renderer, and Stoat's walls and buildings were visually checked after the walk.

## Stock Windows 0.22.1

Recorded on November 22, 2026 with stock Windows MeitouClient `0.22.1+9ee4074bbc6fd7fdeb26108a04afc9f90cb1dc01`. Two fresh profiles with autosave disabled loaded the packages built for `=0.22.1` and the matching Stoat fixture. The original native-input driver passed the complete walk. The replay pairs actual screen captures. [0.22.1 replay](stoat-walk-0.22.1.gif) · [Stock release](https://github.com/VlcekM/MeitouClient/releases/tag/v0.22.1)

| Action | Observed outcome |
| --- | --- |
| The guest clicked the Scout's body and right-clicked terrain while the host selected and moved the Guide. | Both characters moved in both windows. The host accepted the first guest order at tick 254. Each player retained their own selection. |
| The guest panned away, cleared selection and waited, then returned using camera keys. | The Scout went off-screen. The host retained its Guide selection and camera throughout the recorded sequence. |
| The guest clicked the former position, then the Scout's body at arrival. | The old position hit terrain and selected nobody. The destination click selected the same guest-local character `c:0.0` at X 34778.9, Z −70158.0. The host was at tick 920 in the paired reselection capture. |
| The guest sent the Scout walking again, then tried Guide-only and mixed-selection orders. | The host accepted the second Scout move at tick 941. The refused clicks produced zero additional moves. Exactly two guest orders reached the host. The run ended with the Scout selected on the guest and the Guide on the host. |

An initial run completed the walk and destination reselection, then missed the final setup click as the camera shifted between the screen-position query and the click. The fresh run above passed without changes to the input driver or gameplay hooks. Both runs' raw captures and command transcripts were retained locally.

The separate loader test passed handshake, ping, movement, stale-grant, reconnect and world-replacement checks on 0.22.1. A 0.20.0 guest was refused, after which a matching 0.22.1 guest connected. The tagged Maestro role export also loaded on both peers. Current organization source targets the stable 0.22.1 release. The 0.22.2 nightlies require their own compatibility check. [Maestro result](maestro.md) · [Reproduce the walk](https://github.com/Meitou-Modding-Ecosystem/MeitouP2P#walk-stoat-scout-in-two-windows)

## Stock Windows 0.20.1

On November 15, 2026, two stock Windows clients from `8c79093d` ran on one PC over IPv4 loopback. Both loaded the packages built for `=0.20.1` and the Stoat fixture in fresh profiles with autosave off. The guest used body clicks, terrain right-clicks and A/D camera input. The replay pairs captures from the two windows. [0.20.1 replay](stoat-walk-0.20.1.gif) · [Stock release](https://github.com/VlcekM/MeitouClient/releases/tag/v0.20.1)

| Action | Observed in the two clients |
| --- | --- |
| The guest selected the Scout's body and right-clicked the ground. The host selected the Guide and sent him in another direction. | The first Scout move reached the host at tick 289. Both windows showed both characters moving. The guest retained the Scout and the host retained the Guide. |
| The guest panned right, cleared selection and waited for arrival. | The Scout went off-screen. The guest camera pivot moved from X 34665.0 to 35135.2. The host pivot stayed at X 34665.0, Z −70142.0 throughout the walk. |
| The guest panned back and clicked the former body position, then the body at arrival. | Pixel (151, 260) hit terrain and selected nobody. Pixel (724, 265) hit the Scout's original guest-local character `c:0.0` at X 34778.9, Z −70158.0. The guest HUD showed Stoat Scout and the host still had Stoat Guide selected. |
| The guest right-clicked again, then attempted a Guide-only and a mixed-selection order. | The second Scout move reached the host at tick 993. The guest saw the Scout reach X 34692.2, Z −70133.7. The two refused clicks produced zero further host orders. The captured run logged exactly two guest moves. |

The same private input and snapshot hooks worked on both releases. Build the mods for the release you play. In the separate loader tests, mixed 0.20.0 and 0.20.1 peers were refused in both directions. Matching peers passed ping, movement, stale-grant, reconnect and host-world replacement checks. Both 0.20.1 peers also applied the tagged Maestro export. [Maestro role test](maestro.md) · [Reproduce the walk](https://github.com/Meitou-Modding-Ecosystem/MeitouP2P#walk-stoat-scout-in-two-windows)

## Stock Windows 0.20.0

Recorded on November 15, 2026 with stock Windows MeitouClient 0.20.0 (`da6af12e`), two disposable profiles, autosave disabled, and one IPv4 loopback connection. The two windows ran the connector, Co-op and the matching Stoat fixture. The replay combines actual screenshots at selected stages. [Side-by-side replay](stoat-walk.gif)

| Input | Observed outcome |
| --- | --- |
| The guest selected the Scout by her body and right-clicked a destination. The host selected the Guide and right-clicked elsewhere. | Both characters moved in both windows. The host accepted the Scout order at tick 274. |
| The guest panned with the camera keys, cleared selection and waited. | The Scout moved off the guest's screen. The host kept the Guide selected and its camera pivot stayed fixed. |
| The guest returned, clicked the Scout's old position and then clicked her body at arrival. | The old position missed. At the arrival check the guest selected the Scout at X 34778.9, Z −70158.0, while the host retained the Guide. |
| The guest right-clicked a second destination. | The host accepted the second Scout order at tick 1021. Two accepted ground clicks produced exactly two host move orders. |
| The guest selected the Guide alone and then both characters and tried another ground move with each selection. | The HUD explained that only the Scout takes guest orders. The host received zero additional guest move orders. |

In the separate loader test on synthetic flat ground, the guest observed the assigned actor at X 30.00 on tick 3 and X 399.42 on tick 453. An incompatible mode set was refused. Reassignment invalidated the previous grant. Reconnecting and replacing the host world both required a fresh offer. The standalone connector also completed a handshake, ping and leave without Co-op.

The visible trials draw host poses onto matching characters in the guest fixture. Their scope is movement, picking and independent cameras on one PC. Two-machine play, campaign save continuity and whole-world replication need their own checks. [Replay notice](../LICENSING.md#stoat-replay)
