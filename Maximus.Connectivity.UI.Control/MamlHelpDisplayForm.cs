using Microsoft.EnterpriseManagement.Mom.Internal.UI.Controls;

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
  public partial class MamlHelpDisplayForm : Form
  {
    public KnowledgeControl KnowledgeControl => kcTestObjectDocumentation;

    public MamlHelpDisplayForm()
    {
      InitializeComponent();
    }
  }
}
