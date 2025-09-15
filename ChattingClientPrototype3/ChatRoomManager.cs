using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChattingClientPrototype3
{
    public class ChatRoomManager
    {
        private Dictionary<int, ChatForm> chatForms = new Dictionary<int, ChatForm>();

        // 채팅 폼 생성 또는 반환
        public ChatForm GetOrCreateChatForm(int room_id, string loggedInUserId, string otherUserId, MainForm mainForm)
        {
            if (!chatForms.ContainsKey(room_id))
            {
                ChatForm chatForm = new ChatForm(room_id, loggedInUserId, otherUserId, mainForm);  // ChatForm의 올바른 생성자 호출
                chatForms[room_id] = chatForm;

                // 새로운 채팅 폼을 연 후, 폼이 닫힐 때 딕셔너리에서 제거하기
                chatForm.FormClosed += (s, e) => chatForms.Remove(room_id);
            }
            return chatForms[room_id];
        }

        // 특정 채팅 폼에 메시지 전달
        public void SendMessageToRoom(int room_id, string message)
        {
            if (chatForms.ContainsKey(room_id))
            {
                ChatForm chatForm = chatForms[room_id];
                chatForm.PassMessage(room_id, message);  // ChatForm의 메시지 표시 메서드 호출
            }
            else
            {
                // 없으면 저장?
                return;
            }
        }

        public void SendMessageToChatForm(int room_id, string message)
        {
            if (chatForms.TryGetValue(room_id, out ChatForm chatForm))
            {
                chatForm.PassMessage(room_id, message);
            }
            else
            {
                // 해당 room_id에 대한 채팅 폼이 없을 경우 처리
                MessageBox.Show("해당 방이 열려 있지 않습니다.");
            }
        }

        // 특정 room_id의 채팅 방을 닫고 제거
        public void CloseChatRoom(int roomId)
        {
            if (chatForms.ContainsKey(roomId))
            {
                ChatForm chatForm = chatForms[roomId];
                chatForm.Close();  // 채팅 폼 닫기
                chatForms.Remove(roomId);  // 딕셔너리에서 제거
            }
        }
    }
}
