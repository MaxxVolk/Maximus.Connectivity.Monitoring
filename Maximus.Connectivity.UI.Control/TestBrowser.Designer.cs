
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
      this.components = new System.ComponentModel.Container();
      this.bdgvTestList = new System.Windows.Forms.DataGridView();
      this.cStateImage = new System.Windows.Forms.DataGridViewImageColumn();
      this.cState = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cInMaintenanceMode = new System.Windows.Forms.DataGridViewImageColumn();
      this.cDisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cSummary = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cmsTestOperations = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.smiEditTest = new System.Windows.Forms.ToolStripMenuItem();
      this.smiDeleteTest = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.smiRefresh = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.bdgvTestList)).BeginInit();
      this.cmsTestOperations.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
      this.SuspendLayout();
      // 
      // bdgvTestList
      // 
      this.bdgvTestList.AllowUserToAddRows = false;
      this.bdgvTestList.AllowUserToDeleteRows = false;
      this.bdgvTestList.AllowUserToResizeRows = false;
      this.bdgvTestList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
      this.bdgvTestList.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
      this.bdgvTestList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.bdgvTestList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cStateImage,
            this.cState,
            this.cInMaintenanceMode,
            this.cDisplayName,
            this.cSummary});
      this.bdgvTestList.ContextMenuStrip = this.cmsTestOperations;
      this.bdgvTestList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.bdgvTestList.Location = new System.Drawing.Point(0, 0);
      this.bdgvTestList.Name = "bdgvTestList";
      this.bdgvTestList.ReadOnly = true;
      this.bdgvTestList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
      this.bdgvTestList.RowHeadersVisible = false;
      this.bdgvTestList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.bdgvTestList.Size = new System.Drawing.Size(1125, 390);
      this.bdgvTestList.TabIndex = 0;
      this.bdgvTestList.SelectionChanged += new System.EventHandler(this.bdgvTestList_SelectionChanged);
      // 
      // cStateImage
      // 
      this.cStateImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.cStateImage.DataPropertyName = "StateImage";
      this.cStateImage.FillWeight = 25F;
      this.cStateImage.HeaderText = "";
      this.cStateImage.Name = "cStateImage";
      this.cStateImage.ReadOnly = true;
      this.cStateImage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.cStateImage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
      this.cStateImage.Width = 25;
      // 
      // cState
      // 
      this.cState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.cState.DataPropertyName = "State";
      this.cState.HeaderText = "State";
      this.cState.Name = "cState";
      this.cState.ReadOnly = true;
      this.cState.Width = 120;
      // 
      // cInMaintenanceMode
      // 
      this.cInMaintenanceMode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.cInMaintenanceMode.DataPropertyName = "InMaintenanceMode";
      this.cInMaintenanceMode.HeaderText = "";
      this.cInMaintenanceMode.Name = "cInMaintenanceMode";
      this.cInMaintenanceMode.ReadOnly = true;
      this.cInMaintenanceMode.Width = 25;
      // 
      // cDisplayName
      // 
      this.cDisplayName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.cDisplayName.DataPropertyName = "DisplayName";
      this.cDisplayName.FillWeight = 50F;
      this.cDisplayName.HeaderText = "Name";
      this.cDisplayName.Name = "cDisplayName";
      this.cDisplayName.ReadOnly = true;
      // 
      // cSummary
      // 
      this.cSummary.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.cSummary.DataPropertyName = "Summary";
      this.cSummary.HeaderText = "Test Summary";
      this.cSummary.Name = "cSummary";
      this.cSummary.ReadOnly = true;
      // 
      // cmsTestOperations
      // 
      this.cmsTestOperations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smiEditTest,
            this.smiDeleteTest,
            this.toolStripSeparator1,
            this.smiRefresh});
      this.cmsTestOperations.Name = "cmsTestOperations";
      this.cmsTestOperations.Size = new System.Drawing.Size(181, 98);
      // 
      // smiEditTest
      // 
      this.smiEditTest.Name = "smiEditTest";
      this.smiEditTest.Size = new System.Drawing.Size(180, 22);
      this.smiEditTest.Text = "Edit Test";
      this.smiEditTest.Click += new System.EventHandler(this.smiEditTest_Click);
      // 
      // smiDeleteTest
      // 
      this.smiDeleteTest.Name = "smiDeleteTest";
      this.smiDeleteTest.Size = new System.Drawing.Size(180, 22);
      this.smiDeleteTest.Text = "Delte Test";
      this.smiDeleteTest.Click += new System.EventHandler(this.smiDeleteTest_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
      // 
      // smiRefresh
      // 
      this.smiRefresh.Name = "smiRefresh";
      this.smiRefresh.Size = new System.Drawing.Size(180, 22);
      this.smiRefresh.Text = "Refresh";
      this.smiRefresh.Click += new System.EventHandler(this.smiRefresh_Click);
      // 
      // TestBrowser
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.bdgvTestList);
      this.Name = "TestBrowser";
      this.Size = new System.Drawing.Size(1125, 390);
      ((System.ComponentModel.ISupportInitialize)(this.bdgvTestList)).EndInit();
      this.cmsTestOperations.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView bdgvTestList;
    private System.Windows.Forms.DataGridViewImageColumn cStateImage;
    private System.Windows.Forms.DataGridViewTextBoxColumn cState;
    private System.Windows.Forms.DataGridViewImageColumn cInMaintenanceMode;
    private System.Windows.Forms.DataGridViewTextBoxColumn cDisplayName;
    private System.Windows.Forms.DataGridViewTextBoxColumn cSummary;
    private System.Windows.Forms.ContextMenuStrip cmsTestOperations;
    private System.Windows.Forms.ToolStripMenuItem smiEditTest;
    private System.Windows.Forms.ToolStripMenuItem smiDeleteTest;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem smiRefresh;
  }
}
