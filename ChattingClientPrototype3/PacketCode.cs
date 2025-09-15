using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 <패킷 식별 번호 정의>
101: 로그인 패킷 번호
102: 로그아웃 패킷 번호
1010: 로그인 성공 패킷 번호 
1011: 로그인 실패 패킷 번호
1012: 중복 로그인 계정 패킷 번호

201: 채팅방 생성 요청 패킷 번호
2010: 채팅방 생성 성공 패킷 번호
2011: 채팅방 생성 실패 패킷 번호
2012: 중복 채팅방 생성 패킷 번호

301: 채팅 메시지 전송 패킷 번호
3010: 채팅 메시지 전송 성공 패킷 번호
3011: 채팅 메시지 전송 실패 패킷 번호

404: 에러 패킷 번호
 */


namespace ChattingClientPrototype3
{
    internal class PacketCode
    {
        public const int LOGIN = 101;
        public const int LOGOUT = 102;

        public const int SUCCESS_LOGIN = 1010;
        public const int FAILED_LOGIN = 1011;
        public const int EXISTS_LOGIN = 1012;

        public const int CREATE_CHAT_ROOM = 201;

        public const int SUCCESS_CREATE_ROOM = 2010;
        public const int FAILED_CREATE_ROOM = 2011;
        public const int EXISTS_CREATE_ROOM = 2012;

        public const int CHAT_MESSAGE = 301;
        public const int SUCCESS_SEND_MESSAGE = 3010;
        public const int FAILED_SEND_MESSAGE = 3011;

        public const int ER404 = 404;
    }
}
