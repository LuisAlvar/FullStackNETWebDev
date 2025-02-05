Import-Module WebAdministration;
$hostname = [System.Net.Dns]::GetHostName()
$thumbprint = (Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object { $_.Subject -eq "CN=$hostname" }).Thumbprint
# Check if the certificate was found
if ($thumbprint) {
    # Remove the certificate
    Remove-Item -Path "cert:\LocalMachine\My\$thumbprint"
    Write-Host "Self-signed certificate removed."
} else {
    Write-Host "Self-signed certificate not found."
}