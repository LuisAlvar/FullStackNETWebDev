<#
   This script queries the Windows Managment Instrumentaiton (WMI) to
   check if the World Wide Wide Publishing Service (W3SVC) is installed, which is part of the IIS.
#>
$installed = Get-WmiObject -Query "SELECT * FROM Win32_Service Where Name='W3SVC'"
if ($installed)
{
    #Write-Host "IIS is installed"
    echo 'IISINSTALLED'
} else {
    #Write-Host "IIS is not installed."
    echo 'NOIIS'
}