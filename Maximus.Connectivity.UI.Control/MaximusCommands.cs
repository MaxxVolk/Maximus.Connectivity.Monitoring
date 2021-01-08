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
    public static readonly Guid Guid = new Guid("545002C5-DAE4-4D09-B02A-706D11E6BC6A");

    public static readonly CommandID NewDestination = new CommandID(Guid, 0);
    public static readonly CommandID DeleteDestination = new CommandID(Guid, 1);
    public static readonly CommandID EditDestination = new CommandID(Guid, 2);
    public static readonly CommandID DeleteTest = new CommandID(Guid, 3);
    public static readonly CommandID EditTest = new CommandID(Guid, 4);

    public static readonly CommandID DeleteTestAlias = new CommandID(Guid, 5);
    public static readonly CommandID EditTestAlias = new CommandID(Guid, 6);

    public static readonly CommandID TestActionsGroup = new CommandID(Guid, 200);
    public static readonly CommandID TestActionsCommand = new CommandID(Guid, 20);

    public static readonly CommandID AddTestCommandBase = new CommandID(Guid, 400);
    
  }
}
