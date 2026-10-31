# TeumesseCastle branding assets

| File | Role |
|---|---|
| `logo-white.jpg` | Source artwork (Teumesse fox constellation on a white background). Never edited by hand. |
| `logo-white.png` | Same artwork with the white background made transparent (generated). |
| `logo-256.png`, `logo-64.png` | Square, padded, transparent versions (generated). Used by the README and the release page. |
| `TeumesseCastle.ico` | Application icon embedded in `TeumesseCastle.exe` (16, 32, 48 and 256 px, PNG-compressed entries). |
| `report-favicon.b64` | Base64 of a 32x32 PNG, pasted into `Report/ReportBase.cs` (`favicon`). |
| `report-logo.b64` | Base64 of a 60 px-high PNG, pasted into `Report/ReportBase.cs` (`brandLogo`, navigation bar of the HTML reports). |
| `Make-Branding.ps1` | The generator. |

## Regenerating

No ImageMagick needed: the script only uses `System.Drawing` from the .NET Framework.

```powershell
# from the repository root
powershell -ExecutionPolicy Bypass -File assets\branding\Make-Branding.ps1
# optional: another source image or a different white threshold (0-255, default 215)
powershell -ExecutionPolicy Bypass -File assets\branding\Make-Branding.ps1 -Source C:\path\to\logo.jpg -WhiteThreshold 225
```

Then copy the content of the two `.b64` files into `Report/ReportBase.cs` (fields `favicon` and `brandLogo`) and
rebuild. The transparency is produced by treating every pixel whose three channels are above the threshold as
background, with a soft alpha ramp towards pure white so that anti-aliased edges stay smooth.

## Equivalent ImageMagick commands (reference only)

If ImageMagick is available, the same assets can be produced with:

```sh
magick logo-white.jpg -fuzz 8% -transparent white logo-white.png
magick logo-white.png -resize 256x256 -gravity center -background none -extent 256x256 logo-256.png
magick logo-white.png -resize 64x64   -gravity center -background none -extent 64x64   logo-64.png
magick logo-white.png -define icon:auto-resize=256,48,32,16 TeumesseCastle.ico
magick logo-white.png -resize 32x32 -gravity center -background none -extent 32x32 PNG32:- | base64 -w0 > report-favicon.b64
magick logo-white.png -resize x60 PNG32:- | base64 -w0 > report-logo.b64
```

## Trademark note

The fox constellation is the Teumesse logo. The former PingCastle logo and icon are not distributed by this
project; PingCastle is a trademark of its owner.
