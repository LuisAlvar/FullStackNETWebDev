Import-Module WebAdministration

# Variables
$siteName = "HealthCheckApp"
$appPoolName = $siteName

# Check if the site exists
if (Test-Path "IIS:\Sites\$siteName") {
    # Remove the website
    Remove-Item -Path "IIS:\Sites\$siteName" -Recurse -Confirm:$false
    Write-Host "Website $siteName removed successfully."
} else {
    Write-Host "Website $siteName does not exist."
}

# Check if the application pool exists
if (Test-Path "IIS:\AppPools\$appPoolName") {
    # Remove the application pool
    Remove-Item -Path "IIS:\AppPools\$appPoolName" -Recurse -Confirm:$false
    Write-Host "Application Pool $appPoolName removed successfully."
} else {
    Write-Host "Application Pool $appPoolName does not exist."
}
