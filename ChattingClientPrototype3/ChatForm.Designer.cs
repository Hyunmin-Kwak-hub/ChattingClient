namespace ChattingClientPrototype3
{
    partial class ChatForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnBeforeChat = new System.Windows.Forms.Button();
            this.lblTextCnt = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.tbChat = new System.Windows.Forms.TextBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.flpChat = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnBeforeChat);
            this.panel1.Controls.Add(this.lblTextCnt);
            this.panel1.Controls.Add(this.btnSend);
            this.panel1.Controls.Add(this.tbChat);
            this.panel1.Controls.Add(this.lblTo);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(424, 561);
            this.panel1.TabIndex = 0;
            // 
            // btnBeforeChat
            // 
            this.btnBeforeChat.Location = new System.Drawing.Point(337, 23);
            this.btnBeforeChat.Name = "btnBeforeChat";
            this.btnBeforeChat.Size = new System.Drawing.Size(75, 23);
            this.btnBeforeChat.TabIndex = 5;
            this.btnBeforeChat.Text = "이전메시지";
            this.btnBeforeChat.UseVisualStyleBackColor = true;
            // 
            // lblTextCnt
            // 
            this.lblTextCnt.AutoSize = true;
            this.lblTextCnt.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTextCnt.Location = new System.Drawing.Point(186, 534);
            this.lblTextCnt.Name = "lblTextCnt";
            this.lblTextCnt.Size = new System.Drawing.Size(105, 11);
            this.lblTextCnt.TabIndex = 4;
            this.lblTextCnt.Text = "최대 250자(0 / 250)";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(309, 438);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(103, 93);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "전 송";
            this.btnSend.UseVisualStyleBackColor = true;
            // 
            // tbChat
            // 
            this.tbChat.Location = new System.Drawing.Point(12, 438);
            this.tbChat.MaxLength = 250;
            this.tbChat.Multiline = true;
            this.tbChat.Name = "tbChat";
            this.tbChat.Size = new System.Drawing.Size(279, 93);
            this.tbChat.TabIndex = 2;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTo.Location = new System.Drawing.Point(26, 23);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(196, 27);
            this.lblTo.TabIndex = 1;
            this.lblTo.Text = "oo님과의 채팅";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.flpChat);
            this.panel2.Location = new System.Drawing.Point(12, 69);
            this.panel2.MaximumSize = new System.Drawing.Size(400, 350);
            this.panel2.MinimumSize = new System.Drawing.Size(400, 350);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(400, 350);
            this.panel2.TabIndex = 0;
            // 
            // flpChat
            // 
            this.flpChat.AutoScroll = true;
            this.flpChat.BackColor = System.Drawing.Color.White;
            this.flpChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpChat.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpChat.Location = new System.Drawing.Point(0, 0);
            this.flpChat.MaximumSize = new System.Drawing.Size(400, 350);
            this.flpChat.MinimumSize = new System.Drawing.Size(400, 350);
            this.flpChat.Name = "flpChat";
            this.flpChat.Size = new System.Drawing.Size(400, 350);
            this.flpChat.TabIndex = 0;
            this.flpChat.WrapContents = false;
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 561);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(440, 600);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(440, 600);
            this.Name = "ChatForm";
            this.Text = "ChatForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.FlowLayoutPanel flpChat;
        private System.Windows.Forms.TextBox tbChat;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblTextCnt;
        private System.Windows.Forms.Button btnBeforeChat;
    }
}