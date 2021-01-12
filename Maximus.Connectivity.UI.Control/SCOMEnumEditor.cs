using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Configuration;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Maximus.Connectivity.UI.Control
{
  public class SCOMEnumEditor : UITypeEditor
  {
    SCOMEnumListBox EnumValuesComboBox = null;
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }

    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      if (provider != null)
      {
        IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
        if (service != null)
        {
          if (EnumValuesComboBox == null)
          {
            EnumValuesComboBox = new SCOMEnumListBox();
            if ((context.PropertyDescriptor) is CreatableObjectPropertyDescriptor creatableObjectAdapter && creatableObjectAdapter.MPProperty.SystemType == typeof(Enum))
            {
              EnterpriseManagementGroup mg = creatableObjectAdapter.MPProperty.ManagementGroup;
              IList<ManagementPackEnumeration> enumMemebers = mg.EntityTypes.GetChildEnumerations(creatableObjectAdapter.MPProperty.EnumType.Id, Microsoft.EnterpriseManagement.Common.TraversalDepth.OneLevel);
              EnumValuesComboBox.DisplayMember = "FullDisplayName";
              EnumValuesComboBox.ValueMember = "Id";
              EnumValuesComboBox.Items.AddRange(enumMemebers.Select(e=>new ManagementPackEnumerationDisplayWrapper(e)).ToArray());
            }
          }
          EnumValuesComboBox.Begin(service);
          service.DropDownControl(EnumValuesComboBox);

          return (ManagementPackEnumerationDisplayWrapper)EnumValuesComboBox.SelectedItem;
        }
      }
      return value;
    }
  }

  public class SCOMEnumListBox : ListBox
  {
    private IWindowsFormsEditorService svc;

    public SCOMEnumListBox()
    {
      Click += SCOMEnumListBox_Click;
    }

    private void SCOMEnumListBox_Click(object sender, EventArgs e)
    {
      svc.CloseDropDown();
    }

    public void Begin(IWindowsFormsEditorService service)
    {
      svc = service;
    }
  }

  public class ManagementPackEnumerationDisplayWrapper
  {
    public ManagementPackEnumeration ManagementPackEnumeration;

    public ManagementPackEnumerationDisplayWrapper(ManagementPackEnumeration mpEnum)
    {
      ManagementPackEnumeration = mpEnum;
    }

    public override string ToString()
    {
      return ManagementPackEnumeration.DisplayName ?? ManagementPackEnumeration.Name;
    }

    public string FullDisplayName => $"{ManagementPackEnumeration.DisplayName ?? ManagementPackEnumeration.Name} ({ManagementPackEnumeration.Description ?? ""})";

    public Guid Id => ManagementPackEnumeration.Id;
  }
}
