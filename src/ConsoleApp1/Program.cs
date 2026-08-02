using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        const string BaseUrl = "https://hvcnddemos/dvweb/ddi/";

        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            Console.WriteLine(wi.Name);
            using (wi.Impersonate())
            using (WebClient client = new WebClient())
            {
                string url = BaseUrl + "tables";
                //client.Credentials = wi;
                client.Credentials = CredentialCache.DefaultCredentials;
                client.Headers.Add(HttpRequestHeader.Accept, "application/json");
                // DownloadString() doesn't handle UTF-8 strings
                // HttpClient would have probably worked better (not tested):
                // var response = await client.GetAsync(url);
                // string json = await response.Content.ReadAsStringAsync();
                //string response = client.DownloadString(url); 
                byte[] data = client.DownloadData(url);
                string response = Encoding.UTF8.GetString(data);
                Console.WriteLine(response);
            }
        }
    }
}
