using System;  
using System.Net.Sockets;  
using System.IO;  
using System.Threading;  
 
namespace Client  
{  
    public class Read
    {
        public void ReadMessage(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;  
            StreamReader streamReader = new StreamReader(tcpClient.GetStream());  
 
            while (true)  
            {  
                try  
                { 
                    //read incoming message 
                    string message = streamReader.ReadLine();  
                    Console.WriteLine(message);  
                }  
                catch (Exception e)  
                {  
                    Console.WriteLine(e.Message);  
                    break;  
                }  
            }             
        }
    }
    class Program  
    {  
        static void Main(string[] args)  
        {  
            Read read = new Read();
            try  
            {
                //this computer  
                TcpClient tcpClient = new TcpClient("127.0.0.1", 5000);  
                Console.WriteLine("Connected to server.");  
 
                Thread thread = new Thread(read.ReadMessage);  
                thread.Start(tcpClient);  
 
                StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());  
 
                while (true)  
                { 
                    if (tcpClient.Connected)  
                    {
                        //send message  
                        string input = Console.ReadLine();
                        streamWriter.WriteLine(input);
                        streamWriter.Flush();  
                    }  
                }  
 
            }  
            catch (Exception e)  
            {  
                Console.Write(e.Message);  
            }  
 
            Console.ReadKey();  
        }   
    }  
}