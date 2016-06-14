namespace Map_editor
{
    partial class ResourceCollector
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourceCollector));
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("");
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tlsAddress = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateCollisionMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imgTextures = new System.Windows.Forms.ImageList(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ctxFolders = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.stProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tlsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.lstFiles = new System.Windows.Forms.ListView();
            this.fileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bcgMeshBuilder = new System.ComponentModel.BackgroundWorker();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.ctxFolders.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tlsAddress,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripButton2});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(527, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(52, 22);
            this.toolStripLabel1.Text = "Address:";
            // 
            // tlsAddress
            // 
            this.tlsAddress.Name = "tlsAddress";
            this.tlsAddress.Size = new System.Drawing.Size(300, 25);
            this.tlsAddress.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tlsAddress_KeyUp);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.rebuildToolStripMenuItem,
            this.generateCollisionMeshToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(201, 114);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // rebuildToolStripMenuItem
            // 
            this.rebuildToolStripMenuItem.Name = "rebuildToolStripMenuItem";
            this.rebuildToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.rebuildToolStripMenuItem.Text = "Build";
            this.rebuildToolStripMenuItem.Click += new System.EventHandler(this.rebuildToolStripMenuItem_Click);
            // 
            // generateCollisionMeshToolStripMenuItem
            // 
            this.generateCollisionMeshToolStripMenuItem.Name = "generateCollisionMeshToolStripMenuItem";
            this.generateCollisionMeshToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.generateCollisionMeshToolStripMenuItem.Text = "Generate collision mesh";
            this.generateCollisionMeshToolStripMenuItem.Click += new System.EventHandler(this.generateCollisionMeshToolStripMenuItem_Click);
            // 
            // imgTextures
            // 
            this.imgTextures.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgTextures.ImageSize = new System.Drawing.Size(64, 64);
            this.imgTextures.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "texture.bmp";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Multiselect = true;
            // 
            // ctxFolders
            // 
            this.ctxFolders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem1,
            this.newFolderToolStripMenuItem});
            this.ctxFolders.Name = "ctxFolders";
            this.ctxFolders.Size = new System.Drawing.Size(142, 48);
            // 
            // removeToolStripMenuItem1
            // 
            this.removeToolStripMenuItem1.Name = "removeToolStripMenuItem1";
            this.removeToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeToolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
            this.removeToolStripMenuItem1.Text = "Remove";
            this.removeToolStripMenuItem1.Click += new System.EventHandler(this.removeToolStripMenuItem1_Click);
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.newFolderToolStripMenuItem.Text = "New folder";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stProgress,
            this.tlsStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 380);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(527, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // stProgress
            // 
            this.stProgress.Name = "stProgress";
            this.stProgress.Size = new System.Drawing.Size(200, 16);
            this.stProgress.Value = 100;
            // 
            // tlsStatus
            // 
            this.tlsStatus.Name = "tlsStatus";
            this.tlsStatus.Size = new System.Drawing.Size(48, 17);
            this.tlsStatus.Text = "Waiting";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstFiles);
            this.splitContainer1.Size = new System.Drawing.Size(527, 355);
            this.splitContainer1.SplitterDistance = 143;
            this.splitContainer1.TabIndex = 4;
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.ctxFolders;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode2.Name = "Node0";
            treeNode2.Text = "";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.treeView1.PathSeparator = "/";
            this.treeView1.Size = new System.Drawing.Size(143, 355);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick_1);
            // 
            // lstFiles
            // 
            this.lstFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileName,
            this.Size});
            this.lstFiles.ContextMenuStrip = this.contextMenuStrip1;
            this.lstFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFiles.Location = new System.Drawing.Point(0, 0);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(380, 355);
            this.lstFiles.TabIndex = 0;
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            this.lstFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstFiles_MouseDoubleClick_1);
            // 
            // fileName
            // 
            this.fileName.Text = "Name";
            this.fileName.Width = 200;
            // 
            // Size
            // 
            this.Size.Text = "Size";
            this.Size.Width = 100;
            // 
            // bcgMeshBuilder
            // 
            this.bcgMeshBuilder.WorkerReportsProgress = true;
            this.bcgMeshBuilder.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bcgMeshBuilder_DoWork);
            this.bcgMeshBuilder.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bcgMeshBuilder_ProgressChanged);
            this.bcgMeshBuilder.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bcgMeshBuilder_RunWorkerCompleted);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Export resource file";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // ResourceCollector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 402);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip2);
            this.Name = "ResourceCollector";
            this.Text = "Resource collection";
            this.Load += new System.EventHandler(this.ResourceCollection_Load);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ctxFolders.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ImageList imgTextures;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rebuildToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox tlsAddress;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ContextMenuStrip ctxFolders;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ListView lstFiles;
        private System.Windows.Forms.ColumnHeader fileName;
        private System.Windows.Forms.ColumnHeader Size;
        private System.Windows.Forms.ToolStripProgressBar stProgress;
        private System.Windows.Forms.ToolStripStatusLabel tlsStatus;
        private System.ComponentModel.BackgroundWorker bcgMeshBuilder;
        private System.Windows.Forms.ToolStripMenuItem generateCollisionMeshToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}