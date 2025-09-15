using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Data.SQLite;
using System.IO;
using System.Threading;

namespace ChattingClientPrototype3
{
    internal static class Program
    {

        // 애플리케이션 인스턴스에 대한 고유 식별자 설정
        private static Mutex mutex = new Mutex(true, "ChattingClientPrototype3_UniqueInstance");
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 이미 프로그램이 실행 중인지 확인
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show("프로그램이 이미 실행 중입니다.");
                return; // 다른 인스턴스가 실행 중인 경우 종료
            }

            // 데이터베이스 파일 경로 설정
            string dbFilePath = "chat_Database.db"; // 원하는 데이터베이스 파일명

            // 데이터베이스 파일이 존재하지 않으면 생성
            if (!File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
            }

            // 데이터베이스 연결 및 테이블 생성
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
            {
                connection.Open();

                // messagetbl 생성
                string createMessageTable = @"
                CREATE TABLE IF NOT EXISTS messagetbl (
                    message_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    room_id INTEGER,
                    message_sender_id TEXT,
                    message_text TEXT NOT NULL,
                    message_time TEXT NOT NULL,
                    is_read INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (room_id) REFERENCES roomtbl(room_id)
                );";

                // roomtbl 생성
                string createRoomTable = @"
                CREATE TABLE IF NOT EXISTS roomtbl (
                    room_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    loggedinuser_id TEXT NOT NULL,
                    otheruser_id TEXT NOT NULL
                );";

                // 테이블 생성 실행
                using (var command = new SQLiteCommand(createMessageTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createRoomTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());

            // 애플리케이션 종료 시 Mutex 해제
            mutex.ReleaseMutex();
        }
    }
}
