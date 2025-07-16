// FILE          : Program.cs
// PROJECT       : MyOwnWebServer
// PROGRAMMER    : Gaurav Patel 
// FIRST VERSION : 12-11-2024
// DESCRIPTION   : This is the starting point of a custom web server application whether it is a single-threaded or multithreaded application. It handles the options for SERVER settings received by the system through the command line interface.

using System;

namespace MyOwnWebServer
{
    // FILE          : Program.cs
    // PROJECT       : WebServer
    // PROGRAMMER    : Gaurav Patel
    // FIRST VERSION : 12-11-2024
    // DESCRIPTION   : This is the starting point of a custom web server application whether it is a single-threaded or multithreaded application. It handles the options for SERVER settings received by the system through the command line interface and starts the web server.

        public class Program
        {
            public static void Main(string[] args)
            {
                Console.WriteLine("Starting WebServer ...");

            // Typically used to parse the command line arguments and then uses the information obtained to setup the WebServer class.
            if (args.Length != 3)
                {
                    Console.WriteLine("Usage: WebServer and  –root=<webRoot> and  –ip=<serverIP> and –port=<serverPort>");
                    return;
                }

                string webRoot = null;
                string serverIP = null;
                int serverPort = 0;

                foreach (var arg in args)
                {
                    if (arg.StartsWith("-root=")) webRoot = arg.Substring(6);
                    else if (arg.StartsWith("-ip=")) serverIP = arg.Substring(4);
                    else if (arg.StartsWith("-port=")) serverPort = int.Parse(arg.Substring(6));
                }

                if (string.IsNullOrEmpty(webRoot) || string.IsNullOrEmpty(serverIP) || serverPort == 0)
                {
                    Console.WriteLine("It can also be false or not valid arguments provided.");
                    return;
                }

                // initialize the server for staet
                WebServer server = new WebServer(webRoot, serverIP, serverPort);
                server.RunServer();
            }
        }

}
