namespace ChattingClientPrototype3
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pnMain = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnChatList = new System.Windows.Forms.Panel();
            this.lblChatList = new System.Windows.Forms.Label();
            this.lvChatList = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUnreadCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblFriendsList = new System.Windows.Forms.Label();
            this.pnbutton = new System.Windows.Forms.Panel();
            this.pbChats = new System.Windows.Forms.PictureBox();
            this.pbFriends = new System.Windows.Forms.PictureBox();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lbFriends = new System.Windows.Forms.ListBox();
            this.lbMyself = new System.Windows.Forms.ListBox();
            this.pnMain.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnChatList.SuspendLayout();
            this.pnbutton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbChats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFriends)).BeginInit();
            this.SuspendLayout();
            // 
            // pnMain
            // 
            this.pnMain.Controls.Add(this.panel1);
            this.pnMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnMain.Location = new System.Drawing.Point(0, 0);
            this.pnMain.Name = "pnMain";
            this.pnMain.Size = new System.Drawing.Size(424, 561);
            this.pnMain.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnChatList);
            this.panel1.Controls.Add(this.lblFriendsList);
            this.panel1.Controls.Add(this.pnbutton);
            this.panel1.Controls.Add(this.btnLogout);
            this.panel1.Controls.Add(this.lbFriends);
            this.panel1.Controls.Add(this.lbMyself);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.MaximumSize = new System.Drawing.Size(400, 537);
            this.panel1.MinimumSize = new System.Drawing.Size(400, 537);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 537);
            this.panel1.TabIndex = 0;
            // 
            // pnChatList
            // 
            this.pnChatList.Controls.Add(this.lblChatList);
            this.pnChatList.Controls.Add(this.lvChatList);
            this.pnChatList.Location = new System.Drawing.Point(72, 37);
            this.pnChatList.Name = "pnChatList";
            this.pnChatList.Size = new System.Drawing.Size(325, 500);
            this.pnChatList.TabIndex = 7;
            // 
            // lblChatList
            // 
            this.lblChatList.AutoSize = true;
            this.lblChatList.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblChatList.Location = new System.Drawing.Point(37, 33);
            this.lblChatList.Name = "lblChatList";
            this.lblChatList.Size = new System.Drawing.Size(68, 27);
            this.lblChatList.TabIndex = 1;
            this.lblChatList.Text = "채팅";
            // 
            // lvChatList
            // 
            this.lvChatList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colMessage,
            this.colUnreadCount});
            this.lvChatList.HideSelection = false;
            this.lvChatList.Location = new System.Drawing.Point(42, 93);
            this.lvChatList.Name = "lvChatList";
            this.lvChatList.Size = new System.Drawing.Size(254, 330);
            this.lvChatList.TabIndex = 0;
            this.lvChatList.UseCompatibleStateImageBehavior = false;
            this.lvChatList.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "이름";
            this.colName.Width = 90;
            // 
            // colMessage
            // 
            this.colMessage.Text = "메시지 내용";
            this.colMessage.Width = 90;
            // 
            // colUnreadCount
            // 
            this.colUnreadCount.Text = "읽지않음";
            this.colUnreadCount.Width = 70;
            // 
            // lblFriendsList
            // 
            this.lblFriendsList.AutoSize = true;
            this.lblFriendsList.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblFriendsList.Location = new System.Drawing.Point(109, 70);
            this.lblFriendsList.Name = "lblFriendsList";
            this.lblFriendsList.Size = new System.Drawing.Size(68, 27);
            this.lblFriendsList.TabIndex = 6;
            this.lblFriendsList.Text = "친구";
            // 
            // pnbutton
            // 
            this.pnbutton.Controls.Add(this.pbChats);
            this.pnbutton.Controls.Add(this.pbFriends);
            this.pnbutton.Location = new System.Drawing.Point(15, 70);
            this.pnbutton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnbutton.Name = "pnbutton";
            this.pnbutton.Size = new System.Drawing.Size(50, 150);
            this.pnbutton.TabIndex = 3;
            // 
            // pbChats
            // 
            this.pbChats.Image = global::ChattingClientPrototype3.Properties.Resources.채팅목록;
            this.pbChats.Location = new System.Drawing.Point(0, 47);
            this.pbChats.MaximumSize = new System.Drawing.Size(50, 50);
            this.pbChats.MinimumSize = new System.Drawing.Size(50, 50);
            this.pbChats.Name = "pbChats";
            this.pbChats.Size = new System.Drawing.Size(50, 50);
            this.pbChats.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbChats.TabIndex = 4;
            this.pbChats.TabStop = false;
            // 
            // pbFriends
            // 
            this.pbFriends.Image = global::ChattingClientPrototype3.Properties.Resources.친구목록;
            this.pbFriends.Location = new System.Drawing.Point(0, 0);
            this.pbFriends.MaximumSize = new System.Drawing.Size(50, 50);
            this.pbFriends.MinimumSize = new System.Drawing.Size(50, 50);
            this.pbFriends.Name = "pbFriends";
            this.pbFriends.Size = new System.Drawing.Size(50, 50);
            this.pbFriends.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbFriends.TabIndex = 0;
            this.pbFriends.TabStop = false;
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(308, 11);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(64, 22);
            this.btnLogout.TabIndex = 2;
            this.btnLogout.Text = "로그아웃";
            this.btnLogout.UseVisualStyleBackColor = true;
            // 
            // lbFriends
            // 
            this.lbFriends.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbFriends.FormattingEnabled = true;
            this.lbFriends.ItemHeight = 16;
            this.lbFriends.Location = new System.Drawing.Point(114, 153);
            this.lbFriends.Name = "lbFriends";
            this.lbFriends.Size = new System.Drawing.Size(258, 324);
            this.lbFriends.TabIndex = 1;
            // 
            // lbMyself
            // 
            this.lbMyself.Font = new System.Drawing.Font("굴림", 19.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbMyself.FormattingEnabled = true;
            this.lbMyself.ItemHeight = 26;
            this.lbMyself.Location = new System.Drawing.Point(114, 117);
            this.lbMyself.Name = "lbMyself";
            this.lbMyself.Size = new System.Drawing.Size(258, 30);
            this.lbMyself.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 561);
            this.Controls.Add(this.pnMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(440, 600);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(440, 600);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.pnMain.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnChatList.ResumeLayout(false);
            this.pnChatList.PerformLayout();
            this.pnbutton.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbChats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFriends)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnMain;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lbFriends;
        private System.Windows.Forms.ListBox lbMyself;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel pnbutton;
        private System.Windows.Forms.PictureBox pbChats;
        private System.Windows.Forms.PictureBox pbFriends;
        private System.Windows.Forms.Panel pnChatList;
        private System.Windows.Forms.Label lblFriendsList;
        private System.Windows.Forms.Label lblChatList;
        private System.Windows.Forms.ListView lvChatList;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colMessage;
        private System.Windows.Forms.ColumnHeader colUnreadCount;
    }
}