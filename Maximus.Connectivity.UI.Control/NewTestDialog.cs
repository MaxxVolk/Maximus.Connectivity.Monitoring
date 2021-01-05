using Maximus.Library.SCOMId;

using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using Microsoft.EnterpriseManagement.Monitoring;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maximus.Connectivity.UI.Control
{
  public partial class NewTestDialog : Form
  {
    private CreatableObjectAdapter CreatableObjectAdapter;
    private ManagementGroup ManagementGroup;

    public NewTestDialog(ManagementGroup managementGroup, ManagementPackClass managementPackClass, MonitoringObject baseDestinationObject, Guid existingTestObjectId)
    {
      InitializeComponent();
      ManagementGroup = managementGroup;
      if (existingTestObjectId == Guid.Empty)
        Text = $"New {(string.IsNullOrWhiteSpace(managementPackClass.DisplayName) ? managementPackClass.Name : managementPackClass.DisplayName)}";
      else
        Text = $"Edit {(string.IsNullOrWhiteSpace(managementPackClass.DisplayName) ? managementPackClass.Name : managementPackClass.DisplayName)}";

      CreatableEnterpriseManagementObject testObject = new CreatableEnterpriseManagementObject(managementGroup, managementPackClass);
      // bind to host object -- for both new and existing test objects
      testObject[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value = baseDestinationObject[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value; // parent key 1
      testObject[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value = baseDestinationObject[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value; // parent key 2
      // initialize or load other properties
      if (existingTestObjectId == Guid.Empty)
      {
        testObject[IDs.TestBaseClassProperties.TestIdPropertyId].Value = Guid.NewGuid();
        testObject[SystemId.EntityClassProperties.DisplayNamePropertyId].Value = string.IsNullOrWhiteSpace(managementPackClass.DisplayName) ? managementPackClass.Name : managementPackClass.DisplayName;
      }
      else
      {

      }
      CreatableObjectAdapter = new CreatableObjectAdapter(testObject);
      pgObjectEditor.SelectedObject = CreatableObjectAdapter;
    }

    private void NewTestDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.None && DialogResult == DialogResult.OK)
        try
        {
          IncrementalDiscoveryData incrementalDiscovery = new IncrementalDiscoveryData();
          incrementalDiscovery.Add(CreatableObjectAdapter.BaseObject);
          EnterpriseManagementConnector connector = ManagementGroup.ConnectorFramework.GetMonitoringConnector();
          incrementalDiscovery.Commit(connector);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Failed to create new test object.\r\nError: {ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
          e.Cancel = true;
          return;
        }
    }
  }
}
