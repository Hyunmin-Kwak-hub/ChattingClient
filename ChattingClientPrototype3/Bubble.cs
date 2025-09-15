using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChattingClientPrototype3
{
    public partial class Bubble : UserControl
    {
        public Bubble()
        {
            InitializeComponent();
            tbMessage.AutoSize = false;
            tbMessage.Multiline = true; // 여러 줄 입력 가능
            tbMessage.BorderStyle = BorderStyle.None; // 테두리 제거
            tbMessage.TextAlign = HorizontalAlignment.Left; // 텍스트는 왼쪽 정렬
            tbMessage.Dock = DockStyle.None; // Dock 해제
        }

        // 메시지를 설정하는 속성
        public string Message
        {
            get { return tbMessage.Text; }
            set
            {
                tbMessage.Text = InsertLineBreaks(value, 20); // 20자마다 줄바꿈 추가
                AdjustTextBoxSize(); // 텍스트 크기에 맞춰 TextBox 및 말풍선 크기 조정
            }
        }

        // 말풍선의 배경색을 설정하는 속성
        public Color BubbleColor
        {
            get { return pnBubble.BackColor; }
            set
            {
                pnBubble.BackColor = value;
                tbMessage.BackColor = value; // TextBox의 배경색도 동일하게 설정
            }
        }

        // TextBox 및 말풍선 크기 조정
        private void AdjustTextBoxSize()
        {
            // TextBox의 크기를 텍스트 크기에 맞춰 조정
            Size textSize = TextRenderer.MeasureText(tbMessage.Text, tbMessage.Font, new Size(250, 0), TextFormatFlags.WordBreak); // 최대 너비 250

            tbMessage.Width = textSize.Width + 2; // 텍스트 너비에 약간의 여백 추가
            tbMessage.Height = textSize.Height + 5; // 텍스트 높이에 약간의 여백 추가

            // 말풍선 크기를 TextBox 크기에 맞게 조정
            this.Width = tbMessage.Width + 6; // 말풍선 크기 조정 (TextBox 너비에 추가 여백)
            this.Height = tbMessage.Height + 6; // 말풍선 크기 조정 (TextBox 높이에 추가 여백)

            // TextBox를 말풍선의 중앙에 수동으로 배치
            tbMessage.Location = new Point(
                (this.Width - tbMessage.Width) / 2,  // 가로 중앙에 배치
                (this.Height - tbMessage.Height) / 2 // 세로 중앙에 배치
            );
        }

        // 지정된 길이마다 줄바꿈(\n) 추가하는 함수
        private string InsertLineBreaks(string input, int maxLineLength)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string result = "";
            for (int i = 0; i < input.Length; i += maxLineLength)
            {
                if (i + maxLineLength < input.Length)
                    result += input.Substring(i, maxLineLength) + "\n"; // 20자마다 줄바꿈 추가
                else
                    result += input.Substring(i); // 남은 부분 추가
            }

            return result;
        }
    }
}
