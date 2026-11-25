# Remote Stoat Walk preview — Meitou 0.22.1

One host controls Guide and one guest controls Scout. This preview shares their movement, facing, and animation. NPCs, combat, inventory, and campaign state still run independently. Remote validation is pending the volunteer trial. Use the same preview ZIP and the stock Windows x64 0.22.1 release on both PCs.

## Prepare each Windows PC

1. Install your own Kenshi and extract [Meitou 0.22.1](https://github.com/VlcekM/MeitouClient/releases/tag/v0.22.1). Keep both installations intact.
2. Extract this preview ZIP. Open PowerShell in the extracted folder and run `powershell -NoProfile -ExecutionPolicy Bypass -File .\Setup.ps1`. Supply the two installation folders when prompted. The helper creates a separate `profile` with autosave off. It leaves your usual mod selection and saves alone.
3. Install [Tailscale](https://tailscale.com/download/windows) on each PC and sign in using your own accounts. The host [shares their host device](https://tailscale.com/docs/features/sharing) with the volunteer. The volunteer accepts that invitation.
4. Run `powershell -NoProfile -ExecutionPolicy Bypass -File .\Launch.ps1`. This opens one client at its main menu. Player setup needs no SDK, Python, or source checkout. The commands allow these unsigned preview scripts for that PowerShell process only; they do not change your saved execution policy.

The helper uses each person's local game data through a directory junction. The ZIP contains original mod files and tools only. `preview.json` identifies both source commits and the required game build. `checksums.json` lists the packaged files.

Extract the preview onto a drive with room for generated caches: Meitou's terrain and distant-object caches can occupy many gigabytes. Setup defaults to a shared `cache` folder beside the player scripts, and Launch passes it through the stock `--cache-root` option. To keep it elsewhere, add `-CacheDir 'D:\Caches\Meitou'` to Setup; that path is remembered in the profile. Launch also accepts `-CacheDir` as a one-run override, including for older profiles. Both local players can use the same cache directory. It contains generated data, not saves, and is excluded from the ZIP. The isolated launcher clears inherited per-cache environment overrides for its child process so they cannot send those caches to a different drive.

## Host and join

On the host, select **Multiplayer**, select **Co-op / Stoat Walk preview**, and use **Local** to choose the host's Tailscale IPv4 address. Keep port **42871**, then click **Host** and **Copy** to share that address with your volunteer. The address beginning `127.` is only for testing two windows on the same PC.

On the guest, select **Multiplayer**, click the address field, select its contents, paste the host address, and click **Join**. A successful connection reports **Compatible pack**. Click **Ready**. The host then clicks **Start trial**. Both clients prepare the fixed Stoat start with seed 17. Loading can take several minutes.

The host can move Guide normally. The guest clicks Scout's body and right-clicks walkable ground. Use plain right-clicks. The guest receives feedback for Guide-only or mixed selections. Open the **Session** button to leave or rejoin after a disconnection. Rejoining requires Ready again and a fresh control grant. Loading a different world ends the trial: the host must Leave and host a fresh session before starting again.

## Windows firewall and connection help

Hosting listens only on the selected local address. On the host, allow inbound TCP for `MeitouClient.exe` at the chosen port and Tailscale address, restricted to the volunteer's Tailscale IPv4 address. The following is a template to run in an administrator PowerShell after replacing all three example values. Do not disable the firewall or open a router port for this trial.

```powershell
New-NetFirewallRule -DisplayName 'Meitou Stoat volunteer' -Direction Inbound -Action Allow -Protocol TCP -LocalPort 42871 -LocalAddress '<host Tailscale IPv4>' -RemoteAddress '<volunteer Tailscale IPv4>' -Program '<full path to MeitouClient.exe>'
```

Remove that specific rule after testing with `Remove-NetFirewallRule -DisplayName 'Meitou Stoat volunteer'`. Revoke the Tailscale device share when no longer needed. Existing broad firewall rules are unaffected by this command; review their scope separately if you already allowed Meitou through the firewall.

For connection failures, confirm Host is listening on the copied address, both Tailscale clients are connected, and the device invitation was accepted. Use `tailscale ping <host-address>` to check the private path. A successful ping with a failed game connection points to the chosen port, listener, or firewall. `tailscale status` identifies a direct or relayed connection. [Connection troubleshooting](https://tailscale.com/docs/how-to/connect-to-devices)

**Incompatible** identifies a mismatched build, load order, mode, or file. Install the same ZIP in fresh profiles. **Session full** means another guest is connected. **Cancel** cancels a pending connection; **Leave** ends the local session. **Close** only closes the panel.

## Volunteer acceptance run

1. Set Scout's gait to Walk on the host. On the guest, click empty ground and then Scout's body. Verify her name in the HUD.
2. Send Scout toward walkable ground. While she moves, move Guide in another direction on the host. Both players should see both characters move.
3. On the guest, pan Scout off-screen, clear selection, and let her arrive. Pan back manually. Her old spot should miss. Click her body at the destination and right-click a second destination. The host must retain its Guide selection and camera.
4. Select Guide alone on the guest and right-click. Verify visible refusal and no resulting Guide order on the host.
5. Interrupt the guest connection, then reconnect through **Session → Rejoin → Ready**. Repeat a Scout move and verify a new grant in the logs.

Capture both screens, note the Tailscale connection type and any pauses or jumps, then run `powershell -NoProfile -ExecutionPolicy Bypass -File .\Collect-Logs.ps1` on each PC. You can collect logs while the game is running. Review the log ZIPs before sharing them. Report results separately from the earlier same-PC replay. A remote pass requires observations and logs from both PCs.

Developer automation may pass `-TestControl` to the launcher. The normal launch leaves that local test channel disabled. Graphics defaults are deliberately modest, with town buildings and foliage present. The gameplay reference is [nhoral's KenshiCoop](https://github.com/nhoral/KenshiCoop).

The isolated profile uses Meitou's static title (`meitouMainMenu=false`) because the animated 0.22.1 title stalled in the two-client test. This is independent of the Faithful/Meitou gameplay interface setting. The trial loads its normal town scenery.
