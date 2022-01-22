using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace HTTP
{
  internal class Program
  {
    private static HttpListener listener = new HttpListener();
    private static Random random = new Random();

    public static string RandomString(int length) => new string(Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length).Select<string, char>((Func<string, char>) (s => s[Program.random.Next(s.Length)])).ToArray<char>());

    private static bool ProgramIsRunning(string FullPath)
    {
      string directoryName = Path.GetDirectoryName(FullPath);
      string lower = Path.GetFileNameWithoutExtension(FullPath).ToLower();
      bool flag = false;
      foreach (Process process in Process.GetProcessesByName(lower))
      {
        if (process.MainModule.FileName.StartsWith(directoryName, StringComparison.InvariantCultureIgnoreCase))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public static void HTTPHandler()
    {
      string str1 = DateTimeOffset.UtcNow.ToString();
      while (Program.listener.IsListening)
      {
        try
        {
          string empty = string.Empty;
          HttpListenerContext context = Program.listener.GetContext();
          HttpListenerRequest request = context.Request;
          HttpListenerResponse response = context.Response;
          string str2 = request.RemoteEndPoint.ToString().Split(':')[0];
          string host = request.Url.Host;
          string str3 = request.Url.ToString();
          Console.WriteLine("[" + str1 + "] [IP]: " + str2);
          Console.WriteLine("[" + str1 + "] [URL]: " + str3);
          if (host == "growtopia2.com" && request.HttpMethod == "POST" || host == "growtopia1.com" &&  request.HttpMethod == "POST" || host == "rtsoft.com" && request.HttpMethod == "POST" || host == "hamumu.com" && request.HttpMethod == "POST")
          {
            Console.WriteLine("D");
            if (str3.Contains("growtopia/server_data.php"))
            {
              string str4 = "";
              byte[] bytes = Encoding.UTF8.GetBytes(System.IO.File.ReadAllText("server_data.php") + "\n" + str4 + "\nmeta|" + Program.RandomString(30));
              response.ContentLength64 = (long) bytes.Length;
              Stream outputStream = response.OutputStream;
              outputStream.Write(bytes, 0, bytes.Length);
              outputStream.Close();
              response.Close();
            }
          }
          response.Close();
        }
        catch (Exception ex)
        {
          Console.WriteLine("[" + str1 + "] [ERROR]: " + ex.Message + " ");
          Thread.Sleep(1000);
        }
      }
    }

    public static void StartHTTP(string[] prefixes)
    {
      string str = DateTimeOffset.UtcNow.ToString();
      Console.WriteLine("[" + str + "] [INFO]: HTTP Server / Anti DDoS made with C#, based on GrowBrew C# HTTP Server");
      Console.WriteLine("[" + str + "] [INFO]: Setting up HTTP Server...");
      if (!HttpListener.IsSupported)
      {
        Console.WriteLine("[" + str + "] [ERROR]: Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
      }
      else
      {
        if (prefixes == null || prefixes.Length == 0)
          throw new ArgumentException(nameof (prefixes));
        foreach (string prefix in prefixes)
          Program.listener.Prefixes.Add(prefix);
        Program.listener.Start();
        if (Program.listener.IsListening)
          Console.WriteLine("[" + str + "] [INFO]: Listening!");
        else
          Console.WriteLine("[" + str + "] [ERROR]: Could not listen to port 80, an error occured!");
        new Thread(new ThreadStart(Program.HTTPHandler)).Start();
        Console.WriteLine("[" + str + "] [INFO]: HTTP Server is running.");
      }
    }

    private static void Main(string[] args) => Program.StartHTTP(new string[1]
    {
      "http://*:80/"
    });
  }
}
