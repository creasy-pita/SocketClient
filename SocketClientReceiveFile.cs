using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClient
{
    class SocketClientReceiveFile
    {
        private static byte[] result = new byte[1024];
        public static void InitSocket()
        {
            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            //IPAddress ip = IPAddress.Parse("192.168.5.217");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 8885)); //配置需要连接的服务器IP与端口  
                Console.WriteLine("连接服务器成功");
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }
            SendInfo(clientSocket);

        }

        public static void SendInfo(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            string path = Environment.CurrentDirectory + "/a.txt";
            FileStream fs = File.Open(path, FileMode.Open);
            long length = fs.Length;
            //读取文件内容
            byte[] databytes = new byte[length];
            fs.Read(databytes, 0, databytes.Length);
            //组合文件后缀名，文件大小 文件内容
            byte[] fileExtbyte = Encoding.UTF8.GetBytes(string.Format("{0:D3}", "txt"));
            byte[] fileLengthByte = Encoding.UTF8.GetBytes(databytes.Length.ToString("D20"));
            byte[] fileByte = CommbineBinaryArray(fileExtbyte, fileLengthByte);

            byte[] fileInfobytes = CommbineBinaryArray(fileByte, databytes);
            //发送内容
            myClientSocket.Send(fileInfobytes);

            myClientSocket.Receive(result);
            string resStr = Encoding.UTF8.GetString(result);
            Console.WriteLine("发送并接受成功");
            if (resStr == "yes")
            {
                Console.WriteLine("发送并接受susccess");
            }
            Thread.Sleep(10000);

            myClientSocket.Shutdown(SocketShutdown.Both);
            myClientSocket.Close();

        }

        public static byte[] CommbineBinaryArray(byte[] bArr1, byte[] bArr2)
        {

            byte[] fileByte = new byte[bArr1.Length + bArr2.Length];
            int currPos = 0;
            while (currPos < bArr1.Length)
            {
                fileByte[currPos] = bArr1[currPos++];
            }
            while (currPos < fileByte.Length)
            {
                fileByte[currPos] = bArr2[currPos - bArr1.Length];
                currPos++;
            }
            return fileByte;
        }
    }
}
