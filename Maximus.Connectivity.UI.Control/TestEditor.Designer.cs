
namespace Maximus.Connectivity.UI.Control
{
  partial class TestBrowser
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      this.gcTestList = new Microsoft.EnterpriseManagement.Mom.Internal.UI.Controls.GridControl();
      ((System.ComponentModel.ISupportInitialize)(this.gcTestList)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
      this.SuspendLayout();
      // 
      // gcTestList
      // 
      this.gcTestList.AllowCollapsing = true;
      this.gcTestList.AllowUserToAddRows = false;
      this.gcTestList.AllowUserToDeleteRows = false;
      this.gcTestList.AllowUserToOrderColumns = true;
      this.gcTestList.AllowUserToResizeRows = false;
      this.gcTestList.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.gcTestList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.gcTestList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.gcTestList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.gcTestList.DataStore = null;
      this.gcTestList.FullRowContentColumn = null;
      this.gcTestList.HeaderFont = null;
      this.gcTestList.IndentDepth = 10;
      this.gcTestList.Location = new System.Drawing.Point(47, 61);
      this.gcTestList.MinusImage = null;
      this.gcTestList.Name = "gcTestList";
      this.gcTestList.PlusImage = null;
      this.gcTestList.ReadOnly = true;
      this.gcTestList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
      this.gcTestList.Size = new System.Drawing.Size(627, 254);
      this.gcTestList.TabIndex = 0;
      this.gcTestList.Translator = null;
      this.gcTestList.VirtualMode = true;
      // 
      // TestEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.gcTestList);
      this.Name = "TestEditor";
      this.Size = new System.Drawing.Size(754, 390);
      ((System.ComponentModel.ISupportInitialize)(this.gcTestList)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private Microsoft.EnterpriseManagement.Mom.Internal.UI.Controls.GridControl gcTestList;
  }
}
