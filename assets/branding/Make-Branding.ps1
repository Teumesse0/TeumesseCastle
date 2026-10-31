<#
.SYNOPSIS
    Regenerates every TeumesseCastle branding asset from the source logo.

.DESCRIPTION
    Produces, next to this script:
      - logo-white.png        : source logo with the uniform white background made transparent
      - logo-256.png          : 256x256 square version (padded, transparent)
      - logo-64.png           : 64x64 square version
      - TeumesseCastle.ico    : multi-size icon (16, 32, 48, 256) with PNG-compressed entries
      - report-favicon.b64    : base64 of a 32x32 PNG, embedded in the HTML reports as favicon
      - report-logo.b64       : base64 of a 60px-high PNG, embedded in the HTML report navigation bar
    Only System.Drawing is used (no ImageMagick required). Run it from any directory:
        powershell -ExecutionPolicy Bypass -File assets\branding\Make-Branding.ps1 [-Source <path to jpg>]
#>
param(
    [string]$Source = (Join-Path $PSScriptRoot 'logo-white.jpg'),
    [int]$WhiteThreshold = 215   # channel value above which a pixel is considered background
)

Add-Type -AssemblyName System.Drawing
$out = $PSScriptRoot

function Remove-WhiteBackground([System.Drawing.Bitmap]$src) {
    $bmp = New-Object System.Drawing.Bitmap $src.Width, $src.Height, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    for ($y = 0; $y -lt $src.Height; $y++) {
        for ($x = 0; $x -lt $src.Width; $x++) {
            $c = $src.GetPixel($x, $y)
            $min = [Math]::Min([Math]::Min($c.R, $c.G), $c.B)
            if ($min -ge $WhiteThreshold) {
                # soft edge: the closer to pure white, the more transparent
                $alpha = [int](255 - (($min - $WhiteThreshold) * 255 / (255 - $WhiteThreshold)))
                if ($alpha -lt 0) { $alpha = 0 }
                $bmp.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($alpha, $c.R, $c.G, $c.B))
            } else {
                $bmp.SetPixel($x, $y, [System.Drawing.Color]::FromArgb(255, $c.R, $c.G, $c.B))
            }
        }
    }
    return $bmp
}

function Resize-Square([System.Drawing.Bitmap]$src, [int]$size) {
    $bmp = New-Object System.Drawing.Bitmap $size, $size, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.Clear([System.Drawing.Color]::Transparent)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $scale = [Math]::Min($size / $src.Width, $size / $src.Height)
    $w = [int]($src.Width * $scale); $h = [int]($src.Height * $scale)
    $g.DrawImage($src, [int](($size - $w) / 2), [int](($size - $h) / 2), $w, $h)
    $g.Dispose()
    return $bmp
}

function Resize-Height([System.Drawing.Bitmap]$src, [int]$height) {
    $scale = $height / $src.Height
    $w = [int][Math]::Round($src.Width * $scale)
    $bmp = New-Object System.Drawing.Bitmap $w, $height, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.Clear([System.Drawing.Color]::Transparent)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.DrawImage($src, 0, 0, $w, $height)
    $g.Dispose()
    return $bmp
}

function Get-PngBytes([System.Drawing.Bitmap]$bmp) {
    $ms = New-Object IO.MemoryStream
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    return ,[byte[]]$ms.ToArray()   # leading comma: keep the byte[] intact through the pipeline
}

function Write-Ico([string]$path, [System.Drawing.Bitmap]$src, [int[]]$sizes) {
    # ICO container with PNG-compressed images (supported since Windows Vista).
    $entries = New-Object System.Collections.ArrayList
    foreach ($s in $sizes) { [void]$entries.Add(@($s, [byte[]](Get-PngBytes (Resize-Square $src $s)))) }
    $ms = New-Object IO.MemoryStream
    $bw = New-Object IO.BinaryWriter $ms
    $bw.Write([uint16]0); $bw.Write([uint16]1); $bw.Write([uint16]$entries.Count)
    $offset = 6 + 16 * $entries.Count
    foreach ($e in $entries) {
        $s = $e[0]; $png = $e[1]
        $bw.Write([byte]($(if ($s -ge 256) { 0 } else { $s })))   # width (0 = 256)
        $bw.Write([byte]($(if ($s -ge 256) { 0 } else { $s })))   # height
        $bw.Write([byte]0); $bw.Write([byte]0)                     # palette, reserved
        $bw.Write([uint16]1); $bw.Write([uint16]32)                # planes, bpp
        $bw.Write([uint32]$png.Length); $bw.Write([uint32]$offset)
        $offset += $png.Length
    }
    foreach ($e in $entries) { $bw.Write($e[1]) }
    $bw.Flush()
    [IO.File]::WriteAllBytes($path, $ms.ToArray())
}

$jpg = [System.Drawing.Bitmap]::FromFile($Source)
$logo = Remove-WhiteBackground $jpg
$logo.Save((Join-Path $out 'logo-white.png'), [System.Drawing.Imaging.ImageFormat]::Png)
(Resize-Square $logo 256).Save((Join-Path $out 'logo-256.png'), [System.Drawing.Imaging.ImageFormat]::Png)
(Resize-Square $logo 64).Save((Join-Path $out 'logo-64.png'), [System.Drawing.Imaging.ImageFormat]::Png)
Write-Ico (Join-Path $out 'TeumesseCastle.ico') $logo @(16, 32, 48, 256)
[IO.File]::WriteAllText((Join-Path $out 'report-favicon.b64'), [Convert]::ToBase64String((Get-PngBytes (Resize-Square $logo 32))))
[IO.File]::WriteAllText((Join-Path $out 'report-logo.b64'), [Convert]::ToBase64String((Get-PngBytes (Resize-Height $logo 60))))
$jpg.Dispose(); $logo.Dispose()
Get-ChildItem $out | Where-Object { $_.Name -ne 'Make-Branding.ps1' } | Select-Object Name, Length
