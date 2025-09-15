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

namespace ChattingClientPrototype3
{
    public partial class LoginForm : Form
    {
        private Socket clientSocket; // 클라이언트 소켓

        private string serverIP; // 서버 IP
        private int port; // 서버 포트번호

        private string users_path = Application.StartupPath + @"\userList.json"; // 사용자 정보 저장 경로

        private string userId;
        private string password;
        private const int timeout = 5000; // 연결 타임아웃 (밀리초)
        private string logindata;
        public LoginForm()
        {
            InitializeComponent();

            this.Text = $"Login"; // 폼 제목

            this.Load += new EventHandler(LoginForm_Load); // 폼 로드시 이벤트 연결
            this.FormClosed += new FormClosedEventHandler(LoginForm_FormClosed); // 폼 종료 시 이벤트 연결

            btnLogin.Click += new EventHandler(BtnLogin_Click);
            btnTogglePassword.Click += new EventHandler(BtnTogglePassword_Click);

            // 텍스트박스에서 엔터키를 눌렀을때 이벤트 연결
            tbId.KeyDown += new KeyEventHandler(tb_KeyDown);
            tbPw.KeyDown += new KeyEventHandler(tb_KeyDown);

            this.Shown += new EventHandler(Focusing_Textbox); // 폼 로드시 이벤트 연결

        }
        /*========================================================================================================================================================================   
                 @@@@서버 연결 부분@@@@
        =========================================================================================================================================================================*/
        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (LoadConfig()) // 서버 설정이 정상적으로 로드되었을 때만 서버에 연결 시도
            {
                if(ConnectToServer())
                {
                    // 자동 로그인 정보 불러오기
                    LoadLoginInfo();

                }
                else
                {
                    MessageBox.Show("서버 연결에 실패했습니다. 프로그램을 종료합니다.", "실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit(); // 프로그램 종료
                }
            }
        }

        private bool LoadConfig()
        {
            try
            {
                // config.json 파일 경로
                string configPath = "config.json";

                // 파일이 존재하는지 확인
                if (File.Exists(configPath))
                {
                    // json 파일 읽기 및 파싱
                    string jsonContent = File.ReadAllText(configPath);
                    var config = JsonConvert.DeserializeObject<ServerConfig>(jsonContent);

                    // 서버IP와 포트 설정 (null 체크)
                    if (string.IsNullOrWhiteSpace(config.IpAddress) || config.Port <= 0)
                    {
                        throw new InvalidDataException("설정 파일에 잘못된 정보가 있습니다.");
                    }

                    serverIP = config.IpAddress;
                    port = config.Port;
                    return true; // 설정 로드 성공
                }
                else
                {
                    throw new FileNotFoundException("config.json 파일을 찾을 수 없습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정 파일을 불러오는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return false;
            }
        }

        private bool ConnectToServer()
        {
            try
            {
                // TCP 클라이언트 소켓 생성
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //서버의 IP와 포트 지정
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);

                // 비동기 소켓 연결 및 타임아웃 설정
                IAsyncResult result = clientSocket.BeginConnect(remoteEndPoint, null, null);

                // 연결 시도, 지정된 타임아웃 시간만큼 기다림
                bool success = result.AsyncWaitHandle.WaitOne(timeout);

                if (!success)
                {
                    // 타임아웃 발생 시 연결 취소 및 예외 처리
                    clientSocket.Close();
                    return false; // 연결 실패
                }

                // 연결 완료
                clientSocket.EndConnect(result);
                return true; // 연결 성공
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"서버에 연결할 수 없습니다: {ex.Message}", "연결실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // 연결 실패
            }
        }
        public class ServerConfig
        {
            public string IpAddress { get; set; }
            public int Port { get; set; }
            public string UserId { get; set; } // 추가
            public string UserPassword { get; set; } // 추가
        }
/*========================================================================================================================================================================   
         @@@@저장된 로그인 정보 불러오기@@@@
// 시간되면 config.json에서 가져올때 서버 정보랑 로그인 정보 한번에 처리해보기.
=========================================================================================================================================================================*/

        private void LoadLoginInfo()
        {
            try
            {
                // config.json 파일 경로
                string configPath = "config.json";

                string jsonContent = File.ReadAllText(configPath);
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(jsonContent);

                // UserId와 UserPassword가 존재하는지 확인하고 텍스트박스 입력
                if (!string.IsNullOrWhiteSpace(config.UserId) && !string.IsNullOrWhiteSpace(config.UserPassword))
                {
                    tbId.Text = config.UserId; // 저장된 아이디를 텍스트박스에 입력
                    tbPw.Text = config.UserPassword; // 저장된 비밀번호(해싱 이전의 비밀번호)를 텍스트박스에 입력
                    cbAutoLogin.Checked = true; // 자동 로그인 체크박스 자동 체크 
                }
            }
            catch (Exception ex)
            {
                // 오류 발생 시 메시지 출력
                MessageBox.Show($"로그인 정보를 불러오는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }
/*========================================================================================================================================================================   
         @@@@로그인 시도 하기@@@@
=========================================================================================================================================================================*/

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            byte[] loginBytes;
            byte[] logindataBytes;

            userId = tbId.Text; // 텍스트박스에서 아이디 가져오기
            password = tbPw.Text; // 텍스트박스에서 비밀번호 가져오기

            // 비밀번호 해싱
            string hashedPassword = PasswordHashing.HashPassword(password);

            logindata = $"{userId},{hashedPassword}"; // 아이디와 해싱된 비밀번호 조합

            logindataBytes = Encoding.UTF8.GetBytes(logindata);

            Packet login_pkt = new Packet
            {
                packet_id = PacketCode.LOGIN, // 로그인 패킷 ID
                msg_size = logindataBytes.Length,
                message = logindata
            };

            loginBytes = login_pkt.packet_data();

            // 로그인 정보 전송
            clientSocket.Send(loginBytes);

            // 리시브 상태로 전환
            Begin_Recv();
        }

/*========================================================================================================================================================================   
         @@@@로그인 성공,실패여부 응답받기@@@@
=========================================================================================================================================================================*/

        private void Begin_Recv()
        {
            byte[] recvBuff = new byte[1024]; // 리시브 버퍼
            clientSocket.BeginReceive(recvBuff, 0, recvBuff.Length, SocketFlags.None, new AsyncCallback(OnReceive), recvBuff);
        }

        private void OnReceive(IAsyncResult ar)
        {
            if (clientSocket == null || !clientSocket.Connected)
            {
                // 소켓이 유효하지 않으면 리턴
                return;
            }

            try
            {
                byte[] recvBuff = (byte[])ar.AsyncState;
                int bytesRead = clientSocket.EndReceive(ar);

                // 패킷 정보 추출
                Packet login_recvpkt = new Packet();
                login_recvpkt.packet_id = BitConverter.ToInt32(recvBuff, 0); // 패킷 ID
                login_recvpkt.msg_size = BitConverter.ToInt32(recvBuff, 4); // 메시지 사이즈
                login_recvpkt.room_id = BitConverter.ToInt32(recvBuff,8); // 채팅방 ID
                login_recvpkt.message = Encoding.UTF8.GetString(recvBuff, 12, login_recvpkt.msg_size);

                // 메시지 크기만큼의 바이트가 수신되었는지 확인
                if (bytesRead == 12 + login_recvpkt.msg_size)
                {
                    // Invoke 메서드 내에서 switch 처리
                    this.Invoke((MethodInvoker)delegate
                    {
                        switch (login_recvpkt.packet_id)
                        {
                            case PacketCode.SUCCESS_LOGIN:

                                CheckAndDeleteDatabaseIfUserChanged(userId); // 로그인 성공 시 사용자 ID 비교 및 DB 초기화


                                // 자동 로그인 정보 저장
                                if (cbAutoLogin.Checked)
                                {
                                    SaveLoginInfo(userId, password); // 체크박스가 체크되어있으면 정보 저장
                                }

                                // 새로운 사용자 ID를 저장
                                SaveLastLoggedInUser(userId);  // 이 위치로 이동

                                UserList.SaveUserList(users_path, login_recvpkt.message); // 사용자 목록 저장

                                // 메인 폼 생성 및 표시
                                MainForm mainForm = new MainForm(userId, clientSocket, this); // 메인 폼 생성
                                this.Hide(); // 현재 폼 숨김
                                mainForm.Show(); // 메인폼 표시

                                userId = null;
                                break;

                            case PacketCode.FAILED_LOGIN:
                                MessageBox.Show("로그인 실패. 다시 시도하세요.", "로그인 실패", MessageBoxButtons.OK);
                                tbPw.Clear(); // 비밀번호 입력창 초기화
                                tbPw.Focus(); // 비밀번호 입력창에 포커스 설정
                                break;

                            case PacketCode.EXISTS_LOGIN:
                                MessageBox.Show("이미 로그인한 계정입니다.", "중복 로그인", MessageBoxButtons.OK);
                                tbId.Clear(); // 아이디 입력창 초기화
                                tbPw.Clear(); // 비밀번호 입력창 초기화
                                tbId.Focus(); // 아이디 입력창에 포커스 설정
                                break;
                        }
                    });
                }
            }
            catch (ObjectDisposedException)
            {
                // 소켓이 닫힌 경우 예외를 무시하고 리턴
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"소켓 오류: {ex.Message}", "소켓 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
/*========================================================================================================================================================================   
         @@@@로그인 정보 저장(체크박스 체크시)@@@@
=========================================================================================================================================================================*/

        private void SaveLoginInfo(string userId, string Password)
        {
            try
            {
                // config.json 파일 경로
                string configPath = "config.json";

                // 기존 설정 로드
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(File.ReadAllText(configPath));

                // 로그인 정보 저장
                config.UserId = userId;
                config.UserPassword = Password;

                // 업데이트된 설정을 다시 파일에 저장
                string updatedJsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(configPath, updatedJsonContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"로그인 정보를 저장하는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*========================================================================================================================================================================   
         @@@@이전 로그인ID와 비교후 다르면 DB내용 삭제@@@@
        =========================================================================================================================================================================*/

        // 로그인 성공 후 호출
        private void SaveLastLoggedInUser(string userId)
        {
            string configPath = Application.StartupPath + @"\config.json";

            // config.json 파일 읽기
            JObject config;
            if (File.Exists(configPath))
            {
                string jsonData = File.ReadAllText(configPath);
                config = JObject.Parse(jsonData);
            }
            else
            {
                config = new JObject();
            }

            // 로그인한 사용자 ID 저장
            config["last_logged_in_user"] = userId;

            // config.json 파일로 저장
            File.WriteAllText(configPath, config.ToString());
        }

        private string GetLastLoggedInUser()
        {
            string configPath = Application.StartupPath + @"\config.json";
            if (File.Exists(configPath))
            {
                string jsonData = File.ReadAllText(configPath);
                JObject config = JObject.Parse(jsonData);
                return config["last_logged_in_user"]?.ToString();
            }
            return null;
        }

        private void CheckAndDeleteDatabaseIfUserChanged(string newUserId)
        {
            string lastLoggedInUserId = GetLastLoggedInUser();

            // 현재 로그인한 ID와 비교
            if (lastLoggedInUserId == null || lastLoggedInUserId != newUserId)
            {
                // DB 초기화
                DeleteChatData(); // roomtbl과 messagetbl의 데이터 삭제
            }
        }

        private void DeleteChatData()
        {
            string dbPath = Application.StartupPath + @"\chat_Database.db"; // DB 경로

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();

                // roomtbl과 messagetbl의 모든 데이터를 삭제하는 쿼리
                string deleteRoomQuery = "DELETE FROM roomtbl"; // roomtbl의 모든 데이터 삭제
                string deleteMessageQuery = "DELETE FROM messagetbl"; // messagetbl의 모든 데이터 삭제

                using (var command = new SQLiteCommand(deleteRoomQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(deleteMessageQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        /*========================================================================================================================================================================   
         @@@@엔터키 이벤트@@@@
        =========================================================================================================================================================================*/
        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // 엔터키 입력을 폼에서 처리했음을 명시
                btnLogin.PerformClick(); // 로그인 버튼 클릭
            }
        }
/*========================================================================================================================================================================   
         @@@@비밀번호 문자 변환@@@@
=========================================================================================================================================================================*/
        private void BtnTogglePassword_Click(object sender, EventArgs e)
        {
            // 현재 비밀번호가 특수문자로 가려진 상태인지 확인
            if (tbPw.PasswordChar == '●')
            {
                // 비밀번호 가리기 해제 (특수문자 가리기 해제)
                tbPw.PasswordChar = '\0'; // 비밀번호 원래 문자로 표시
                btnTogglePassword.Text = "가리기"; // 버튼 텍스트 변경
            }
            else
            {
                // 비밀번호 다시 가리기 (특수문자로 표시)
                tbPw.PasswordChar = '●'; // 비밀번호 특수문자로 가리기
                btnTogglePassword.Text = "보이기"; // 버튼 텍스트 변경
            }
        }

        /*========================================================================================================================================================================   
         @@@@포커싱 부분@@@@
=========================================================================================================================================================================*/
        // 텍스트박스 포커싱
        private void Focusing_Textbox(object sender, EventArgs e)
        {
            if (cbAutoLogin.Checked)
            {
                tbPw.Focus(); // 체크되어 있으면 비밀번호 텍스트박스에 포커스
            }
            else
            {
                tbId.Focus(); // 체크가 안 되어 있으면 아이디 텍스트박스에 포커스
            }
        }

        /*========================================================================================================================================================================   
                 @@@@폼 종료@@@@
        =========================================================================================================================================================================*/
        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClearUserListJson(); // JSON 파일 초기화
            clientSocket.Close(); // 소켓 닫기
            Application.Exit();
        }

        private void ClearUserListJson()
        {
            // userList.json 파일을 빈 배열로 초기화
            string users_path = Application.StartupPath + @"\userList.json";
            File.WriteAllText(users_path, "[]"); // 빈 배열로 초기화
        }
    }
}
