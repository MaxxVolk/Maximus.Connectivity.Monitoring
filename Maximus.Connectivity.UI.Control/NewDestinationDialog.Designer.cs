
namespace Maximus.Connectivity.UI.Control
{
  partial class NewDestinationDialog
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
      this.btOK = new System.Windows.Forms.Button();
      this.btCancel = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.tbDisplayName = new System.Windows.Forms.TextBox();
      this.tbFQDN = new System.Windows.Forms.TextBox();
      this.cbAllowDuplicates = new System.Windows.Forms.CheckBox();
      this.nudTargetIndex = new System.Windows.Forms.NumericUpDown();
      this.tbDescription = new System.Windows.Forms.TextBox();
      this.gbObjectData = new System.Windows.Forms.GroupBox();
      this.gbMAPDetails = new System.Windows.Forms.GroupBox();
      this.cbUnlockControls = new System.Windows.Forms.CheckBox();
      this.lMAPOutOfSyncWarning = new System.Windows.Forms.Label();
      this.tbMAPDetails = new System.Windows.Forms.TextBox();
      this.btAgentLookup = new System.Windows.Forms.Button();
      this.lbAgentList = new System.Windows.Forms.ListBox();
      this.tbAgentLookup = new System.Windows.Forms.TextBox();
      this.cbResourcePools = new System.Windows.Forms.ComboBox();
      this.rbAgent = new System.Windows.Forms.RadioButton();
      this.rbPool = new System.Windows.Forms.RadioButton();
      this.label5 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.nudTargetIndex)).BeginInit();
      this.gbObjectData.SuspendLayout();
      this.gbMAPDetails.SuspendLayout();
      this.SuspendLayout();
      // 
      // btOK
      // 
      this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btOK.Location = new System.Drawing.Point(400, 465);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(75, 23);
      this.btOK.TabIndex = 0;
      this.btOK.Text = "OK";
      this.btOK.UseVisualStyleBackColor = true;
      // 
      // btCancel
      // 
      this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btCancel.Location = new System.Drawing.Point(481, 465);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(75, 23);
      this.btCancel.TabIndex = 1;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(73, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Display name:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 55);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(139, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Fully qualified domain name:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(409, 55);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(69, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Target index:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 94);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(63, 13);
      this.label4.TabIndex = 5;
      this.label4.Text = "Description:";
      // 
      // tbDisplayName
      // 
      this.tbDisplayName.Location = new System.Drawing.Point(6, 32);
      this.tbDisplayName.Name = "tbDisplayName";
      this.tbDisplayName.Size = new System.Drawing.Size(270, 20);
      this.tbDisplayName.TabIndex = 6;
      // 
      // tbFQDN
      // 
      this.tbFQDN.Location = new System.Drawing.Point(6, 71);
      this.tbFQDN.Name = "tbFQDN";
      this.tbFQDN.Size = new System.Drawing.Size(270, 20);
      this.tbFQDN.TabIndex = 7;
      // 
      // cbAllowDuplicates
      // 
      this.cbAllowDuplicates.AutoSize = true;
      this.cbAllowDuplicates.Location = new System.Drawing.Point(282, 71);
      this.cbAllowDuplicates.Name = "cbAllowDuplicates";
      this.cbAllowDuplicates.Size = new System.Drawing.Size(102, 17);
      this.cbAllowDuplicates.TabIndex = 8;
      this.cbAllowDuplicates.Text = "Allow duplicates";
      this.cbAllowDuplicates.UseVisualStyleBackColor = true;
      this.cbAllowDuplicates.CheckedChanged += new System.EventHandler(this.cbAllowDuplicates_CheckedChanged);
      // 
      // nudTargetIndex
      // 
      this.nudTargetIndex.Enabled = false;
      this.nudTargetIndex.Location = new System.Drawing.Point(412, 71);
      this.nudTargetIndex.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
      this.nudTargetIndex.Name = "nudTargetIndex";
      this.nudTargetIndex.ReadOnly = true;
      this.nudTargetIndex.Size = new System.Drawing.Size(120, 20);
      this.nudTargetIndex.TabIndex = 9;
      // 
      // tbDescription
      // 
      this.tbDescription.Location = new System.Drawing.Point(6, 110);
      this.tbDescription.Multiline = true;
      this.tbDescription.Name = "tbDescription";
      this.tbDescription.Size = new System.Drawing.Size(270, 87);
      this.tbDescription.TabIndex = 10;
      // 
      // gbObjectData
      // 
      this.gbObjectData.Controls.Add(this.tbDescription);
      this.gbObjectData.Controls.Add(this.nudTargetIndex);
      this.gbObjectData.Controls.Add(this.label4);
      this.gbObjectData.Controls.Add(this.cbAllowDuplicates);
      this.gbObjectData.Controls.Add(this.tbDisplayName);
      this.gbObjectData.Controls.Add(this.tbFQDN);
      this.gbObjectData.Controls.Add(this.label1);
      this.gbObjectData.Controls.Add(this.label2);
      this.gbObjectData.Controls.Add(this.label3);
      this.gbObjectData.Location = new System.Drawing.Point(12, 12);
      this.gbObjectData.Name = "gbObjectData";
      this.gbObjectData.Size = new System.Drawing.Size(541, 204);
      this.gbObjectData.TabIndex = 11;
      this.gbObjectData.TabStop = false;
      this.gbObjectData.Text = "Destination object details";
      // 
      // gbMAPDetails
      // 
      this.gbMAPDetails.Controls.Add(this.cbUnlockControls);
      this.gbMAPDetails.Controls.Add(this.lMAPOutOfSyncWarning);
      this.gbMAPDetails.Controls.Add(this.tbMAPDetails);
      this.gbMAPDetails.Controls.Add(this.btAgentLookup);
      this.gbMAPDetails.Controls.Add(this.lbAgentList);
      this.gbMAPDetails.Controls.Add(this.tbAgentLookup);
      this.gbMAPDetails.Controls.Add(this.cbResourcePools);
      this.gbMAPDetails.Controls.Add(this.rbAgent);
      this.gbMAPDetails.Controls.Add(this.rbPool);
      this.gbMAPDetails.Controls.Add(this.label5);
      this.gbMAPDetails.Location = new System.Drawing.Point(12, 222);
      this.gbMAPDetails.Name = "gbMAPDetails";
      this.gbMAPDetails.Size = new System.Drawing.Size(541, 237);
      this.gbMAPDetails.TabIndex = 12;
      this.gbMAPDetails.TabStop = false;
      this.gbMAPDetails.Text = "Management action point details";
      // 
      // cbUnlockControls
      // 
      this.cbUnlockControls.AutoSize = true;
      this.cbUnlockControls.Location = new System.Drawing.Point(282, 69);
      this.cbUnlockControls.Name = "cbUnlockControls";
      this.cbUnlockControls.Size = new System.Drawing.Size(60, 17);
      this.cbUnlockControls.TabIndex = 9;
      this.cbUnlockControls.Text = "Unlock";
      this.cbUnlockControls.UseVisualStyleBackColor = true;
      this.cbUnlockControls.Visible = false;
      this.cbUnlockControls.CheckedChanged += new System.EventHandler(this.cbUnlockControls_CheckedChanged);
      // 
      // lMAPOutOfSyncWarning
      // 
      this.lMAPOutOfSyncWarning.AutoSize = true;
      this.lMAPOutOfSyncWarning.Location = new System.Drawing.Point(279, 19);
      this.lMAPOutOfSyncWarning.MaximumSize = new System.Drawing.Size(250, 0);
      this.lMAPOutOfSyncWarning.Name = "lMAPOutOfSyncWarning";
      this.lMAPOutOfSyncWarning.Size = new System.Drawing.Size(250, 39);
      this.lMAPOutOfSyncWarning.TabIndex = 8;
      this.lMAPOutOfSyncWarning.Text = "Warning: management action point is out of sync. This is normal, if action point " +
    "was changed recently. If this problem persists, use \'Unlock\' to override.";
      this.lMAPOutOfSyncWarning.Visible = false;
      // 
      // tbMAPDetails
      // 
      this.tbMAPDetails.Location = new System.Drawing.Point(6, 206);
      this.tbMAPDetails.Name = "tbMAPDetails";
      this.tbMAPDetails.ReadOnly = true;
      this.tbMAPDetails.Size = new System.Drawing.Size(270, 20);
      this.tbMAPDetails.TabIndex = 6;
      this.tbMAPDetails.TabStop = false;
      // 
      // btAgentLookup
      // 
      this.btAgentLookup.Enabled = false;
      this.btAgentLookup.Location = new System.Drawing.Point(180, 118);
      this.btAgentLookup.Name = "btAgentLookup";
      this.btAgentLookup.Size = new System.Drawing.Size(96, 23);
      this.btAgentLookup.TabIndex = 5;
      this.btAgentLookup.Text = "Find agent -->";
      this.btAgentLookup.UseVisualStyleBackColor = true;
      this.btAgentLookup.Click += new System.EventHandler(this.btAgentLookup_Click);
      // 
      // lbAgentList
      // 
      this.lbAgentList.DisplayMember = "DisplayName";
      this.lbAgentList.Enabled = false;
      this.lbAgentList.FormattingEnabled = true;
      this.lbAgentList.Location = new System.Drawing.Point(282, 92);
      this.lbAgentList.Name = "lbAgentList";
      this.lbAgentList.Size = new System.Drawing.Size(250, 134);
      this.lbAgentList.TabIndex = 4;
      this.lbAgentList.ValueMember = "Id";
      // 
      // tbAgentLookup
      // 
      this.tbAgentLookup.Enabled = false;
      this.tbAgentLookup.Location = new System.Drawing.Point(6, 92);
      this.tbAgentLookup.Name = "tbAgentLookup";
      this.tbAgentLookup.Size = new System.Drawing.Size(270, 20);
      this.tbAgentLookup.TabIndex = 3;
      // 
      // cbResourcePools
      // 
      this.cbResourcePools.DisplayMember = "DisplayName";
      this.cbResourcePools.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbResourcePools.FormattingEnabled = true;
      this.cbResourcePools.Location = new System.Drawing.Point(6, 42);
      this.cbResourcePools.Name = "cbResourcePools";
      this.cbResourcePools.Size = new System.Drawing.Size(270, 21);
      this.cbResourcePools.TabIndex = 2;
      this.cbResourcePools.ValueMember = "Id";
      // 
      // rbAgent
      // 
      this.rbAgent.AutoSize = true;
      this.rbAgent.Location = new System.Drawing.Point(6, 69);
      this.rbAgent.Name = "rbAgent";
      this.rbAgent.Size = new System.Drawing.Size(120, 17);
      this.rbAgent.TabIndex = 1;
      this.rbAgent.Text = "Use indivisual agent";
      this.rbAgent.UseVisualStyleBackColor = true;
      this.rbAgent.CheckedChanged += new System.EventHandler(this.UpdateControls);
      // 
      // rbPool
      // 
      this.rbPool.AutoSize = true;
      this.rbPool.Checked = true;
      this.rbPool.Location = new System.Drawing.Point(9, 19);
      this.rbPool.Name = "rbPool";
      this.rbPool.Size = new System.Drawing.Size(117, 17);
      this.rbPool.TabIndex = 0;
      this.rbPool.TabStop = true;
      this.rbPool.Text = "Use Resource Pool";
      this.rbPool.UseVisualStyleBackColor = true;
      this.rbPool.CheckedChanged += new System.EventHandler(this.UpdateControls);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 190);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(154, 13);
      this.label5.TabIndex = 7;
      this.label5.Text = "Selected Pool / Health Service";
      // 
      // NewDestinationDialog
      // 
      this.AcceptButton = this.btOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btCancel;
      this.ClientSize = new System.Drawing.Size(568, 502);
      this.Controls.Add(this.gbMAPDetails);
      this.Controls.Add(this.gbObjectData);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btOK);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "NewDestinationDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "New/Edit Destination";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewDestinationDialog_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.nudTargetIndex)).EndInit();
      this.gbObjectData.ResumeLayout(false);
      this.gbObjectData.PerformLayout();
      this.gbMAPDetails.ResumeLayout(false);
      this.gbMAPDetails.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btOK;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox tbDisplayName;
    private System.Windows.Forms.TextBox tbFQDN;
    private System.Windows.Forms.CheckBox cbAllowDuplicates;
    private System.Windows.Forms.NumericUpDown nudTargetIndex;
    private System.Windows.Forms.TextBox tbDescription;
    private System.Windows.Forms.GroupBox gbObjectData;
    private System.Windows.Forms.GroupBox gbMAPDetails;
    private System.Windows.Forms.Button btAgentLookup;
    private System.Windows.Forms.ListBox lbAgentList;
    private System.Windows.Forms.TextBox tbAgentLookup;
    private System.Windows.Forms.ComboBox cbResourcePools;
    private System.Windows.Forms.RadioButton rbAgent;
    private System.Windows.Forms.RadioButton rbPool;
    private System.Windows.Forms.TextBox tbMAPDetails;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.CheckBox cbUnlockControls;
    private System.Windows.Forms.Label lMAPOutOfSyncWarning;
  }
}