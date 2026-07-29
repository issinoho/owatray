# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

OWAtray ("OWA Tray Monitor") is a Windows system-tray application that polls an Exchange/Office 365
mailbox via Exchange Web Services (EWS) and pops up a tray-balloon notification (which Windows 10/11
itself renders as a modern Action Center toast) plus an optional sound when new mail or upcoming
calendar appointments arrive. It also registers itself as a Simple MAPI provider so other Windows apps
can send mail through it, opening compose windows in a browser against Outlook Web Access.

This is old, StyleCop-formatted C# (targets .NET Framework 4.0, VS2012-era `.csproj`/`.sln` format) plus
one native C++ DLL. The end-to-end app (GUI, ShellIntegration, Mapi) only builds and runs on Windows —
GUI/ShellIntegration are WinForms, Mapi is native C++ requiring the VC++ toolset. The non-UI class
libraries (`Connections.*`, `Framework`, `Logging`, and the `Tests` project) have no Windows-only
dependencies and build/run fine on Mono/Linux too (verified with Mono's MSBuild + `mono`); that's what
makes the unit test suite runnable in a Linux dev/CI environment.

## Build

On Windows: Visual Studio (or MSBuild) and the VC++ toolset for the native project. NuGet package
restore is via `.nuget/NuGet.exe` — but that bundled `nuget.exe` is a ~2011-era client (v1.6) that can no
longer talk to nuget.org (the legacy V2 OData feed it uses was retired); restore with a modern NuGet
client instead (Visual Studio's own restore, a current `nuget.exe`, or `dotnet restore`/`dotnet add
package` into the same `packages/` layout). `.nuget/nuget.targets` and `.nuget/nuget.exe` are symlinks to
the real (differently-cased) `NuGet.targets`/`NuGet.exe` files, added so the `.csproj` files' lowercase
`Import`/tool references resolve on case-sensitive filesystems (Linux/Mac) as well as Windows.

```
nuget restore OWAtray.sln
msbuild OWAtray.sln /p:Configuration=Release
```

### Tests

`src/Tests` (`DrunkenBakery.OWAtray.Tests`, NUnit 3) covers the non-UI, non-network logic: `Scenario`
XML save/load round-tripping, `AbstractConnection` defaults and encrypted-password storage,
`ConnectionFactory`, the `EmailType`/`ConnectionState` description enums, and `ExchangeVersionResolver`.
It does not and cannot cover `EwsConnection`'s actual EWS network calls, the WinForms `GUI` project, or
`ShellIntegration`/`Mapi` (native, `SendKeys`/registry/COM-driven) — those need manual testing against a
real Exchange/EWS account per the note below. Run the suite with NUnit's console runner:

```
msbuild OWAtray.sln /t:DrunkenBakery_OWAtray_Tests /p:Configuration=Debug
nunit3-console src/Tests/bin/Debug/DrunkenBakery.OWAtray.Tests.dll
```

(On Linux, that's `mono path/to/nunit3-console.exe ...` — NUnit's console runner isn't bundled in the
repo; fetch `NUnit.ConsoleRunner` the same way as any other package.) Validate everything else by
building in Visual Studio and running `DrunkenBakery.OWAtray.GUI.exe` manually against a real (or test)
Exchange/EWS account.

### Linting

Every C# project (all of `src/` except the native `Mapi`) has [StyleCop.Analyzers](
https://github.com/DotNetAnalyzers/StyleCopAnalyzers) wired in as a Roslyn analyzer (`<Analyzer
Include="...">` + `<AdditionalFiles Include="...\stylecop.json">` in each `.csproj`, config at
`stylecop.json` and `.editorconfig` at the repo root) — it runs automatically as part of a normal build
(`msbuild ...`, no separate lint step) and the whole solution is currently warning-clean. Two rule
categories are deliberately turned off repo-wide via `.editorconfig` rather than "fixed", because
enforcing them would mean fighting the codebase's own long-standing conventions instead of catching real
issues:
- **SA1633–SA1641 (file headers)**: every file already carries a copyright banner, just not in the exact
  machine-checkable form StyleCop expects.
- **SA1124 (no regions)**: `#region` blocks (Constants and Fields, Public Properties, ...) are used with
  total consistency in every hand-written file — a deliberate structural convention from the codebase's
  original (classic) StyleCop pass, not something to strip out.

`src/Tests/.editorconfig` additionally turns off SA1600 (elements must be documented) and SA0001 (doc
analysis disabled) for that project only — test methods/classes are self-documenting via their names, and
the test assembly generates no `<DocumentationFile>` for anything to consume. All other projects have
`<DocumentationFile>` enabled so both StyleCop's own documentation-content rules and the C# compiler's
native `CS1591`/`CS1574` doc-completeness/cref-resolution warnings run for real.

`GUI` has a `<COMReference>` to `IWshRuntimeLibrary` (used by `WindowsShortcut.cs`), which makes
`msbuild` invoke `ResolveComReferences`/`AxImp.exe` — unavailable under Mono, so `GUI` can't be built via
`msbuild` on Linux at all (Mono's MSBuild also segfaults on this specific failure rather than reporting
it cleanly). Its StyleCop compliance was instead verified by invoking `csc` directly on its source files
with the same `/analyzer:`/`/additionalfile:`/`/analyzerconfig:` flags MSBuild would pass, referencing a
local throwaway stub assembly in place of `IWshRuntimeLibrary` purely to get past type resolution — that
stub is not part of the repo or the real build.

The installer is built from `Installer/OWAtray.nsi` with NSIS (`makensis`), producing `OWAtray.exe`. Its
`PRODUCT_VERSION` define and the `AssemblyVersion`/`AssemblyFileVersion` in
`src/GUI/Properties/AssemblyInfo.cs` are bumped together for each release (see recent "Version to
x.y.z" commits) — that assembly is the one authoritative version number for the product.

### CI

`.github/workflows/build.yml` builds the binaries and the NSIS installer on every push to `main`, on a
`windows-latest` runner (the only OS this solution actually builds on). It restores with a modern NuGet
client (see above), builds `GUI`+`ShellIntegration` (which pulls in every other C# project transitively
via their `..\..\bin\` `OutputPath`, so no manual staging step is needed for those), builds `Mapi` for
both `Win32` and `x64` overriding its old `v110` toolset to `v143` on the command line (VS2022 doesn't
ship v110), mirrors the built GUI/ShellIntegration/library binaries into `bin\Secure\` (see the workflow
file for why — that folder isn't produced by anything else in this repo), installs NSIS plus the
third-party `nsProcess` plugin the `.nsi` requires, and uploads both the installer and the raw `bin\` as
build artifacts. No test execution is wired in — just the build.

Pushing a version tag (`v*`, e.g. `v3.5.1`) does the same build and additionally publishes a GitHub
Release named after the tag with the installer attached, via `softprops/action-gh-release`. An ordinary
push to `main` never creates a release — only a tag push does, so cutting a release is a deliberate,
separate action. The website's Download button links to this repo's releases page, so a release needs
to exist there for that link to have anything to show.

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
  upcoming appointments on a second timer. Supported server versions (the `ServerVersion`/`Version`
  strings, selectable in the GUI's `cmbExchangeVersion`) are `Default` (no explicit version — server
  auto-negotiates), `Exchange2007_SP1`, `Exchange2010`, `Exchange2010_SP1`, `Exchange2010_SP2`,
  `Exchange2010_SP3`, `Exchange2013`, `Exchange2013_SP1`, `Exchange2016`, `Exchange2019`, and
  `ExchangeServerSE` (Office 365 reports as Exchange2013-family). The bundled EWS Managed API predates
  Exchange 2016/2019/SE and its `ExchangeVersion` enum has no distinct member for any of them (nor for
  2010 SP3) — `ExchangeVersionResolver` maps each of those onto the closest wire-compatible enum value
  (2010 SP3 → 2010 SP2, 2016/2019/SE → 2013 SP1) when calling into the API, while `Connect()`/the
  `Version` getter still display and persist the user's original selection. When adding a future
  Exchange version this way, add it to `ExchangeVersionResolver`, the `cmbExchangeVersion` items in
  `Form1.Designer.cs`, and `ShellIntegration.Program.ModernComposeUrlVersions` if it uses the post-2013
  OWA compose UI.
  This only covers on-premises Exchange over Basic Auth — Exchange Online/Office 365 disabled EWS Basic
  Auth for most tenants in October 2022, so connecting to a modern Microsoft 365 mailbox would require
  adding OAuth 2.0 ("Modern Auth") support, which does not exist yet.
- **`Connections/Proxy`** — `ConnectionFactory.CreateConnection(EmailType)`, a thin indirection layer so
  `Framework`/`GUI` never reference `Connections.EWS` directly. Add new providers here + a new
  `EmailType` value + a new `Connections/<Provider>` project, following the `EwsConnection` shape.
- **`Logging`** — `LoggerProxy` wraps NLog (configured via `packages.config`/NLog.config) and defines the
  `Severity` enum used by connection log events.
- **`Audio`** — `AudioHelper`, plays the notification sound.
- **`GUI`** — the actual tray app. `Program.cs` is the WinForms entry point; almost all application
  logic (config UI, tray icon/menu, wiring connection events to balloon/audio notifications, polling
  timer, About/ChangeLog/SysInfo dialogs) lives in the large `Form1` partial class. Localized
  strings live in per-culture `.resx` files under `Properties/` (currently: de, it, tr, ca, mk, ru, pl,
  fr, es, cs) — add new user-facing strings to `Resources.resx` and mirror the key into the other
  culture files (existing translations can lag/be left in English for new keys).
- **`ShellIntegration`** — a separate small executable (`DrunkenBakery.OWAtray.ShellIntegration.exe`)
  invoked by the native MAPI DLL. It reads settings written by the main app, opens a browser to OWA's
  "new mail"/"new appointment" URL (with MIME/attachment handling), and can drive an auto-login via
  `SendKeys` against the IE/OWA login window. The compose-URL format branches on the same
  `ServerVersion` string as `EwsConnection`, via `Program.ModernComposeUrlVersions`: Exchange 2013 and
  newer use the newer compose-URL format, everything from 2007 SP1 through 2010 SP3 uses the legacy
  URL/MIME-URL format.
- **`Mapi`** (`MapiDll.vcxproj`, native C++) — implements the classic Simple MAPI entry points
  (`MAPILogon`, `MAPISendMail`, etc., see `Mapi32.DEF`) so third-party Windows apps that "send via MAPI"
  hand off to OWAtray, which shells out to `ShellIntegration.exe` to actually compose the mail in a
  browser.
- **`Tests`** — NUnit unit tests for `Connections.*`/`Framework`/`Logging`; see "Tests" under Build above.

Data flow at a glance: `Form1` loads a `Scenario` (XML) → builds an `EwsConnection` via
`ConnectionFactory` → connection polls EWS on timers and raises `NewMail`/`NewAppointment`/`MessageCount`
events → `Form1` fans those out to the tray balloon and `AudioHelper`, and logs via `LoggerProxy`. Separately, any app doing a classic MAPI send goes through `Mapi`'s native DLL →
`ShellIntegration.exe` → browser against the OWA URLs read from that scenario's connection settings.

## Conventions

- Code follows StyleCop formatting/documentation conventions throughout (see the "Run through StyleCop"
  history, and the StyleCop.Analyzers wiring under "Linting" above, which now enforces this on every
  build) — full XML doc comments on public members, `#region` blocks grouping constants/fields,
  constructors, properties, and methods. Match this style in existing files rather than introducing a
  different convention.
- File headers use a standard copyright banner (`Copyright (c) 2009-<year> The Drunken Bakery`) — update
  the end year when bumping the version, and reuse the existing banner text for new files.
