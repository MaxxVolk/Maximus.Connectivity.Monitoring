using Maximus.Library.SCOMId;

using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maximus.Connectivity.UI.Control
{
  public class CreatableObjectPropertyDescriptor : PropertyDescriptor
  {
    private ManagementPackProperty MPProperty { get; }

    public CreatableObjectPropertyDescriptor(ManagementPackProperty mpProperty) : base(mpProperty.DisplayName ?? mpProperty.Name, 
      new Attribute[3]
      { 
        new BrowsableAttribute(IsBrowsableProperty(mpProperty)),
        new CategoryAttribute(PropertyCategoryName(mpProperty)),
        new DescriptionAttribute(mpProperty.Description)
      })
    {
      MPProperty = mpProperty;
    }

    public CreatableObjectPropertyDescriptor(ManagementPackProperty mpProperty, Type editorType) : base(mpProperty.DisplayName ?? mpProperty.Name,
      new Attribute[4]
      {
        new BrowsableAttribute(IsBrowsableProperty(mpProperty)),
        new CategoryAttribute(PropertyCategoryName(mpProperty)),
        new DescriptionAttribute(mpProperty.Description),
        new EditorAttribute(editorType, typeof(System.Drawing.Design.UITypeEditor))
      })
    {
      MPProperty = mpProperty;
    }

    private static string PropertyCategoryName(ManagementPackProperty mpProperty)
    {
      if (mpProperty.Id == IDs.TestBaseClassProperties.TemplateReferencePropertyId)
        return "Template Settings";
      else if (mpProperty.ParentElement.Id == SystemId.EntityClassId)
        return "Generic";
      else if (mpProperty.ParentElement.Id == IDs.TestBaseClassId)
        return "Common Monitoring Settings";
      else
        return "Test Specific Parameters";
    }

    private static bool IsBrowsableProperty(ManagementPackProperty mpProperty)
    {
      if (mpProperty.Id == IDs.FullyQualifiedDomainNameClassProperties.FullyQualifiedDomainNamePropertyId) // parent key
        return false;
      if (mpProperty.Id == IDs.FullyQualifiedDomainNameClassProperties.TargetIndexPropertyId) // parent key
        return false;
      if (mpProperty.Id == IDs.TestBaseClassProperties.TestIdPropertyId) // key
        return false;
      return true;
    }

    public override Type ComponentType => typeof(CreatableObjectAdapter);

    public override bool IsReadOnly => MPProperty.Key; // not a generic implementation, usually for new objects key is editable, bu it will be assigning it programmatically

    public override Type PropertyType => MPProperty.SystemType;

    public override bool CanResetValue(object component) => !MPProperty.Key;

    public override object GetValue(object component)
    {
      if (component is CreatableObjectAdapter cmo)
      {
        if (cmo[MPProperty].Value == null && MPProperty.SystemType.IsValueType)
          return Activator.CreateInstance(MPProperty.SystemType);
        else
          return cmo[MPProperty].Value;
      }
      else
        throw new Exception("Unexpected type.");
    }

    public override void ResetValue(object component)
    {
      if (component is CreatableObjectAdapter cmo)
      {
        if (string.IsNullOrWhiteSpace(MPProperty.DefaultValue))
          cmo[MPProperty].Value = MPProperty.SystemType.IsValueType ? Activator.CreateInstance(MPProperty.SystemType) : null;
        else
          cmo[MPProperty].Value = Converter.ConvertFromString(MPProperty.DefaultValue);
      }
      else
        throw new Exception("Unexpected type");
    }

    public override void SetValue(object component, object value)
    {
      if (component is CreatableObjectAdapter cmo)
      {
        cmo[MPProperty].Value = value;
      }
      else
        throw new Exception("Unexpected type");
    }

    public override bool ShouldSerializeValue(object component) => false;
  }
}
