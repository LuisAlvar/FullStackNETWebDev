$CurrentDirectory = "C:\Workstation\FullStackNETWebDev\HealthCheck\HealthCheckAPI\bin\Release\net6.0\publish"

$Files = Get-ChildItem -Path $CurrentDirectory -File

foreach ($File in $Files)
{
	Write-Output $File.Name
}