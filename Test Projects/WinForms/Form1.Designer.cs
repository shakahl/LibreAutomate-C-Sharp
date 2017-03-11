namespace WinForms
{
	partial class Form1
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
			this.components = new System.ComponentModel.Container();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.treeListView1 = new BrightIdeasSoftware.TreeListView();
			this.treeListView2 = new BrightIdeasSoftware.TreeListView();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.treeListView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.treeListView2)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Location = new System.Drawing.Point(0, 32);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.treeListView1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.treeListView2);
			this.splitContainer1.Size = new System.Drawing.Size(816, 560);
			this.splitContainer1.SplitterDistance = 389;
			this.splitContainer1.TabIndex = 1;
			// 
			// treeListView1
			// 
			this.treeListView1.CellEditUseWholeCell = false;
			this.treeListView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeListView1.Location = new System.Drawing.Point(0, 0);
			this.treeListView1.Name = "treeListView1";
			this.treeListView1.ShowGroups = false;
			this.treeListView1.Size = new System.Drawing.Size(389, 560);
			this.treeListView1.TabIndex = 0;
			this.treeListView1.UseCompatibleStateImageBehavior = false;
			this.treeListView1.View = System.Windows.Forms.View.Details;
			this.treeListView1.VirtualMode = true;
			// 
			// treeListView2
			// 
			this.treeListView2.CellEditUseWholeCell = false;
			this.treeListView2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeListView2.Location = new System.Drawing.Point(0, 0);
			this.treeListView2.Name = "treeListView2";
			this.treeListView2.ShowGroups = false;
			this.treeListView2.Size = new System.Drawing.Size(423, 560);
			this.treeListView2.TabIndex = 0;
			this.treeListView2.UseCompatibleStateImageBehavior = false;
			this.treeListView2.View = System.Windows.Forms.View.Details;
			this.treeListView2.VirtualMode = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(816, 727);
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "Form1";
			this.Text = "Form1";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.treeListView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.treeListView2)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.SplitContainer splitContainer1;
		private BrightIdeasSoftware.TreeListView treeListView1;
		private BrightIdeasSoftware.TreeListView treeListView2;
	}
}

