using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Maximus.Connectivity.UI.Control.Properties;
using Maximus.Library.GridView;

using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.Mom.Internal.UI;
using Microsoft.EnterpriseManagement.Mom.Internal.UI.Cache;
using Microsoft.EnterpriseManagement.Mom.Internal.UI.Common;
using Microsoft.EnterpriseManagement.Mom.Internal.UI.Controls;
using Microsoft.EnterpriseManagement.Mom.UI;
using Microsoft.EnterpriseManagement.Monitoring;

namespace Maximus.Connectivity.UI.Control
{
  public partial class FQDNGridView : SimpleGridViewWithDetails
  {
    private Dictionary<Guid, ManagementPackClass> TestClassesAddCommands = new Dictionary<Guid, ManagementPackClass>(); // command Guid = Class Id
    private readonly int TestClassCommandId = 206386;
    private bool TestClassesInitialized = false;

    public FQDNGridView() : base()
    {
      InitializeComponent();
      Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      UseRowContextMenu = false;
      Grid.SelectionChanged += Grid_SelectionChanged;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (DetailView is TestBrowser tb)
        tb.MasterView = this;
    }

    private void Grid_SelectionChanged(object sender, EventArgs e)
    {
      ForceCommandUpdate(MaximusCommands.DeleteDestination, true);
      ForceCommandUpdate(MaximusCommands.EditDestination, true);
      ForceCommandUpdate(MaximusCommands.DeleteTest, true);
      ForceCommandUpdate(MaximusCommands.EditTest, true);
      ForceCommandUpdate(MaximusCommands.MaintenanceModeStart, true);
      ForceCommandUpdate(MaximusCommands.MaintenanceModeEdit, true);
      ForceCommandUpdate(MaximusCommands.MaintenanceModeEnd, true);
    }

    protected override void AddUserActions()
    {
      try
      {
        RegisterCommands();

        // Standard commands in the standard container
        AddTaskItem(TaskCommands.ActionsTaskGroup, MaximusCommands.MaintenanceModeStart);
        AddTaskItem(TaskCommands.ActionsTaskGroup, MaximusCommands.MaintenanceModeEdit);
        AddTaskItem(TaskCommands.ActionsTaskGroup, MaximusCommands.MaintenanceModeEnd);

        // Custom commands in custom containers
        ICommandControlContainer actionsTaskContainer = ((ICommandControlService)GetService(typeof(ICommandControlService))).GetContainer(this, "Actions");

        ICommandTaskGroup destinationsTaskGroup = (ICommandTaskGroup)actionsTaskContainer.CreateCommandControl(this, typeof(ICommandTaskGroup), MaximusCommands.DestinationGroup);
        actionsTaskContainer.Items.Add(destinationsTaskGroup);
        AddTaskItem(MaximusCommands.DestinationGroup, MaximusCommands.NewDestination); // , OnNewDestination); -- do it just ONCE !!!
        AddTaskItem(MaximusCommands.DestinationGroup, MaximusCommands.EditDestination); // , OnEditDestination, OnEditDestinationStatus); -- do it just ONCE !!!
        AddTaskItem(MaximusCommands.DestinationGroup, MaximusCommands.DeleteDestination); // , OnDeleteDestination, OnDeleteDestinationStatus); -- do it just ONCE !!!

        ICommandTaskGroup testsTaskGroup = (ICommandTaskGroup)actionsTaskContainer.CreateCommandControl(this, typeof(ICommandTaskGroup), MaximusCommands.TestGroup);
        actionsTaskContainer.Items.Add(testsTaskGroup);
        if (TestClassesInitialized)
        {
          ICommandTaskDropDown parent = AddTaskDropDownItem(MaximusCommands.TestGroup, MaximusCommands.AddTestDropDown);

          foreach (KeyValuePair<Guid, ManagementPackClass> commandAndClass in TestClassesAddCommands)
          {
            parent.Items.Add(parent.CreateCommandControl(this, typeof(ICommandMenuItem), new CommandID(commandAndClass.Key, TestClassCommandId)));
          }
        }
        AddTaskItem(MaximusCommands.TestGroup, MaximusCommands.EditTest, OnEditTest, null); // status us in the child form
        AddTaskItem(MaximusCommands.TestGroup, MaximusCommands.DeleteTest, OnDeleteTest, null); // status us in the child form

        ICommandTaskGroup configurationTaskGroup = (ICommandTaskGroup)actionsTaskContainer.CreateCommandControl(this, typeof(ICommandTaskGroup), MaximusCommands.ConfigurationGroup);
        actionsTaskContainer.Items.Add(configurationTaskGroup);
        AddTaskItem(MaximusCommands.ConfigurationGroup, MaximusCommands.EditTemplates);
        AddTaskItem(MaximusCommands.ConfigurationGroup, MaximusCommands.BulkExport, OnBulkExport, null); 
        AddTaskItem(MaximusCommands.ConfigurationGroup, MaximusCommands.BulkImport, OnBulkImport, null); 
      }
      catch (Exception e)
      {
        MessageBox.Show($"In {System.Reflection.MethodBase.GetCurrentMethod().Name} {e.GetType().Name} said {e.Message} at {e.StackTrace}");
      }
    }

    private void OnBulkImport(object sender, CommandEventArgs e)
    {
      MessageBox.Show("Not implemented yet.");
    }

    private void OnBulkExport(object sender, CommandEventArgs e)
    {
      MessageBox.Show("Not implemented yet.");
    }

    private void OnNewDestination(object sender, CommandEventArgs e)
    {
      using (NewDestinationDialog newDestinationDialog = new NewDestinationDialog(ManagementGroup, Guid.Empty))
      {
        newDestinationDialog.ShowDialog();
      }
      OnRefreshCommand(this, e);
    }

    protected override void AddUserContextMenu(ContextMenuHelper contextMenu)
    {
      try
      {
        RegisterCommands();
        if (contextMenu != null)
        {
          contextMenu.AddContextMenuItem(MaximusCommands.NewDestination, OnNewDestination); // can register callback only ONCE
          contextMenu.AddContextMenuItem(MaximusCommands.EditDestination, OnEditDestination, OnSingleRowSelectedStatus); // can register callback only ONCE
          contextMenu.AddContextMenuItem(MaximusCommands.DeleteDestination, OnDeleteDestination, OnDeleteDestinationStatus); // can register callback only ONCE
          contextMenu.AddContextMenuSeparator();
          ICommandMenuItem subMenu = contextMenu.AddContextMenuItem(MaximusCommands.AddTestDropDown, null);
          if (TestClassesInitialized)
          {
            foreach(KeyValuePair<Guid, ManagementPackClass> commandAndClass in TestClassesAddCommands)
            {
              contextMenu.AddContextMenuItem(new CommandID(commandAndClass.Key, TestClassCommandId), OnAddNewTest, OnSingleRowSelectedStatus, subMenu); // can register callback only ONCE
            }
          }
          contextMenu.AddContextMenuSeparator();
          contextMenu.AddContextMenuItem(MaximusCommands.MaintenanceModeStart, OnMaintenanceModeStart, OnMaintenanceModeStartStatus); // can register callback only ONCE
          contextMenu.AddContextMenuItem(MaximusCommands.MaintenanceModeEdit, OnMaintenanceModeEdit, OnMaintenanceModeEditStatus); // can register callback only ONCE
          contextMenu.AddContextMenuItem(MaximusCommands.MaintenanceModeEnd, OnMaintenanceModeEnd, OnMaintenanceModeEditStatus); // can register callback only ONCE
          contextMenu.AddContextMenuSeparator();
        }
      }
      catch (Exception e)
      {
        MessageBox.Show($"In {System.Reflection.MethodBase.GetCurrentMethod().Name} it said {e.Message}");
      }
    }

    private void OnMaintenanceModeEnd(object sender, CommandEventArgs e)
    {
      ConsoleJobs.RunJob(this, (param0, param1) =>
      {
        IEnumerable<InstanceState> monitoringObjects = GridSelectedItems.Where(si => si.InMaintenanceMode);
        if (!monitoringObjects.Any())
          return;
        using (MaintenanceModeExitDialog maintenanceModeExitDialog = new MaintenanceModeExitDialog())
        {
          if (maintenanceModeExitDialog.ShowDialog() != DialogResult.Yes)
            return;
          foreach (InstanceState monitoringObject in monitoringObjects)
            monitoringObject.GetPartialMonitoringObject(ManagementGroup).StopMaintenanceMode(DateTime.UtcNow, maintenanceModeExitDialog.ApplyToContained ? TraversalDepth.Recursive : TraversalDepth.OneLevel);
          UpdateCache();
        }
      });
    }

    private void OnMaintenanceModeEditStatus(object sender, CommandStatusEventArgs e)
    {
      e.CommandStatus.Enabled = false;
      ConsoleUserSettings service = (ConsoleUserSettings)GetService(typeof(ConsoleUserSettings));
      if (!ManagementGroupSession.IsUserOperator || service != null && Grid.SelectedRows.Count > service.MaxItemsForMaintenanceMode)
      {
        e.CommandStatus.Visible = false;
      }
      else
      {
        e.CommandStatus.Visible = true;
        e.CommandStatus.Enabled = GridSelectedItems?.All(si => si.InMaintenanceMode) ?? false;
      }
    }

    private void OnMaintenanceModeEdit(object sender, CommandEventArgs e)
    {
      ConsoleJobs.RunJob(this, (param0, param1) =>
      {
        IEnumerable<InstanceState> monitoringObjects = GridSelectedItems.Where(si => si.InMaintenanceMode);
        if (!monitoringObjects.Any())
          return;
        using (MaintenanceModeDialog maintenanceModeDialog = new MaintenanceModeDialog())
        {
          Site.Container.Add(maintenanceModeDialog);
          foreach (InstanceState monitoringObject in monitoringObjects)
            maintenanceModeDialog.EntityItems.Add(monitoringObject.GetPartialMonitoringObject(ManagementGroup));
          if (maintenanceModeDialog.ShowDialog() != DialogResult.OK)
            return;
          UpdateCache(UpdateReason.Refresh);
        }
      });
    }

    private void OnMaintenanceModeStartStatus(object sender, CommandStatusEventArgs e)
    {
      e.CommandStatus.Enabled = false;
      ConsoleUserSettings service = (ConsoleUserSettings)GetService(typeof(ConsoleUserSettings));
      if (!ManagementGroupSession.IsUserOperator || service != null && Grid.SelectedRows.Count > service.MaxItemsForMaintenanceMode)
      {
        e.CommandStatus.Visible = false;
      }
      else
      {
        e.CommandStatus.Visible = true;
        e.CommandStatus.Enabled = GridSelectedItems?.All(si => !si.InMaintenanceMode) ?? false;
      }
    }

    private void OnMaintenanceModeStart(object sender, CommandEventArgs e)
    {
      ConsoleJobs.RunJob(this, (param0, param1) =>
      {
        IEnumerable<InstanceState> monitoringObjects = GridSelectedItems.Where(i => !i.InMaintenanceMode);
        if (!monitoringObjects.Any())
          return;
        using (MaintenanceModeDialog maintenanceModeDialog = new MaintenanceModeDialog())
        {
          Site.Container.Add(maintenanceModeDialog);
          foreach (InstanceState monitoringObject in monitoringObjects)
            maintenanceModeDialog.EntityItems.Add(monitoringObject.GetPartialMonitoringObject(ManagementGroup));
          if (maintenanceModeDialog.ShowDialog() != DialogResult.OK)
            return;
          UpdateCache(UpdateReason.Refresh);
        }
      });
    }

    private void OnDeleteTest(object sender, CommandEventArgs e)
    {
      if (DetailView is TestBrowser tb && tb.Grid.SelectedRows != null && tb.Grid.SelectedRows.Count > 0)
        try
        {
          TestObjectAdapter firstDataItem = (TestObjectAdapter)tb.Grid.SelectedRows[0].DataBoundItem;

          bool doDelete = false;
          if (tb.Grid.SelectedRows.Count == 1)
            doDelete = MessageBox.Show($"You're about to delete the {firstDataItem.DisplayName} destination.\r\nAre you sure?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
          else
            doDelete = MessageBox.Show($"You're about to delete multiple destinations.\r\nAre you sure?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
          if (doDelete)
          {
            EnterpriseManagementConnector connector = ManagementGroup.ConnectorFramework.GetMonitoringConnector();
            IncrementalDiscoveryData incrementalDiscovery = new IncrementalDiscoveryData();
            ManagementPackClass fqdnClass = ManagementGroup.EntityTypes.GetClass(IDs.FullyQualifiedDomainNameClassId);

            foreach (DataGridViewRow gridRow in tb.Grid.SelectedRows)
            {
              TestObjectAdapter dataItem = (TestObjectAdapter)gridRow.DataBoundItem;
              MonitoringObject currentObject = ManagementGroup.EntityObjects.GetObject<MonitoringObject>(dataItem.Source.Id, ObjectQueryOptions.Default);
              CreatableEnterpriseManagementObject oldInstance = new CreatableEnterpriseManagementObject(ManagementGroup, currentObject.GetMostDerivedClasses().First());
              // host keys
              oldInstance[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value = currentObject[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value;
              oldInstance[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value = currentObject[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value;
              // key
              oldInstance[IDs.TestBaseClassProperties.TestIdPropertyId].Value = currentObject[IDs.TestBaseClassProperties.TestIdPropertyId].Value;
              incrementalDiscovery.Remove(oldInstance);
            }

            incrementalDiscovery.Commit(connector);
          }

          // emulate Refresh command for detail view
          SendRefreshCommandToDetailView();
        }
        catch (Exception ex)
        {
          MessageBox.Show($"In {System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().Name} said {ex.Message} at {ex.StackTrace}");
        }
    }

    private void OnEditTest(object sender, CommandEventArgs e)
    {
      if (DetailView is TestBrowser tb && tb.Grid.SelectedRows != null && tb.Grid.SelectedRows.Count == 1)
      {
        TestObjectAdapter targetTestObject = (TestObjectAdapter)tb.Grid.SelectedRows[0].DataBoundItem;
        using (NewTestDialog ntd = new NewTestDialog(ManagementGroup,  null, null, targetTestObject.Source.Id))
        {
          ntd.ShowDialog();
        }
        // emulate Refresh command for detail view
        SendRefreshCommandToDetailView();
      }  
    }

    protected override void OnRefreshCommand(object sender, CommandEventArgs args)
    {
      base.OnRefreshCommand(sender, args);
      // emulate Refresh command for detail view
      SendRefreshCommandToDetailView();
    }

    private void OnAddNewTest(object sender, CommandEventArgs e)
    {
      if (Grid.SelectedRows != null && Grid.SelectedRows.Count == 1 && TestClassesInitialized)
        try
        {
          InstanceState dataItem = (InstanceState)(Grid.SelectedRows[0].Cells[0].Tag as GridDataItem).DataItem;
          Guid currentObjectId = dataItem.PartialMonitoringObjectId;
          if (currentObjectId == Guid.Empty && dataItem.ManagedEntityId != Guid.Empty)
            currentObjectId = dataItem.ManagedEntityId;
          if (currentObjectId == Guid.Empty)
            currentObjectId = dataItem.GetPartialMonitoringObject(ManagementGroup).Id;
          using (NewTestDialog newTestDialog = new NewTestDialog(ManagementGroup, TestClassesAddCommands[e.Id.Guid], ManagementGroup.EntityObjects.GetObject<MonitoringObject>(currentObjectId, ObjectQueryOptions.Default), Guid.Empty))
          {
            newTestDialog.ShowDialog();
          }
          SendRefreshCommandToDetailView();
        }
        catch (Exception ex)
        {
          MessageBox.Show($"In {System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().Name} said {ex.Message} at {ex.StackTrace}");
        }
    }

    private void SendRefreshCommandToDetailView()
    {
      if (DetailView != null)
      {
        RegisteredCommand refreshCommand = CommandService.Find(ViewCommands.Refresh);
        refreshCommand.Invoke(DetailView, this, null);
      }
    }

    private void OnSingleRowSelectedStatus(object sender, CommandStatusEventArgs e)
    {
      if (Grid.SelectedRows != null && Grid.SelectedRows.Count == 1)
        e.CommandStatus.Enabled = true;
      else
        e.CommandStatus.Enabled = false;
    }

    private void OnEditDestination(object sender, CommandEventArgs e)
    {
      if (Grid.SelectedRows != null && Grid.SelectedRows.Count == 1)
        try
        {
          InstanceState dataItem = (InstanceState)(Grid.SelectedRows[0].Cells[0].Tag as GridDataItem).DataItem;
          Guid currentObjectId = dataItem.PartialMonitoringObjectId;
          if (currentObjectId == Guid.Empty && dataItem.ManagedEntityId != Guid.Empty)
            currentObjectId = dataItem.ManagedEntityId;
          if (currentObjectId == Guid.Empty)
            currentObjectId = dataItem.GetPartialMonitoringObject(ManagementGroup).Id;
          using (NewDestinationDialog newDestinationDialog = new NewDestinationDialog(ManagementGroup, currentObjectId))
          {
            newDestinationDialog.ShowDialog();
          }

          // emulate Refresh command
          OnRefreshCommand(sender, e);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"In {System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().Name} said {ex.Message} at {ex.StackTrace}");
        }
    }

    private void OnDeleteDestinationStatus(object sender, CommandStatusEventArgs e)
    {
      if (Grid.SelectedRows != null && Grid.SelectedRows.Count >= 1)
        e.CommandStatus.Enabled = true;
      else
        e.CommandStatus.Enabled = false;
    }

    private void OnDeleteDestination(object sender, CommandEventArgs e)
    {
      if (Grid.SelectedRows != null && Grid.SelectedRows.Count > 0)
        try
        {
          InstanceState firstDataItem = (InstanceState)(Grid.SelectedRows[0].Cells[0].Tag as GridDataItem).DataItem;

          bool doDelete = false;
          if (Grid.SelectedRows.Count == 1)
            doDelete = MessageBox.Show($"You're about to delete the {firstDataItem.DisplayName} destination.\r\nAre you sure?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
          else
            doDelete = MessageBox.Show($"You're about to delete multiple destinations.\r\nAre you sure?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
          if (doDelete)
          {
            EnterpriseManagementConnector connector = ManagementGroup.ConnectorFramework.GetMonitoringConnector();
            IncrementalDiscoveryData incrementalDiscovery = new IncrementalDiscoveryData();
            ManagementPackClass fqdnClass = ManagementGroup.EntityTypes.GetClass(IDs.FullyQualifiedDomainNameClassId);

            foreach (DataGridViewRow gridRow in Grid.SelectedRows)
            {
              InstanceState dataItem = (InstanceState)(gridRow.Cells[0].Tag as GridDataItem).DataItem;
              MonitoringObject currentObject = dataItem.GetMonitoringObject(ManagementGroup, ObjectQueryOptions.Default, retrieveAllProperties: true);
              CreatableEnterpriseManagementObject oldInstance = new CreatableEnterpriseManagementObject(ManagementGroup, fqdnClass);
              oldInstance[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value = currentObject[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value;
              oldInstance[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value = currentObject[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value;
              incrementalDiscovery.Remove(oldInstance);
            }

            incrementalDiscovery.Commit(connector);
          }

          // emulate Refresh command
          OnRefreshCommand(sender, e);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"In {System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().Name} said {ex.Message} at {ex.StackTrace}");
        }
    }

    private bool TryAddCommand(CommandID id, string text, string toolTip, Image image = null, bool enabled = true, bool visible = true)
    {
      if (CommandService.Find(id) != null)
        return true;
      try
      {
        Command newCommand = new Command(id)
        {
          Text = text,
          Enabled = enabled,
          Visible = visible,
          ToolTip = toolTip,
          Image = image
        };
        CommandService.Add(newCommand);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private void RegisterCommands()
    {
      // Destination
      TryAddCommand(MaximusCommands.DestinationGroup, "Destination", "");
      TryAddCommand(MaximusCommands.NewDestination, "New Destination", "Create a new destination FQDN object.", Resources.NewDestination);
      TryAddCommand(MaximusCommands.DeleteDestination, "Delete Destination", "Delete the selected destination.", CommandHelpers.GetImage(StandardCommands.Delete));
      TryAddCommand(MaximusCommands.EditDestination, "Edit Destination", "Edit the selected destination.", CommandHelpers.GetImage(ViewCommands.ViewProperties));
      // Tests
      TryAddCommand(MaximusCommands.TestGroup, "Test Object", "");
      TryAddCommand(MaximusCommands.AddTestDropDown, "Add Test...", "Add a test to the selected destination.");
      TryAddCommand(MaximusCommands.EditTest, "Edit Test", "Edit the currently selected test for the selected destination.");
      TryAddCommand(MaximusCommands.DeleteTest, "Delete Test", "Delete the currently selected test for the selected destination.", CommandHelpers.GetImage(StandardCommands.Delete));
      // Configuration
      TryAddCommand(MaximusCommands.ConfigurationGroup, "Configuration", "");
      TryAddCommand(MaximusCommands.BulkExport, "Bulk Export", "Exports all destinations and optionally export tests.");
      TryAddCommand(MaximusCommands.BulkImport, "Bulk Import", "Import destinations and/or test settings.");
      TryAddCommand(MaximusCommands.EditTemplates, "Edit Templates", "Edit Templates and apply changes to referenced tests.", enabled: false);
      // Maintenance mode
      TryAddCommand(MaximusCommands.MaintenanceModeStart, "Start Maintenance Mode...", "", (Bitmap)ConsoleResources.GetObject("MaintenanceModeEnterImage", CurrentCulture));
      TryAddCommand(MaximusCommands.MaintenanceModeEdit, "Edit Maintenance Mode Settings...", "", (Bitmap)ConsoleResources.GetObject("MaintenanceModeChangeImage", CurrentCulture));
      TryAddCommand(MaximusCommands.MaintenanceModeEnd, "Stop Maintenance Mode", "", (Bitmap)ConsoleResources.GetObject("MaintenanceModeExitImage", CurrentCulture));

      // dynamic commands
      if (!TestClassesInitialized)
      {
        ManagementPackClass baseClass = ManagementGroup.EntityTypes.GetClass(IDs.TestBaseClassId);
        IList<ManagementPackType> testClasses = baseClass.GetDerivedTypes(TraversalDepth.Recursive);
        foreach(ManagementPackType testClass in testClasses)
        {
          if (testClass.Abstract)
            continue;
          CommandID testClassCommand = new CommandID(testClass.Id, TestClassCommandId);
          TestClassesAddCommands.Add(testClassCommand.Guid, ManagementGroup.EntityTypes.GetClass(testClass.Id));
          string commandText = string.IsNullOrWhiteSpace(testClass.DisplayName) ? testClass.Name : testClass.DisplayName;
          ManagementPack testClassMP = testClass.GetManagementPack();
          commandText += " (" + (testClassMP.DisplayName ?? testClassMP.Name) + ")";
          TryAddCommand(testClassCommand, $"Add {commandText}", "Adds a test to the selected destination.");
        }
        TestClassesInitialized = true;
      }
    }

    protected override Type GetDetailedViewControlType()
    {
      return typeof(TestBrowser);
    }

    protected ManagementPackClass ViewClassCached = null;
    protected override ManagementPackClass GetViewClass()
    {
      if (ViewClassCached == null || !ViewClassCached.ManagementGroup.IsConnected)
        ViewClassCached = ManagementGroup.EntityTypes.GetClass(IDs.FullyQualifiedDomainNameClassId);
      return ViewClassCached;
    }
  }
}
