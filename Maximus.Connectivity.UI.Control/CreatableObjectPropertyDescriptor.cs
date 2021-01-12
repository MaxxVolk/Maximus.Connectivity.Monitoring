using Maximus.Library.SCOMId;

using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maximus.Connectivity.UI.Control
{
  public class CreatableObjectPropertyDescriptor : PropertyDescriptor
  {
    public ManagementPackProperty MPProperty { get; }

    public CreatableObjectPropertyDescriptor(ManagementPackProperty mpProperty) : base(mpProperty.DisplayName ?? mpProperty.Name, GetAttibuteList(mpProperty))
    {
      MPProperty = mpProperty;
    }

    public CreatableObjectPropertyDescriptor(ManagementPackProperty mpProperty, Type editorType) : base(mpProperty.DisplayName ?? mpProperty.Name, GetAttibuteList(mpProperty, editorType))
    {
      MPProperty = mpProperty;
    }

    private static Attribute[] GetAttibuteList(ManagementPackProperty mpProperty, Type editorType = null)
    {
      List<Attribute> result = new List<Attribute>
      {
        new BrowsableAttribute(IsBrowsableProperty(mpProperty)),
        new CategoryAttribute(PropertyCategoryName(mpProperty)),
        new DescriptionAttribute(mpProperty.Description)
      };
      if (editorType != null)
        result.Add(new EditorAttribute(editorType, typeof(System.Drawing.Design.UITypeEditor)));
      if (mpProperty.SystemType == typeof(Enum))
      {
        result.Add(new EditorAttribute(typeof(SCOMEnumEditor), typeof(UITypeEditor)));
        //result.Add(new TypeConverterAttribute(typeof(SCOMEnumConverter)));
      }
        
      return result.ToArray();
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

    public override Type PropertyType
    {
      get
      {
        if (MPProperty.SystemType == typeof(Enum))
          return typeof(ManagementPackEnumerationDisplayWrapper);
        else
          return MPProperty.SystemType;
      }
      
    }

    public override bool CanResetValue(object component)
    {
      if (MPProperty.SystemType == typeof(Enum))
        return false;
      return !MPProperty.Key;
    }
    

    public override object GetValue(object component)
    {
      if (component is CreatableObjectAdapter cmo)
      {
        if (MPProperty.SystemType == typeof(Enum))
        {
          if (cmo[MPProperty].Value == null)
            return null;
          return new ManagementPackEnumerationDisplayWrapper((ManagementPackEnumeration)cmo[MPProperty].Value);
        }

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
        if (MPProperty.SystemType == typeof(Enum) && value is ManagementPackEnumerationDisplayWrapper mpEnumValue)
        {
          cmo[MPProperty].Value = mpEnumValue.ManagementPackEnumeration;
          // MessageBox.Show($"{value.GetType().Name}");

          //EnterpriseManagementSimpleObject simpleValue = cmo[MPProperty];
          //EnterpriseManagementGroup mg = MPProperty.ManagementGroup;
          //ManagementPackEnumeration enumValue = mg.EntityTypes.GetChildEnumerations(MPProperty.EnumType.Id, TraversalDepth.OneLevel).Where(e=>(e.DisplayName ?? e.Name)== enumStringValue).FirstOrDefault();

          //MessageBox.Show($"{enumValue?.Name ?? "enumValue?.Name == NULL"} / {enumValue?.Ordinal?.ToString() ?? "enumValue?.Ordinal == null"}: {enumStringValue}; {simpleValue?.Type?.Name}");
          //simpleValue.Value = (ManagementPackEnumeration)value;
        }
        else
          cmo[MPProperty].Value = value;
      }
      else
        throw new Exception("Unexpected type");
    }

    public override bool ShouldSerializeValue(object component) => false;
  }
}
