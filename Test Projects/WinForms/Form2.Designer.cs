namespace WinForms
{
	partial class Form2
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
			if(disposing && (components != null)) {
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
			this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
			this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
			this.SuspendLayout();
			// 
			// objectListView1
			// 
			this.objectListView1.AllColumns.Add(this.olvColumn1);
			this.objectListView1.AllColumns.Add(this.olvColumn2);
			this.objectListView1.AllColumns.Add(this.olvColumn3);
			this.objectListView1.AllColumns.Add(this.olvColumn4);
			this.objectListView1.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
			this.objectListView1.CellEditUseWholeCell = false;
			this.objectListView1.CheckedAspectName = "Three";
			this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4});
			this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
			this.objectListView1.FullRowSelect = true;
			this.objectListView1.HeaderWordWrap = true;
			this.objectListView1.Location = new System.Drawing.Point(0, 72);
			this.objectListView1.Name = "objectListView1";
			this.objectListView1.ShowGroups = false;
			this.objectListView1.ShowImagesOnSubItems = true;
			this.objectListView1.ShowItemToolTips = true;
			this.objectListView1.Size = new System.Drawing.Size(344, 120);
			this.objectListView1.TabIndex = 1;
			this.objectListView1.UseCompatibleStateImageBehavior = false;
			this.objectListView1.UseExplorerTheme = true;
			this.objectListView1.UseHotItem = true;
			this.objectListView1.UseSubItemCheckBoxes = true;
			this.objectListView1.View = System.Windows.Forms.View.Details;
			// 
			// olvColumn1
			// 
			this.olvColumn1.AspectName = "One";
			this.olvColumn1.Text = "One";
			this.olvColumn1.Width = 200;
			this.olvColumn1.WordWrap = true;
			// 
			// olvColumn2
			// 
			this.olvColumn2.AspectName = "Two";
			this.olvColumn2.Text = "Two test wrap";
			// 
			// olvColumn3
			// 
			this.olvColumn3.AspectName = "Three";
			this.olvColumn3.CheckBoxes = true;
			this.olvColumn3.IsHeaderVertical = true;
			this.olvColumn3.Text = "Three";
			// 
			// olvColumn4
			// 
			this.olvColumn4.AspectName = "Four";
			this.olvColumn4.Text = "Four";
			// 
			// Form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(383, 261);
			this.Controls.Add(this.objectListView1);
			this.Name = "Form2";
			this.Text = "Form2";
			((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private BrightIdeasSoftware.ObjectListView objectListView1;
		private BrightIdeasSoftware.OLVColumn olvColumn1;
		private BrightIdeasSoftware.OLVColumn olvColumn2;
		private BrightIdeasSoftware.OLVColumn olvColumn3;
		private BrightIdeasSoftware.OLVColumn olvColumn4;
	}
}