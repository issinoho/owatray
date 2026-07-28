# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

OWAtray ("OWA Tray Monitor") is a Windows system-tray application that polls an Exchange/Office 365
mailbox via Exchange Web Services (EWS) and pops up notifications (balloon tip, Growl, or Snarl) plus
an optional sound when new mail or upcoming calendar appointments arrive. It also registers itself as
a Simple MAPI provider so other Windows apps can send mail through it, opening compose windows in a
browser against Outlook Web Access.

This is old, StyleCop-formatted C# (targets .NET Framework 4.0, VS2012-era `.csproj`/`.sln` format) plus
one native C++ DLL. There is no cross-platform build; it only builds and runs on Windows.

## Build

Requires Windows with Visual Studio (or MSBuild) and the VC++ toolset for the native project. NuGet
package restore is via `.nuget/NuGet.exe` (old-style packages.config restore, not `PackageReference`).

```
nuget restore OWAtray.sln
msbuild OWAtray.sln /p:Configuration=Release
```

There is no CLI test runner and no test project in the solution — there is currently no automated test
suite. Validate changes by building in Visual Studio and running `DrunkenBakery.OWAtray.GUI.exe` manually
against a real (or test) Exchange/EWS account.

The installer is built from `Installer/OWAtray.nsi` with NSIS (`makensis`), producing `OWAtray.exe`. Its
`PRODUCT_VERSION` define and the `AssemblyVersion`/`AssemblyFileVersion` in
`src/GUI/Properties/AssemblyInfo.cs` are bumped together for each release (see recent "Version to
x.y.z" commits) — that assembly is the one authoritative version number for the product.

## Branching / release convention

Releases are done on `release/x.y.z` branches, merged back into the mainline via a merge commit (see
`git log`). When asked to cut a release, follow that pattern rather than tagging directly on the
mainline.

## Architecture

The solution is split into small single-purpose class libraries under `src/`, wired together by the
GUI's `Form1`:

- **`Framework`** (`Scenario`, `ScenarioFactory`) — loads/saves the user's account configuration
  (a "Scenario") to/from an XML file, and constructs `IEmailInterface` connections from it via the
  connection proxy factory.
- **`Connections/AbstractConnection`** — the `IEmailInterface` contract and `AbstractConnection` base
  class every mail-provider connection implements: connect/disconnect, polling `Interval`, encrypted
  password storage (`EncryptedPassword`/`Password` via `SecurityExtensions.Encrypt/Decrypt`), and events
  (`NewMail`, `NewAppointment`, `MessageCount`, `LogMessage`, `LogException`, `ConnectedStateChange`).
  `EmailType` is the enum of supported providers (currently only `Exchange`).
- **`Connections/EWS`** — `EwsConnection`, the only concrete connection today. Wraps the EWS Managed API
  (`Microsoft.Exchange.WebServices.dll` in `lib/`) to autodiscover/connect to Exchange or Office 365,
  poll the inbox on a `System.Timers.Timer` for unread count / new mail, and poll the calendar for
  upcoming appointments on a second timer.
- **`Connections/Proxy`** — `ConnectionFactory.CreateConnection(EmailType)`, a thin indirection layer so
  `Framework`/`GUI` never reference `Connections.EWS` directly. Add new providers here + a new
  `EmailType` value + a new `Connections/<Provider>` project, following the `EwsConnection` shape.
- **`Logging`** — `LoggerProxy` wraps NLog (configured via `packages.config`/NLog.config) and defines the
  `Severity` enum used by connection log events.
- **`Growl`** / **`Snarl`** — thin static helper wrappers (`GrowlHelper`, `SnarlHelper`) around the
  third-party Growl (`lib/`, via NuGet `Growl` package) and Snarl (`lib/SnarlConnector.dll`) notification
  systems, registered/used from `Form1` alongside the native Windows balloon-tip `NotifyIcon`.
- **`Audio`** — `AudioHelper`, plays the notification sound.
- **`GUI`** — the actual tray app. `Program.cs` is the WinForms entry point; almost all application
  logic (config UI, tray icon/menu, wiring connection events to Growl/Snarl/balloon/audio notifications,
  polling timer, About/ChangeLog/SysInfo dialogs) lives in the large `Form1` partial class. Localized
  strings live in per-culture `.resx` files under `Properties/` (currently: de, it, tr, ca, mk, ru, pl,
  fr, es, cs) — add new user-facing strings to `Resources.resx` and mirror the key into the other
  culture files (existing translations can lag/be left in English for new keys).
- **`ShellIntegration`** — a separate small executable (`DrunkenBakery.OWAtray.ShellIntegration.exe`)
  invoked by the native MAPI DLL. It reads settings written by the main app, opens a browser to OWA's
  "new mail"/"new appointment" URL (with Exchange-version-specific URL formats and MIME/attachment
  handling), and can drive an auto-login via `SendKeys` against the IE/OWA login window.
- **`Mapi`** (`MapiDll.vcxproj`, native C++) — implements the classic Simple MAPI entry points
  (`MAPILogon`, `MAPISendMail`, etc., see `Mapi32.DEF`) so third-party Windows apps that "send via MAPI"
  hand off to OWAtray, which shells out to `ShellIntegration.exe` to actually compose the mail in a
  browser.

Data flow at a glance: `Form1` loads a `Scenario` (XML) → builds an `EwsConnection` via
`ConnectionFactory` → connection polls EWS on timers and raises `NewMail`/`NewAppointment`/`MessageCount`
events → `Form1` fans those out to the tray balloon, `GrowlHelper`, `SnarlHelper`, and `AudioHelper`, and
logs via `LoggerProxy`. Separately, any app doing a classic MAPI send goes through `Mapi`'s native DLL →
`ShellIntegration.exe` → browser against the OWA URLs read from that scenario's connection settings.

## Conventions

- Code follows StyleCop formatting/documentation conventions throughout (see the "Run through StyleCop"
  history) — full XML doc comments on public members, `#region` blocks grouping constants/fields,
  constructors, properties, and methods. Match this style in existing files rather than introducing a
  different convention.
- File headers use a standard copyright banner (`Copyright (c) 2009-<year> The Drunken Bakery`) — update
  the end year when bumping the version, and reuse the existing banner text for new files.
