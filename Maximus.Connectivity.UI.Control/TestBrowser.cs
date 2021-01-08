using Maximus.Library.GridView;

using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.Mom.Internal.UI;
using Microsoft.EnterpriseManagement.Mom.Internal.UI.Cache;
using Microsoft.EnterpriseManagement.Mom.Internal.UI.Common;
using Microsoft.EnterpriseManagement.Mom.Internal.UI.Controls;
using Microsoft.EnterpriseManagement.Mom.UI;
using Microsoft.EnterpriseManagement.Monitoring;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maximus.Connectivity.UI.Control
{
  public partial class TestBrowser : SimpleGridViewDetailsPlane
  {
    protected ManagementPackRelationship _TestHostedRelationship;
    protected readonly Guid TestHostedRelationshipId = new Guid("dad87fc9-64bb-32b8-ec3c-476c1c8b1b8b");
    private readonly Dictionary<HealthState, Image> HealthStateImages;
    private readonly Dictionary<HealthState, string> HealthStrings;
    private readonly Dictionary<string, Image> MaintenanceModeImages;
    protected readonly ResourceManager ConsoleResources;
    protected readonly CultureInfo CurrentCulture = Thread.CurrentThread.CurrentUICulture;
    protected Guid CurrentParentId;

    public DataGridView Grid => bdgvTestList;
    public FQDNGridView MasterView = null;
    protected ManagementPackRelationship TestHostedRelationship => _TestHostedRelationship == null ? _TestHostedRelationship = ManagementGroup.EntityTypes.GetRelationshipClass(TestHostedRelationshipId) : _TestHostedRelationship;

    public TestBrowser() : base()
    {
      InitializeComponent();
      bdgvTestList.AutoGenerateColumns = false;
      bdgvTestList.DefaultCellStyle.Font = new Font("Segoe UI", bdgvTestList.DefaultCellStyle.Font.Size);
      bdgvTestList.BorderStyle = BorderStyle.None;
      bdgvTestList.BackgroundColor = BrandedColors.GridBackground;
      bdgvTestList.DefaultCellStyle.SelectionForeColor = BrandedColors.GridSelectedItemText;
      bdgvTestList.DefaultCellStyle.SelectionBackColor = BrandedColors.GridSelectedItem;

      ConsoleResources = new ResourceManager("Microsoft.EnterpriseManagement.Mom.Internal.UI.Views.SharedResources", typeof(UrlView).Assembly);
      HealthStateImages = new Dictionary<HealthState, Image>
            {
                {
                    HealthState.Success,
                    (Bitmap)ConsoleResources.GetObject("StateGreen1Image", CurrentCulture)
                },
                {
                    HealthState.Warning,
                    (Bitmap)ConsoleResources.GetObject("StateYellow1Image", CurrentCulture)
                },
                {
                    HealthState.Error,
                    (Bitmap)ConsoleResources.GetObject("StateRed1Image", CurrentCulture)
                },
                {
                    HealthState.Uninitialized,
                    (Bitmap)ConsoleResources.GetObject("StateGray1Image", CurrentCulture)
                }
            };
      HealthStrings = new Dictionary<HealthState, string>
            {
                {
                    HealthState.Success,
                    ConsoleResources.GetString("StateGreen1Text", CurrentCulture)
                },
                {
                    HealthState.Warning,
                    ConsoleResources.GetString("StateYellow1Text", CurrentCulture)
                },
                {
                    HealthState.Error,
                    ConsoleResources.GetString("StateRed1Text", CurrentCulture)
                },
                {
                    HealthState.Uninitialized,
                    ConsoleResources.GetString("StateGray1Text", CurrentCulture)
                }
            };
      MaintenanceModeImages = new Dictionary<string, Image>
            {
                {
                    true.ToString(),
                    (Image)ConsoleResources.GetObject("MaintenanceModeImage", CurrentCulture)
                },
                {
                    false.ToString(),
                    new Bitmap(16, 16)
                }
            };

      // this.conte
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      RegisteredCommand editTestCommand = CommandHelpers.RegisterForNotification(MaximusCommands.EditTest, null, OnEditTestStatus); // exec is in the parent control
      RegisteredCommand deleteTestCommand = CommandHelpers.RegisterForNotification(MaximusCommands.DeleteTest, null, OnDeleteTestStatus); // exec is in the parent control
      CommandHelpers.RegisterForNotification(ViewCommands.Refresh, OnRefreshCommand, null);
      //if (editTestCommand != null)
      //{
      //  CommandService.AddAlias(MaximusCommands.EditTest, MaximusCommands.EditTestAlias);
      //  CommandService.AddAlias(MaximusCommands.DeleteTest, MaximusCommands.DeleteTestAlias);
      //  RegisteredCommand editTestAliasedCommand = CommandService.Find(MaximusCommands.EditTestAlias);
      //}
    }

    protected override void AddContextMenu(ContextMenuHelper contextMenu)
    {
      // in MomViewVase in OnLoad() (!(this is IChildView)) == false, therefor this method is never called.
      base.AddContextMenu(contextMenu);
    }

    private void OnDeleteTestStatus(object sender, CommandStatusEventArgs e)
    {
      if (Grid.SelectedRows != null && Grid.SelectedRows.Count >= 1)
        e.CommandStatus.Enabled = true;
      else
        e.CommandStatus.Enabled = false;
    }

    private void OnEditTestStatus(object sender, CommandStatusEventArgs e)
    {
      if (Grid.SelectedRows != null && Grid.SelectedRows.Count == 1)
        e.CommandStatus.Enabled = true;
      else
        e.CommandStatus.Enabled = false;
    }

    private void bdgvTestList_SelectionChanged(object sender, EventArgs e)
    {
      ForceCommandUpdate(MaximusCommands.DeleteTest, true);
      ForceCommandUpdate(MaximusCommands.EditTest, true);
    }

    protected override string GetViewName() => "Connectivity Tests";

    protected override void OnMasterViewSelectedObjectChange(PartialMonitoringObject monitoringObjectContext)
    {
      CurrentParentId = monitoringObjectContext.Id;
      RegisteredCommand refreshCommand = CommandService.Find(ViewCommands.Refresh);
      refreshCommand?.Invoke(this, this, null);
    }

    protected override void OnRefreshCommand(object sender, CommandEventArgs args)
    {
      base.OnRefreshCommand(sender, args);
      List<TestObjectAdapter> gridDataItemList = new List<TestObjectAdapter>();
      IList<MonitoringObject> testObjects = ManagementGroup.EntityObjects.GetRelatedObjects<MonitoringObject>(CurrentParentId, TestHostedRelationship, TraversalDepth.Recursive, ObjectQueryOptions.Default);
      if (testObjects.Count > 0)
      {
        foreach (MonitoringObject rootEntity in testObjects)
          gridDataItemList.Add(new TestObjectAdapter(rootEntity, MaintenanceModeImages, HealthStateImages, HealthStrings));
        bdgvTestList.DataSource = new BindingSource { DataSource = gridDataItemList };
        HideStatusMessage();
      }
      else
      {
        bdgvTestList.DataSource = null;
        ShowStatusMessage("No test created yet. Use 'Add Test...' from either the Action section in the Tasks pane or from the destination context menu.", showAnimation: false);
      }
    }

    protected override string NoItemSelectedMessage => "Select an item to display its properties and edit test. If no items, select 'New Destination' in the context menu or the task plane.";

    private void smiEditTest_Click(object sender, EventArgs e)
    {
      if (MasterView != null)
      {
        RegisteredCommand editTestCommand = CommandService.Find(MaximusCommands.EditTest);
        if (editTestCommand != null)
          editTestCommand.Invoke(MasterView, this, null);
      }
    }

    private void smiDeleteTest_Click(object sender, EventArgs e)
    {
      if (MasterView != null)
      {
        RegisteredCommand deleteTestCommand = CommandService.Find(MaximusCommands.DeleteTest);
        if (deleteTestCommand != null)
          deleteTestCommand.Invoke(MasterView, this, null);
      }
    }

    private void smiRefresh_Click(object sender, EventArgs e)
    {
      RegisteredCommand refreshCommand = CommandService.Find(ViewCommands.Refresh);
      refreshCommand?.Invoke(this, this, null);
    }
  }
}
