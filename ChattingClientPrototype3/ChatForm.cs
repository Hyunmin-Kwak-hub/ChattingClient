using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Linq.Expressions;
using System.Data.SQLite;
using System.Xml.Linq;

namespace ChattingClientPrototype3
{
    public partial class ChatForm : Form
    {
        public int UniqueId { get; private set; }

        public int room_id;
        private string my_id;
        private string friend_id;
        private string friend_name;
        private string friend_nickname;
        private MainForm mainForm;
        private string dbPath = Application.StartupPath + @"\chat_Database.db"; // 데이터베이스 파일 경로

        public event EventHandler MessagesUpdated; // 메시지 업데이트 이벤트

                                                      // 가장 오래된 날짜와 가장 최근 날짜를 별도 변수로 선언
        private DateTime? oldestMessageDate = null;  // 가장 오래된 메시지 날짜
        private DateTime? latestMessageDate = null;  // 가장 최근 메시지 날짜

        private int LastLoadedMessageId = 0; // 현재까지 불러온 가장 오래된 메시지 ID를 저장하는 변수

        public ChatForm(int roomId, string loggedInUserId, string otherUserId, MainForm mainForm)
        {
            InitializeComponent();

            this.mainForm = mainForm; // MainForm의 인스턴스를 저장

            UniqueId = roomId;
            room_id = roomId;
            my_id = loggedInUserId;
            friend_id = otherUserId;

            LoadFriendInfo(friend_id); // 친구 정보를 비동기로 로드

            // UI 업데이트는 UI 스레드에서 수행해야 함
            lblTo.Text = $"{friend_name}님과의 채팅"; // 레이블에 닉네임과 함께 대화 중임을 표시
            this.Text = $"Chat Room - {friend_name}"; // 제목에 room_id 추가

            // 최근 메시지 로드
            LoadMessages(room_id); // 최근 메시지 로드

            // Shown 이벤트 핸들러 연결
            this.Shown += new EventHandler(ChatForm_Shown);


            // 전송 버튼 클릭 이벤트
            btnSend.Click += new EventHandler(BtnSend_Click);
            // 이전 메시지 불러오기 버튼 클릭 이벤트
            btnBeforeChat.Click += new EventHandler(BtnBeforeChat_Click);

            // 텍스트박스에서 엔터키를 눌렀을때 이벤트 연결
            tbChat.KeyDown += new KeyEventHandler(tb_KeyDown);

            // TextChanged 이벤트를 코드에서 설정
            tbChat.TextChanged += tbChat_TextChanged;

            // 폼 닫기 이벤트 핸들러 등록 (테스트)
            this.FormClosed += new FormClosedEventHandler(ChatForm_FormClosed);
        }

        private void ChatForm_Shown(object sender, EventArgs e)
        {
            // 폼이 다 열린 후에 실행할 코드
            LoadEvent();
        }


        private void LoadEvent()
        {
            // 이벤트 발생
            MessagesUpdated?.Invoke(this, EventArgs.Empty); // 이벤트 발생

            tbChat.Focus(); // 텍스트박스에 포커싱 주기

            // 채팅창 스크롤을 맨 아래로 이동
            flpChat.VerticalScroll.Value = flpChat.VerticalScroll.Maximum;
            flpChat.PerformLayout(); // 레이아웃 다시 그리기
        }

        /*==============================================================================================================================================================
         *   @@@@친구 정보 가져오기(userList.json)@@@@
         * ============================================================================================================================================================*/
        private void LoadFriendInfo(string friendId)
        {
            // userList.json 파일 경로 가져오기
            string usersPath = Application.StartupPath + @"\userList.json";

            // JSON 파일에서 친구 정보 가져오기
            try
            {
                string jsonData = File.ReadAllText(usersPath);
                JObject userListObject = JObject.Parse(jsonData);
                JArray userList = (JArray)userListObject["userList"];

                // 친구 ID에 맞는 정보를 찾음
                var friendInfo = userList
                    .FirstOrDefault(u => u["user_id"] != null && u["user_id"].ToString() == friendId);

                if (friendInfo != null)
                {
                    // 친구 정보 변수에 저장
                    friend_name = friendInfo["user_name"]?.ToString() ?? "알 수 없음";
                    friend_nickname = friendInfo["user_nickname"]?.ToString() ?? "알 수 없음";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"친구 정보를 불러오는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /*==============================================================================================================================================================
         *   @@@@전송버튼 클릭 이벤트@@@@
         * ============================================================================================================================================================*/

        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbChat.Text))
            {
                MessageBox.Show("메시지를 입력하세요.");
                return;
            }
            string message = tbChat.Text; // 입력된 메시지를 가져옴

            mainForm.SendMessage(room_id, message); // MainForm의 SendMessage 메서드 호출
            tbChat.Clear(); // 텍스트박스 초기화

        }

        /*==============================================================================================================================================================
         *   @@@@메시지 채팅창 추가@@@@
         * ============================================================================================================================================================*/

        public void AddMessage(string sender_id, string message, string message_time)
        {
            // UI 스레드에서 실행해야 하는지 확인
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddMessage(sender_id, message, message_time)));
                return; // 메서드 종료
            }

            // message_time을 DateTime으로 변환
            if (!DateTime.TryParse(message_time, out DateTime messageDateTime))
            {
                messageDateTime = DateTime.Now; // 변환 실패 시 기본 값
            }

            // 최근 메시지 날짜와 비교하여 날짜 경계선 추가
            if (latestMessageDate == null || messageDateTime.Date > latestMessageDate.Value.Date)
            {
                // 새로운 날짜 경계선 추가 및 최근 메시지 날짜 갱신
                AddDateBoundaryPanel(messageDateTime);
                latestMessageDate = messageDateTime.Date;
            }
            else if (messageDateTime.Date == latestMessageDate.Value.Date)
            {
                // 같은 날짜라면 날짜 경계선을 추가하지 않고, latestMessageDate를 유지합니다.
                latestMessageDate = messageDateTime.Date;
            }

            // 시간을 HH:mm 형식으로 변환
            string formattedTime = messageDateTime.ToString("HH:mm");


            if (sender_id == my_id)
            {
                Bubble myBubble = new Bubble
                {
                    Message = message,
                    BubbleColor = Color.LightBlue // 내 채팅 말풍선 색
                };

                // 시간 레이블 생성
                Label timeLabel = new Label
                {
                    Text = formattedTime,
                    AutoSize = true,
                    Font = new Font("Arial", 8, FontStyle.Regular), // 원하는 폰트 설정
                    ForeColor = Color.Gray // 시간 레이블 색상
                };

                // 말풍선 패널이 들어갈 패널 생성
                Panel MessagePanel = new Panel
                {
                    Size = new Size(350, myBubble.Height + 10), // 최대 가로 폭 설정
                    AutoSize = true, // 높이는 내용에 맞게 자동 조정
                    Dock = DockStyle.Bottom // 위에서 아래로 쌓이게 설정
                };

                // myBubble의 위치 설정: 350에서 myBubble의 가로 길이를 뺀 위치에 배치
                myBubble.Location = new Point(350 - myBubble.Width, 6); // 5px 여백
                timeLabel.Location = new Point(350 - myBubble.Width - 35, myBubble.Height - 5); // 시간 레이블 위치 설정

                // 메시지 패널에 말풍선 추가
                MessagePanel.Controls.Add(myBubble);
                MessagePanel.Controls.Add(timeLabel);

                // FlowLayouPanel에 추가
                flpChat.Controls.Add(MessagePanel);

                // 채팅창 스크롤을 맨 아래로 이동
                flpChat.VerticalScroll.Value = flpChat.VerticalScroll.Maximum;
                flpChat.PerformLayout(); // 레이아웃 다시 그리기
            }
            else
            {

                // 채팅 상대방의 이름을 표시할 레이블 생성
                Label nameLabel = new Label
                {
                    Text = friend_name, // 상대방의 이름 설정
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold), // 원하는 폰트 설정
                    ForeColor = Color.Black // 이름 레이블 색상
                };


                // 시간 레이블 생성
                Label timeLabel = new Label
                {
                    Text = formattedTime,
                    AutoSize = true,
                    Font = new Font("Arial", 8, FontStyle.Regular), // 원하는 폰트 설정
                    ForeColor = Color.Gray // 시간 레이블 색상
                };

                // 프로필 패널 생성 (예: "Alice"라는 이름의 첫 글자를 표시)
                Profile userProfile = new Profile
                {
                    Size = new Size(30, 30), // 원하는 크기로 설정
                    Location = new Point(0, 0) // 위치는 기본값으로 설정
                };

                userProfile.SetName(friend_name);


                Bubble othersBubble = new Bubble
                {
                    Message = message,
                    BubbleColor = Color.LightGreen, // 상대 채팅 말풍선 색
                };

                // 말풍선의 높이를 측정
                othersBubble.PerformLayout(); // 레이아웃을 강제 수행하여 말풍선 크기 계산
                int bubbleHeight = othersBubble.Height;

                // 이름 레이블의 높이를 가져옴
                int nameLabelHeight = nameLabel.Height;

                // 말풍선 패널이 들어갈 패널 생성
                Panel MessagePanel = new Panel
                {
                    Size = new Size(350, Math.Max(bubbleHeight + nameLabelHeight, 41)), // 말풍선 높이가 32보다 크면 그 높이에 맞추고, 작으면 32로 유지
                    MinimumSize = new Size(350, 30 + nameLabelHeight), // 최소 폭 350, 최소 높이 30
                    MaximumSize = new Size(350, 0), // 가로 최대 350으로 고정, 세로는 자유롭게 커짐
                    AutoSize = true, // 높이는 내용에 맞게 자동 조정
                    Dock = DockStyle.Bottom // 위에서 아래로 쌓이게 설정
                };

                // 메시지 패널에 프로필 추가
                userProfile.Dock = DockStyle.Left; // 프로필을 왼쪽에 고정
                MessagePanel.Controls.Add(userProfile);

                // 이름 레이블의 위치를 프로필 오른쪽에 설정
                nameLabel.Location = new Point(userProfile.Width + 5, 0); // 프로필 오른쪽에 이름 레이블 배치
                MessagePanel.Controls.Add(nameLabel); // 메시지 패널에 이름 레이블 추가

                // 메시지 패널에 말풍선 추가
                othersBubble.Location = new Point(userProfile.Width + 5, nameLabel.Height + 2);
                MessagePanel.Controls.Add(othersBubble);

                // 시간 레이블 위치 설정
                timeLabel.Location = new Point(userProfile.Width + othersBubble.Width + 5, othersBubble.Height + 5);
                MessagePanel.Controls.Add(timeLabel); // 메시지 패널에 시간 레이블 추가

                // FlowLayouPanel에 추가
                flpChat.Controls.Add(MessagePanel);

            }
            // 채팅창 스크롤을 맨 아래로 이동
            flpChat.VerticalScroll.Value = flpChat.VerticalScroll.Maximum;
            flpChat.PerformLayout(); // 레이아웃 다시 그리기

            // 이벤트 발생
            MessagesUpdated?.Invoke(this, EventArgs.Empty); // 이벤트 발생

            tbChat.Focus();
        }

        /*==============================================================================================================================================================
         *   @@@@채팅방 처음 열때 이전 메시지 불러오기@@@@
         * ============================================================================================================================================================*/

        private void LoadMessages(int roomId)
        {
           roomId = room_id;
            try
            {
                // SQLite 연결
                using (var connection = new SQLiteConnection("Data Source=chat_database.db;VerSion=3;"))
                {
                    connection.Open();

                    // 메시지가 있는지 확인하는 쿼리
                    string checkMessagesQuery = @"
                        SELECT COUNT(*)
                        FROM messagetbl
                        WHERE room_id = @room_id";

                    using (var checkCommand = new SQLiteCommand(checkMessagesQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@room_id", roomId);
                        int messageCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        // 메시지가 없는 경우 이전 메시지 버튼을 비활성화하고 메서드를 종료
                        if (messageCount == 0)
                        {
                            btnBeforeChat.Enabled = false; // 이전 메시지 버튼 비활성화
                            return;
                        }
                    }

                    // 읽지 않은 메시지 개수를 확인하는 쿼리
                    int unreadMessageCount = 0;
                    string countQuery = @"
                        SELECT COUNT(*)
                        FROM messagetbl
                        WHERE room_id = @room_id AND is_read = 0";

                    using (var countCommand = new SQLiteCommand(countQuery, connection))
                    {
                        countCommand.Parameters.AddWithValue("@room_id", roomId);
                        unreadMessageCount = Convert.ToInt32(countCommand.ExecuteScalar());
                    }

                    // 불러올 메시지 개수를 조건에 따라 설정
                    int limit = unreadMessageCount >= 20 ? unreadMessageCount : 20;

                        // room_id가 일치하는 메시지를 조회 (조건에 따라 설정된 limit을 사용)
                        string selectQuery = $@"
                        SELECT message_sender_id, message_text, message_time, message_id
                        FROM messagetbl
                        WHERE room_id = @room_id
                        ORDER BY message_id DESC
                        LIMIT {limit}"; // message_id 순으로 정렬하여 메시지를 최근 limit 갯수만큼 가져옴.

                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@room_id", roomId);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                return;
                            }

                            // 조회한 메시지를 리스트에 저장
                            var messages = new List<(string senderId, string messageText, string messageTime, int messageId)>();


                            // 조회한 메시지들을 AddMessage에 추가
                            while (reader.Read())
                            {
                                string senderId = reader["message_sender_id"].ToString();
                                string messageText = reader["message_text"].ToString();
                                string messageTime = reader["message_time"].ToString(); // message_time 추가
                                int messageId = Convert.ToInt32(reader["message_id"]);

                                messages.Add((senderId, messageText, messageTime, messageId));
                            }
                            // 메시지의 is_read 값을 1로 업데이트
                            UpdateReadStatus(connection, messages);


                            // 가장 오래된 메시지와 가장 최근 메시지의 날짜 저장
                            if (DateTime.TryParse(messages.First().messageTime, out DateTime oldestDate))
                            {
                                oldestMessageDate = oldestDate;
                            }
                            if (DateTime.TryParse(messages.First().messageTime, out DateTime latestDate))
                            {
                                latestMessageDate = latestDate;
                            }

                            // 정렬된 메시지를 AddPreviousMessage에 추가
                            foreach (var message in messages)
                            {
                                // AddPreviousMessage를 호출합니다.
                                AddPreviousMessage(message.senderId, message.messageText, message.messageTime);
                            }

                            // 가장 오래된 메시지 ID를 LastLoadedMessageId에 저장하여 중복 조회 방지
                            LastLoadedMessageId = messages.Last().messageId;

                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"LoadMessages() 오류: {ex.Message}");
            }

            // 채팅창 스크롤을 맨 아래로 이동
            flpChat.VerticalScroll.Value = flpChat.VerticalScroll.Maximum;
            flpChat.PerformLayout(); // 레이아웃 다시 그리기
            // 이벤트 발생
            MessagesUpdated?.Invoke(this, EventArgs.Empty); // 이벤트 발생

            tbChat.Focus();
        }

        /*==============================================================================================================================================================
         *   @@@@이전 메시지 불러오기 버튼 이벤트@@@@
         * ============================================================================================================================================================*/

        private void BtnBeforeChat_Click(object sender, EventArgs e)
        {
            LoadPreviousMessages();
        }

        private void LoadPreviousMessages()
        {

            try
            {
                // SQLite 연결
                using (var connection = new SQLiteConnection("Data Source=chat_database.db;Version=3"))
                {
                    connection.Open();

                    // 가장 오래된 message_id보다 작은 메시지 중 최신 20건을 조회
                    string selectQuery = @"
                SELECT message_sender_id, message_text, message_id, message_time
                FROM messagetbl
                WHERE room_id = @room_id AND message_id < @lastLoadedMessageId
                ORDER BY message_id DESC
                LIMIT 20"; // 이전 메시지 20건을 가져옴

                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@room_id", room_id);
                        command.Parameters.AddWithValue("@lastLoadedMessageId", LastLoadedMessageId);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                AddBeforeDateBoundaryPanel(oldestMessageDate.Value);

                                AddLastMessagePanel();

                                btnBeforeChat.Enabled = false; // 버튼 비활성화

                                // 스크롤 맨 위로 올리기
                                flpChat.VerticalScroll.Value = flpChat.VerticalScroll.Minimum;
                                flpChat.PerformLayout(); // 레이아웃을 다시 그려 스크롤 위치가 적용되도록 합니다.

                                tbChat.Focus();

                                return;
                            }

                            var messages = new List<(string senderId, string messageText, string messageTime, int messageId)>();

                            // 조회한 메시지를 리스트에 저장
                            while (reader.Read())
                            {
                                string senderId = reader["message_sender_id"].ToString();
                                string messageText = reader["message_text"].ToString();
                                int messageId = Convert.ToInt32(reader["message_id"]);
                                string messageTime = reader["message_time"].ToString(); // 메시지 전송 시간 가져오기

                                messages.Add((senderId, messageText, messageTime, messageId));
                            }

                            // 메시지의 is_read 값을 1로 업데이트
                            UpdateReadStatus(connection, messages);


                            // **리스트가 비어있지 않을 경우에만 메시지를 추가**
                            if (messages.Count > 0)
                            {
                                foreach (var message in messages)
                                {
                                    AddPreviousMessage(message.senderId, message.messageText, message.messageTime);
                                }

                                // 가장 오래된 메시지 ID 업데이트
                                LastLoadedMessageId = messages.Last().messageId; // LastLoadedMessageId 업데이트
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"LoadPreviousMessages() 오류: {ex.Message}");
            }

            // 스크롤 맨 위로 올리기
            flpChat.VerticalScroll.Value = flpChat.VerticalScroll.Minimum;
            flpChat.PerformLayout(); // 레이아웃을 다시 그려 스크롤 위치가 적용되도록 합니다.


            // 이벤트 발생
            MessagesUpdated?.Invoke(this, EventArgs.Empty); // 이벤트 발생

            tbChat.Focus();
        }


        private void AddPreviousMessage(string sender_id, string message, string message_time)
        {
            // UI 스레드에서 실행해야하는지 확인
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddPreviousMessage(sender_id, message, message_time)));
                return; // 메서드 종료
            }

            // message_time을 DateTime으로 변환
            if (!DateTime.TryParse(message_time, out DateTime messageDateTime))
            {
                messageDateTime = DateTime.Now; // 변환 실패 시 기본 값
            }

            // 가장 오래된 메시지의 날짜와 비교하여 경계선을 추가
            if (oldestMessageDate == null || oldestMessageDate.Value.Date != messageDateTime.Date)
            {
                // 이전 날짜를 기록하여 경계선에 표시
                DateTime previousDate = oldestMessageDate.HasValue ? oldestMessageDate.Value.Date : messageDateTime.Date;

                // 기록한 이전 날짜를 기준으로 경계선 추가
                AddBeforeDateBoundaryPanel(previousDate);

                // 가장 오래된 날짜 갱신
                oldestMessageDate = messageDateTime.Date;
            }

            // 시간을 HH:mm 형식으로 변환
            string formattedTime = messageDateTime.ToString("HH:mm");

            if (sender_id == my_id)
            {
                Bubble myBubble = new Bubble
                {
                    Message = message,
                    BubbleColor = Color.LightBlue // 내 채팅 말풍선 색
                };

                // 시간 레이블 생성
                Label timeLabel = new Label
                {
                    Text = formattedTime,
                    AutoSize = true,
                    Font = new Font("Arial", 8, FontStyle.Regular), // 원하는 폰트 설정
                    ForeColor = Color.Gray // 시간 레이블 색상
                };

                Panel MessagePanel = new Panel
                {
                    Size = new Size(350, myBubble.Height + 10),
                    AutoSize = true,
                    Dock = DockStyle.Bottom // 위쪽에 쌓이도록 설정
                };

                myBubble.Location = new Point(350 - myBubble.Width, 6); // 5px 여백
                timeLabel.Location = new Point(350 - myBubble.Width - 35, myBubble.Height - 5); // 시간 레이블 위치 설정

                MessagePanel.Controls.Add(myBubble);
                MessagePanel.Controls.Add(timeLabel);

                // 기존 메시지의 위에 추가
                flpChat.Controls.Add(MessagePanel);
                flpChat.Controls.SetChildIndex(MessagePanel, 0); // 패널을 가장 위로 이동

            }
            else
            {
                Label nameLabel = new Label
                {
                    Text = friend_name,
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    ForeColor = Color.Black
                };


                // 시간 레이블 생성
                Label timeLabel = new Label
                {
                    Text = formattedTime,
                    AutoSize = true,
                    Font = new Font("Arial", 8, FontStyle.Regular), // 원하는 폰트 설정
                    ForeColor = Color.Gray // 시간 레이블 색상
                };

                Profile userProfile = new Profile
                {
                    Size = new Size(30, 30),
                    Location = new Point(0, 0)
                };

                userProfile.SetName(friend_name);

                Bubble othersBubble = new Bubble
                {
                    Message = message,
                    BubbleColor = Color.LightGreen
                };

                othersBubble.PerformLayout();
                int bubbleHeight = othersBubble.Height;
                int nameLabelHeight = nameLabel.Height;

                Panel MessagePanel = new Panel
                {
                    Size = new Size(350, Math.Max(bubbleHeight + nameLabelHeight, 41)),
                    MinimumSize = new Size(350, 30 + nameLabelHeight),
                    MaximumSize = new Size(350, 0),
                    AutoSize = true,
                    Dock = DockStyle.Top // 위쪽에 쌓이도록 설정
                };

                userProfile.Dock = DockStyle.Left;
                MessagePanel.Controls.Add(userProfile);

                nameLabel.Location = new Point(userProfile.Width + 5, 0);
                MessagePanel.Controls.Add(nameLabel);

                othersBubble.Location = new Point(userProfile.Width + 5, nameLabel.Height + 2);
                MessagePanel.Controls.Add(othersBubble);

                // 시간 레이블 위치 설정
                timeLabel.Location = new Point(userProfile.Width + othersBubble.Width + 5, othersBubble.Height + 5);
                MessagePanel.Controls.Add(timeLabel); // 메시지 패널에 시간 레이블 추가


                // 기존 메시지의 위에 추가
                flpChat.Controls.Add(MessagePanel);
                flpChat.Controls.SetChildIndex(MessagePanel, 0); // 패널을 가장 위로 이동

            }
        }

        // 마지막 메시지 패널을 추가하는 메서드
        private void AddLastMessagePanel()
        {
            var lastMessagePanel = new Panel
            {
                Size = new Size(350, 30),
                BackColor = Color.Transparent, // 배경색 투명
                Dock = DockStyle.Top // 위쪽으로 쌓이도록 설정
            };

            var lastMessageLabel = new Label
            {
                Text = "마지막 메시지입니다.",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.DarkGray
            };

            lastMessagePanel.Controls.Add(lastMessageLabel);
            flpChat.Controls.Add(lastMessagePanel);
            flpChat.Controls.SetChildIndex(lastMessagePanel, 0); // 마지막 메시지 패널을 가장 위로 이동
        }

        /*==============================================================================================================================================================
         *   @@@@날짜경계선@@@@
         * ============================================================================================================================================================*/

        // 메시지 수신시 날짜 경계선 패널 추가 메서드
        private void AddDateBoundaryPanel(DateTime date)
        {
            Panel dateBoundaryPanel = new Panel
            {
                Size = new Size(350, 30),
                BackColor = Color.Transparent, // 배경색 투명
                Dock = DockStyle.Bottom
            };

            Label dateLabel = new Label
            {
                Text = date.ToString("yyyy년 MM월 dd일"),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.DarkGray
            };

            dateBoundaryPanel.Controls.Add(dateLabel);
            flpChat.Controls.Add(dateBoundaryPanel);
        }

        // 이전 메시지 불러올때 날짜 경계선 패널 추가 메서드
        private void AddBeforeDateBoundaryPanel(DateTime date)
        {
            Panel dateBoundaryPanel = new Panel
            {
                Size = new Size(350, 30),
                BackColor = Color.Transparent, // 배경색 투명
                Dock = DockStyle.Top // 위쪽에 쌓이도록 설정
            };

            Label dateLabel = new Label
            {
                Text = date.ToString("yyyy년 MM월 dd일"),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.DarkGray
            };

            dateBoundaryPanel.Controls.Add(dateLabel);

            // 날짜 경계선을 flpChat의 가장 위에 추가
            flpChat.Controls.Add(dateBoundaryPanel);
            flpChat.Controls.SetChildIndex(dateBoundaryPanel, 0); // 날짜 패널을 가장 위로 이동
        }

        /*==============================================================================================================================================================
         *   @@@@읽은 메시지 상태 변경 (is_read 값으로 변환)@@@@
         * ============================================================================================================================================================*/

        private void UpdateReadStatus(SQLiteConnection connection, List<(string senderId, string messageText, string messageTime, int messageId)> messages)
        {
            // is_read 값을 1로 업데이트하는 쿼리
            string updateQuery = @"
                UPDATE messagetbl
                SET is_read = 1
                WHERE message_id = @message_id";

            foreach (var message in messages)
            {
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@message_id", message.messageId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /*==============================================================================================================================================================
         *   @@@@수신받은 메시지 받아오기@@@@
         * ============================================================================================================================================================*/

        public void PassMessage(int roomId, string message_data)
        {
            try
            {
                // 서버로부터 받은 메시지를 정리 (null 문자 제거)
                string cleanedMessage = message_data.Replace("\0", string.Empty);

                if (!string.IsNullOrEmpty(cleanedMessage))
                {
                    JObject add_json = JObject.Parse(cleanedMessage);

                    // JArray에서 메시지 배열을 가져옴
                    JArray messagesArray = (JArray)add_json["message_text"];

                    foreach (var message in messagesArray)
                    {
                        string senderId = message["message_user_id"].ToString();
                        string messageText = message["message_text"].ToString();
                        string messageTimestamp = message["message_timestamp"]?.ToString(); // 타임스탬프 가져오기
                        DateTime messageTime = DateTime.Now; // 기본값

                        AddMessage(senderId, messageText, messageTimestamp);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Add_Message() 오류: {ex.Message}");
            }
        }

        private void tbChat_TextChanged(object sender, EventArgs e)
        {
            int maxCharacters = 250;
            int characterCount = tbChat.Text.Length;
            int remainingCharacters = maxCharacters - characterCount;
            lblTextCnt.Text = $"최대 250자({characterCount} / {maxCharacters})";
        }


        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // 엔터키 입력을 폼에서 처리했음을 명시
                btnSend.PerformClick(); // 로그인 버튼 클릭
            }
        }

        private void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // SendForm에서 해당 폼의 ID를 통해 딕셔너리에서 제거하는 메서드 호출
            MainForm parentForm = (MainForm)this.Owner;

            // 부모 폼이 null인지 체크
            if (parentForm != null)
            {
                parentForm.RemoveChatForm(room_id);
            }
            else
            {
                // 소유자가 null일 때 로그 메시지 추가
                MessageBox.Show($"소유자가 설정되지 않았습니다. 방 ID: {room_id}");
            }
        }
    }
}
