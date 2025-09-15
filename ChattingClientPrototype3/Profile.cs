using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChattingClientPrototype3
{
    public partial class Profile : UserControl
    {
        private Panel circularPanel; // 원형 패널
        private Label lastnameLabel; // 성씨 레이블

        public Profile()
        {
            InitializeComponent();
            InitializeProfile();
        }

        private void InitializeProfile()
        {
            // 프로필 패널의 크기를 정사각형으로 설정
            this.Size = new Size(30, 30); // 원하는 정사각형 크기로 설정

            // 원형으로 만들기 위한 Region 설정
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, this.Width, this.Height);
            this.Region = new Region(path); // UserControl을 원형으로 설정

            // 원형 패널 생성
            circularPanel = new Panel
            {
                Size = new Size(30, 30), // 원형 패널 크기
                Location = new Point(0, 0), // 중앙에 위치
                BackColor = Color.LightCyan, // 배경색
                BorderStyle = BorderStyle.None // 테두리 설정
            };
            circularPanel.Paint += CircularPanel_Paint; // 원형으로 그리기 위한 이벤트 핸들러 추가

            // 이름 레이블 생성
            lastnameLabel = new Label
            {
                Text = "A", // 기본값 (이름의 첫 글자로 변경 필요)
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 13, FontStyle.Bold), // 글꼴 설정
                BackColor = Color.Transparent // 레이블 배경 투명
            };

            // 원형 패널에 레이블 추가
            circularPanel.Controls.Add(lastnameLabel);
            pnProfile.Controls.Add(circularPanel); // pnProfile에 원형 패널 추가
        }
        private void CircularPanel_Paint(object sender, PaintEventArgs e)
        {
            // 원형으로 그리기
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 부드럽게
            g.FillEllipse(new SolidBrush(circularPanel.BackColor), 0, 0, circularPanel.Width, circularPanel.Height);
        }

        // 이름의 첫 글자를 설정하는 메서드
        public void SetName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                lastnameLabel.Text = name.Substring(0, 1).ToUpper(); // 첫 글자 대문자로 설정
            }
            else
            {
                lastnameLabel.Text = " "; // 빈 경우에는 공백
            }
        }
    }
}
