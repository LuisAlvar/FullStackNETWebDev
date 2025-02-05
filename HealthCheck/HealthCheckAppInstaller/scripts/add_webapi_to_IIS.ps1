Import-Module WebAdministration;

# Variables
$siteName = 'HealthCheckAPI'; 
$appPoolName = 'HealthCheckAppPool';
$physicalPath = 'C:\inetpub\wwwroot\HealthCheckAPI'; 
$portHttp = 40080
$portHttps = 40443
$bindingInfoHttp = "http/*:${portHttp}:localhost";
$bindingInfoHttps = "https/*:${portHttps}:localhost"; 

# Create the physicalPath folder if it doesn't exist
if (!(Test-Path -Path $physicalPath)) {
    New-Item -Path $physicalPath -ItemType Directory
}

# Check if the site already exists
if (Test-Path "IIS:\Sites\$siteName") {
    Write-Host "Website $siteName already exists."
} else {
    # Create a new application pool
    if (!(Test-Path "IIS:\AppPools\$appPoolName")) {
        New-WebAppPool -Name $appPoolName;
        Write-Host "Application Pool $appPoolName created successfully."
    } else {
        Write-Host "Application Pool $appPoolName already exists."
    }

    # Create a new IIS website with HTTP binding
    New-Website -Name $siteName -PhysicalPath $physicalPath -ApplicationPool $appPoolName -Port $portHttp -HostHeader 'localhost'
    Write-Host "Website $siteName created successfully."

    # Configure the site binding for https
    $cert = Get-ChildItem -Path cert:\LocalMachine\My | Where-Object { $_.FriendlyName -eq 'HealthCheckApp_Cert' }
    if ($cert) {
        New-WebBinding -Name $siteName -Protocol 'https' -Port $portHttps -HostHeader 'localhost'
        Push-Location IIS:\SslBindings
        New-Item "0.0.0.0!$portHttps" -Value $cert
        Pop-Location
        Write-Host "HTTPS binding configured successfully."
    } else {
        Write-Host "Certificate 'HealthCheckAPI_Cert' not found."
    }

    Write-Host "HTTP and HTTPS bindings configured successfully."
}
