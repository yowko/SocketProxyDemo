using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    public class ThreadWork
    {
        public static void Send(object obj)
        {
            var client = ((Socket)obj);
            while (true)
            {
                var input = Console.ReadLine();
                if (input == "exit")
                {
                    client.Close();
                    Environment.Exit(0);
                }

                Console.WriteLine("You: " + input);

                client.Send(Encoding.UTF8.GetBytes(input));
            }
        }

        public static void Receive(object obj)
        {
            var client = ((Socket)obj);
            while (true)
            {
                var data = new byte[1024];

                var receiveData = client.Receive(data);

                if (receiveData == 0)
                {
                    Console.WriteLine("Disconnected from {0}", client.RemoteEndPoint);

                    break;
                }


                Console.WriteLine("Client: " + Encoding.UTF8.GetString(data, 0, receiveData));
            }

            Environment.Exit(0);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

            var newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            newSocket.Bind(ipEndPoint);

            newSocket.Listen(10);


            AcceptClient(newSocket);
        }

        private static void AcceptClient(Socket newSocket)
        {
            Console.WriteLine("Waiting for a client...");

            var client = newSocket.Accept();

            var clientEndpoint = (IPEndPoint)client.RemoteEndPoint;

            Console.WriteLine("Connected with {0} at port {1}", clientEndpoint.Address, clientEndpoint.Port);

            const string welcome = "Welcome to my test server";

            var data = Encoding.UTF8.GetBytes(welcome);

            client.Send(data, data.Length, SocketFlags.None);


            var sendThread = new Thread(ThreadWork.Send);
            var receiveThread = new Thread(ThreadWork.Receive);

            sendThread.Start(client);
            receiveThread.Start(client);
        }
    }
}