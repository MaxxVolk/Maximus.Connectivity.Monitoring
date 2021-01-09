
namespace Maximus.Connectivity.UI.Control
{
  partial class NewTestDialog
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
      this.pgObjectEditor = new System.Windows.Forms.PropertyGrid();
      this.pControls = new System.Windows.Forms.Panel();
      this.btCancel = new System.Windows.Forms.Button();
      this.btOK = new System.Windows.Forms.Button();
      this.pEditor = new System.Windows.Forms.Panel();
      this.pDescription = new System.Windows.Forms.Panel();
      this.rtbDescription = new System.Windows.Forms.RichTextBox();
      this.pControls.SuspendLayout();
      this.pEditor.SuspendLayout();
      this.pDescription.SuspendLayout();
      this.SuspendLayout();
      // 
      // pgObjectEditor
      // 
      this.pgObjectEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pgObjectEditor.Location = new System.Drawing.Point(0, 0);
      this.pgObjectEditor.Name = "pgObjectEditor";
      this.pgObjectEditor.Size = new System.Drawing.Size(659, 462);
      this.pgObjectEditor.TabIndex = 1;
      // 
      // pControls
      // 
      this.pControls.Controls.Add(this.btCancel);
      this.pControls.Controls.Add(this.btOK);
      this.pControls.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.pControls.Location = new System.Drawing.Point(0, 550);
      this.pControls.Name = "pControls";
      this.pControls.Size = new System.Drawing.Size(659, 48);
      this.pControls.TabIndex = 2;
      // 
      // btCancel
      // 
      this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btCancel.Location = new System.Drawing.Point(572, 13);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(75, 23);
      this.btCancel.TabIndex = 1;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      // 
      // btOK
      // 
      this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btOK.Location = new System.Drawing.Point(491, 13);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(75, 23);
      this.btOK.TabIndex = 0;
      this.btOK.Text = "OK";
      this.btOK.UseVisualStyleBackColor = true;
      // 
      // pEditor
      // 
      this.pEditor.Controls.Add(this.pgObjectEditor);
      this.pEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pEditor.Location = new System.Drawing.Point(0, 88);
      this.pEditor.Name = "pEditor";
      this.pEditor.Size = new System.Drawing.Size(659, 462);
      this.pEditor.TabIndex = 3;
      // 
      // pDescription
      // 
      this.pDescription.Controls.Add(this.rtbDescription);
      this.pDescription.Dock = System.Windows.Forms.DockStyle.Top;
      this.pDescription.Location = new System.Drawing.Point(0, 0);
      this.pDescription.Name = "pDescription";
      this.pDescription.Size = new System.Drawing.Size(659, 88);
      this.pDescription.TabIndex = 4;
      // 
      // rtbDescription
      // 
      this.rtbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rtbDescription.Location = new System.Drawing.Point(0, 0);
      this.rtbDescription.Name = "rtbDescription";
      this.rtbDescription.ReadOnly = true;
      this.rtbDescription.Size = new System.Drawing.Size(659, 88);
      this.rtbDescription.TabIndex = 0;
      this.rtbDescription.Text = "";
      // 
      // NewTestDialog
      // 
      this.AcceptButton = this.btOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btCancel;
      this.ClientSize = new System.Drawing.Size(659, 598);
      this.Controls.Add(this.pEditor);
      this.Controls.Add(this.pDescription);
      this.Controls.Add(this.pControls);
      this.MinimizeBox = false;
      this.Name = "NewTestDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "NewTestDialog";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewTestDialog_FormClosing);
      this.pControls.ResumeLayout(false);
      this.pEditor.ResumeLayout(false);
      this.pDescription.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PropertyGrid pgObjectEditor;
    private System.Windows.Forms.Panel pControls;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Button btOK;
    private System.Windows.Forms.Panel pEditor;
    private System.Windows.Forms.Panel pDescription;
    private System.Windows.Forms.RichTextBox rtbDescription;
  }
}