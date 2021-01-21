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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maximus.Connectivity.UI.Control
{
  public partial class NewTestDialog : Form
  {
    private CreatableObjectAdapter CreatableObjectAdapter;
    private ManagementGroup ManagementGroup;
    private ManagementPackClass TargetClass;
    private MonitoringObject ExistingObject;

    public NewTestDialog(ManagementGroup managementGroup, ManagementPackClass managementPackClass, MonitoringObject baseDestinationObject, Guid existingTestObjectId)
    {
      InitializeComponent();
      ManagementGroup = managementGroup;

      if (existingTestObjectId == Guid.Empty)
      {
        if (baseDestinationObject == null)
          throw new ArgumentNullException(nameof(baseDestinationObject));
        TargetClass = managementPackClass ?? throw new ArgumentNullException(nameof(managementPackClass));
      }
      else
      {
        ExistingObject = ManagementGroup.EntityObjects.GetObject<MonitoringObject>(existingTestObjectId, ObjectQueryOptions.Default);
        TargetClass = ExistingObject.GetMostDerivedClasses().First();
      }
      rtbDescription.Text = TargetClass.Description;
      try
      {
        ManagementPackKnowledgeArticle kb =TargetClass.GetKnowledgeArticle(Thread.CurrentThread.CurrentUICulture);
        btDocumentation.Enabled = kb != null;
        llLearnMore.Enabled = kb != null;
      }
      catch (ObjectNotFoundException)
      {
        btDocumentation.Enabled = false;
        llLearnMore.Enabled = false;
      }

      if (existingTestObjectId == Guid.Empty)
        Text = $"New {(string.IsNullOrWhiteSpace(TargetClass.DisplayName) ? TargetClass.Name : TargetClass.DisplayName)}";
      else
        Text = $"Edit {(string.IsNullOrWhiteSpace(TargetClass.DisplayName) ? TargetClass.Name : TargetClass.DisplayName)}";

      CreatableEnterpriseManagementObject testObject = new CreatableEnterpriseManagementObject(managementGroup, TargetClass);
      // initialize or load other properties
      if (existingTestObjectId == Guid.Empty)
      {
        testObject[IDs.TestBaseClassProperties.TestIdPropertyId].Value = Guid.NewGuid();
        testObject[SystemId.EntityClassProperties.DisplayNamePropertyId].Value = string.IsNullOrWhiteSpace(TargetClass.DisplayName) ? TargetClass.Name : TargetClass.DisplayName;
        // bind to host object -- from explicit parent
        testObject[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value = baseDestinationObject[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value; // parent key 1
        testObject[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value = baseDestinationObject[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value; // parent key 2
      }
      else
      {
        // host object binding will be done automatically as a part of property copying
        foreach (ManagementPackProperty mpProperty in ExistingObject.GetProperties())
          testObject[mpProperty].Value = ExistingObject[mpProperty].Value;
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
          if (ExistingObject == null)
            incrementalDiscovery.Commit(connector); // new
          else
            incrementalDiscovery.Overwrite(connector); // update
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Failed to create new test object.\r\nError: {ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
          e.Cancel = true;
          return;
        }
    }

    private void btDocumentation_Click(object sender, EventArgs e)
    {
      using (MamlHelpDisplayForm helpForm = new MamlHelpDisplayForm())
      {
        helpForm.KnowledgeControl.KnowledgeType = Microsoft.EnterpriseManagement.Mom.Internal.UI.Controls.KnowledgeType.Any;
        helpForm.KnowledgeControl.DefaultContent = "Empty";
        helpForm.KnowledgeControl.Group = ManagementGroup;
        helpForm.KnowledgeControl.KnowledgeId = TargetClass.Id;
        helpForm.ShowDialog();
      }
    }

    private void llLearnMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      btDocumentation_Click(sender, null);
    }
  }
}
