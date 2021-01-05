using Maximus.Library.GridView;

using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Mom.Internal.UI.Controls;
using Microsoft.EnterpriseManagement.Mom.UI;
using Microsoft.EnterpriseManagement.Monitoring;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maximus.Connectivity.UI.Control
{
  public partial class TestBrowser : SimpleGridViewDetailsPlane
  {
    protected ManagementPackRelationship TestHostedRelationship;
    protected readonly Guid TestHostedRelationshipId = new Guid("dad87fc9-64bb-32b8-ec3c-476c1c8b1b8b");

    public TestBrowser() : base()
    {
      InitializeComponent();
      TestHostedRelationship = ManagementGroup.EntityTypes.GetRelationshipClass(TestHostedRelationshipId);
    }

    protected override string GetViewName() => "Connectivity Tests";

    protected override void OnMasterViewSelectedObjectChange(PartialMonitoringObject monitoringObjectContext)
    {

      gcTestList.SuspendLayout();

      gcTestList.Translator = new StateDetailView.SubRoleStateTranslator();
      gcTestList.Columns.Clear();
      DetailViewSupport.AddColumn((DataGridView)gcTestList, (DataGridViewColumn)new GridControlImageTextColumn() // StateDetailView.imageTable, StateDetailView.stringTable
      {
        DefaultKey = HealthState.Uninitialized.ToString(),
        Frozen = true
      }, "State", StateDetailView.SubRoleStateTranslator.StateSelection);

      List<GridDataItem> gridDataItemList = new List<GridDataItem>();
      IList<MonitoringObject> monitoringObjects1 = ManagementGroup.EntityObjects.GetRelatedObjects<MonitoringObject>(monitoringObjectContext.Id, TestHostedRelationship, TraversalDepth.Recursive, ObjectQueryOptions.Default);


      if (monitoringObjects1.Count > 0 && definitionMonitors.Count > 0)
      {
        Dictionary<MonitoringObject, IList<MonitoringState>> stateForObjects = ManagementGroup.EntityObjects.GetStateForObjects<MonitoringObject>(monitoringObjects1, (IEnumerable<ManagementPackMonitor>)definitionMonitors);
        ManagementPackClass managementPackClass = ManagementGroup.EntityTypes.GetClass(SystemClass.Entity);
        foreach (MonitoringObject rootEntity in monitoringObjects1)
        {
          ReadOnlyCollection<PartialMonitoringObject> monitoringObjects2 = rootEntity.GetRelatedPartialMonitoringObjects(managementPackClass, TraversalDepth.OneLevel);
          gridDataItemList.Add(new GridDataItem(new StateDetailView.SubroleState(rootEntity, type, monitoringObjects2, relatedClasses, stateForObjects[rootEntity]), 0));
        }

      }
      gcTestList.DataStore = new VirtualList<GridDataItem>(new ListBackingStore(gridDataItemList), false);
      // gcTestList.HorizontalScrollingOffset = num;
      gcTestList.ResumeLayout();
    }

    protected override string NoItemSelectedMessage => "Select an item to display its properties and edit test. If no items, select 'New Destination' in the context menu or the task plane.";
  }
}
