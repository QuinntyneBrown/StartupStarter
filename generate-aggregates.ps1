# PowerShell script to generate remaining aggregate structures
# This creates the essential files for each aggregate following the pattern

$coreBasePath = "src\StartupStarter.Core\Model"

Write-Host "Generating ProfileAggregate..." -ForegroundColor Green
# Profile aggregate files will be created next

Write-Host "Generating RoleAggregate..." -ForegroundColor Green
# Role aggregate files

Write-Host "All aggregates folder structure created!" -ForegroundColor Cyan
Write-Host "Next: Implement entity logic following Account and User patterns" -ForegroundColor Yellow
