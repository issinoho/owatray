# Registry changes: becoming the default mail handler

This documents exactly what OWAtray writes to the Windows registry when it registers itself as the
system's Simple MAPI / default mail handler, and what it restores when that's switched off. All of this
logic lives in `src/ShellIntegration/Program.cs`; the GUI never touches the registry directly — it shells
out to `DrunkenBakery.OWAtray.ShellIntegration.exe` with an elevated (`runas`) process so the writes land
in `HKEY_LOCAL_MACHINE`.

## Trigger paths

| GUI action (Advanced menu) | `Form1.cs` handler | `shellPath` argument | `ShellIntegration.exe` behavior |
|---|---|---|---|
| "Make OWA the default mail handler" | `MakeOwaDefaultToolStripMenuItemClick` | `registry` | `SaveCurrentKey()` then `InitRegistry()` |
| "Switch off shell integration" | `SwitchOffToolStripMenuItemClick` | `restore` | `RestoreKey()` |

Both handlers check `IsUserAdministrator()` first and log a warning (not a hard stop) if the current user
isn't an admin, since the writes below all target `HKEY_LOCAL_MACHINE` and require elevation. On Vista and
later (`Environment.OSVersion.Version.Major >= 6`) the child process is started with `Verb = "runas"`,
which triggers a UAC prompt.

`shellPath` is `DrunkenBakery.OWAtray.ShellIntegration.exe` in the install directory; `shell` in the
tables below refers to that same full path (used as the target of the `open\command`/`DefaultIcon`
values so Windows shells back out to it). `bridge` is
`DrunkenBakery.OWAtray.Mapi.dll` (`Settings.Default.MAPIBridge`) alongside it — the native Simple MAPI
DLL that Windows loads in-process for `MAPILogon`/`MAPISendMail` calls, per `Mapi32.DEF`.

## What "Make OWA the default mail handler" backs up (`SaveCurrentKey`, before writing anything)

Before overwriting anything, the previous values are read and — if they aren't already OWAtray's own
values — stashed in OWAtray's own `Settings.settings` (a user-scope app-settings file, not the registry)
so they can be put back later:

| Registry value read | Stashed as |
|---|---|
| `HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice` → `Progid` | `Settings.Default.CurrentKey` |
| `HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail` → `(Default)` | `Settings.Default.DefaultMail` |
| `HKEY_CURRENT_USER\SOFTWARE\Clients\Mail` → `(Default)` | `Settings.Default.DefaultMailUser` |
| `HKEY_CLASSES_ROOT\mailto\DefaultIcon` → `(Default)` | `Settings.Default.DefaultIcon` |
| `HKEY_CLASSES_ROOT\mailto\shell\open\command` → `(Default)` | `Settings.Default.DefaultOpen` |

Each is only stashed if it doesn't already equal what OWAtray itself would write (so re-running this
twice in a row doesn't clobber the real previous handler with OWAtray's own values).

## What "Make OWA the default mail handler" writes (`InitRegistry`)

All under `HKEY_CLASSES_ROOT` (`HKCR`) or `HKEY_LOCAL_MACHINE` (`HKLM`); all string values are `REG_SZ`
unless noted.

**A private URL protocol class for OWA**, mirroring what a normal `mailto` handler looks like:

| Key | Value | Data |
|---|---|---|
| `HKCR\OWA.Url.Mailto` | `(Default)` | `URL:MailTo Protocol` |
| `HKCR\OWA.Url.Mailto` | `URL Protocol` | `""` (empty — presence of the value, not its content, marks it as a protocol handler) |
| `HKCR\OWA.Url.Mailto` | `EditFlags` | `02 00 00 00` (`REG_BINARY`) |
| `HKCR\OWA.Url.Mailto\DefaultIcon` | `(Default)` | `"<shell>",0` |
| `HKCR\OWA.Url.Mailto\shell\open\command` | `(Default)` | `"<shell>" mailto %1` |

**Rewiring the system's `mailto:` association to OWAtray:**

| Key | Value | Data |
|---|---|---|
| `HKCR\mailto\DefaultIcon` | `(Default)` | `"<shell>",0` |
| `HKCR\mailto\shell\open\command` | `(Default)` | `"<shell>" mailto %1` |
| `HKCU\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\mailto\UserChoice` | `Progid` | `OWA.Url.Mailto` (`Settings.Default.MailtoClass`) |

**Registering `OWAMapi` as a Windows "Mail" client**, under the standard
`SOFTWARE\Clients\Mail\<ProgId>` layout Windows uses for Default Programs / Set Program Access:

| Key | Value | Data |
|---|---|---|
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi` | `(Default)` | `Outlook Web App` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi` | `DLLPath` | `<bridge>` (full path to `DrunkenBakery.OWAtray.Mapi.dll`) |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi` | `EXE` | `"<shell>"` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi` | `Parameters` | `mapi %1` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\Capabilities` | `ApplicationDescription` | `Integrate Outlook Web Access into the desktop.` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\Capabilities\FileAssociations` | *(key created, no values)* | — |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\Capabilities\Start Menu` | `Mail` | `OWA` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\Capabilities\URLAssociations` | `mailto` | `OWA.Url.Mailto` (`Settings.Default.MailtoClass`) |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto` | `(Default)` | `URL:MailTo Protocol` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto` | `EditFlags` | `02 00 00 00` (`REG_BINARY`) |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto` | `URL Protocol` | `""` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto\DefaultIcon` | `(Default)` | `"<shell>",0` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\Protocols\mailto\shell\open\command` | `(Default)` | `"<shell>" mailto %1` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\shell\open\command` | `(Default)` | `"<shell>" owa` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\DefaultIcon` | `(Default)` | `"<shell>",0` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo` | `HideIconsCommand` | `"<shell>" restore` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo` | `ReinstallCommand` | `"<shell>" registry` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo` | `ShowIconsCommand` | `"<shell>" registry` |
| `HKLM\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo` | `IconsVisible` | `1` (`REG_DWORD`) |

**Making it a selectable option in Windows' "Default Programs" / "Set Program Access and Defaults" UI,
and actually making it the default:**

| Key | Value | Data |
|---|---|---|
| `HKLM\SOFTWARE\RegisteredApplications` | `OWA` | `Software\Clients\Mail\OWAMapi\Capabilities` |
| `HKLM\SOFTWARE\Clients\Mail` | `(Default)` | `OWAMapi` |
| `HKCU\SOFTWARE\Clients\Mail` | `(Default)` | `OWAMapi` |

Any failure partway through (e.g. a value it couldn't write) is caught, logged to the console, and
otherwise swallowed — `InitRegistry` doesn't roll back partial writes.

## What "Switch off shell integration" restores (`RestoreKey`)

Puts back exactly the five values `SaveCurrentKey` stashed (only the ones that had non-empty stashed
data — if nothing was ever stashed, e.g. `registry` was never run first, this is a no-op for that value):

| Registry value | Restored from |
|---|---|
| `HKCU\...\UrlAssociations\mailto\UserChoice` → `Progid` | `Settings.Default.CurrentKey` |
| `HKLM\SOFTWARE\Clients\Mail` → `(Default)` | `Settings.Default.DefaultMail` |
| `HKCU\SOFTWARE\Clients\Mail` → `(Default)` | `Settings.Default.DefaultMailUser` |
| `HKCR\mailto\DefaultIcon` → `(Default)` | `Settings.Default.DefaultIcon` |
| `HKCR\mailto\shell\open\command` → `(Default)` | `Settings.Default.DefaultOpen` |

It also always resets `HKLM\SOFTWARE\Clients\Mail\OWAMapi\InstallInfo\IconsVisible` to `0` (`REG_DWORD`),
regardless of whether anything else was restored. Note this only reverts the four association/default
values above — it does **not** delete the `OWAMapi` tree under `SOFTWARE\Clients\Mail\OWAMapi`,
`OWA.Url.Mailto` under `HKCR`, or the `RegisteredApplications\OWA` entry; those are left in place (just
no longer selected as the active default) unless the app is uninstalled.

## Separately: what the installer/uninstaller itself touches

`Installer/OWAtray.nsi` writes its own, unrelated set of keys purely for the Windows "installed programs"
registration (`Add or Remove Programs`) — these have nothing to do with mail-handler registration above:

| Key | Value | Data |
|---|---|---|
| `HKLM\Software\Microsoft\Windows\CurrentVersion\App Paths\OWAtray` | `(Default)` | install path to `DrunkenBakery.OWAtray.GUI.exe` |
| `HKLM\...\Uninstall\OWA Tray Monitor` | `DisplayName`, `UninstallString`, `DisplayIcon`, `DisplayVersion`, `URLInfoAbout`, `Publisher` | standard uninstall-entry metadata |

Both are removed by the uninstaller (`DeleteRegKey`).

The uninstaller also cleans up the default-handler registration from the section above, since none of
that is scoped to the app's own `Software\Microsoft\...\App Paths`/`Uninstall` keys and would otherwise
be left dangling once the files it points at are gone:

1. If `DrunkenBakery.OWAtray.ShellIntegration.exe` is still present, it's run once more with the
   `restore` argument (elevated via `runas`, same as "Switch off shell integration" in the GUI) to put
   back whichever mail handler was active before OWAtray registered itself.
2. `RestoreKey()` (run in step 1) only restores those five association values — it deliberately leaves
   the `OWAMapi` client registration tree itself in place, so toggling shell integration off and back on
   doesn't need to fully re-register. Since there's no "back on" coming at uninstall time, the
   uninstaller additionally deletes that tree directly: `HKLM\SOFTWARE\Clients\Mail\OWAMapi` (recursive),
   `HKCR\OWA.Url.Mailto` (recursive), and the `OWA` value under
   `HKLM\SOFTWARE\RegisteredApplications`.

Step 1 needs elevation (`RequestExecutionLevel user` means neither the installer nor the generated
uninstaller auto-elevates) — if the user declines the UAC prompt, or the process otherwise can't reach
`HKEY_LOCAL_MACHINE`, both step 1 and step 2's `HKLM` deletions silently no-op rather than failing the
uninstall.
