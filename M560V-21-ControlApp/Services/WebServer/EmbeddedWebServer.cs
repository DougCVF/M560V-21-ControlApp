using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;

namespace M560V_21_ControlApp.Services.WebServer
{
    public class EmbeddedWebServer
    {
        private readonly HttpListener _listener;
        private readonly string _rootPath;
        private readonly string _password;
        private bool _isRunning;
        private Thread _serverThread;

        public int Port { get; }
        public string LocalIP { get; private set; }

        public EmbeddedWebServer(int port = 8080)
        {
            Port = port;
            _listener = new HttpListener();
            _rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebRoot");

            // Load password from App.config (key: WebPassword)
            _password = ConfigurationManager.AppSettings["WebPassword"] ?? "admin";

            LocalIP = GetLocalIPAddress();

            // Use wildcard '+' so it matches URL ACL registration
            _listener.Prefixes.Add($"http://+:{Port}/");
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _listener.Start();
            _serverThread = new Thread(ListenLoop) { IsBackground = true };
            _serverThread.Start();

            Console.WriteLine($"[WebServer] Running at http://{LocalIP}:{Port}/");
        }

        public void Stop()
        {
            _isRunning = false;
            try
            {
                _listener.Stop();
                _serverThread?.Join();
            }
            catch { }
        }

        private void ListenLoop()
        {
            while (_isRunning)
            {
                try
                {
                    var context = _listener.GetContext();
                    Task.Run(() => HandleRequest(context));
                }
                catch (HttpListenerException)
                {
                    // Listener stopped
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WebServer] Error: {ex.Message}");
                }
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath.TrimStart('/');
                string remoteIP = context.Request.RemoteEndPoint?.Address.ToString() ?? "unknown";
                Console.WriteLine($"[WebServer] {remoteIP} -> {path}");

                // Authentication check first
                if (!Authenticate(context))
                    return;

                // --- API routes ---
                if (path.Equals("api/test", StringComparison.OrdinalIgnoreCase))
                {
                    HandleApiTest(context);
                    return;
                }

                if (path.Equals("api/parts", StringComparison.OrdinalIgnoreCase))
                {
                    HandleApiParts(context);
                    return;
                }
                // ------------------

                // --- Friendly redirect ---
                if (path.Equals("parts", StringComparison.OrdinalIgnoreCase))
                {
                    path = "parts.html";
                }
                // --------------------------

                // Default file serving
                if (string.IsNullOrEmpty(path))
                    path = "index.html";

                string filePath = Path.Combine(_rootPath, path);

                if (File.Exists(filePath))
                {
                    byte[] buffer = File.ReadAllBytes(filePath);
                    context.Response.ContentType = GetContentType(filePath);
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    context.Response.StatusCode = 404;
                    WriteString(context, "<h1>404 Not Found</h1>");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WebServer] Exception: {ex.Message}");
                try
                {
                    context.Response.StatusCode = 500;
                    WriteString(context, "<h1>500 Internal Server Error</h1>");
                }
                catch { }
            }
            finally
            {
                try { context.Response.Close(); } catch { }
            }
        }

        private bool Authenticate(HttpListenerContext context)
        {
            try
            {
                string auth = context.Request.Headers["Authorization"];

                if (auth == null || !auth.StartsWith("Basic "))
                {
                    context.Response.AddHeader("WWW-Authenticate", "Basic realm=\"M560V21\"");
                    context.Response.StatusCode = 401;
                    WriteString(context, "<h3>401 Unauthorized</h3>");
                    context.Response.Close();
                    return false;
                }

                string encoded = auth.Substring("Basic ".Length).Trim();
                string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));

                // Format: user:password
                string[] parts = decoded.Split(':');
                if (parts.Length == 2 && parts[1] == _password)
                    return true;

                context.Response.StatusCode = 403;
                WriteString(context, "<h3>403 Forbidden - Invalid Password</h3>");
                context.Response.Close();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WebServer] Auth error: {ex.Message}");
                try
                {
                    context.Response.StatusCode = 500;
                    WriteString(context, "<h3>Authentication Error</h3>");
                }
                catch { }
                return false;
            }
        }

        private void HandleApiTest(HttpListenerContext context)
        {
            try
            {
                var json = "{ \"status\": \"ok\", \"message\": \"Web API is running\", \"time\": \"" +
                           DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\" }";

                byte[] buffer = Encoding.UTF8.GetBytes(json);
                context.Response.ContentType = "application/json";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WebServer] /api/test error: {ex.Message}");
                context.Response.StatusCode = 500;
                WriteString(context, "{ \"error\": \"Internal Server Error\" }");
            }
        }

        using System.Collections.Generic; // make sure this is at the top with other usings

private void HandleApiParts(HttpListenerContext context)
    {
        try
        {
            List<M560V_21_ControlApp.Models.Part> parts;

            try
            {
                parts = M560V_21_ControlApp.Data.Repository.GetAllParts();
            }
            catch (Exception exRepo)
            {
                Console.WriteLine($"[WebServer] Database access error: {exRepo.Message}");
                context.Response.StatusCode = 500;
                WriteString(context, "{ \"error\": \"Database access failed\" }");
                return;
            }

            var sb = new StringBuilder();
            sb.Append("[");

            for (int i = 0; i < parts.Count; i++)
            {
                var p = parts[i];

                // Build JSON manually to stay compatible with older C# versions
                sb.Append("{")
                  .AppendFormat("\"Id\":{0}", p.Id)
                  .AppendFormat(",\"PartNumber\":\"{0}\"", EscapeJson(p.PartNumber))
                  .AppendFormat(",\"Description\":\"{0}\"", EscapeJson(p.Description))
                  .AppendFormat(",\"StockWidth\":{0}", p.StockWidth)
                  .AppendFormat(",\"StockDepth\":{0}", p.StockDepth)
                  .AppendFormat(",\"StockHeight\":{0}", p.StockHeight)

                  // Op10 Pick Offsets
                  .AppendFormat(",\"Op10PickXOffset\":{0}", p.Op10PickXOffset)
                  .AppendFormat(",\"Op10PickYOffset\":{0}", p.Op10PickYOffset)
                  .AppendFormat(",\"Op10PickZOffset\":{0}", p.Op10PickZOffset)

                  // Op20 Pick Offsets
                  .AppendFormat(",\"Op20PickXOffset\":{0}", p.Op20PickXOffset)
                  .AppendFormat(",\"Op20PickYOffset\":{0}", p.Op20PickYOffset)
                  .AppendFormat(",\"Op20PickZOffset\":{0}", p.Op20PickZOffset)

                  // Op20 Fin Offsets (optional)
                  .AppendFormat(",\"Op20FinXOffset\":{0}", p.Op20FinXOffset)
                  .AppendFormat(",\"Op20FinYOffset\":{0}", p.Op20FinYOffset)
                  .AppendFormat(",\"Op20FinZOffset\":{0}", p.Op20FinZOffset)

                  // Vise PSI values
                  .AppendFormat(",\"Op10VisePSI\":{0}", p.Op10VisePSI)
                  .AppendFormat(",\"Op20VisePSI\":{0}", p.Op20VisePSI)

                  // Program names
                  .AppendFormat(",\"Op10ProgramName\":\"{0}\"", EscapeJson(p.Op10ProgramName))
                  .AppendFormat(",\"Op20ProgramName\":\"{0}\"", EscapeJson(p.Op20ProgramName))

                  // Cycle times
                  .AppendFormat(",\"Op10CycleTime\":{0}", p.Op10CycleTime)
                  .AppendFormat(",\"Op20CycleTime\":{0}", p.Op20CycleTime)
                  .Append("}");

                if (i < parts.Count - 1)
                    sb.Append(",");
            }

            sb.Append("]");

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WebServer] /api/parts error: {ex.Message}");
            context.Response.StatusCode = 500;
            WriteString(context, "{ \"error\": \"Internal Server Error\" }");
        }
    }


    private string EscapeJson(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "").Replace("\r", "");
        }


        private void WriteString(HttpListenerContext ctx, string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            ctx.Response.ContentType = "text/html";
            try
            {
                ctx.Response.OutputStream.Write(data, 0, data.Length);
            }
            catch { /* ignore if already closed */ }
        }

        private string GetContentType(string file)
        {
            string ext = Path.GetExtension(file).ToLower();
            if (ext == ".html") return "text/html";
            if (ext == ".js") return "application/javascript";
            if (ext == ".css") return "text/css";
            if (ext == ".json") return "application/json";
            if (ext == ".png") return "image/png";
            if (ext == ".jpg" || ext == ".jpeg") return "image/jpeg";
            return "text/plain";
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            return ip != null ? ip.ToString() : "127.0.0.1";
        }
    }
}
