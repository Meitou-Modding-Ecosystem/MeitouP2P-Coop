# Stoat Scout playtest

Recorded on November 15, 2026 with stock Windows MeitouClient 0.20.0 (`da6af12e`), two disposable profiles, autosave disabled, and one IPv4 loopback connection. The two windows ran the connector, Co-op and the matching Stoat fixture. The replay combines actual screenshots at selected stages. [Side-by-side replay](stoat-walk.gif)

| Input | Observed outcome |
| --- | --- |
| The guest selected the Scout by her body and right-clicked a destination. The host selected the Guide and right-clicked elsewhere. | Both characters moved in both windows. The host accepted the Scout order at tick 274. |
| The guest panned with the camera keys, cleared selection and waited. | The Scout moved off the guest's screen. The host kept the Guide selected and its camera pivot stayed fixed. |
| The guest returned, clicked the Scout's old position and then clicked her body at arrival. | The old position missed. At the arrival check the guest selected the Scout at X 34778.9, Z −70158.0, while the host retained the Guide. |
| The guest right-clicked a second destination. | The host accepted the second Scout order at tick 1021. Two accepted ground clicks produced exactly two host move orders. |
| The guest selected the Guide alone and then both characters and tried another ground move with each selection. | The HUD explained that only the Scout takes guest orders. The host received zero additional guest move orders. |

In the separate loader test on synthetic flat ground, the guest observed the assigned actor at X 30.00 on tick 3 and X 399.42 on tick 453. An incompatible mode set was refused. Reassignment invalidated the previous grant. Reconnecting and replacing the host world both required a fresh offer. The standalone connector also completed a handshake, ping and leave without Co-op.

The visible trial draws host poses onto matching characters in the guest fixture. Its scope is movement, picking and independent cameras. Two-machine play, campaign save continuity, whole-world replication and game-version upgrades need their own checks. [Reproduce the windowed trial](https://github.com/mattresspad/MeitouP2P#walk-stoat-scout-in-two-windows) · [Replay notice](../LICENSING.md#stoat-replay)
