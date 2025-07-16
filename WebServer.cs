// FILE          : WebServer.cs
// PROJECT       : MyOwnWebServer
// PROGRAMMER    : Gaurav Patel
// FIRST VERSION : 12-11-2024
// DESCRIPTION   : The web server is a simple HTTP web server that listens for HTTP GET requests for serving static files from a root directory.

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MyOwnWebServer
{
    public class WebServer
    {
        private readonly string webRoot;
        private readonly string serverIP;
        private readonly int serverPort;

        public WebServer(string webRoot, string serverIP, int serverPort)
        {
            this.webRoot = webRoot;
            this.serverIP = serverIP;
            this.serverPort = serverPort;
        }

        // FUNCTION      : RunServer
        // DESCRIPTION   : Responsible for the beginning of the operation of the web-server and for the acceptance of connection requests.
        // RETURNS       : void
        public void RunServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(serverIP), serverPort);
            listener.Start();
            Console.WriteLine($"WebServer running at {serverIP}:{serverPort}...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ProcessRequest(client);
            }
        }

        // FUNCTION      : ProcessRequest
        // DESCRIPTION   : Process a single client request and produces the response needed by the client.
        // PARAMETERS    : cclient (TcpClient) - Connected client.
        // RETURNS       : void
        private void ProcessRequest(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                {
                    string requestLine = reader.ReadLine();
                    if (string.IsNullOrEmpty(requestLine)) return;

                    Log($"Request received: {requestLine}");

                    string[] requestParts = requestLine.Split(' ');
                    if (requestParts.Length < 2 || requestParts[0] != "GET")
                    {
                        SendResponse(writer, 501, "Not Implemented", "At the present this server only processes HTTP GET requests.");
                        return;
                    }

                    string resourcePath = ResolveResourcePath(requestParts[1]);
                    if (File.Exists(resourcePath))
                    {
                        ServeFile(writer, stream, resourcePath);
                    }
                    else
                    {
                        SendResponse(writer, 404, "Not Found", "The requested resource was not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error processing request: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        // FUNCTION      : ResolveResourcePath
        // DESCRIPTION   : Casts a URL path requested through the server to a file system path.
        private string ResolveResourcePath(string urlPath)
        {
            string relativePath = urlPath == "/" ? "/index.html" : urlPath;
            return Path.Combine(webRoot, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        }

        // FUNCTION      : ServeFile
        // DESCRIPTION   : Constructs an HTTP response to the client, containing the contents of a passed file..
        // PARAMETERS    : 
        //   writer (StreamWriter) - The output stream for headers.
        //   stream (NetworkStream) -  The data that flows over the network stream of a binary type.
        //   filePath (string) - The location of the resource as a file system path.
        private void ServeFile(StreamWriter writer, NetworkStream stream, string filePath)
        {
            byte[] fileContents = File.ReadAllBytes(filePath);
            string mimeType = GetMimeType(filePath);

            writer.WriteLine("HTTP/1.1 200 OK");
            writer.WriteLine($"Content-Type: {mimeType}");
            writer.WriteLine($"Content-Length: {fileContents.Length}");
            writer.WriteLine();
            stream.Write(fileContents, 0, fileContents.Length);

            Log($"File served: {filePath} ({fileContents.Length} bytes, {mimeType})");
        }

        // FUNCTION      : SendResponse
        // DESCRIPTION   : Sends an HTTP response back to the client according to the input arguments passed in the request.
        private void SendResponse(StreamWriter writer, int statusCode, string statusDescription, string message)
        {
            writer.WriteLine($"HTTP/1.1 {statusCode} {statusDescription}");
            writer.WriteLine("Content-Type: text/plain");
            writer.WriteLine($"Content-Length: {message.Length}");
            writer.WriteLine();
            writer.Write(message);

            Log($"Response sent: {statusCode} {statusDescription}");
        }

        // FUNCTION      : GetMimeType
        // DESCRIPTION   : Defines the MIME type of a file depending on its extension.
        // PARAMETERS    : filePath (string) - The file system path.
        // RETURNS       : string - The MIME type.
        private string GetMimeType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".html":
                case ".htm":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".js":
                    return "application/javascript";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                default:
                    return "application/octet-stream";
            }
        }


        // FUNCTION      : Log
        // DESCRIPTION   : Records a log message to a file.
        private void Log(string message)
        {
            string logFilePath = "myOwnWebServer.log";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            File.AppendAllText(logFilePath, $"{timestamp} {message}{Environment.NewLine}");
        }
    }
}
