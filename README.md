<p align="center">
  <img src="assets/branding/logo-256.png" alt="TeumesseCastle" width="160">
</p>

# TeumesseCastle

**Active Directory security audit, free and open source forever.**

TeumesseCastle is the community fork of PingCastle 3.0.0.0, the last version of PingCastle published under a
free license (NPOSL-3.0). It audits an Active Directory domain in minutes, scores its exposure with a
risk-based maturity model and produces a self-contained HTML report, without any license, edition or
telemetry.

> This repository is being prepared for its 1.0.0 release. Until then, see `docs/PROGRESS.md` for the current
> state and `CHANGELOG.md` for what has already changed compared to PingCastle 3.0.0.0.
> *Version française : `README.fr.md` (à venir).*

## Build from source

```powershell
git clone https://github.com/Teumesse0/TeumesseCastle.git
cd TeumesseCastle
dotnet build TeumesseCastle.sln -c Release
.\bin\Release\TeumesseCastle.exe --help
```

Requirements: Windows, .NET SDK 8.x (the tool itself targets .NET Framework 4.5.2, present on every supported
Windows version).

## License and credits

TeumesseCastle is licensed under the Non-Profit Open Software License 3.0, exactly like the PingCastle code
it derives from. See [LICENSE.md](LICENSE.md) and [NOTICE.md](NOTICE.md).

Based on PingCastle 3.0.0.0 by Vincent Le Toux (NPOSL-3.0). PingCastle is a trademark of its owner;
TeumesseCastle is not affiliated with or endorsed by PingCastle or Netwrix.

Published by [Teumesse](https://github.com/Teumesse0).
