# ProductService.Domain - Kontrollü silme
$productDomainPath = "ProductService\ProductService.Domain\Events"
if (Test-Path $productDomainPath) {
    Get-ChildItem "$productDomainPath\*.cs" | Remove-Item -Force
    Write-Host "Product Domain Events cleaned." -ForegroundColor Cyan
}

# ProductService.Application - Kontrollü silme
$productAppPath = "ProductService\ProductService.Application\EventHandlers"
if (Test-Path $productAppPath) {
    Get-ChildItem "$productAppPath\*.cs" | Remove-Item -Force
    Write-Host "Product Application Handlers cleaned." -ForegroundColor Cyan
}

# AuthService.Application - Klasör varsa temizle
$authDtoPath = "AuthService\AuthService.Application\DTOs"
if (Test-Path $authDtoPath) {
    Get-ChildItem "$authDtoPath\*.cs" | Where-Object { $_.Name -ne "AuthDtos.cs" } | Remove-Item -Force
    Write-Host "Auth DTOs cleaned." -ForegroundColor Cyan
} else {
    Write-Host "Auth DTOs path not found, skipping..." -ForegroundColor Yellow
}

Write-Host "Cleanup completed successfully!" -ForegroundColor Green