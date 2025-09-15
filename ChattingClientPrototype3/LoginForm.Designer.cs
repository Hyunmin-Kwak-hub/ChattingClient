namespace ChattingClientPrototype3
{
    partial class LoginForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnTogglePassword = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.tbPw = new System.Windows.Forms.TextBox();
            this.tbId = new System.Windows.Forms.TextBox();
            this.cbAutoLogin = new System.Windows.Forms.CheckBox();
            this.LoginTitle = new System.Windows.Forms.Label();
            this.PWlabel = new System.Windows.Forms.Label();
            this.IDlabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.btnTogglePassword);
            this.panel1.Controls.Add(this.btnLogin);
            this.panel1.Controls.Add(this.tbPw);
            this.panel1.Controls.Add(this.tbId);
            this.panel1.Controls.Add(this.cbAutoLogin);
            this.panel1.Controls.Add(this.LoginTitle);
            this.panel1.Controls.Add(this.PWlabel);
            this.panel1.Controls.Add(this.IDlabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panel1.MaximumSize = new System.Drawing.Size(787, 1922);
            this.panel1.MinimumSize = new System.Drawing.Size(787, 1122);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(787, 1122);
            this.panel1.TabIndex = 0;
            // 
            // btnTogglePassword
            // 
            this.btnTogglePassword.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnTogglePassword.Location = new System.Drawing.Point(455, 400);
            this.btnTogglePassword.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnTogglePassword.Name = "btnTogglePassword";
            this.btnTogglePassword.Size = new System.Drawing.Size(85, 46);
            this.btnTogglePassword.TabIndex = 8;
            this.btnTogglePassword.Text = "보이기";
            this.btnTogglePassword.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(568, 334);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(139, 112);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "로그인";
            this.btnLogin.UseVisualStyleBackColor = false;
            // 
            // tbPw
            // 
            this.tbPw.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbPw.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbPw.Location = new System.Drawing.Point(202, 396);
            this.tbPw.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tbPw.Name = "tbPw";
            this.tbPw.PasswordChar = '●';
            this.tbPw.Size = new System.Drawing.Size(238, 42);
            this.tbPw.TabIndex = 5;
            // 
            // tbId
            // 
            this.tbId.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbId.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbId.Location = new System.Drawing.Point(202, 334);
            this.tbId.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tbId.Name = "tbId";
            this.tbId.Size = new System.Drawing.Size(238, 42);
            this.tbId.TabIndex = 4;
            // 
            // cbAutoLogin
            // 
            this.cbAutoLogin.AutoSize = true;
            this.cbAutoLogin.Location = new System.Drawing.Point(202, 472);
            this.cbAutoLogin.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbAutoLogin.Name = "cbAutoLogin";
            this.cbAutoLogin.Size = new System.Drawing.Size(218, 28);
            this.cbAutoLogin.TabIndex = 3;
            this.cbAutoLogin.Text = "로그인정보 저장";
            this.cbAutoLogin.UseVisualStyleBackColor = true;
            // 
            // LoginTitle
            // 
            this.LoginTitle.AutoSize = true;
            this.LoginTitle.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LoginTitle.Location = new System.Drawing.Point(295, 120);
            this.LoginTitle.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.LoginTitle.Name = "LoginTitle";
            this.LoginTitle.Size = new System.Drawing.Size(185, 54);
            this.LoginTitle.TabIndex = 2;
            this.LoginTitle.Text = "로그인";
            // 
            // PWlabel
            // 
            this.PWlabel.AutoSize = true;
            this.PWlabel.Location = new System.Drawing.Point(93, 408);
            this.PWlabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.PWlabel.Name = "PWlabel";
            this.PWlabel.Size = new System.Drawing.Size(106, 24);
            this.PWlabel.TabIndex = 1;
            this.PWlabel.Text = "비밀번호";
            // 
            // IDlabel
            // 
            this.IDlabel.AutoSize = true;
            this.IDlabel.Location = new System.Drawing.Point(115, 346);
            this.IDlabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.IDlabel.Name = "IDlabel";
            this.IDlabel.Size = new System.Drawing.Size(82, 24);
            this.IDlabel.TabIndex = 0;
            this.IDlabel.Text = "아이디";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 1058);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(795, 1129);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(795, 1129);
            this.Name = "LoginForm";
            this.Text = "Login";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnTogglePassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox tbPw;
        private System.Windows.Forms.TextBox tbId;
        private System.Windows.Forms.CheckBox cbAutoLogin;
        private System.Windows.Forms.Label LoginTitle;
        private System.Windows.Forms.Label PWlabel;
        private System.Windows.Forms.Label IDlabel;
    }
}

