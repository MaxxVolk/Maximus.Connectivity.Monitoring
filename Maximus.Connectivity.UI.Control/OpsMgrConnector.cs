using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.ConnectorFramework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maximus.Connectivity.UI.Control
{
  public static class OpsMgrConnector
  {
    private static readonly Guid connectorID = new Guid("76AF656D-81A4-40CD-89B0-E0DF577A8AE9");

    public static MonitoringConnector GetMonitoringConnector(this IConnectorFrameworkManagement connectorFramework)
    {
      MonitoringConnector myConnnector;
      
      try
      {
        myConnnector = connectorFramework.GetConnector(connectorID);
        if (!myConnnector.Initialized)
          myConnnector.Initialize();
      }
      catch (ObjectNotFoundException)
      {
        myConnnector = connectorFramework.Setup(new ConnectorInfo()
        {
          Name = "Maximus Connectivity Monitoring Connector",
          DisplayName = "Maximus Connectivity Monitoring Connector",
          Description = "Connector for Maximus Connectivity Monitoring Management Pack",
          DiscoveryDataIsManaged = true
        }, connectorID);
        if (!myConnnector.Initialized)
          myConnnector.Initialize();
      }
      return myConnnector;
    }
  }
}
