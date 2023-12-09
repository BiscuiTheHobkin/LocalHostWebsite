using System;
using System.IO;
using System.Net;

class Program
{
    static void Main()
    {
        
        int port = 8080; 
        string rootDirectory = "Website"; 

        
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");

        try
        {
            listener.Start();
            Console.WriteLine($"Done -> http://localhost:{port}/");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                if (request.HttpMethod == "GET")
                {
                    string path = request.Url.LocalPath;
                    string fullPath = Path.Combine(rootDirectory, path.TrimStart('/'));

                    if (File.Exists(fullPath))
                    {
                        string contentType = GetContentType(fullPath);
                        byte[] buffer = File.ReadAllBytes(fullPath);

                        HttpListenerResponse response = context.Response;
                        response.ContentType = contentType;
                        response.ContentLength64 = buffer.Length;

                        
                        using (Stream output = response.OutputStream)
                        {
                            output.Write(buffer, 0, buffer.Length);
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                }

                context.Response.Close();
            }
        }
        catch (HttpListenerException e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        finally
        {
            listener.Close();
        }
    }

    private static string GetContentType(string filePath)
    {
        if (filePath.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
        {
            return "text/html";
        }
        else if (filePath.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
        {
            return "text/css";
        }

        return "application/octet-stream";
    }
}
