using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingClientPrototype3
{
    public struct Packet
    {
        public int packet_id;
        public int msg_size;
        public int room_id;
        public string message;
        public byte[] packet_data()
        {
            byte[] packet_idBytes = BitConverter.GetBytes(packet_id);
            byte[] msg_sizeBytes = BitConverter.GetBytes(msg_size);
            byte[] room_idBytes = BitConverter.GetBytes(room_id);
            byte[] message_Bytes = Encoding.UTF8.GetBytes(message);

            byte[] packet_bytes = new byte[12 + message_Bytes.Length];

            Buffer.BlockCopy(packet_idBytes, 0, packet_bytes, 0, 4); // 패킷 ID (4바이트)
            Buffer.BlockCopy(msg_sizeBytes, 0, packet_bytes, 4, 4); // 메시지 크기 (4바이트)
            Buffer.BlockCopy(room_idBytes, 0, packet_bytes, 8, 4); // 룸 ID (4바이트)
            Buffer.BlockCopy(message_Bytes, 0, packet_bytes, 12, message_Bytes.Length); // 메시지

            return packet_bytes;
        }
    }
}
