using CnD.Core.Security.Principal;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TestBootstrap.Infrastructure;
using System.IO;

namespace TestBootstrap
{
    public partial class DdiBrowser : System.Web.UI.Page
    {
        const string BaseUrl = "http://localhost/dvweb/ddi/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        /// <summary>
        /// 'load on expand' mode
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<TreeNode> GetRootNodes()
        {
            HttpContext context = HttpContext.Current;

            var roots = new List<TreeNode>();
            var rootData = GetRootDataFromApi();

            foreach (var item in rootData)
            {
                roots.Add(new TreeNode
                {
                    id = item.Id,
                    delimitedFlags = item.DelimitedFlags,
                    text = item.Text,
                    //lazyLoad = item.HasChildren,
                    //nodes = item.HasChildren ? new List<TreeNode>() : null,
                    //image = item.HasChildren ? "/images/folder.png" : "/images/file.png"
                    lazyLoad = true,
                    nodes = new List<TreeNode>(),
                    image = VirtualPathUtility.ToAbsolute("~/images/folder.png")
                });
            }

            return roots;
        }

        /// <summary>
        /// 'load on expand' mode.
        /// *** Note about the 2 parameters:
        /// in the .ASPX file, the parameter sent is a json string with 2 values (parentId and delimitedFlags).
        /// ASP.NET detects JSON, and extract the values. Since there are 2 values, 2 parameters have to be defined for GetChildrenNodes().
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<TreeNode> GetChildrenNodes(string parentId, string delimitedFlags)
        {
            HttpContext context = HttpContext.Current;

            var children = new List<TreeNode>();
            var childrenData = GetChildrenDataFromDb(parentId);

            foreach (var item in childrenData)
            {
                children.Add(new TreeNode
                {
                    id = item.Id,
                    delimitedFlags = item.DelimitedFlags,
                    text = item.Text,
                    //lazyLoad = item.HasChildren,
                    //nodes = item.HasChildren ? new List<TreeNode>() : null,
                    lazyLoad = true,
                    nodes = new List<TreeNode>(),
                    image = item.HasChildren ?
                    VirtualPathUtility.ToAbsolute("~/images/folder.png") :
                    VirtualPathUtility.ToAbsolute("~/images/file.png")
                });
            }

            return children;
        }

        class NodeData
        {
            public string Id { get; set; }
            public string DelimitedFlags { get; set; }
            public string Text { get; set; }
            public bool HasChildren { get; set; }
        }

        private static string GetConnectionString()
        {
            //return ConfigurationManager.ConnectionStrings["YourConnectionStringName"].ConnectionString;
            return "server=.; initial catalog=DataSetReport; User ID=sa; Password=M-anager98;";
        }

        private static List<NodeData> GetRootDataFromApi()
        {
            var result = new List<NodeData>();

            string url = BaseUrl + "tables";

            dynamic obj;
            try
            {
                ImpersonateUser hlp = new ImpersonateUser();

                WindowsIdentity wi = (WindowsIdentity)HttpContext.Current.User.Identity;
                using (IImpersonatedUser impersonatedUser = hlp.Impersonate(wi))
                {
                    using (WebClient client = new WebClient())
                    {
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
                        obj = NewtonsoftExtensions.FromJson<dynamic>(response);
                    }
                }
                foreach (var v in obj)
                {
                    string delimitedFlags = GetDelimitedFlags(v.Value);
                    result.Add(new NodeData
                    {
                        Id = v.Value,
                        DelimitedFlags = delimitedFlags,
                        Text = v.Value,
                        //HasChildren = reader.GetInt32(reader.GetOrdinal("HasChildren")) == 1,
                        HasChildren = true,
                    });
                }
                return result;
            }
            catch(Exception ex)
            {
                int a = 123;
                throw;
            }
        }

        private static List<NodeData> GetChildrenDataFromDb(string parentId)
        {
            var result = new List<NodeData>();

            string alias = parentId.Split('/').First();
            string urlInfo = BaseUrl + $"info/{alias}";
            string url = BaseUrl + $"tables/{parentId}";

            JToken root;
            DdiConnectionInfo ddiConnectionInfo;
            Newtonsoft.Json.Linq.JToken jInfo;
            int minLevel;

            try
            {
                ImpersonateUser hlp = new ImpersonateUser();

                WindowsIdentity wi = (WindowsIdentity)HttpContext.Current.User.Identity;
                using (IImpersonatedUser impersonatedUser = hlp.Impersonate(wi))
                {
                    using (WebClient client = new WebClient())
                    {
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
                        root = JsonConvert.DeserializeObject<JToken>(response); // or JArray.Parse(response)

                        data = client.DownloadData(urlInfo);
                        response = Encoding.UTF8.GetString(data);
                        ddiConnectionInfo = new DdiConnectionInfo();
                        var serializer = new Newtonsoft.Json.JsonSerializer();
                        using (var reader = new StringReader(response))
                        using (JsonReader jsonReader = new JsonTextReader(reader))
                        {
                            ddiConnectionInfo = (DdiConnectionInfo)serializer.Deserialize(jsonReader, typeof(DdiConnectionInfo));
                        }
                        var info = NewtonsoftExtensions.FromJson<dynamic>(response);
                        jInfo = (JObject)info;
                        var aaa = ((JToken)jInfo).Values();
                        var bbbbbbbbbb = ((JObject)jInfo);
                        var bbbbb = typeof(JObject).GetProperties();
                        var cccccc = typeof(JObject).GetProperties();
                        var cccc = typeof(JObject).GetProperty("Item", typeof(string));
                        //minLevel = int.Parse(jInfo.GetValue("MinLevel"));
                    }
                }
                var tablePathObj = root.First;
                while (null != tablePathObj)
                {
                    var children = tablePathObj.Children().ToArray();
                    string path = children[0].First().ToString();
                    //string path = "bbb";
                    // #TODO: don't hardcode new char[] { '"' } (= add some info in DdiConnectionInfos if possible)
                    string[] identifiers = CnD.Data.Helper.SplitPath(path, ddiConnectionInfo.Separators, new char[] { '"' });
                    string delimitedFlags = GetDelimitedFlags(identifiers);
                    var identifiers2 = (from data in identifiers
                                        select @"""" + data + @"""").ToList();
                    path = string.Join("/", identifiers2);
                    string id = parentId + "/" + path;
                    string comment = children[1].First().ToString();

                    result.Add(new NodeData
                    {
                        Id = id,
                        DelimitedFlags = delimitedFlags,
                        Text = path,
                        //HasChildren = reader.GetInt32(reader.GetOrdinal("HasChildren")) == 1,
                        HasChildren = true
                    });
                    tablePathObj = tablePathObj.Next;
                }
                return result;
            }
            catch (Exception ex)
            {
                int a = 123;
                throw;
            }
        }

        private static string GetDelimitedFlags(string identifier)
        {
            return GetDelimitedFlags(new string[] { identifier });
        }

        private static string GetDelimitedFlags(string[] identifiers)
        {
            string ret = string.Empty;
            foreach (string identifier in identifiers)
            {
                if (CnD.Data.Helper.IsDelimited(identifier, '"'))
                    ret += "1/";
                else
                    ret += "0/";
            }
            return ret;
        }

        public class TreeNode
        {
            public string id { get; set; }
            public string delimitedFlags { get; set; }
            public string text { get; set; }
            public bool lazyLoad { get; set; }
            public List<TreeNode> nodes { get; set; }
            public string icon { get; set; }
            public string image { get; set; }   // e.g. "/images/tree-icons/folder.png"
            /// <summary>
            /// custom property
            /// </summary>
        }
   }
}