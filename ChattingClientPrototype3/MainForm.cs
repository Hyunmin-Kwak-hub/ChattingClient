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
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Timers;


namespace ChattingClientPrototype3
{
    public partial class MainForm : Form
    {
        private Socket clientSocket;

        private string users_path = Application.StartupPath + @"\userList.json"; // 사용자 정보 저장 경로

        private string dbPath = Application.StartupPath + @"\chat_Database.db"; // 데이터베이스 파일 경로

        private Dictionary<int, ChatForm> chatFormsDict = new Dictionary<int, ChatForm>();

        private JArray userList; // userList 변수를 클래스 내에서 정의

        public event Action MessagesUpdated; // 이벤트 정의

        private string loggedInUserId;
        private int roomId;
        private string friend_id;

        private System.Timers.Timer connectionCheckTimer;
        private readonly int checkInterval = 30000; // 60초마다 체크

        // 대용량 패킷을 처리하기 위한 상태 변수
        private int totalMessageSize = 0;  // 총 메시지 크기
        private int receivedMessageSize = 0;  // 현재까지 받은 메시지 크기
        private List<byte> messageBuffer = new List<byte>(); // 대용량 메시지를 위한 버퍼
        private byte[] recvBuffer = new byte[1024]; // 대용량 수신시 수신 버퍼
        private LoginForm loginForm;
        private bool isLoggingOut = false; // 로그아웃 상태 변수

        private Thread receiveThread;
        private Thread sendThread;
        private bool isSendThreadRunning = false; // 송신 스레드가 실행 중인지 여부
        private Queue<(int roomId, string message)> sendQueue = new Queue<(int, string)>();
        private ManualResetEvent sendSignal = new ManualResetEvent(false); // 메시지 전송 신호
        


        public MainForm(string userId, Socket clientSocket, LoginForm loginForm)
        {
            InitializeComponent();

            this.clientSocket = clientSocket; // 로그인 폼에서 전달받은 소켓 저장

            this.loginForm = loginForm; // 전달받은 로그인 폼 저장

            loggedInUserId = userId;

            // pnChatList를 처음에는 보이지 않게 설정
            pnChatList.Visible = false;

            LoadFriendList(); // 폼이 로드될 때 친구 목록을 로드

            this.Text = $"NSTalk"; // 폼 제목

            StartReceiveThread();

            lvChatList.ColumnWidthChanging += LvChatList_ColumnWidthChanging; // 열너비 고정 이벤트

            LoadChatList(); // 폼이 로드될 때 채팅 목록을 로드
/*
            // 소켓연결 핑 체크
            connectionCheckTimer = new System.Timers.Timer(checkInterval);
            connectionCheckTimer.Elapsed += CheckConnection;
            connectionCheckTimer.AutoReset = true;
            connectionCheckTimer.Enabled = true;
*/

            lbFriends.DoubleClick += LbFriends_DoubleClick;
            lbMyself.DoubleClick += LbMyself_DoubleClick;

            // 친구목록, 채팅목록 변환 이벤트핸들러 추가
            pbFriends.Click += PbFriends_Click;
            pbChats.Click += PbChats_Click;

            lvChatList.MouseClick += lvChatList_MouseClick; // 채팅목록 클릭 이벤트

            // 전송 버튼 클릭 이벤트
            btnLogout.Click += new EventHandler(BtnLogout_Click);

            this.FormClosed += MainForm_FormClosed; // FormClosed 이벤트

        }
        /*==============================================================================================================================================================
         *   @@@@스레드 시작@@@@
         * ============================================================================================================================================================*/

        private void StartReceiveThread()
        {
            // 리시브 스레드 시작
            receiveThread = new Thread(new ThreadStart(Begin_recv));
            receiveThread.IsBackground = true; // 백그라운드 스레드로 설정
            receiveThread.Start();
        }
        private void StartSendThread()
        {
            if (!isSendThreadRunning) // 스레드가 이미 실행 중이지 않은 경우에만 시작
            {
                sendThread = new Thread(new ThreadStart(SendMessagesFromQueue));
                sendThread.IsBackground = true; // 백그라운드 스레드로 설정
                sendThread.Start();
                isSendThreadRunning = true; // 스레드가 실행 중임을 표시
            }
        }

        /*==============================================================================================================================================================
         *   @@@@ 친구 목록 로드@@@@
         * ============================================================================================================================================================*/

        private void LoadFriendList()
        {
            try
            {
                // JSON 파일 읽기
                JArray userList = UserList.LoadUserList(users_path);

                // 사용자 정보 리스트로 변환 후 이름순 정렬
                var userListData = userList
                    .Select(u => new
                    {
                        UserId = (u["user_id"] != null) ? u["user_id"].ToString() : string.Empty,
                        UserName = (u["user_name"] != null) ? u["user_name"].ToString() : string.Empty,
                        UserNicname = (u["user_nickname"] != null) ? u["user_nickname"].ToString() : string.Empty
                    })
                    .ToList();

                // 로그인한 사용자의 정보를 찾음
                var loggedInUser = userListData
                    .FirstOrDefault(u => u.UserId == loggedInUserId);

                // 로그인한 사용자가 있을 경우 lbMyself에 추가
                lbMyself.Items.Clear(); // 초기화
                if (loggedInUser != null)
                {
                    lbMyself.Items.Add(new ListBoxItem
                    {
                        Display = $"{loggedInUser.UserName}({loggedInUser.UserNicname})",
                        UserId = loggedInUser.UserId
                    });
                }

                // 로그인한 사용자를 제외한 친구 목록을 이름순으로 정렬
                var sortedFriendList = userListData
                    .Where(u => u.UserId != loggedInUserId && !string.IsNullOrEmpty(u.UserId)) // 로그인한 사용자 제외
                    .OrderBy(u => u.UserName, StringComparer.CurrentCulture) // 이름순 정렬
                    .ToList();

                // ListBox에 추가
                lbFriends.Items.Clear(); // 초기화
                foreach (var user in sortedFriendList)
                {
                    lbFriends.Items.Add(new ListBoxItem
                    {
                        Display = $"{user.UserName}({user.UserNicname})",
                        UserId = user.UserId
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"친구 목록을 로드하는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class ListBoxItem
        {
            public string Display { get; set; }
            public string UserId { get; set; }

            public override string ToString() => Display; // 표시할 문자열 정의
        }


        /*==============================================================================================================================================================
         *   @@@@채팅방 생성 요청@@@@
         * ============================================================================================================================================================*/
        // 본인 채팅방 생성
        private void LbMyself_DoubleClick(object sender, EventArgs e)
        {
            byte[] sendBytes;
            byte[] messageBytes;
            try
            {
                if (clientSocket == null || !clientSocket.Connected)
                {
                    ShowConnectionClosedMessage();
                    MainForm_FormClosed(this, new FormClosedEventArgs(CloseReason.None));
                    return;
                }

                if (lbMyself.SelectedItem != null)
                {
                    ListBoxItem selectedItem = (ListBoxItem)lbMyself.SelectedItem;
                    friend_id = selectedItem.UserId;

                    // 내 ID를 가져옴
                    string my_id = loggedInUserId; // 현재 로그인된 사용자 ID

                    // DB에서 채팅방 ID 조회
                    int? room_id = GetChatRoomId(my_id, friend_id);

                    // room_id가 존재하는지 확인
                    if (room_id.HasValue)
                    {
                        // 채팅방 ID가 존재할 경우 해당 채팅방이 이미 열려 있는지 확인
                        if (chatFormsDict.ContainsKey(room_id.Value))
                        {
                            // 이미 존재하는 방의 경우 해당 폼을 가져와서 최상단으로 노출
                            ChatForm existingChatForm = chatFormsDict[room_id.Value];
                            existingChatForm.BringToFront();
                            existingChatForm.Activate();
                        }
                        else
                        {
                            messageBytes = Encoding.UTF8.GetBytes(friend_id);

                            // 채팅방 생성 요청 패킷 보내기
                            Packet cr_pkt = new Packet
                            {
                                packet_id = PacketCode.CREATE_CHAT_ROOM,
                                msg_size = messageBytes.Length,
                                message = friend_id
                            };

                            sendBytes = cr_pkt.packet_data();

                            clientSocket.Send(sendBytes);
                        }
                    }
                    else
                    {
                        // room_id가 null인 경우, 즉 채팅방이 없을 때 채팅방 생성 요청 패킷 보내기
                        messageBytes = Encoding.UTF8.GetBytes(friend_id);

                        // 채팅방 생성 요청 패킷
                        Packet cr_pkt = new Packet
                        {
                            packet_id = PacketCode.CREATE_CHAT_ROOM,
                            msg_size = messageBytes.Length,
                            message = friend_id
                        };

                        sendBytes = cr_pkt.packet_data();

                        clientSocket.Send(sendBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"친구 선택 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        // 친구와 채팅방 생성
        private void LbFriends_DoubleClick(object sender, EventArgs e)
        {
            byte[] sendBytes;
            byte[] messageBytes;

            if (clientSocket == null || !clientSocket.Connected)
            {
                ShowConnectionClosedMessage();
                MainForm_FormClosed(this, new FormClosedEventArgs(CloseReason.None));
                return;
            }
            try
            {
                if (lbFriends.SelectedItem != null)
                {
                    ListBoxItem selectedItem = (ListBoxItem)lbFriends.SelectedItem;
                    friend_id = selectedItem.UserId;

                    // 내 ID를 가져옴
                    string my_id = loggedInUserId; // 현재 로그인된 사용자 ID

                    // DB에서 채팅방 ID 조회
                    int? room_id = GetChatRoomId(my_id, friend_id);

                    // room_id가 존재하는지 확인
                    if (room_id.HasValue)
                    {
                        // 채팅방 ID가 존재할 경우 해당 채팅방이 이미 열려 있는지 확인
                        if (chatFormsDict.ContainsKey(room_id.Value))
                        {
                            // 이미 존재하는 방의 경우 해당 폼을 가져와서 최상단으로 노출
                            ChatForm existingChatForm = chatFormsDict[room_id.Value];
                            existingChatForm.BringToFront();
                            existingChatForm.Activate();
                        }
                        else
                        {
                            messageBytes = Encoding.UTF8.GetBytes(friend_id);

                            // 채팅방 생성 요청 패킷 보내기
                            Packet cr_pkt = new Packet();

                            cr_pkt.packet_id = PacketCode.CREATE_CHAT_ROOM;
                            cr_pkt.msg_size = messageBytes.Length;
                            cr_pkt.message = friend_id;

                            sendBytes = cr_pkt.packet_data();

                            clientSocket.Send(sendBytes);
                        }
                    }
                    else
                    {
                        // room_id가 null인 경우, 즉 채팅방이 없을 때 채팅방 생성 요청 패킷 보내기
                        messageBytes = Encoding.UTF8.GetBytes(friend_id);

                        // 채팅방 생성 요청 패킷
                        Packet cr_pkt = new Packet
                        {
                            packet_id = PacketCode.CREATE_CHAT_ROOM,
                            msg_size = messageBytes.Length,
                            message = friend_id
                        };

                        sendBytes = cr_pkt.packet_data();

                        clientSocket.Send(sendBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"친구 선택 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        /*==============================================================================================================================================================
         *   @@@@메시지 전송을 위한 스레드 처리@@@@
         * ============================================================================================================================================================*/
        private void SendMessagesFromQueue()
        {
            while (true)
            {
                sendSignal.WaitOne(); // 메시지 대기
                lock (sendQueue)
                {
                    while (sendQueue.Count > 0)
                    {
                        var (roomId, message) = sendQueue.Dequeue();
                        SendMessageToServer(roomId, message);
                    }
                }
                sendSignal.Reset(); // 대기 상태로 전환
            }
        }
        private void QueueMessage(int roomId, string message)
        {
            lock (sendQueue)
            {
                sendQueue.Enqueue((roomId, message));
            }
            sendSignal.Set(); // 메시지가 큐에 추가되면 신호 전송
        }

        /*==============================================================================================================================================================
         *   @@@@메시지 전송@@@@
         * ============================================================================================================================================================*/
        public void SendMessage(int roomId, string message)
        {
            if (clientSocket == null || !clientSocket.Connected)
            {
                ShowConnectionClosedMessage();
                MainForm_FormClosed(this, new FormClosedEventArgs(CloseReason.None));
                return;
            }
            // 메시지를 큐에 넣고 전송 스레드에게 신호를 보냄
            QueueMessage(roomId, message);

            // 스레드가 아직 시작되지 않았으면 시작
            if (!isSendThreadRunning)
            {
                StartSendThread(); // 센드 스레드 시작
            }

        }

        public void SendMessageToServer(int chatroom_id, string message)
        {
            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                Packet msg_pkt = new Packet
                {
                    packet_id = PacketCode.CHAT_MESSAGE,
                    msg_size = messageBytes.Length,
                    room_id = chatroom_id,
                    message = message
                };

                byte[] sendBytes = msg_pkt.packet_data();
                clientSocket.Send(sendBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        /*==============================================================================================================================================================
         *   @@@@메시지 수신@@@@
         * ============================================================================================================================================================*/

        private void Begin_recv()
        {
            byte[] headerBuffer = new byte[12]; // 헤더 버퍼 (12바이트)
            if (clientSocket.Connected)
            {
                clientSocket.BeginReceive(headerBuffer, 0, headerBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), headerBuffer);
            }
            else
            {
                ShowConnectionClosedMessage();
            }

        }

        private void OnReceive(IAsyncResult ar)
        {
            if (clientSocket == null || !clientSocket.Connected)
            {
                ShowConnectionClosedMessage();
                MainForm_FormClosed(this, new FormClosedEventArgs(CloseReason.None));
                return;
            }

            try
            {
                
                byte[] headerBuffer = (byte[])ar.AsyncState;
                int bytesRead = clientSocket.EndReceive(ar);

                // 수신된 바이트 수가 12바이트 미만인 경우 다 받을때까지 수신 반복
                while (bytesRead < 12)
                {
                    int additionalBytes = clientSocket.Receive(headerBuffer, bytesRead, 12 - bytesRead, SocketFlags.None);
                    if (additionalBytes == 0)
                    {
                        throw new Exception("소켓이 닫혔습니다.");
                    }
                    bytesRead += additionalBytes; // 수신한 바이트 수를 업데이트

                }

                // 패킷정보 추출
                Packet recv_pkt = new Packet();
                recv_pkt.packet_id = BitConverter.ToInt32(headerBuffer, 0); // 패킷 ID
                recv_pkt.msg_size = BitConverter.ToInt32(headerBuffer, 4); // 메시지 사이즈
                recv_pkt.room_id = BitConverter.ToInt32(headerBuffer, 8); // 채팅방 ID


                // 메시지 크기만큼 바이트가 수신되었는지 확인
                if (recv_pkt.msg_size > 1012)
                {

                    ReceiveLargeMessage(recv_pkt.msg_size, largeMessageBytes =>
                    {
                        // 수신 완료 후 처리
                        string completeMessage = Encoding.UTF8.GetString(largeMessageBytes);
                        Packet large_pkt = new Packet
                        {
                            packet_id = recv_pkt.packet_id,
                            msg_size = recv_pkt.msg_size,
                            room_id = recv_pkt.room_id,
                            message = completeMessage
                        };

                        // 패킷 처리
                        ProcessPacket(large_pkt);

                    });
                }
                else
                {
                    // 일반 메시지 수신
                    byte[] bodyBuffer = new byte[recv_pkt.msg_size];
                    int totalBodyBytesRead = 0;

                    // 바디 전체가 수신될 때까지 반복
                    while (totalBodyBytesRead < recv_pkt.msg_size)
                    {
                        int bodyBytes = clientSocket.Receive(bodyBuffer, totalBodyBytesRead, recv_pkt.msg_size - totalBodyBytesRead, SocketFlags.None);
                        if (bodyBytes == 0)
                        {
                            throw new Exception("소켓이 닫혔습니다.");
                        }
                        totalBodyBytesRead += bodyBytes;
                    }
                

                    // 수신된 메시지 처리
                    recv_pkt.message = Encoding.UTF8.GetString(bodyBuffer);

                    ProcessPacket(recv_pkt);// 일반 패킷 처리

                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionReset || ex.SocketErrorCode == SocketError.HostUnreachable)
                {
                    MessageBox.Show($"서버 연결이 끊어졌습니다: {ex.Message}");
                    // 재연결 로직
                }
                else
                {
                    MessageBox.Show($"소켓 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"I/O 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Recv_Code 에러: {ex.Message}");
            }
        }

        /*==============================================================================================================================================================
         *   @@@@대용량 메시지 수신@@@@
         * ============================================================================================================================================================*/

        // 동기 방식으로 
        private void ReceiveLargeMessage(int totalSize, Action<byte[]> onComplete)
        {
            int stack_msgSize = 0;
            byte[] largeMessageBytes = new byte[totalSize];
            try
            {
                // 데이터를 전부 받을때 까지 반복
                while (stack_msgSize < totalSize)
                {
                    byte[] recvBytes = new byte[12];

                    int headerBytesRead = 0;

                    // 헤더를 12바이트 받기 위한 반복
                    while (headerBytesRead < 12)
                    {
                        int bytesRead = clientSocket.Receive(recvBytes, headerBytesRead, 12 - headerBytesRead, SocketFlags.None);
                        if (bytesRead == 0)
                        {
                            throw new Exception("소켓이 닫혔습니다.");
                        }
                        headerBytesRead += bytesRead; // 수신한 바이트 수를 업데이트
                    }

                    // 헤더 처리
                    Packet recv_pkt = new Packet();
                    recv_pkt.packet_id = BitConverter.ToInt32(recvBytes, 0);
                    recv_pkt.msg_size = BitConverter.ToInt32(recvBytes, 4); // 현재 패킷의 메시지 크기
                    recv_pkt.room_id = BitConverter.ToInt32(recvBytes, 8);

                    // 현재 메시지를 리시브 하기 위한 byte[] 생성
                    byte[] recv_msgBytes = new byte[recv_pkt.msg_size];

                    // 현재 메시지의 크기만큼 메시지 데이터 수신
                    int msgBytesRead = 0;
                    while (msgBytesRead < recv_pkt.msg_size)
                    {
                        int bytesRead = clientSocket.Receive(recv_msgBytes, msgBytesRead, recv_pkt.msg_size - msgBytesRead, SocketFlags.None);
                        if (bytesRead == 0)
                        {
                            throw new Exception("소켓이 닫혔습니다.");
                        }
                        msgBytesRead += bytesRead; // 수신한 메시지 바이트 수를 업데이트
                    }

                    // 메시지를 largeMessageBytes에 복사
                    Buffer.BlockCopy(recv_msgBytes, 0, largeMessageBytes, stack_msgSize, recv_pkt.msg_size);


                    stack_msgSize += recv_pkt.msg_size; // 누적 메시지 증가
                }
            }
            
            catch (Exception ex)
            {
                MessageBox.Show($"대용량 메시지 수신 오류 {ex.Message}");
                Begin_recv();
            }
            // 메시지 수신 완료 후 콜백 호출
            onComplete(largeMessageBytes);
        }
        /*==============================================================================================================================================================
         *   @@@@받은 패킷을 처리하는 부분@@@@
         * ============================================================================================================================================================*/

        private void ProcessPacket(Packet recv_pkt)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ProcessPacket(recv_pkt)));
                return;
            }
            int room_id = 0;
            string otherUserId = friend_id;

            switch (recv_pkt.packet_id)
            {
                case PacketCode.SUCCESS_CREATE_ROOM:

                    room_id = recv_pkt.room_id;

                    SaveChatRoom(room_id, loggedInUserId, otherUserId); // 채팅방 정보 저장

                    // 채팅방 열기
                    OpenChatRoom(room_id, otherUserId);

                    Begin_recv();

                    break;

                case PacketCode.EXISTS_CREATE_ROOM:

                    room_id = recv_pkt.room_id;
                    string previousMessagesJson = recv_pkt.message; // 이전 채팅 내용이 담긴 Json 형식.

                    SaveChatRoom(room_id, loggedInUserId, otherUserId); // 채팅방 정보 저장

                    string cleanMessage = previousMessagesJson.Replace("\0", string.Empty);
                    if (cleanMessage != string.Empty)
                    {
                        SavePreviousMessageToData(room_id, previousMessagesJson); // 이전 메시지 저장
                    }

                    OpenChatRoom(room_id, otherUserId);

                    Begin_recv();

                    break;

                case PacketCode.FAILED_CREATE_ROOM:

                    MessageBox.Show("채팅방 생성에 실패했습니다.", "실패", MessageBoxButtons.OK);

                    Begin_recv();

                    break;

                case PacketCode.CHAT_MESSAGE:

                    room_id = recv_pkt.room_id;
                    string message = recv_pkt.message; // 수신한 메시지를 가져옵니다.

                    SendMessageToRoom(room_id, message); // 수신 메시지 채팅 폼에 전달

                    Begin_recv();

                    break;

                case PacketCode.SUCCESS_SEND_MESSAGE:

                    room_id = recv_pkt.room_id;
                    string Sendmessage = recv_pkt.message; // 수신한 메시지를 가져옵니다.

                    SendMessageToRoom(room_id, Sendmessage); // 수신 메시지 채팅 폼에 전달

                    Begin_recv();

                    break;

                case PacketCode.FAILED_SEND_MESSAGE:

                    MessageBox.Show("메시지 전송에 실패했습니다.", "실패", MessageBoxButtons.OK);

                    Begin_recv();

                    break;

                case PacketCode.LOGOUT:

                    isLoggingOut = true; // 로그아웃 상태로 설정

                    // 메인폼을 닫기
                    this.Close();

                    // 로그인 폼 다시 보이기
                    loginForm.Show();

                    break;

            }

            friend_id = null;
            roomId = 0;
            
        }
        /*==============================================================================================================================================================
         *   @@@@채팅방 생성@@@@
         * ============================================================================================================================================================*/
        // 채팅방 생성
        private void OpenChatRoom(int room_id, string  friend_id)
        {
            if (!chatFormsDict.ContainsKey(room_id))
            {
                ChatForm newChatForm = new ChatForm(room_id, loggedInUserId, friend_id, this);
                chatFormsDict.Add(room_id, newChatForm);
                newChatForm.Owner = this; // 소유자 설정

                // 메시지 업데이트 이벤트 구독
                newChatForm.MessagesUpdated += (sender, e) => UpdateChatList();

                newChatForm.Show();
            }
            else
            {
                // 이미 존재하는 방의 경우 해당 폼을 가져와서 최상단으로 노출
                ChatForm existingChatForm = chatFormsDict[room_id];
                existingChatForm.BringToFront();
                existingChatForm.Activate();
            }
        }

        /*==============================================================================================================================================================
         *   @@@@채팅방 데이터 roomtbl에 저장@@@@
         * ============================================================================================================================================================*/

        private void SaveChatRoom(int room_id, string my_id, string friend_id)
        {
            using (var connection = new SQLiteConnection("Data Source=" + Path.Combine(Application.StartupPath, "chat_database.db")))
            {
                connection.Open();

                // room_id가 이미 존재하는지 확인하는 쿼리
                string checkRoomQuery = "SELECT COUNT(*) FROM roomtbl WHERE room_id = @room_id";

                using (var checkCommand = new SQLiteCommand(checkRoomQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@room_id", room_id);
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    // room_id가 존재하지 않을 때만 삽입
                    if (count == 0)
                    {
                        string insertRoomQuery = "INSERT INTO roomtbl (room_id, loggedinuser_id, otheruser_id) VALUES (@room_id, @loggedInUserId, @otherUserId)";

                        using (var insertCommand = new SQLiteCommand(insertRoomQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@room_id", room_id);
                            insertCommand.Parameters.AddWithValue("@loggedInUserId", my_id);
                            insertCommand.Parameters.AddWithValue("@otherUserId", friend_id);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /*==============================================================================================================================================================
         *   @@@@roomtbl에서 채팅방 ID 찾기@@@@
         * ============================================================================================================================================================*/

        private int? GetChatRoomId(string my_id, string friend_id)
        {
            using (var connection = new SQLiteConnection("Data Source=" + Path.Combine(Application.StartupPath, "chat_database.db")))
            {
                connection.Open();

                // roomtbl에서 해당 친구와 나의 ID로 room_id 조회
                string query = @"
            SELECT room_id 
            FROM roomtbl 
            WHERE (loggedinuser_id = @my_id AND otheruser_id = @friend_id) 
            OR (loggedinuser_id = @friend_id AND otheruser_id = @my_id)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@my_id", my_id);
                    command.Parameters.AddWithValue("@friend_id", friend_id);

                    var result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : (int?)null; // room_id가 없으면 null 반환
                }
            }
        }


        /*==============================================================================================================================================================
         *   @@@@수신한 메시지 채팅방에 전달@@@@
         * ============================================================================================================================================================*/
        private void SendMessageToRoom(int room_id, string message)
        {

            // 딕셔너리에 해당 ID를 가진 폼이 있는지 확인
            if (chatFormsDict.ContainsKey(room_id))
            {
                // 해당 ID의 폼을  찾아와서 상호작용
                ChatForm targetForm = chatFormsDict[room_id];

                targetForm.PassMessage(room_id, message); // 메시지를 채팅폼에 전달

                SaveMessageToData(room_id, message, true); // 메시지 저장
            }
            else
            {
                SaveMessageToData(room_id, message, false); // 메시지 저장
            }
        }

        /*==============================================================================================================================================================
         *   @@@@채팅방 제거@@@@
         * ============================================================================================================================================================*/

        public void RemoveChatForm(int roomId)
        {
            if (chatFormsDict.ContainsKey(roomId))
            {
                chatFormsDict.Remove(roomId);
            }
        }


        /*==============================================================================================================================================================
         *   @@@@이전 채팅 저장@@@@
         * ============================================================================================================================================================*/
        // 이전 채팅 저장
        private void SavePreviousMessageToData(int room_id, string jsonData)
        {
            try
            {
                // JSON 데이터 파싱
                JObject jsonObject = JObject.Parse(jsonData);
                JArray messages = (JArray)jsonObject["message_text"];

                // DB에서 해당 채팅방(room_id)의 가장 최근 타임스탬프를 불러오기
                DateTime latestTimestamp = DateTime.MinValue;
                // jsonData 파싱 및 데이터베이스에 저장
                using (var connection = new SQLiteConnection("Data Source=" + Path.Combine(Application.StartupPath, "chat_database.db")))
                {
                    connection.Open();

                    // 채팅방에서 가장 최근 메시지 시간 가져오기
                    using (var command = new SQLiteCommand("SELECT MAX(message_time) FROM messagetbl WHERE room_id = @room_id", connection))
                    {
                        command.Parameters.AddWithValue("@room_id", room_id);
                        var result = command.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            latestTimestamp = Convert.ToDateTime(result);
                        }
                    }

                    // 메시지 반복 처리
                    foreach (var item in messages)
                    {
                        string messageText = item["message_text"]?.ToString();
                        string senderId = item["message_user_id"]?.ToString();

                        // 서버에서 받은 timestamp를 DateTime으로 변환
                        string timestamp = item["message_timestamp"]?.ToString();

                        if (DateTime.TryParse(timestamp, out DateTime messageTime))
                        {
                            // 가장 최근에 저장된 시간 이후의 메시지만 저장
                            if (messageTime > latestTimestamp)
                            {
                                using (var insertCommand = new SQLiteCommand("INSERT INTO messagetbl (room_id, message_sender_id, message_text, message_time, is_read) VALUES (@room_id, @message_sender_id, @message_text, @message_time, @is_read)", connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@room_id", room_id);
                                    insertCommand.Parameters.AddWithValue("@message_sender_id", senderId);
                                    insertCommand.Parameters.AddWithValue("@message_text", messageText);
                                    insertCommand.Parameters.AddWithValue("@message_time", messageTime);
                                    insertCommand.Parameters.AddWithValue("@is_read", 0); // 기본값: 읽지 않음
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (JsonReaderException ex) // JSON 파싱 오류 처리
            {
                MessageBox.Show("JSON 데이터 파싱 중 오류가 발생했습니다: " + ex.Message);
                // 예외 발생 시 적절한 처리를 여기에 추가할 수 있습니다.
                Begin_recv();
            }
            catch (Exception ex) // 다른 예외 처리
            {
                MessageBox.Show("오류가 발생했습니다: " + ex.Message);
                Begin_recv();
            }
        }

        /*==============================================================================================================================================================
         *   @@@@수신한 채팅 저장@@@@
         * ============================================================================================================================================================*/
        private void SaveMessageToData(int room_id, string message_Data, bool isRead)
        {
            try
            {
                // 서버로부터 받은 메시지를 정리
                string cleanedMessage = message_Data.Replace("\0", string.Empty);

                if (!string.IsNullOrEmpty(cleanedMessage))
                {
                    JObject add_Json = JObject.Parse(cleanedMessage);

                    // JArray에서 메시지 배열을 가져옴
                    JArray messageArray = (JArray)add_Json["message_text"];

                    using (var connection = new SQLiteConnection($"Data Source=" + Path.Combine(Application.StartupPath, "chat_database.db")))
                    {
                        connection.Open();

                        foreach (var message in messageArray)
                        {
                            string senderId = message["message_user_id"].ToString();
                            string messageText = message["message_text"].ToString();
                            string messageTimestamp = message["message_timestamp"]?.ToString();
                            DateTime messageTime = DateTime.Now; // 기본값

                            // 서버에서 받은 타임스탬프가 유효하면 변환하여 사용
                            if (DateTime.TryParse(messageTimestamp, out DateTime parsedTime))
                            {
                                messageTime = parsedTime;
                            }

                            // 메시지 삽입 쿼리
                            using (var command = new SQLiteCommand("INSERT INTO messagetbl (room_id, message_sender_id, message_text, message_time, is_read) VALUES (@room_id, @message_sender_id, @message_text, @message_time, @is_read)", connection))
                            {
                                command.Parameters.AddWithValue("@room_id", room_id);
                                command.Parameters.AddWithValue("@message_sender_id", senderId);
                                command.Parameters.AddWithValue("@message_text", messageText);
                                command.Parameters.AddWithValue("@message_time", messageTime); // message_time 매개변수 추가
                                command.Parameters.AddWithValue("@is_read", isRead ? 1:0); // 기본값: 읽지 않음

                                command.ExecuteNonQuery(); // SQL 명령 실행
                            }
                            // 메시지가 수신된 후 해당 채팅방 정보가 없다면 roomtbl에 저장
                            if (!RoomExists(room_id))
                            {
                                string otherUserId = senderId; // 메시지 보낸 사람을 상대방 아이디로 설정
                                SaveRoomToData(room_id, otherUserId);
                            }

                            UpdateChatList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"메시지 저장 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Begin_recv();
            }
        }

        // 수신메시지 채팅방 정보 저장
        private void SaveRoomToData(int room_id, string otherUserId)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data Source=" + Path.Combine(Application.StartupPath, "chat_database.db")))
                {
                    connection.Open();

                    // 채팅방이 존재 하는지 확인
                    using (var checkCommand = new SQLiteCommand("SELECT COUNT(*) FROM roomtbl WHERE room_id = @room_id", connection))
                    {
                        checkCommand.Parameters.AddWithValue("@room_id", room_id);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count == 0)
                        {
                            // roomtbl에 채팅방 정보가 없다면 새로 추가
                            using (var insertCommand = new SQLiteCommand("INSERT INTO roomtbl (room_id, loggedinuser_id, otheruser_id) VALUES (@room_id, @loggedinuser_id, @otheruser_id)", connection))
                            {
                                insertCommand.Parameters.AddWithValue("@room_id", room_id);
                                insertCommand.Parameters.AddWithValue("@loggedinuser_id", loggedInUserId);
                                insertCommand.Parameters.AddWithValue("@otheruser_id", otherUserId);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"채팅방 정보 저장 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // room_id에 해당하는 채팅방이 이미 존재하는지 확인하는 함수
        private bool RoomExists(int room_id)
        {
            using (var connection = new SQLiteConnection($"Data Source=" + Path.Combine(Application.StartupPath, "chat_database.db")))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT COUNT(*) FROM roomtbl WHERE room_id = @room_id", connection))
                {
                    command.Parameters.AddWithValue("@room_id", room_id);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        /*==============================================================================================================================================================
         *   @@@@친구목록, 채팅목록 변환 클릭 이벤트@@@@
         * ============================================================================================================================================================*/

        // pbFriends 클릭 이벤트 핸들러
        private void PbFriends_Click(object sender, EventArgs e)
        {
            // pnChatList를 숨깁니다.
            pnChatList.Visible = false;
        }

        // pbChats 클릭 이벤트 핸들러
        private void PbChats_Click(object sender, EventArgs e)
        {
            // pnChatList를 보이게 합니다.
            pnChatList.Visible = true;
        }

        // 열 너비 고정 이벤트 핸들러
        private void LvChatList_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true; // 크기 조정 취소
            e.NewWidth = lvChatList.Columns[e.ColumnIndex].Width; // 기존 너비로 설정
        }

        /*==============================================================================================================================================================
        *   @@@@채팅목록에 상대방 이름과 room_id를 저장하기 위한 부분@@@@
        * ============================================================================================================================================================*/

        private string GetOtherUserIdFromRoomId(int roomId)
        {
            using (var connection = new SQLiteConnection("Data Source=" + Path.Combine(Application.StartupPath, "chat_database.db")))
            {
                connection.Open();
                string query = "SELECT otheruser_id FROM roomtbl WHERE room_id = @roomId";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomId", roomId);
                    var result = command.ExecuteScalar();
                    return result != null ? result.ToString() : string.Empty; // 결과가 없으면 빈 문자열 반환
                }
            }
        }
        private string GetFriendNameFromUserList(string friendId)
        {
            JArray userList = UserList.LoadUserList(users_path);
            var friend = userList.FirstOrDefault(u => u["user_id"].ToString() == friendId);

            // 친구가 존재하는 경우 이름과 닉네임을 조합하여 반환
            if (friend != null)
            {
                string userName = friend["user_name"].ToString();
                string userNickname = friend["user_nickname"].ToString(); // 닉네임도 가져오기
                return $"{userName} ({userNickname})"; // "이름(닉네임)" 형식으로 반환
            }

            return "Unknown"; // 결과가 없으면 "Unknown" 반환
        }

        /*==============================================================================================================================================================
         *   @@@@ 채팅목록 불러오기 @@@@
         * ============================================================================================================================================================*/
        private void LoadChatList()
        {
            lvChatList.Items.Clear(); // 리스트뷰 초기화

            using (var connection = new SQLiteConnection("Data Source=" + Path.Combine(Application.StartupPath, "chat_database.db")))
            {
                connection.Open();

                string query = @"
                SELECT 
                    room_id, 
                    message_text, 
                    COUNT(CASE WHEN is_read = 0 THEN 1 END) AS unread_count 
                FROM 
                    messagetbl 
                WHERE 
                    room_id IS NOT NULL 
                GROUP BY 
                    room_id 
                ORDER BY 
                    MAX(message_time) DESC";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int roomId = reader.GetInt32(0);
                        string messageText = reader.GetString(1);
                        int unreadCount = reader.GetInt32(2);

                        // 메시지가 너무 길 경우 잘라내기
                        if (messageText.Length > 10)
                        {
                            messageText = messageText.Substring(0, 10) + "...";
                        }

                        // 상대방 아이디 가져오기
                        string friendId = GetOtherUserIdFromRoomId(roomId);
                        string friendName = GetFriendNameFromUserList(friendId); // 상대방 이름 가져오기

                        // 리스트뷰에 아이템 추가 (roomId는 비가시로 추가)
                        ListViewItem item = new ListViewItem(friendName); // 친구 이름으로 추가
                        item.SubItems.Add(messageText);
                        item.SubItems.Add(unreadCount.ToString());
                        item.SubItems.Add(friendId); // 친구 ID 추가 (비가시 항목)
                        item.SubItems.Add(roomId.ToString()); // 룸 ID 추가 (비가시 항목)
                        lvChatList.Items.Add(item);
                    }
                }
            }
        }

        /*==============================================================================================================================================================
        *   @@@@채팅목록 업데이트@@@@
        * ============================================================================================================================================================*/
        private void UpdateChatList()
        {
            LoadChatList();
        }

        /*==============================================================================================================================================================
        *   @@@@채팅목록 클릭이벤트@@@@
        * ============================================================================================================================================================*/

        // ListView 클릭 이벤트 핸들러
        private void lvChatList_MouseClick(object sender, MouseEventArgs e)
        {
            // 클릭한 위치에 항목이 있는지 확인
            ListViewItem item = lvChatList.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                // 클릭한 항목의 정보 가져오기
                string friendId = item.SubItems[item.SubItems.Count - 2].Text; // Friend ID (비가시 항목)
                int roomId = int.Parse(item.SubItems[item.SubItems.Count - 1].Text); // Room ID (비가시 항목)

                friend_id = friendId;

                // 내 ID를 가져옴
                string my_id = loggedInUserId; // 현재 로그인된 사용자 ID

                // DB에서 채팅방 ID 조회
                int? room_id = GetChatRoomId(my_id, friendId);

                // room_id가 존재하는지 확인
                if (room_id.HasValue)
                {
                    // 채팅방 ID가 존재할 경우 해당 채팅방이 이미 열려 있는지 확인
                    if (chatFormsDict.ContainsKey(room_id.Value))
                    {
                        // 이미 존재하는 방의 경우 해당 폼을 가져와서 최상단으로 노출
                        ChatForm existingChatForm = chatFormsDict[room_id.Value];
                        existingChatForm.BringToFront();
                        existingChatForm.Activate();
                    }
                    else
                    {
                        byte[] messageBytes = Encoding.UTF8.GetBytes(friendId);

                        // 채팅방 생성 요청 패킷 보내기
                        Packet cr_pkt = new Packet();

                        cr_pkt.packet_id = PacketCode.CREATE_CHAT_ROOM;
                        cr_pkt.msg_size = messageBytes.Length;
                        cr_pkt.message = friendId;

                        byte[] sendBytes = cr_pkt.packet_data();

                        clientSocket.Send(sendBytes);
                    }
                }
                else
                {
                    // room_id가 null인 경우, 즉 채팅방이 없을 때 채팅방 생성 요청 패킷 보내기
                    byte[] messageBytes = Encoding.UTF8.GetBytes(friendId);

                    // 채팅방 생성 요청 패킷
                    Packet cr_pkt = new Packet
                    {
                        packet_id = PacketCode.CREATE_CHAT_ROOM,
                        msg_size = messageBytes.Length,
                        message = friendId
                    };

                    byte[] sendBytes = cr_pkt.packet_data();

                    clientSocket.Send(sendBytes);
                }
            }
        }

        /*==============================================================================================================================================================
         *   @@@@소켓 연결 확인 타이머@@@@
         * ============================================================================================================================================================*/
        private void CheckConnection(object sender, ElapsedEventArgs e)
        {
            if (clientSocket == null || !clientSocket.Connected)
            {
                // 연결이 끊어진 경우 메시지 표시 후 종료
                Invoke((MethodInvoker)delegate
                {
                    ShowConnectionClosedMessage();
                    MainForm_FormClosed(this, new FormClosedEventArgs(CloseReason.None));
                });
            }
            else
            {
                try
                {
                    // 핑 메시지 전송
                    byte[] pingMessage = Encoding.UTF8.GetBytes("ping");
                    clientSocket.Send(pingMessage);
                }
                catch (SocketException)
                {
                    // 핑 전송에 실패하면 연결이 끊긴 것으로 간주
                    Invoke((MethodInvoker)delegate
                    {
                        ShowConnectionClosedMessage();
                        MainForm_FormClosed(this, new FormClosedEventArgs(CloseReason.None));
                    });
                }
            }
        }



        /*==============================================================================================================================================================
         *   @@@@로그아웃@@@@
         * ============================================================================================================================================================*/
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            byte[] LogoutBytes;
            byte[] idBytes;

            Packet logout_pkt = new Packet();
            logout_pkt.packet_id = PacketCode.LOGOUT;

            idBytes = Encoding.UTF8.GetBytes(loggedInUserId);

            logout_pkt.msg_size = idBytes.Length;
            logout_pkt.message = loggedInUserId;

            LogoutBytes = logout_pkt.packet_data();

            clientSocket.Send(LogoutBytes);

            return;
        }

        /*==============================================================================================================================================================
         *   @@@@소켓 연결 끊김 메시지@@@@
         * ============================================================================================================================================================*/

        private void ShowConnectionClosedMessage()
        {
            MessageBox.Show("연결이 끊겼습니다.");
        }

        /*==============================================================================================================================================================
         *   @@@@메인 폼 종료@@@@
         * ============================================================================================================================================================*/
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (components != null)
            {
                components.Dispose();
            }
            if (sendThread != null && sendThread.IsAlive)
            {
                sendThread.Abort(); // 전송 스레드 중지
            }

            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Abort(); // 수신 스레드 중지
            }

            if (!isLoggingOut) // 로그아웃 상태가 아닐때만 프로그램 종료
            {
                clientSocket.Close(); // 소켓 닫기
                Application.Exit();
            }
        }
    }
}
