Import-Module WebAdministration;
Import-Module IISAdministration;

# Function to remove the certificate
function Remove-Certificate {
    param (
        [string]$certFriendlyName
    )
    $cert = Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object { $_.FriendlyName -eq $certFriendlyName };
    if ($cert) {
        $cert | Remove-Item
        Write-Host 'Certificate '$certFriendlyName' removed successfully.'
    } else {
        Write-Host 'Certificate '$certFriendlyName' not found.'
    }
}

# Variables
$siteName = 'HealthCheckAPI'
$appPoolName = 'HealthCheckAppPool'
$certFriendlyName = 'HealthCheckApp_Cert'

# Remove the website if it exists
if (Test-Path 'IIS:\Sites\$siteName') {
    Remove-Website -Name $siteName
    Write-Host 'Website '$siteName' removed successfully.'
} else {
    Write-Host 'Website '$siteName' not found.'
}

# Remove the application pool if it exists
if (Test-Path 'IIS:\AppPools\$appPoolName') {
    Remove-WebAppPool -Name $appPoolName
    Write-Host 'Application Pool '$appPoolName' removed successfully.'
} else {
    Write-Host 'Application Pool '$appPoolName' not found.'
}

# Remove the self-signed certificate if it exists
Remove-Certificate -certFriendlyName $certFriendlyName

Write-Host 'Uninstallation completed successfully.'