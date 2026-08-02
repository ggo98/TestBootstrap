using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Web;


namespace WebApplication1
{
    public class ApiProxy : IHttpHandler
    {
        const string BaseUrl = "https://localhost/dvweb/ddi";

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);

        public void ProcessRequest(HttpContext context)
        {
            string endpoint = context.Request.QueryString["q"];
            OutputDebugString("ENDPOINT: "+endpoint + "\n");
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            WindowsImpersonationContext wic = null;
            try
            {
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                //WriteLine(context, wi.Name);
                using (WebClient client = new WebClient())
                {
                    string url = BaseUrl + endpoint;
                    OutputDebugString("URL: " + url + "\n");
                    client.Credentials = CredentialCache.DefaultCredentials;
                    client.Headers.Add(HttpRequestHeader.Accept, "application/json");
                    byte[] data = client.DownloadData(url);
                    string response = Encoding.UTF8.GetString(data);
                    context.Response.ContentType = "application/json";
                    context.Response.Write(response);
                    //WriteLine(context, response);
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 400;
//                WriteLine(context, ex.Message);
            }
            finally
            {
                if (null != wic)
                    wic.Undo();
            }
        }

        private void WriteLine(HttpContext context, string s)
        {
            context.Response.Write(s + "<br/>");
        }

        public bool IsReusable => false;
    }
}