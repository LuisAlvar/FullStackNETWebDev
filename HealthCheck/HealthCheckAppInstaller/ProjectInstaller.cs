using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

namespace HealthCheckAppInstaller
{
  [RunInstaller(true)]
  public class ProjectInstaller: Installer
  {

    public override void Install(IDictionary stateSaver)
    {
      base.Install(stateSaver);

      
    }

  }
}
