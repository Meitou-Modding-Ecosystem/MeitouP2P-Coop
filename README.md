# MeitouP2P-Coop

A two-player **developer preview** for stock Windows MeitouClient **0.20.0**. On the same PC, a guest can select Stoat Scout, right-click a destination and watch her walk while the host moves Stoat Guide. Both players retain their own cameras and selections.

![Host and guest moving the two Wanderers in stock MeitouClient 0.20.0](docs/stoat-walk.gif)

Watch the right window as the guest pans away. On returning, a click at the Scout's old spot selects nobody. Clicking her body at the destination selects her again and sends her on a second walk. The host keeps the Guide selected on the left. [Test results](docs/stoat-results.md)

## Run the preview

Install the .NET 11 SDK, Python 3, your own Kenshi game and an extracted Windows x64 MeitouClient 0.20.0 release. Clone `MeitouP2P` and `MeitouP2P-Coop` as sibling folders. Build the two mods from the connector checkout and launch its disposable **Stoat Scout Walk** fixture. It supplies the matching characters and trial environment for the visible walk. [Build both mods](https://github.com/mattresspad/MeitouP2P#build-and-test-with-co-op) · [Launch the two windows](https://github.com/mattresspad/MeitouP2P#walk-stoat-scout-in-two-windows) · [Native-input test driver](https://github.com/mattresspad/MeitouP2P/blob/main/tools/stoat-native-walk.py)

MeitouP2P owns the loopback session. Co-op gives the guest permission to move one character in the current host world. The host checks the peer, grant and character before submitting a normal `MoveOrder`. In the named fixture, the guest draws both matching characters from the same host tick using their position, facing and animation. The guest simulation and the rest of the world run locally. The drawing and input hooks depend on private methods in 0.20.0. Martin confirmed that the release has no remote-character-state hook. [Upstream answer](https://github.com/VlcekM/MeitouClient/issues/749#issuecomment-6898903424)

## Try the underlying grant through F10

Enable **MeitouP2P** and then **MeitouP2P-Coop** in both disposable profiles. Start the session with the connector's F10 Host and Join actions and keep the host simulation running.

1. Select one player character on the host and choose **Co-op Offer selected**.
2. On the guest, choose **Co-op Observe**, then **Co-op Reply**. Read the role, position and host tick.
3. Pan the guest camera to walkable ground and choose **Co-op Move to camera**. Observe again after movement.
4. Offer a different character on the host. Repeating the guest's previous move requires a fresh observation. Rejoining or replacing the host world requires a fresh offer.

## Author a guest role

Install the provider in Maestro and choose **Label a guest character template** in a separate author project. Select a Character record, enter a pack ID such as `stoat`, a role ID such as `stoat.scout`, and a label such as **Stoat Scout**. Review, save and export. Enable the exported pack after Co-op. The label comes from the pack and the host grants control during play. [Provider descriptor](pack/maestro.json) · [Maestro verification](docs/maestro.md)

The gameplay reference is nhoral's allied two-squad design in *KenshiCoop*, including zeroit789's two-Wanderer start. This preview uses one granted Scout and a Guide in a matching test squad. [KenshiCoop](https://github.com/nhoral/KenshiCoop) · [Licensing and replay notice](LICENSING.md)
