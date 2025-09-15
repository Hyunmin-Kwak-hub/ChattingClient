using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClientPrototype3
{
    internal class UserList
    {
        public static void SaveUserList(string users_path, string users_data)
        {
            JObject users_json = JObject.Parse(users_data); // users_data를 파싱합니다.

            // 사용자 목록을 JSON 파일에 저장
            File.WriteAllText(users_path, users_json.ToString()); // 사용자 목록을 users_path에 쓰기
            return;
        }

        public static JArray LoadUserList(string users_path)
        {
            // JSON 파일 읽기
            string jsonData = File.ReadAllText(users_path);

            // JSON 파싱
            JObject userListObject = JObject.Parse(jsonData);
            JArray userList = (JArray)userListObject["userList"];

            return userList;
        }
    }
}
