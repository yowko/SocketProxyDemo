using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient
{
    public class ThreadWork
    {
        public static void Send(object obj)
        {
            while (true)
            {
                var input = Console.ReadLine();
                var server = ((Socket)obj);

                if (input == "exit")
                {
                    Console.WriteLine("Disconnecting from server...");

                    server.Shutdown(SocketShutdown.Both);

                    server.Close();

                    Console.WriteLine("Disconnected!");

                    Console.ReadLine();
                    Environment.Exit(0);
                }


                Console.WriteLine("You: " + input);

                server.Send(Encoding.UTF8.GetBytes(input));
            }
        }

        public static void Receive(object server)
        {
            while (true)
            {
                var data = new byte[1024];

                var receiveData = ((Socket)server).Receive(data);

                var stringData = Encoding.UTF8.GetString(data, 0, receiveData);

                Console.WriteLine("Server: " + stringData);
            }
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var data = new byte[1024];
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var ipEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            
            try
            {
                server.Connect(ipEndpoint);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Unable to connect to server.");

                Console.WriteLine(e.ToString());

                return;
            }

            var receiveData = server.Receive(data);

            var stringData = Encoding.UTF8.GetString(data, 0, receiveData);

            Console.WriteLine(stringData);

            var sendThread = new Thread(ThreadWork.Send);
            sendThread.Start(server);

            var receiveThread = new Thread(ThreadWork.Receive);
            receiveThread.Start(server);
        }
    }
}