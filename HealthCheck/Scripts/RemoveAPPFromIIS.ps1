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
$siteName = 'HealthCheckApp'

# Remove the website if it exists
if (Test-Path 'IIS:\Sites\$siteName') {
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
