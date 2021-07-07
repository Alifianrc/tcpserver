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
                BroadCast(message, tcpClient, null);
                string chat = "Announce : " + message;
                Console.WriteLine(chat);

                //save to txt
                program.writeMessage(chat);
            }
        }
 
        public void ClientListener(object obj)  
        {  
            TcpClient tcpClient = (TcpClient)obj;  
            StreamReader reader = new StreamReader(tcpClient.GetStream());  
            string name = reader.ReadLine();
            
            Console.WriteLine(name + " connected");  
            while (true)  
            {
                try
                {
                    //record chat  
                    string message = reader.ReadLine();
                    BroadCast(message, tcpClient, name);
                    string chat = "Client " + name + " : " + message;
                    Console.WriteLine(chat);

                    //save to txt
                    program.writeMessage(chat);
                }
                catch (Exception e)  
                {  
                    Console.WriteLine(e.Message);  
                    program.removeClient(tcpClient);
                    break;  
                }  
            }
        }
 
        public void BroadCast(string msg, TcpClient excludeClient, string n)  
        {  
            foreach (TcpClient client in Program.tcpClientsList)  
            {   
                //broadcast to all client except sender
                StreamWriter sWriter = new StreamWriter(client.GetStream());  
                if(n == null)
                {
                    sWriter.WriteLine("Server : "+ msg); 
                }
                else if (client != excludeClient)  
                { 
                    sWriter.WriteLine("Client "+ n + " : "+ msg);  
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
            HandleClient handleClient = new HandleClient();

            //start process
            tcpListener = new TcpListener(IPAddress.Any, 5000);  
            tcpListener.Start();  
            Console.WriteLine("Server created");
            while (true)  
            {
                //add clients to list
                TcpClient tcpClient = tcpListener.AcceptTcpClient();  
                tcpClientsList.Add(tcpClient);

                //broadcast from server
                Thread bc = new Thread(() =>handleClient.Announce(tcpClient));
                bc.Start();

                //start listener
                Thread startListen = new Thread(() => handleClient.ClientListener(tcpClient));
                startListen.Start();
            }          
        }

        public void writeMessage(string chat)
        {
            messagesToSave.Add(chat);
            File.WriteAllLines("C:/Users/natan/tea/myapp/server/chats.txt", messagesToSave);
        }

        public void removeClient(TcpClient client)
        {
            tcpClientsList.Remove(client);
        }
    }  
} 