using System;
using System.Collections.Generic;
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
    }

    protected override void AddUserActions()
    {
      try
      {
        RegisterCommands();
        AddTaskItem(TaskCommands.ActionsTaskGroup, MaximusCommands.NewDestination); // , OnNewDestination); -- do it just ONCE !!!
        AddTaskItem(TaskCommands.ActionsTaskGroup, MaximusCommands.EditDestination); // , OnEditDestination, OnEditDestinationStatus); -- do it just ONCE !!!
        AddTaskItem(TaskCommands.ActionsTaskGroup, MaximusCommands.DeleteDestination); // , OnDeleteDestination, OnDeleteDestinationStatus); -- do it just ONCE !!!
        AddTaskSeparatorItem(TaskCommands.ActionsTaskGroup);
        if (TestClassesInitialized)
        {
          ICommandTaskDropDown parent = AddTaskDropDownItem(TaskCommands.ActionsTaskGroup, MaximusCommands.TestActionsCommand);
          
          foreach (KeyValuePair<Guid, ManagementPackClass> commandAndClass in TestClassesAddCommands)
          {
            parent.Items.Add(parent.CreateCommandControl(this, typeof(ICommandMenuItem), new CommandID(commandAndClass.Key, TestClassCommandId)));
          }
        }
        AddTaskItem(TaskCommands.ActionsTaskGroup, MaximusCommands.EditTest, OnEditTest, null); // status us in the child form
        AddTaskItem(TaskCommands.ActionsTaskGroup, MaximusCommands.DeleteTest, OnDeleteTest, null); // status us in the child form
        AddTaskSeparatorItem(TaskCommands.ActionsTaskGroup);
        AddTaskItem(TaskCommands.ActionsTaskGroup, MaximusCommands.BulkExport, OnBulkExport, null); 
        AddTaskItem(TaskCommands.ActionsTaskGroup, MaximusCommands.BulkImport, OnBulkImport, null); 
        AddTaskSeparatorItem(TaskCommands.ActionsTaskGroup);
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
          ICommandMenuItem subMenu = contextMenu.AddContextMenuItem(MaximusCommands.TestActionsCommand, null);
          if (TestClassesInitialized)
          {
            foreach(KeyValuePair<Guid, ManagementPackClass> commandAndClass in TestClassesAddCommands)
            {
              contextMenu.AddContextMenuItem(new CommandID(commandAndClass.Key, TestClassCommandId), OnAddNewTest, OnSingleRowSelectedStatus, subMenu); // can register callback only ONCE
            }
          }
          contextMenu.AddContextMenuSeparator();
        }
      }
      catch (Exception e)
      {
        MessageBox.Show($"In {System.Reflection.MethodBase.GetCurrentMethod().Name} it said {e.Message}");
      }
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
      TryAddCommand(MaximusCommands.NewDestination, "New Destination", "Create a new destination FQDN object.", Resources.NewDestination);
      TryAddCommand(MaximusCommands.DeleteDestination, "Delete Destination", "Delete the selected destination.", CommandHelpers.GetImage(StandardCommands.Delete));
      TryAddCommand(MaximusCommands.EditDestination, "Edit Destination", "Delete the selected destination.", CommandHelpers.GetImage(ViewCommands.ViewProperties));
      TryAddCommand(MaximusCommands.TestActionsCommand, "Add Test...", "Add a test to the selected destination.");
      TryAddCommand(MaximusCommands.EditTest, "Edit Test", "Edit the currently selected test for the selected destination.");
      TryAddCommand(MaximusCommands.DeleteTest, "Delete Test", "Delete the currently selected test for the selected destination.");
      TryAddCommand(MaximusCommands.BulkExport, "Bulk Export", "Exports all destinations and optionally export tests.");
      TryAddCommand(MaximusCommands.BulkImport, "Bulk Import", "Import destinations and/or test settings.");

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
