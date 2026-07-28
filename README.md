# OWAtray — OWA Tray Monitor

OWAtray is a Windows system-tray application that watches an Exchange or Office 365 mailbox and lets
you know when new mail or upcoming calendar appointments arrive, without needing Outlook running. It
can also register itself as your system's default (Simple MAPI) mail handler, so that "Send to Mail
Recipient" style actions from other Windows apps open a compose window in Outlook Web Access instead of
launching Outlook.

Website: http://www.owatray.com · Support: support@owatray.com

## Features

- Polls a mailbox over Exchange Web Services (EWS) on a configurable interval and shows the unread
  count and new-mail details.
- Calendar polling with reminders for upcoming appointments.
- Autodiscovery of the EWS service URL and OWA URL from an email address, with the option to override
  either manually (e.g. for on-premise Exchange with non-standard URLs, or split-DNS setups).
- Works against on-premise Exchange (with a selectable server version, e.g. Exchange 2007 SP1 through
  2013 SP1) and Office 365.
- Notifications via the native Windows tray balloon, [Growl](http://growl.info), or
  [Snarl](http://snarlapp.com), plus an optional notification sound — pick one or combine them from the
  tray icon's Notifications menu.
- Acts as a Simple MAPI provider: other applications that "send via MAPI" get handed off to OWAtray,
  which opens a new-message (or new-appointment) compose window in a browser against OWA instead of
  requiring Outlook to be installed. Can be switched on/off per-machine from the Advanced menu ("Make
  OWA the default mail handler" / "Switch off shell integration").
- Optional auto-login: OWAtray can drive the OWA/Office 365 login page for you using a saved,
  encrypted password.
- Runs at Windows startup, works with the system default web proxy, and can operate against machines
  joined to (or not joined to) a Windows domain.
- Localized into English, German, Italian, Turkish, Catalan, Macedonian, Russian, Polish, French,
  Spanish and Czech.

## Requirements

- Windows, with the .NET Framework 4.0 runtime.
- An Exchange (on-premise, with EWS enabled) or Office 365 mailbox to connect to.

## Installing

Download and run the installer built from `Installer/OWAtray.nsi` (an NSIS script producing
`OWAtray.exe`), or build it yourself — see below. The installer installs per-user (`$APPDATA\OWAtray`,
`RequestExecutionLevel user`) and does not require admin rights.

## Building from source

Requires Windows with Visual Studio (or MSBuild) and the VC++ toolset (for the native `Mapi` project).

```
nuget restore OWAtray.sln
msbuild OWAtray.sln /p:Configuration=Release
```

There is no automated test suite — validate changes by running `DrunkenBakery.OWAtray.GUI.exe` against
a real (or test) Exchange/EWS mailbox. See `CLAUDE.md` for a fuller breakdown of the project layout and
conventions if you're working on the codebase.

## Configuration

Settings are entered on the main window's **Account** tab (email address, username/password, server,
domain, description, poll interval) and **Advanced** tab (autodiscovery on/off with manual EWS/OWA URL
override, Exchange server version, Office 365 toggle, always-use-Internet-Explorer, disable calendar
polling, auto-login, run on startup, use the system default web proxy, on-Windows-domain, override
SSL/TLS certificate validation, override autodiscovery validation). Settings are persisted to an XML
"Scenario" file so they survive restarts.

## License

OWAtray is freeware (see `src/GUI/License.txt`). You may use it freely, including within a company,
but you may not bundle it with or sell it as part of a commercial product. If you roll it out in a
corporate environment, make sure your users direct support questions to you rather than to the OWAtray
author.

## Translations

Thanks to the following volunteer translators (from the app's Contact Us dialog):

| Language | Translator |
|---|---|
| Catalan | Daniel Sabater |
| German | Christian Treudler |
| Spanish | Daniel Sabater |
| Turkish | pi511 |
| French | Marc Lairet |
| Italian | Marco Procida |
| Russian | Aleksandr Bembel |
| Polish | Ryszard Ostrowski |
| Macedonian | Igor Vojnoski |
| Czech | Jiri Kubinek |
