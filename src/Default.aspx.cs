using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestBootstrap
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only load data on the initial page load (not on postbacks)
            if (!IsPostBack)
            {
                DataTable dt = GetProductData();

                rptProducts.DataSource = dt;
                rptProducts.DataBind();

                gvProducts.DataSource = dt;
                gvProducts.DataBind();
            }
        }

        /// <summary>
        /// 'load all once' mode
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<TreeNode> GetTreeData()
        {
            // pull everything from database 2
            var flatRows = GetCategoryRows();
            return BuildTree(flatRows, null);
        }

        class CategoryRow
        {
            public int ID { get; set; }
            public int? ParentID { get; set; }
            public string Name { get; set; }
            /// <summary>
            /// used only for the 'load on expand' version
            /// </summary>
            public bool HasChildren { get; set; }
        };

        /// <summary>
        /// 'load on expand' mode
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<TreeNode> GetRootNodes()
        {
            var roots = new List<TreeNode>();
            var rootData = GetRootDataFromDb();

            foreach (var item in rootData)
            {
                roots.Add(new TreeNode
                {
                    id = item.Id,
                    text = item.Text,
                    lazyLoad = item.HasChildren,
                    nodes = item.HasChildren ? new List<TreeNode>() : null,
                    //image = item.HasChildren ? "/images/folder48x48.png" : null
                    image = item.HasChildren ? "/images/folder48x48.png" : "/images/file.png"
                });
            }

            return roots;
        }

        /// <summary>
        /// 'load on expand' mode
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<TreeNode> GetChildNodes(int parentId)
        {
            var children = new List<TreeNode>();
            var childData = GetChildDataFromDb(parentId);

            foreach (var item in childData)
            {
                children.Add(new TreeNode
                {
                    id = item.Id,
                    text = item.Text,
                    lazyLoad = item.HasChildren,
                    nodes = item.HasChildren ? new List<TreeNode>() : null,
                    //image = item.HasChildren ? "/images/folder48x48.png" : null
                    image = item.HasChildren ? "/images/folder48x48.png" : "/images/file.png"
                    //image = "/images/folder48x48.png"
                });
            }

            return children;
        }
        
        class NodeData
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public bool HasChildren { get; set; }
        }

        private static string GetConnectionString()
        {
            //return ConfigurationManager.ConnectionStrings["YourConnectionStringName"].ConnectionString;
            return "server=.; initial catalog=DataSetReport; User ID=sa; Password=M-anager98;";
        }

        private static List<NodeData> GetRootDataFromDb()
        {
            var result = new List<NodeData>();

            const string sql = @"
        SELECT 
            n.Id,
            n.Name AS Text,
            CASE WHEN EXISTS (SELECT 1 FROM Categories c WHERE c.ParentId = n.Id) 
                 THEN 1 ELSE 0 END AS HasChildren
        FROM Categories n
        WHERE n.ParentId IS NULL";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new NodeData
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            HasChildren = reader.GetInt32(reader.GetOrdinal("HasChildren")) == 1,
                            //image = "/images/folder48x48.png"
                        });
                    }
                }
            }

            return result;
        }

        private static List<NodeData> GetChildDataFromDb(int parentId)
        {
            var result = new List<NodeData>();

            const string sql = @"
        SELECT 
            n.Id,
            n.Name AS Text,
            CASE WHEN EXISTS (SELECT 1 FROM Categories c WHERE c.ParentId = n.Id) 
                 THEN 1 ELSE 0 END AS HasChildren
        FROM Categories n
        WHERE n.ParentId = @ParentId";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ParentId", parentId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new NodeData
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            HasChildren = reader.GetInt32(reader.GetOrdinal("HasChildren")) == 1,
                            //image = "/images/folder48x48.png"
                        });
                    }
                }
            }

            return result;
        }
        private static List<CategoryRow> GetCategoryRows()
        {
            try
            {
                var rows = new List<CategoryRow>();
                string connStr = "server=.; initial catalog=DataSetReport; User ID=sa; Password=M-anager98;";
                //System.Configuration.ConfigurationManager
                //.ConnectionStrings["YourConnStringName"].ConnectionString;

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("SELECT ID, ParentID, Name FROM Categories", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rows.Add(new CategoryRow
                            {
                                ID = reader.GetInt32(0),
                                ParentID = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                                Name = reader.GetString(2)
                            });
                        }
                    }
                }
                return rows;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static List<TreeNode> BuildTree(List<CategoryRow> allRows, int? parentId)
        {
            return allRows
                .Where(r => r.ParentID == parentId)
                .Select(r => new TreeNode
                {
                    text = r.Name,
                    nodes = BuildTree(allRows, r.ID), // recurse for children
                })
                .ToList();
        }

        public class TreeNode
        {
            public int id { get; set; }
            public string text { get; set; }
            public bool lazyLoad { get; set; }
            public List<TreeNode> nodes { get; set; }
            public string icon { get; set; }
            public string image { get; set; }   // e.g. "/images/tree-icons/folder.png"
        }

        private DataTable GetProductData()
        {
            DataTable dt = new DataTable();

            // Define the columns
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("Price", typeof(decimal));

            // Add sample rows (In a real app, you'd query a database here)
            dt.Rows.Add(1, "Wireless Mouse", "Electronics", 29.99);
            dt.Rows.Add(2, "Notebook", "Stationery", 5.49);
            dt.Rows.Add(3, "Desk Lamp", "Furniture", 45.00);
            dt.Rows.Add(4, "USB-C Cable", "Electronics", 12.99);

            return dt;
        }
    }
}