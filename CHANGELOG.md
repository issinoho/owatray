# Changelog

This starts at [v3.5.1](https://github.com/issinoho/owatray/releases/tag/v3.5.1), the first release
published from this repository on GitHub. OWAtray is much older than that (see `git log` for the full
history back to 2009) — nothing before it was tracked in a changelog, so it isn't reconstructed here.

## [v3.5.3](https://github.com/issinoho/owatray/releases/tag/v3.5.3) — 2026-07-29

- Fixed the native Mapi DLL (`src/Mapi/MapiDll.cpp`) writing its debug log and per-send attachment
  files to a hardcoded, world-writable `C:\temp\owamapi\` — moved to
  `%LOCALAPPDATA%\OWAtray\{logs,mapi}\`, matching how the rest of the app avoids fixed drive paths.
- Fixed every exported MAPI function except `MAPISendMail` silently failing to log anything on a
  machine where `MAPISendMail` hadn't already run once (only it created the log directory).
- Fixed a same-second collision in per-send attachment temp folder names that could mix attachments
  from two different sends together.
- `CreateProcess`/`RegOpenKeyEx` failures inside `MAPISendMail` are now logged instead of silently
  ignored; two incidental bugs fixed in the same code (`RegCloseKey` called on an uninitialized handle
  on failure, and leaked `CreateProcess` process/thread handles).
- Logging is now timestamped, thread-safe (concurrent MAPI calls could previously interleave log
  writes or race-delete the same temp folder), log-rotated (~2MB cap), and old per-send temp folders
  are swept automatically instead of accumulating forever.
- Fixed the uninstaller leaving OWAtray registered as the Windows default mail handler (pointing at
  now-deleted files) if it was uninstalled without first switching off shell integration — it now runs
  the same restore step as "Switch off shell integration" and removes the `OWAMapi`/`OWA.Url.Mailto`
  registry keys directly.
- Added `MAPI.md` and `REGISTRY.md`, documenting exactly what the native Mapi DLL does and what
  registry keys OWAtray reads/writes when it becomes the default mail handler.

## [v3.5.2](https://github.com/issinoho/owatray/releases/tag/v3.5.2) — 2026-07-29

- Fixed the About box showing "3.5.1.0" instead of "3.5.1" (it printed all four `Version` components;
  `AssemblyInfo.cs` only sets three, so .NET pads the unused fourth with `0`).
- Fixed the NSIS installer's `PRODUCT_VERSION` being stuck at "3.5" instead of the actual release
  version, so Add/Remove Programs showed a stale version too.
- Removed Growl and Snarl notification support entirely (both were long-discontinued third-party
  notification daemons; the native Windows tray balloon — rendered as a modern Action Center toast on
  Windows 10/11 — covers the same need on its own).
- Removed dead/legacy external links from the README and website, keeping only verified-live ones; the
  website's Download button and the README now link to the real GitHub release instead of a
  placeholder.
- Added real screenshots of the app to the website and README, recovered from the Internet Archive's
  copy of the original owatray.com (v3.4.0.0, circa 2014).
- The built website (`docs/`) is now attached as a release asset (`OWAtray-website.zip`) alongside the
  installer.

## [v3.5.1](https://github.com/issinoho/owatray/releases/tag/v3.5.1) — 2026-07-29

First release published from GitHub, and the point this changelog starts. Bundles everything added
while getting the existing 3.5.1 codebase (unreleased since a 2017 Bitbucket-era commit) onto GitHub
with CI and documentation:

- Added `CLAUDE.md` and `README.md` documenting the codebase, build, and configuration.
- Added on-premises support for Exchange 2016, 2019, and Server SE (the bundled EWS Managed API predates
  all three; `ExchangeVersionResolver` maps them onto the closest wire-compatible protocol version).
- Added an NUnit unit test suite (`src/Tests`) covering `Scenario` save/load, `AbstractConnection`
  defaults and encrypted password storage, `ConnectionFactory`, and `ExchangeVersionResolver` — and
  fixed two pre-existing bugs found while writing it.
- Added StyleCop.Analyzers linting across every C# project, and a GitHub Actions workflow that builds
  the binaries and the NSIS installer on every push.
- Added a tag-triggered release step to the workflow, publishing the installer to GitHub Releases.
- Added a landing page under `docs/`, served via GitHub Pages.
