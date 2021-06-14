using System;  
using System.Collections.Generic;  
using System.Net;  
using System.Net.Sockets;  
using System.IO;  
using System.Threading;  
 
namespace Server  
{
    class HandleClient
    {
        Program program = new Program();
        public void Announce(object obj)  
        {  
            TcpClient tcpClient = (TcpClient)obj;  
            StreamReader reader = new StreamReader(tcpClient.GetStream());  
 
            while (true)  
            {
                //server broadcast 
                string message = Console.ReadLine();
                BroadCast(message, tcpClient, 0);
                string chat = "Announce : " + message;
                Console.WriteLine(chat);

                //save to txt
                program.writeMessage(chat);
            }
        }
 
        public void ClientListener(object obj, int num)  
        {  
            TcpClient tcpClient = (TcpClient)obj;  
            StreamReader reader = new StreamReader(tcpClient.GetStream());  
 
            Console.WriteLine("Client connected");  
 
            while (true)  
            {
                //record chat  
                string message = reader.ReadLine();
                BroadCast(message, tcpClient,num);
                string chat = "Client " + num.ToString() + " : " + message;
                Console.WriteLine(chat);

                //save to txt
                program.writeMessage(chat);
            }
        }
 
        public void BroadCast(string msg, TcpClient excludeClient, int n)  
        {  
            foreach (TcpClient client in Program.tcpClientsList)  
            {   
                //broadcast to all client except sender
                StreamWriter sWriter = new StreamWriter(client.GetStream());  
                if(n == 0)
                {
                    sWriter.WriteLine("Server : "+ msg); 
                }
                else if (client != excludeClient)  
                { 
                    sWriter.WriteLine("Client "+n.ToString() +" : "+ msg);  
                }
                sWriter.Flush();    
            }  
        }
    }  
    class Program  
    {  
        public static TcpListener tcpListener;  
        public static List<TcpClient> tcpClientsList = new List<TcpClient>();  
        private List<string> messagesToSave = new List<string>();

        static void Main(string[] args)  
        {
            int numClients = -1;
            HandleClient handleClient = new HandleClient();

            //start process
            tcpListener = new TcpListener(IPAddress.Any, 5000);  
            tcpListener.Start();  
            Console.WriteLine("Server created");
            while (true)  
            {
                //add clients to list
                numClients++;  
                TcpClient tcpClient = tcpListener.AcceptTcpClient();  
                tcpClientsList.Add(tcpClient);

                //broadcast from server
                Thread bc = new Thread(() =>handleClient.Announce(tcpClient));
                bc.Start();

                //start listener
                Thread startListen = new Thread(() => handleClient.ClientListener(tcpClient, numClients));
                startListen.Start();
            }          
        }

        public void writeMessage(string chat)
        {
            messagesToSave.Add(chat);
            File.WriteAllLines("C:/Users/natan/tea/myapp/server/chats.txt", messagesToSave);
        }
    }  
} 