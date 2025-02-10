Import-Module WebAdministration;
Import-Module IISAdministration;

# Function to check if the certificate exists
function Get-Certificate {
  param (
    [string]$certFriendlyName
  )
  Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object { $_.FriendlyName -eq $certFriendlyName }
}

$MISSINGIISWINDOWSFEATURE = 41
$CORSIISMODULEMISSING = 42
$hostname = [System.Net.Dns]::GetHostName();
$certSelfSignedName = 'HealthCheckApp_Cert'
$moduleCORSName = 'CorsModule'
$moduleRewriteName = 'RewriteModule'

# Check if IIS Windows Feature is enabled
$installed = Get-WmiObject -Query 'SELECT * FROM Win32_Service Where Name=''W3SVC'''
if (-not $installed) {
  exit $MISSINGIISWINDOWSFEATURE
}
Write-Host '[x] IIS enabled'

# Check if IIS Module CORS Module is installed
$installedCORSModule = Get-WebGlobalModule | Where-Object { $_.Name -eq $moduleCORSName }
if (-not $installedCORSModule) {
  exit $CORSIISMODULEMISSING
}
Write-Host '[x] IIS module CORS Installed'

# Check if IIS Module URL Rewrite Module is installed
$installedRewriteModule = Get-WebGlobalModule | Where-Object { $_.Name -eq $moduleRewriteName }
if (-not $installedRewriteModule) {
  exit 
}
Write-Host '[x] IIS module URL Rewrite Installed'


# Check if the certificate exists 
$cert = Get-Certificate -certFriendlyName $certSelfSignedName
# Only add if the certificate does not exist
if ($null -eq $cert) {
  New-SelfSignedCertificate -DnsName $hostname -CertStoreLocation 'Cert:\LocalMachine\My' -FriendlyName $certSelfSignedName
  Write-Host 'Self-signed certificate '$certSelfSignedName' created successfully.'
} else {
  Write-Host 'Certificate '$certSelfSignedName' already exists.'
}

# Variables
$siteName = 'HealthCheckApp'; 
$appPoolName = 'HealthCheckAppPool';
$physicalPath = 'C:\inetpub\wwwroot\HealthCheckApp'; 
$portHttps = 4200

# Create the physicalPath folder if it doesn't exist
if (!(Test-Path -Path $physicalPath)) {
  New-Item -Path $physicalPath -ItemType Directory
  Write-Host 'Directory '$physicalPath' created successfully.'
} else {
  Write-Host 'Directory '$physicalPath' already exists.'
}

# Check if the site already exists
if (Test-Path 'IIS:\Sites\$siteName') {
  Write-Host 'Website '$siteName' already exists.'
} else {

  # Configure the site binding for HTTPS
  $cert = Get-Certificate -certFriendlyName $certSelfSignedName

  if ($cert) {
    # Create a new IIS website
    New-Website -Name $siteName -PhysicalPath $physicalPath -ApplicationPool $appPoolName
    Write-Host 'Website '$siteName' created successfully.'

    # Bind the HTTPS certificate to the website
    New-WebBinding -Name $siteName -IPAddress '*' -Port $portHttps -Protocol https
    $binding = Get-WebBinding -Name $siteName -Protocol 'https'
    $binding.AddSslCertificate($cert.Thumbprint, 'My')
    Write-Host 'HTTPS binding created successfully.'

    # Remove the default HTTP binding on port 80
    Remove-WebBinding -Name $siteName -IPAddress '*' -Port 80 -Protocol http
    Write-Host 'HTTP binding on port 80 removed successfully.'

  } else {
    Write-Host 'Certificate '$certSelfSignedName' not found.'
  }

  Write-Host 'HTTP and HTTPS bindings configured successfully.'
}

# Variables
$webConfigPath = 'C:\inetpub\wwwroot\HealthCheckApp\web.config'

# XML Content for web.config
$webConfigContent = @'
<configuration>
    <system.webServer>
        <rewrite>
            <rules>
                <rule name='AngularRoutes' stopProcessing='true'>
                    <match url='.*' />
                    <conditions logicalGrouping='MatchAll'>
                        <add input='{REQUEST_FILENAME}' matchType='IsFile' negate='true' />
                        <add input='{REQUEST_FILENAME}' matchType='IsDirectory' negate='true' />
                    </conditions>
                    <action type='Rewrite' url='/' />
                </rule>
            </rules>
        </rewrite>
    </system.webServer>
</configuration>
'@

# Create or overwrite the web.config file with the content
Set-Content -Path $webConfigPath -Value $webConfigContent

Write-Host 'web.config created successfully at '$webConfigPath''

exit 0;