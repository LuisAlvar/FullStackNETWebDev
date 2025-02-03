Import-Module WebAdministration

# Variables
$siteName = "HealthCheckAPI"
$appPoolName = $siteName
$sitePath = "C:\inetpub\wwwroot\HealthCheckAPI"
$sitePort = 40080
$bindingInfo = "localhost:${sitePort}:"

# Create the site folder if it doesn't exist
if (!(Test-Path -Path $sitePath)) {
    New-Item -Path $sitePath -ItemType Directory
}

# Check if the site already exists
if (Test-Path "IIS:\Sites\$siteName") {
    Write-Host "Website $siteName already exists."
} else {
    # Create a new application pool
    if (!(Test-Path "IIS:\AppPools\$appPoolName")) {
        New-Item -ItemType "apppool" -Path "IIS:\AppPools\$appPoolName"
        Write-Host "Application Pool $appPoolName created successfully."
    } else {
        Write-Host "Application Pool $appPoolName already exists."
    }
    
    # Create a new IIS website
    New-Item -ItemType Directory -Path "IIS:\Sites\$siteName"
    Set-ItemProperty -Path "IIS:\Sites\$siteName" -Name physicalPath -Value $sitePath
    
    # Assign the application pool to the website
    Set-ItemProperty -Path "IIS:\Sites\$siteName" -Name applicationPool -Value $appPoolName
    
    # Configure the site bindings
    $binding = New-Object psobject -Property @{
        protocol = "http"
        bindingInformation = $bindingInfo
    }
    Set-ItemProperty -Path "IIS:\Sites\$siteName" -Name bindings -Value @($binding)
    
    Write-Host "Website $siteName created successfully on port $sitePort with path $sitePath and assigned to application pool $appPoolName"
}
