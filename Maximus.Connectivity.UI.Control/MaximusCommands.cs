using Microsoft.EnterpriseManagement.Mom.Internal.UI.Common;

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maximus.Connectivity.UI.Control
{
  public static class MaximusCommands
  {
    private static readonly Guid MaximusCommandsBaseId = new Guid("545002C5-DAE4-4D09-B02A-706D11E6BC6A");

    public static readonly CommandID DestinationGroup = new CommandID(MaximusCommandsBaseId, 100);
    public static readonly CommandID NewDestination = new CommandID(MaximusCommandsBaseId, 0);
    public static readonly CommandID DeleteDestination = new CommandID(MaximusCommandsBaseId, 1);
    public static readonly CommandID EditDestination = new CommandID(MaximusCommandsBaseId, 2);

    public static readonly CommandID TestGroup = new CommandID(MaximusCommandsBaseId, 200);
    public static readonly CommandID AddTestDropDown = new CommandID(MaximusCommandsBaseId, 20);
    public static readonly CommandID DeleteTest = new CommandID(MaximusCommandsBaseId, 3);
    public static readonly CommandID EditTest = new CommandID(MaximusCommandsBaseId, 4);

    public static readonly CommandID ConfigurationGroup = new CommandID(MaximusCommandsBaseId, 300);
    public static readonly CommandID BulkImport = new CommandID(MaximusCommandsBaseId, 5);
    public static readonly CommandID BulkExport = new CommandID(MaximusCommandsBaseId, 6);
    public static readonly CommandID EditTemplates = new CommandID(MaximusCommandsBaseId, 7);
  }

  public static class TestActions
  {
    //public static readonly Guid DestinationGroupId = new Guid("4564A340-B18A-43D7-9ECD-65B7A3F5DE37");
    //public static readonly Guid TestGroupId = new Guid("33ECEC17-5C2C-4FAB-AFC8-1B75D613BDCC");
    //public static readonly Guid ConfigurationGroupId = new Guid("4ECD72C2-8485-45CC-8980-F6A4EF1C0E7F");

    //public static readonly CommandID DestinationGroup = CommandHelpers.RegisterCommand(DestinationGroupId, 0, "Destinations", null);
    //public static readonly CommandID TestGroup = CommandHelpers.RegisterCommand(TestGroupId, 0, "Tests", null);
    //public static readonly CommandID ConfigurationGroup = CommandHelpers.RegisterCommand(ConfigurationGroupId, 0, "Configuration", null);

    // public static readonly CommandID CreateNewGroupCommand = CommandHelpers.RegisterCommand(DestinationGroupId, 1, "New Destination", (Image)GroupsResources.GroupImage);
  }
}
