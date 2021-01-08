using Maximus.Library.SCOMId;

using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Monitoring;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maximus.Connectivity.UI.Control
{
  public class TestObjectAdapter
  {
    private MonitoringObject entity;
    private Dictionary<string, Image> mmImages;
    private Dictionary<HealthState, Image> hsImages;
    private Dictionary<HealthState, string> hsStrings;

    public TestObjectAdapter(MonitoringObject rootEntity, Dictionary<string, Image> mainteanceModeImages, Dictionary<HealthState, Image> healthStateImages,  Dictionary<HealthState, string> healthStrings)
    {
      if (rootEntity == null)
        throw new ArgumentNullException(nameof(rootEntity));
      if (!rootEntity.IsInstanceOf(rootEntity.ManagementGroup.EntityTypes.GetClass(IDs.TestBaseClassId)))
        throw new InvalidCastException($"Monitoring object passed in the {nameof(rootEntity)} parameter must be of a child class type of 'Maximus.Connectivity.Monitoring.Test'.");
      entity = rootEntity;
      mmImages = mainteanceModeImages;
      hsImages = healthStateImages;
      hsStrings = healthStrings;
    }

    public string DisplayName => entity.DisplayName;
    public string State => hsStrings[entity.HealthState];
    public Image InMaintenanceMode => mmImages[entity.InMaintenanceMode.ToString()];
    public Image StateImage => hsImages[entity.HealthState];
    public string Summary
    {
      get
      {
        string MostDerivedProperties = "";
        string CommonProperties = $"Common settings: (Template: {entity[IDs.TestBaseClassProperties.TemplateReferencePropertyId].Value ?? "<No template>"}; Interval: {entity[IDs.TestBaseClassProperties.IntervalSecondsPropertyId].Value}s; Collect performance: {entity[IDs.TestBaseClassProperties.CollectPerformanceDataPropertyId].Value}; Alert if {entity[IDs.TestBaseClassProperties.MatchCountPropertyId].Value} out of {entity[IDs.TestBaseClassProperties.SampleCountPropertyId].Value} probes fail.)";
        foreach (ManagementPackProperty mpProperty in entity.GetProperties())
        {
          if (mpProperty.ParentElement.Id == SystemId.EntityClassId) // don't list display name
            continue;
          if (mpProperty.ParentElement.Id == IDs.TestBaseClassId) // don't list common properties, see above.
            continue;
          // don't list parent keys and own keys
          if (mpProperty.Id == IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId ||
            mpProperty.Id == IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId ||
            mpProperty.Id == IDs.TestBaseClassProperties.TestIdPropertyId)
            continue;
          MostDerivedProperties += $"{mpProperty.DisplayName ?? mpProperty.Name}: '{entity[mpProperty].Value ?? ""}'; ";
        }
        return MostDerivedProperties + CommonProperties;
      }
    }

    public MonitoringObject Source => entity;
  }
}
