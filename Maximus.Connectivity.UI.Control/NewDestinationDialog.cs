using Maximus.Library.Helpers;
using Maximus.Library.SCOMId;

using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using Microsoft.EnterpriseManagement.Monitoring;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Maximus.Connectivity.UI.Control
{
  public partial class NewDestinationDialog : Form
  {
    private ManagementGroup ManagementGroup;
    private Guid ExistingObjectId;
    private MonitoringObject  ExistingObject;
    private ManagementPackRelationship relResourcePoolShouldManageEntity, relHealthServiceShouldManageEntiry, relMAPManagesEntity;
    private ManagementPackClass fqdnClass, poolClass, healthServiceClass, managementServiceClass;

    public NewDestinationDialog(ManagementGroup managementGroup, Guid moId)
    {
      ManagementGroup = managementGroup;
      ExistingObjectId = moId;
      InitializeComponent();

      relResourcePoolShouldManageEntity = ManagementGroup.EntityTypes.GetRelationshipClass(SystemCenterId.ManagementActionPointShouldManageEntityRelationshipId);
      relHealthServiceShouldManageEntiry = ManagementGroup.EntityTypes.GetRelationshipClass(SystemCenterId.HealthServiceShouldManageEntityRelationshipId);
      relMAPManagesEntity = ManagementGroup.EntityTypes.GetRelationshipClass(SystemCenterId.ManagementActionPointManagesEntityRelationshipId);
      fqdnClass = ManagementGroup.EntityTypes.GetClass(IDs.FullyQualifiedDomainNameClassId);

      cbResourcePools.Items.Clear();
      poolClass = ManagementGroup.EntityTypes.GetClass(SystemCenterId.ManagementServicePoolClassId);
      healthServiceClass = ManagementGroup.EntityTypes.GetClass(SystemCenterId.HealthServiceClassId);
      managementServiceClass = ManagementGroup.EntityTypes.GetClass(SystemCenterId.ManagementServiceClassId);
      cbResourcePools.Items.AddRange(ManagementGroup.EntityObjects.GetObjectReader<MonitoringObject>(poolClass, ObjectQueryOptions.Default).ToArray());

      if (ExistingObjectId != Guid.Empty)
      {
        // object
        ExistingObject = ManagementGroup.EntityObjects.GetObject<MonitoringObject>(ExistingObjectId, ObjectQueryOptions.Default);
        tbDisplayName.Text = ExistingObject[SystemId.EntityClassProperties.DisplayNamePropertyId].Value?.ToString() ?? "";
        tbFQDN.Text = ExistingObject[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value?.ToString() ?? "";
        tbFQDN.ReadOnly = true;
        cbAllowDuplicates.Enabled = false;
        nudTargetIndex.Value = Convert.ToDecimal(ExistingObject[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value ?? 0);
        nudTargetIndex.Enabled = false;
        tbDescription.Text = ExistingObject[IDs.FullyQualifiedDomainNameClassProperties.DescriptionPropertyId].Value?.ToString() ?? "";

        // not SHOULD, but MANAGES, i.e. factual
        IEnumerable<MonitoringObject> allManagingObject = FindActionPoints(ExistingObject);
        // this is what we submit as recommendations
        IEnumerable<EnterpriseManagementRelationshipObject<EnterpriseManagementObject>> allMAPRelations = ManagementGroup.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(ExistingObject.Id, relResourcePoolShouldManageEntity, DerivedClassTraversalDepth.Recursive, TraversalDepth.OneLevel, ObjectQueryOptions.Default).Where(x => !x.IsDeleted);
        IEnumerable<EnterpriseManagementRelationshipObject<EnterpriseManagementObject>> allHSRelations = ManagementGroup.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(ExistingObject.Id, relHealthServiceShouldManageEntiry, DerivedClassTraversalDepth.Recursive, TraversalDepth.OneLevel, ObjectQueryOptions.Default).Where(x => !x.IsDeleted);
        bool outOfSync = (!allManagingObject.Any() && (allMAPRelations.Any() || allHSRelations.Any()));
        if (allManagingObject.Any())
        {
          if (allManagingObject.Count() > 1)
            MessageBox.Show($"There are more than one management action point associated with the current destination object. This may cause unpredictable results.\r\nTo fix the issue, please reassign different management action point.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Information);
          EnterpriseManagementObject managingObject = allManagingObject.First();
          outOfSync = outOfSync || (allMAPRelations.Any() && allMAPRelations.First().SourceObject.Id != managingObject.Id) || (allHSRelations.Any() && allHSRelations.First().SourceObject.Id != managingObject.Id);
          
          if (managingObject.IsInstanceOf(poolClass))
          {
            rbPool.Checked = true;
            int newIndex = -1;
            for (int idx = 0; idx < cbResourcePools.Items.Count; idx++)
              if (((MonitoringObject)cbResourcePools.Items[idx]).Id == managingObject.Id)
                newIndex = idx;
            cbResourcePools.SelectedIndex = newIndex;
          } else if (managingObject.IsInstanceOf(healthServiceClass))
          {
            rbAgent.Checked = true;
            lbAgentList.Items.Clear();
            lbAgentList.Items.Add(ManagementGroup.EntityObjects.GetObject<MonitoringObject>(managingObject.Id, ObjectQueryOptions.Default));
            lbAgentList.SelectedIndex = 0;
          }
          else
          {
            MessageBox.Show($"Unexpected result while looking for management action point.");
            rbAgent.Checked = false;
            rbPool.Checked = false;
          }
        }
        else
        {
          rbAgent.Checked = false;
          rbPool.Checked = false;
        }
        if (outOfSync)
        {
          lMAPOutOfSyncWarning.Visible = true;
          cbUnlockControls.Visible = true;
          cbUnlockControls.Checked = true;

          rbPool.Enabled = false;
          cbResourcePools.Enabled = false;
          rbAgent.Enabled = false;
          tbAgentLookup.Enabled = false;
          btAgentLookup.Enabled = false;
          lbAgentList.Enabled = false;
        }

      }
    }

    private void cbAllowDuplicates_CheckedChanged(object sender, EventArgs e)
    {
      if (cbAllowDuplicates.Checked)
        nudTargetIndex.Enabled = true;
      else
        nudTargetIndex.Enabled = false;
    }

    private void UpdateControls(object sender, EventArgs e)
    {
      if (rbAgent.Checked)
      {
        cbResourcePools.Enabled = false;
        tbAgentLookup.Enabled = true;
        btAgentLookup.Enabled = true;
        lbAgentList.Enabled = true;
      }
      if (rbPool.Checked)
      {
        cbResourcePools.Enabled = true;
        tbAgentLookup.Enabled = false;
        btAgentLookup.Enabled = false;
        lbAgentList.Enabled = false;
      }
    }

    char[] advancedExpression = new char[] { '_', '%', '\'' };

    private void btAgentLookup_Click(object sender, EventArgs e)
    {
      IObjectReader<MonitoringObject> objectReader;
      if (!string.IsNullOrWhiteSpace(tbAgentLookup.Text))
      {
        string expression;
        if (tbAgentLookup.Text.IndexOfAny(advancedExpression) >= 0)
          expression = $"DisplayName like '{tbAgentLookup.Text}'";
        else
          expression = $"DisplayName like '%{tbAgentLookup.Text}%'";
        EnterpriseManagementObjectGenericCriteria searchExpression = new EnterpriseManagementObjectGenericCriteria(expression);
        try
        {
          objectReader = ManagementGroup.EntityObjects.GetObjectReader<MonitoringObject>(searchExpression, healthServiceClass, ObjectQueryOptions.Default);
        }
        catch
        {
          objectReader = ManagementGroup.EntityObjects.GetObjectReader<MonitoringObject>(healthServiceClass, ObjectQueryOptions.Default);
        }
      }
      else
      {
        objectReader = ManagementGroup.EntityObjects.GetObjectReader<MonitoringObject>(healthServiceClass, ObjectQueryOptions.Default);
      }
      if (objectReader.Any())
        lbAgentList.DataSource = new BindingSource { DataSource = objectReader };
      else
        lbAgentList.DataSource = null;
    }

    private void cbUnlockControls_CheckedChanged(object sender, EventArgs e)
    {
      if (!cbUnlockControls.Checked)
      {
        rbPool.Enabled = true;
        cbResourcePools.Enabled = rbPool.Checked;
        rbAgent.Enabled = true;
        tbAgentLookup.Enabled = rbAgent.Checked;
        btAgentLookup.Enabled = rbAgent.Checked;
        lbAgentList.Enabled = rbAgent.Checked;
      }
    }

    private void NewDestinationDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.None && DialogResult == DialogResult.OK)
      {
        string fqdnValue = tbFQDN.Text;
        int targetIndexValue = 0;
        if (cbAllowDuplicates.Checked)
          targetIndexValue = Convert.ToInt32(nudTargetIndex.Value);
        object selectedMAP = null;
        if (rbPool.Checked)
          selectedMAP = cbResourcePools.SelectedItem;
        if (rbAgent.Checked)
          selectedMAP = lbAgentList.SelectedItem;
        if (selectedMAP is MonitoringObject mo)
          tbMAPDetails.Text = $"{mo.DisplayName ?? "N/A"} of {mo.GetType().Name}";
        else
          tbMAPDetails.Text = $"{selectedMAP?.ToString() ?? "NULL"} of {selectedMAP?.GetType().Name ?? "N/A"}";

        // basic preliminary checks
        if (string.IsNullOrWhiteSpace(fqdnValue))
        {
          MessageBox.Show("FQDN value cannot be empty.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          e.Cancel = true;
          return;
        }
        bool ipResolved;
        try
        {
          IPAddress[] ipList = Dns.GetHostAddresses(fqdnValue);
          if (ipList == null || ipList.Length == 0)
            ipResolved = false;
          else
            ipResolved = true;
        }
        catch
        {
          ipResolved = false;
        }
        if (!ipResolved)
        {
          if (MessageBox.Show("FQDN is specified, but cannot be resolved into an IP address in the current location, i.e. at the machine where SCOM Console application is running. However, the selected action point might be able to resolve it.\r\nContinue or cancel?", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
          {
            e.Cancel = true;
            return;
          }
        }
        if (selectedMAP == null)
        {
          MessageBox.Show("Select an agent or a resource pool, which is to handle tests for the destination.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          e.Cancel = true;
          return;
        }

        // Scenarios: 1) new object 2) edit object 3) move object to a new MAP/HS 4) 2 + 3
        #region New Object
        if (ExistingObjectId == Guid.Empty)
        {
          // try to find an existing object, to avoid update instead of insert/add

          if (IsFQDNClassInstanceExists(fqdnValue, targetIndexValue))
          {
            if (cbAllowDuplicates.Checked)
            {
              if (MessageBox.Show("Duplicates are allowed, but a duplicate with the same target index already exists.\r\nFind another index automatically?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
              {
                int newIndex = 0;
                while (IsFQDNClassInstanceExists(fqdnValue, newIndex))
                  newIndex++;
                targetIndexValue = newIndex;
                nudTargetIndex.Value = newIndex;
              }
              else
              {
                e.Cancel = true;
                return;
              }
            }
            else
            {
              MessageBox.Show("This destination already exists.\r\nSelect 'Allow duplicates' to create a duplicate destination, or reuse the existing destination to add tests.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
              e.Cancel = true;
              return;
            }
          }

          // finally, trying to create a new destination
          try
          {
            IncrementalDiscoveryData incrementalDiscovery = new IncrementalDiscoveryData();
            CreatableEnterpriseManagementObject newInstance = new CreatableEnterpriseManagementObject(ManagementGroup, fqdnClass);
            CreatableEnterpriseManagementRelationshipObject newSomethingShouldManageInstance = null;

            newInstance[SystemId.EntityClassProperties.DisplayNamePropertyId].Value = string.IsNullOrEmpty(tbDisplayName.Text) ? fqdnValue : tbDisplayName.Text;
            newInstance[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value = fqdnValue;
            newInstance[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value = targetIndexValue;
            newInstance[IDs.FullyQualifiedDomainNameClassProperties.DescriptionPropertyId].Value = string.IsNullOrEmpty(tbDescription.Text) ? "" : tbDescription.Text;

            if (rbAgent.Checked)
              newSomethingShouldManageInstance = new CreatableEnterpriseManagementRelationshipObject(ManagementGroup, relHealthServiceShouldManageEntiry);
            if (rbPool.Checked)
              newSomethingShouldManageInstance = new CreatableEnterpriseManagementRelationshipObject(ManagementGroup, relResourcePoolShouldManageEntity);
            newSomethingShouldManageInstance?.SetTarget(newInstance);
            newSomethingShouldManageInstance?.SetSource(selectedMAP as MonitoringObject);

            incrementalDiscovery.Add(newInstance);
            if (newSomethingShouldManageInstance != null)
              incrementalDiscovery.Add(newSomethingShouldManageInstance);

            EnterpriseManagementConnector connector = ManagementGroup.ConnectorFramework.GetMonitoringConnector();
            incrementalDiscovery.Commit(connector);
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Failed to create new destination.\r\nError: {ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.Cancel = true;
            return;
          }

          // END SCENARIO #1
          return;
        }
        #endregion
        #region Update Object details
        if (ExistingObject != null)
        {
          bool objectChanged = false;
          objectChanged = objectChanged || (ExistingObject[SystemId.EntityClassProperties.DisplayNamePropertyId].Value?.ToString() != tbDisplayName.Text);
          objectChanged = objectChanged || (ExistingObject[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value?.ToString() != fqdnValue);
          objectChanged = objectChanged || (ExistingObject[IDs.FullyQualifiedDomainNameClassProperties.DescriptionPropertyId].Value?.ToString() != tbDescription.Text);
          // TargetIndex cannot be changed for existing objects
          if (objectChanged)
            try
            {
              IncrementalDiscoveryData incrementalDiscovery = new IncrementalDiscoveryData();
              CreatableEnterpriseManagementObject modifiedInstance = new CreatableEnterpriseManagementObject(ManagementGroup, fqdnClass);

              modifiedInstance[SystemId.EntityClassProperties.DisplayNamePropertyId].Value = string.IsNullOrEmpty(tbDisplayName.Text) ? "" : tbDisplayName.Text;
              modifiedInstance[IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId].Value = fqdnValue;
              modifiedInstance[IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId].Value = targetIndexValue;
              modifiedInstance[IDs.FullyQualifiedDomainNameClassProperties.DescriptionPropertyId].Value = string.IsNullOrEmpty(tbDescription.Text) ? "" : tbDescription.Text;

              incrementalDiscovery.Add(modifiedInstance);
              EnterpriseManagementConnector connector = ManagementGroup.ConnectorFramework.GetMonitoringConnector();
              incrementalDiscovery.Overwrite(connector);
            }
            catch (Exception ex)
            {
              MessageBox.Show($"Failed to update the destination details.\r\nError: {ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
              e.Cancel = true;
              return;
            }
        }
        #endregion
        #region Move object servicing point
        if (ExistingObject != null)
        {
          IEnumerable<MonitoringObject> currentActionPoints = FindActionPoints(ExistingObject);
          if (currentActionPoints.Any() && HasMAPChanged(currentActionPoints.First()))
            try
            {
              MonitoringObject newMAP = selectedMAP as MonitoringObject;
              // remove all existing SHOULD-relations
              EnterpriseManagementConnector connector = ManagementGroup.ConnectorFramework.GetMonitoringConnector();
              IncrementalDiscoveryData RemovalDiscovery = new IncrementalDiscoveryData();
              bool commitOverwrite = false;
              IEnumerable<EnterpriseManagementRelationshipObject<EnterpriseManagementObject>> allMAPRelations = ManagementGroup.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(ExistingObject.Id, relResourcePoolShouldManageEntity, DerivedClassTraversalDepth.Recursive, TraversalDepth.OneLevel, ObjectQueryOptions.Default).Where(x => !x.IsDeleted);
              IEnumerable<EnterpriseManagementRelationshipObject<EnterpriseManagementObject>> allHSRelations = ManagementGroup.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(ExistingObject.Id, relHealthServiceShouldManageEntiry, DerivedClassTraversalDepth.Recursive, TraversalDepth.OneLevel, ObjectQueryOptions.Default).Where(x => !x.IsDeleted);
              foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in allMAPRelations)
              {
                if (rel.SourceObject.Id != newMAP.Id)
                {
                  // remove this relationship
                  RemovalDiscovery.Remove(rel);
                  commitOverwrite = true;
                }
              }
              foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in allHSRelations)
              {
                if (rel.SourceObject.Id != newMAP.Id)
                {
                  // remove this relationship
                  RemovalDiscovery.Remove(rel);
                  commitOverwrite = true;
                }
              }
              if (commitOverwrite)
                RemovalDiscovery.Overwrite(connector);

              // add the new relationship
              IncrementalDiscoveryData incrementalDiscovery = new IncrementalDiscoveryData();
              CreatableEnterpriseManagementRelationshipObject newSomethingShouldManageInstance = null;
              if (rbAgent.Checked)
                newSomethingShouldManageInstance = new CreatableEnterpriseManagementRelationshipObject(ManagementGroup, relHealthServiceShouldManageEntiry);
              if (rbPool.Checked)
                newSomethingShouldManageInstance = new CreatableEnterpriseManagementRelationshipObject(ManagementGroup, relResourcePoolShouldManageEntity);
              newSomethingShouldManageInstance?.SetTarget(ExistingObject);
              newSomethingShouldManageInstance?.SetSource(newMAP);

              if (newSomethingShouldManageInstance != null)
                incrementalDiscovery.Add(newSomethingShouldManageInstance);

              incrementalDiscovery.Commit(connector);
            }
            catch (Exception ex)
            {
              MessageBox.Show($"Failed to update destination's management action point.\r\nError: {ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
              e.Cancel = true;
              return;
            }
        }
        #endregion
      }
    }

    private bool HasMAPChanged(MonitoringObject mapObject)
    {
      if (mapObject == null)
        return true;
      if (mapObject.IsInstanceOf(poolClass))
      {
        if (rbAgent.Checked) // current MAP type is POOL, but AGENT is selected
          return true;
        if (cbResourcePools.SelectedItem == null)
          return false;
        if (((MonitoringObject)cbResourcePools.SelectedItem).Id == mapObject.Id)
          return false;
        else
          return true;
      }
      if (mapObject.IsInstanceOf(healthServiceClass))
      {
        if (rbPool.Checked) // current MAP type is AGENT, but POOL is selected
          return true;
        if (lbAgentList.SelectedItem == null)
          return false;
        if (((MonitoringObject)lbAgentList.SelectedItem).Id == mapObject.Id)
          return false;
        else
          return true;
      }

      return false;
    }

    private bool IsFQDNClassInstanceExists(string fqdnValue, int targetIndexValue)
    {
      try
      {
        if (ManagementGroup.EntityObjects.GetObjectInstance<MonitoringObject>(IDs.FullyQualifiedDomainNameClassId, new Dictionary<Guid, object>
            {
              { IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId, fqdnValue.ToUpperInvariant() },
              { IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId, targetIndexValue }
            }, ObjectQueryOptions.Default) == null)
          return false;
        else
          return true;
      }
      catch (ObjectNotFoundException)
      {
        return false;
      }
    }

    private IEnumerable<MonitoringObject> FindActionPoints(MonitoringObject servedInstance) => servedInstance.ManagementGroup.EntityObjects.GetRelationshipObjectsWhereTarget<MonitoringObject>(servedInstance.Id, relMAPManagesEntity, DerivedClassTraversalDepth.Recursive, TraversalDepth.OneLevel, ObjectQueryOptions.Default)
        .Where(x => !x.IsDeleted)
        .Select(a=> 
        { 
          if (a.SourceObject.IsInstanceOf(managementServiceClass))
            return ManagementGroup.EntityObjects.GetObject<MonitoringObject>((Guid)a.SourceObject[SystemCenterId.ManagementServiceClassProperties.HealthServiceIdPropertyId].Value, ObjectQueryOptions.Default);
          else
            return a.SourceObject;
        });
  }
}
