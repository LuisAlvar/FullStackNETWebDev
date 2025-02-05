using System;
using System.Collections;
using System.Management.Automation;

namespace HealthCheckAppInstaller
{
  public class IISScripts
  {
    public static int CheckForIIS()
    {
      string script = @"
        <#
           This script queries the Windows Managment Instrumentaiton (WMI) to
           check if the World Wide Wide Publishing Service (W3SVC) is installed, which is part of the IIS.
        #>
        $installed = Get-WmiObject -Query 'SELECT * FROM Win32_Service Where Name=\'W3SVC\''
        if ($installed)
        {
            echo 'true'
        } else {
            echo 'false'
        }
      ";

      using (PowerShell powerShell = powerShell.Create())
      {

      }

      return 0;
    }
  }
}
