namespace WebservicesSage
{
    partial class MappingExpeditionMode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MappingExpeditionMode));
            this.Header = new System.Windows.Forms.Panel();
            this.CloseButton = new WindowsFormsControlLibrary1.CustomImageButto();
            this.bunifuCustomLabel1 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.DefaultExpeditinMode = new System.Windows.Forms.Panel();
            this.DefaultExpedition = new Bunifu.Framework.UI.BunifuDropdown();
            this.bunifuCustomLabel2 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.ExpeditionOrderDataGrid = new System.Windows.Forms.Panel();
            this.bunifuCustomLabel8 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.ExpeditionSage1 = new Bunifu.Framework.UI.BunifuDropdown();
            this.ExpeditionSage2 = new Bunifu.Framework.UI.BunifuDropdown();
            this.ExpeditionSage3 = new Bunifu.Framework.UI.BunifuDropdown();
            this.bunifuCustomLabel6 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.bunifuCustomLabel5 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.bunifuCustomLabel4 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.bunifuCustomLabel3 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.SaveExpidationMode = new System.Windows.Forms.Button();
            this.CancelExpéditionMode = new System.Windows.Forms.Button();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.Header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloseButton)).BeginInit();
            this.DefaultExpeditinMode.SuspendLayout();
            this.ExpeditionOrderDataGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // Header
            // 
            this.Header.Controls.Add(this.CloseButton);
            this.Header.Controls.Add(this.bunifuCustomLabel1);
            this.Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.Header.Location = new System.Drawing.Point(0, 0);
            this.Header.Name = "Header";
            this.Header.Size = new System.Drawing.Size(808, 79);
            this.Header.TabIndex = 0;
            // 
            // CloseButton
            // 
            this.CloseButton.Image = ((System.Drawing.Image)(resources.GetObject("CloseButton.Image")));
            this.CloseButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("CloseButton.ImageHover")));
            this.CloseButton.ImageNormale = ((System.Drawing.Image)(resources.GetObject("CloseButton.ImageNormale")));
            this.CloseButton.Location = new System.Drawing.Point(757, 12);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(28, 32);
            this.CloseButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CloseButton.TabIndex = 2;
            this.CloseButton.TabStop = false;
            this.CloseButton.Click += new System.EventHandler(this.MappingExpeditionModeCloseButton_Click);
            // 
            // bunifuCustomLabel1
            // 
            this.bunifuCustomLabel1.AutoSize = true;
            this.bunifuCustomLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuCustomLabel1.Location = new System.Drawing.Point(12, 30);
            this.bunifuCustomLabel1.Name = "bunifuCustomLabel1";
            this.bunifuCustomLabel1.Size = new System.Drawing.Size(305, 25);
            this.bunifuCustomLabel1.TabIndex = 0;
            this.bunifuCustomLabel1.Text = "Mappage des modes d\'expédition";
            // 
            // DefaultExpeditinMode
            // 
            this.DefaultExpeditinMode.Controls.Add(this.DefaultExpedition);
            this.DefaultExpeditinMode.Controls.Add(this.bunifuCustomLabel2);
            this.DefaultExpeditinMode.Dock = System.Windows.Forms.DockStyle.Top;
            this.DefaultExpeditinMode.Location = new System.Drawing.Point(0, 79);
            this.DefaultExpeditinMode.Name = "DefaultExpeditinMode";
            this.DefaultExpeditinMode.Size = new System.Drawing.Size(808, 77);
            this.DefaultExpeditinMode.TabIndex = 1;
            // 
            // DefaultExpedition
            // 
            this.DefaultExpedition.BackColor = System.Drawing.Color.Transparent;
            this.DefaultExpedition.BorderRadius = 3;
            this.DefaultExpedition.DisabledColor = System.Drawing.Color.Gray;
            this.DefaultExpedition.ForeColor = System.Drawing.Color.White;
            this.DefaultExpedition.Items = new string[0];
            this.DefaultExpedition.Location = new System.Drawing.Point(424, 17);
            this.DefaultExpedition.Name = "DefaultExpedition";
            this.DefaultExpedition.NomalColor = System.Drawing.Color.Gainsboro;
            this.DefaultExpedition.onHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.DefaultExpedition.selectedIndex = -1;
            this.DefaultExpedition.Size = new System.Drawing.Size(277, 35);
            this.DefaultExpedition.TabIndex = 1;
            // 
            // bunifuCustomLabel2
            // 
            this.bunifuCustomLabel2.AutoSize = true;
            this.bunifuCustomLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuCustomLabel2.Location = new System.Drawing.Point(13, 32);
            this.bunifuCustomLabel2.Name = "bunifuCustomLabel2";
            this.bunifuCustomLabel2.Size = new System.Drawing.Size(226, 20);
            this.bunifuCustomLabel2.TabIndex = 0;
            this.bunifuCustomLabel2.Text = "Mode d\'expédition par defaut : ";
            // 
            // ExpeditionOrderDataGrid
            // 
            this.ExpeditionOrderDataGrid.AutoScroll = true;
            this.ExpeditionOrderDataGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ExpeditionOrderDataGrid.Controls.Add(this.bunifuCustomLabel8);
            this.ExpeditionOrderDataGrid.Controls.Add(this.ExpeditionSage1);
            this.ExpeditionOrderDataGrid.Controls.Add(this.ExpeditionSage2);
            this.ExpeditionOrderDataGrid.Controls.Add(this.ExpeditionSage3);
            this.ExpeditionOrderDataGrid.Controls.Add(this.bunifuCustomLabel6);
            this.ExpeditionOrderDataGrid.Controls.Add(this.bunifuCustomLabel5);
            this.ExpeditionOrderDataGrid.Controls.Add(this.bunifuCustomLabel4);
            this.ExpeditionOrderDataGrid.Controls.Add(this.bunifuCustomLabel3);
            this.ExpeditionOrderDataGrid.Location = new System.Drawing.Point(0, 154);
            this.ExpeditionOrderDataGrid.Name = "ExpeditionOrderDataGrid";
            this.ExpeditionOrderDataGrid.Padding = new System.Windows.Forms.Padding(0, 0, 0, 300);
            this.ExpeditionOrderDataGrid.Size = new System.Drawing.Size(808, 398);
            this.ExpeditionOrderDataGrid.TabIndex = 2;
            // 
            // bunifuCustomLabel8
            // 
            this.bunifuCustomLabel8.AutoSize = true;
            this.bunifuCustomLabel8.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuCustomLabel8.Location = new System.Drawing.Point(72, 16);
            this.bunifuCustomLabel8.Name = "bunifuCustomLabel8";
            this.bunifuCustomLabel8.Size = new System.Drawing.Size(253, 25);
            this.bunifuCustomLabel8.TabIndex = 10;
            this.bunifuCustomLabel8.Text = "Mode d\'expédition Magento";
            // 
            // ExpeditionSage1
            // 
            this.ExpeditionSage1.BackColor = System.Drawing.Color.Transparent;
            this.ExpeditionSage1.BorderRadius = 3;
            this.ExpeditionSage1.DisabledColor = System.Drawing.Color.Gray;
            this.ExpeditionSage1.ForeColor = System.Drawing.Color.White;
            this.ExpeditionSage1.Items = new string[0];
            this.ExpeditionSage1.Location = new System.Drawing.Point(461, 86);
            this.ExpeditionSage1.Name = "ExpeditionSage1";
            this.ExpeditionSage1.NomalColor = System.Drawing.Color.Gainsboro;
            this.ExpeditionSage1.onHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.ExpeditionSage1.selectedIndex = -1;
            this.ExpeditionSage1.Size = new System.Drawing.Size(277, 35);
            this.ExpeditionSage1.TabIndex = 9;
            // 
            // ExpeditionSage2
            // 
            this.ExpeditionSage2.BackColor = System.Drawing.Color.Transparent;
            this.ExpeditionSage2.BorderRadius = 3;
            this.ExpeditionSage2.DisabledColor = System.Drawing.Color.Gray;
            this.ExpeditionSage2.ForeColor = System.Drawing.Color.White;
            this.ExpeditionSage2.Items = new string[0];
            this.ExpeditionSage2.Location = new System.Drawing.Point(461, 165);
            this.ExpeditionSage2.Name = "ExpeditionSage2";
            this.ExpeditionSage2.NomalColor = System.Drawing.Color.Gainsboro;
            this.ExpeditionSage2.onHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.ExpeditionSage2.selectedIndex = -1;
            this.ExpeditionSage2.Size = new System.Drawing.Size(277, 35);
            this.ExpeditionSage2.TabIndex = 8;
            // 
            // ExpeditionSage3
            // 
            this.ExpeditionSage3.BackColor = System.Drawing.Color.Transparent;
            this.ExpeditionSage3.BorderRadius = 3;
            this.ExpeditionSage3.DisabledColor = System.Drawing.Color.Gray;
            this.ExpeditionSage3.ForeColor = System.Drawing.Color.White;
            this.ExpeditionSage3.Items = new string[0];
            this.ExpeditionSage3.Location = new System.Drawing.Point(461, 240);
            this.ExpeditionSage3.Name = "ExpeditionSage3";
            this.ExpeditionSage3.NomalColor = System.Drawing.Color.Gainsboro;
            this.ExpeditionSage3.onHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.ExpeditionSage3.selectedIndex = -1;
            this.ExpeditionSage3.Size = new System.Drawing.Size(277, 35);
            this.ExpeditionSage3.TabIndex = 7;
            // 
            // bunifuCustomLabel6
            // 
            this.bunifuCustomLabel6.AutoSize = true;
            this.bunifuCustomLabel6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuCustomLabel6.Location = new System.Drawing.Point(36, 250);
            this.bunifuCustomLabel6.Name = "bunifuCustomLabel6";
            this.bunifuCustomLabel6.Size = new System.Drawing.Size(114, 25);
            this.bunifuCustomLabel6.TabIndex = 4;
            this.bunifuCustomLabel6.Text = "Chronopost";
            // 
            // bunifuCustomLabel5
            // 
            this.bunifuCustomLabel5.AutoSize = true;
            this.bunifuCustomLabel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuCustomLabel5.Location = new System.Drawing.Point(36, 175);
            this.bunifuCustomLabel5.Name = "bunifuCustomLabel5";
            this.bunifuCustomLabel5.Size = new System.Drawing.Size(97, 25);
            this.bunifuCustomLabel5.TabIndex = 3;
            this.bunifuCustomLabel5.Text = "Colissimo";
            // 
            // bunifuCustomLabel4
            // 
            this.bunifuCustomLabel4.AutoSize = true;
            this.bunifuCustomLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuCustomLabel4.Location = new System.Drawing.Point(477, 16);
            this.bunifuCustomLabel4.Name = "bunifuCustomLabel4";
            this.bunifuCustomLabel4.Size = new System.Drawing.Size(223, 25);
            this.bunifuCustomLabel4.TabIndex = 2;
            this.bunifuCustomLabel4.Text = "Mode d\'expédition Sage";
            // 
            // bunifuCustomLabel3
            // 
            this.bunifuCustomLabel3.AutoSize = true;
            this.bunifuCustomLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuCustomLabel3.Location = new System.Drawing.Point(36, 96);
            this.bunifuCustomLabel3.Name = "bunifuCustomLabel3";
            this.bunifuCustomLabel3.Size = new System.Drawing.Size(135, 25);
            this.bunifuCustomLabel3.TabIndex = 1;
            this.bunifuCustomLabel3.Text = "Mondial Relay";
            // 
            // SaveExpidationMode
            // 
            this.SaveExpidationMode.Location = new System.Drawing.Point(664, 568);
            this.SaveExpidationMode.Name = "SaveExpidationMode";
            this.SaveExpidationMode.Size = new System.Drawing.Size(75, 23);
            this.SaveExpidationMode.TabIndex = 3;
            this.SaveExpidationMode.Text = "Enregistrer";
            this.SaveExpidationMode.UseVisualStyleBackColor = true;
            this.SaveExpidationMode.Click += new System.EventHandler(this.SaveExpidationMode_Click);
            // 
            // CancelExpéditionMode
            // 
            this.CancelExpéditionMode.Location = new System.Drawing.Point(561, 568);
            this.CancelExpéditionMode.Name = "CancelExpéditionMode";
            this.CancelExpéditionMode.Size = new System.Drawing.Size(75, 23);
            this.CancelExpéditionMode.TabIndex = 4;
            this.CancelExpéditionMode.Text = "Annuler";
            this.CancelExpéditionMode.UseVisualStyleBackColor = true;
            this.CancelExpéditionMode.Click += new System.EventHandler(this.CancelExpéditionMode_Click);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // MappingExpeditionMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 603);
            this.Controls.Add(this.CancelExpéditionMode);
            this.Controls.Add(this.SaveExpidationMode);
            this.Controls.Add(this.ExpeditionOrderDataGrid);
            this.Controls.Add(this.DefaultExpeditinMode);
            this.Controls.Add(this.Header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MappingExpeditionMode";
            this.Text = "Form1";
            this.Header.ResumeLayout(false);
            this.Header.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloseButton)).EndInit();
            this.DefaultExpeditinMode.ResumeLayout(false);
            this.DefaultExpeditinMode.PerformLayout();
            this.ExpeditionOrderDataGrid.ResumeLayout(false);
            this.ExpeditionOrderDataGrid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Header;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel1;
        private System.Windows.Forms.Panel DefaultExpeditinMode;
        private Bunifu.Framework.UI.BunifuDropdown DefaultExpedition;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel2;
        private WindowsFormsControlLibrary1.CustomImageButto CloseButton;
        private System.Windows.Forms.Panel ExpeditionOrderDataGrid;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel4;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel3;
        private System.Windows.Forms.Button SaveExpidationMode;
        private System.Windows.Forms.Button CancelExpéditionMode;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel8;
        private Bunifu.Framework.UI.BunifuDropdown ExpeditionSage1;
        private Bunifu.Framework.UI.BunifuDropdown ExpeditionSage2;
        private Bunifu.Framework.UI.BunifuDropdown ExpeditionSage3;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel6;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel5;
    }
}