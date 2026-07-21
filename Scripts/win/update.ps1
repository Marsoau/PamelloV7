$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12  # older Windows default to old TLS

$Repo        = 'Marsoau/PamelloV7'
$Rid         = 'win-x64'
$PreReleases = $true

$Dir = $PSScriptRoot

$api = if ($PreReleases) { "https://api.github.com/repos/$Repo/releases?per_page=1" }
       else               { "https://api.github.com/repos/$Repo/releases/latest" }

$rel = Invoke-RestMethod -Uri $api -Headers @{ 'User-Agent' = 'PamelloV7-Updater' }
if ($PreReleases) { $rel = $rel[0] }

$tag   = $rel.tag_name
$asset = $rel.assets | Where-Object { $_.name -like "*-$Rid.zip" } | Select-Object -First 1
if (-not $tag -or -not $asset) { Write-Host "Could not find a $Rid release for $Repo."; return }
$url = $asset.browser_download_url

$exe = Join-Path $Dir 'PamelloV7.Server.exe'
$current = ''
if (Test-Path $exe) { $current = "$(& $exe --version 2>$null)".Trim() }

if ($current -eq $tag) { Write-Host "Already up to date ($tag)."; return }

$shown = if ($current) { $current } else { 'none' }
Write-Host "New version: $shown -> $tag"
Write-Host "This will DELETE everything in: $Dir"
if ((Read-Host 'Continue? [y/N]') -notmatch '^[yY]$') { Write-Host 'Aborted.'; return }

$tmp = Join-Path $env:TEMP ([guid]::NewGuid())
New-Item -ItemType Directory -Path $tmp | Out-Null
try {
    Write-Host "Downloading $tag ..."
    Invoke-WebRequest -Uri $url -OutFile "$tmp\release.zip" -UseBasicParsing
    Expand-Archive -Path "$tmp\release.zip" -DestinationPath "$tmp\out" -Force

    Write-Host 'Installing ...'
    $keep = @('update.bat', 'update.ps1')
    Get-ChildItem -LiteralPath $Dir -Force |
        Where-Object { $keep -notcontains $_.Name } |
        Remove-Item -Recurse -Force -ErrorAction SilentlyContinue

    $inner = Get-ChildItem -LiteralPath "$tmp\out" -Directory | Select-Object -First 1
    Get-ChildItem -LiteralPath $inner.FullName -Force |
        Where-Object { $keep -notcontains $_.Name } |
        Move-Item -Destination $Dir -Force

    Write-Host "Done. Now on $tag"
}
finally {
    Remove-Item -LiteralPath $tmp -Recurse -Force -ErrorAction SilentlyContinue
}
