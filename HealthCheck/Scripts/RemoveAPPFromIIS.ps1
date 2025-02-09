Import-Module WebAdministration;
Import-Module IISAdministration;

# Variables
$siteName = 'HealthCheckApp'

# Remove the website if it exists
$siteExists = Get-Website | Where-Object { $_.Name -eq $siteName }
if ($siteExists) {
    Remove-Website -Name $siteName
    Write-Host 'Website '$siteName' removed successfully.'
} else {
    Write-Host 'Website '$siteName' not found.'
}

# Variables
$webConfigPath = 'C:\inetpub\wwwroot\HealthCheckApp\web.config'

# Check if the web.config file exists and remove it
if (Test-Path -Path $webConfigPath) {
    Remove-Item -Path $webConfigPath -Force
    Write-Host 'web.config removed successfully from $webConfigPath'
} else {
    Write-Host 'web.config not found at $webConfigPath'
}

Write-Host 'Uninstallation completed successfully.'
