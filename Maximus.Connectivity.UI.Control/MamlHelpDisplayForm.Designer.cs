
namespace Maximus.Connectivity.UI.Control
{
  partial class MamlHelpDisplayForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.kcTestObjectDocumentation = new Microsoft.EnterpriseManagement.Mom.Internal.UI.Controls.KnowledgeControl();
      this.SuspendLayout();
      // 
      // kcTestObjectDocumentation
      // 
      this.kcTestObjectDocumentation.AllowWebBrowserDrop = false;
      this.kcTestObjectDocumentation.ArticleManagementPack = null;
      this.kcTestObjectDocumentation.DefaultContent = "";
      this.kcTestObjectDocumentation.Dock = System.Windows.Forms.DockStyle.Fill;
      this.kcTestObjectDocumentation.DoItNowEnabled = true;
      this.kcTestObjectDocumentation.EnableCustomHandlers = true;
      this.kcTestObjectDocumentation.Entity = null;
      this.kcTestObjectDocumentation.Group = null;
      this.kcTestObjectDocumentation.IsWebBrowserContextMenuEnabled = false;
      this.kcTestObjectDocumentation.KnowledgeId = new System.Guid("00000000-0000-0000-0000-000000000000");
      this.kcTestObjectDocumentation.KnowledgeType = Microsoft.EnterpriseManagement.Mom.Internal.UI.Controls.KnowledgeType.Any;
      this.kcTestObjectDocumentation.Location = new System.Drawing.Point(0, 0);
      this.kcTestObjectDocumentation.MinimumSize = new System.Drawing.Size(20, 20);
      this.kcTestObjectDocumentation.Name = "kcTestObjectDocumentation";
      this.kcTestObjectDocumentation.Size = new System.Drawing.Size(800, 450);
      this.kcTestObjectDocumentation.TabIndex = 0;
      // 
      // MamlHelpDisplayForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.kcTestObjectDocumentation);
      this.Name = "MamlHelpDisplayForm";
      this.Text = "MamlHelpDisplayForm";
      this.ResumeLayout(false);

    }

    #endregion

    private Microsoft.EnterpriseManagement.Mom.Internal.UI.Controls.KnowledgeControl kcTestObjectDocumentation;
  }
}