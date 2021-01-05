using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maximus.Connectivity.UI.Control
{
  public class CreatableObjectAdapter : ICustomTypeDescriptor
  {
    public CreatableEnterpriseManagementObject BaseObject { get; }

    public CreatableObjectAdapter(CreatableEnterpriseManagementObject baseObject)
    {
      BaseObject = baseObject;
    }

    public EnterpriseManagementSimpleObject this[ManagementPackProperty managementPackProperty] =>BaseObject[managementPackProperty];

    #region ICustomTypeDescriptor implementation
    public AttributeCollection GetAttributes()
    {
      return new AttributeCollection(new Attribute[0]);
    }

    public string GetClassName() => BaseObject.GetMostDerivedClasses().FirstOrDefault()?.DisplayName ?? BaseObject.GetMostDerivedClasses().FirstOrDefault()?.Name ?? "Unknown";

    public string GetComponentName() => BaseObject.DisplayName;

    public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this, true);

    public EventDescriptor GetDefaultEvent() => null;

    public PropertyDescriptor GetDefaultProperty() => null;

    public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);

    public EventDescriptorCollection GetEvents() => new EventDescriptorCollection(new EventDescriptor[0]);

    public EventDescriptorCollection GetEvents(Attribute[] attributes) => new EventDescriptorCollection(new EventDescriptor[0]);

    public PropertyDescriptorCollection GetProperties() => new PropertyDescriptorCollection(BaseObject.GetProperties().Select(p => new CreatableObjectPropertyDescriptor(p)).ToArray());

    // no filtering
    public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();

    public object GetPropertyOwner(PropertyDescriptor pd) => this;
    #endregion
  }
}
