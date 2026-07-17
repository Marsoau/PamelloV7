Set-Location -Path $PSScriptRoot

& "$PSScriptRoot\PamelloV7.Server.exe" @args
$code = $LASTEXITCODE

Write-Host ""
if ($code -eq 0) {
    Write-Host "PamelloV7.Server exited normally (code 0)." -ForegroundColor Green
} else {
    Write-Host "PamelloV7.Server exited with code $code." -ForegroundColor Yellow
}
Read-Host "Press Enter to close"
