namespace ChattingClientPrototype3
{
    partial class Bubble
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnBubble = new System.Windows.Forms.Panel();
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.pnBubble.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnBubble
            // 
            this.pnBubble.Controls.Add(this.tbMessage);
            this.pnBubble.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnBubble.Location = new System.Drawing.Point(0, 0);
            this.pnBubble.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.pnBubble.Name = "pnBubble";
            this.pnBubble.Size = new System.Drawing.Size(279, 300);
            this.pnBubble.TabIndex = 0;
            // 
            // tbMessage
            // 
            this.tbMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbMessage.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbMessage.Location = new System.Drawing.Point(41, 44);
            this.tbMessage.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tbMessage.Multiline = true;
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.ReadOnly = true;
            this.tbMessage.Size = new System.Drawing.Size(186, 42);
            this.tbMessage.TabIndex = 0;
            // 
            // Bubble
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnBubble);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "Bubble";
            this.Size = new System.Drawing.Size(279, 300);
            this.pnBubble.ResumeLayout(false);
            this.pnBubble.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnBubble;
        private System.Windows.Forms.TextBox tbMessage;
    }
}
