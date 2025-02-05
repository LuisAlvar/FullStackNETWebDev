Import-Module WebAdministration;
$hostname = [System.Net.Dns]::GetHostName()
New-SelfSignedCertificate -DnsName $hostname -CertStoreLocation 'Cert:\LocalMachine\My' -FriendlyName 'HealthCheckApp_Cert'
