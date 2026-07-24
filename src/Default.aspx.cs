using System;
using System.Collections.Generic;
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
                // 1. Get your data (from a database, list, or hardcoded here for demo)
                DataTable dt = GetProductData();

                // 2. Bind the data to the Repeater control
                rptProducts.DataSource = dt;
                rptProducts.DataBind();

                gvProducts.DataSource = dt;
                gvProducts.DataBind();
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<TreeNode> GetTreeData()
        {
            /*
CREATE TABLE Categories (
    ID INT PRIMARY KEY IDENTITY,
    ParentID INT NULL,        -- NULL = root/top-level node
    Name NVARCHAR(200) NOT NULL
);

INSERT INTO Categories
values
(NULL,	'Parent 1'),
(1,	'Child 1'),
(2, 'Grandchild 1'),
(2,	'Grandchild 2'),
(1,	'Child 2'),
(NULL,	'Parent 2')

ID	ParentID	Name
1	NULL	Parent 1
2	1	Child 1
3	2	Grandchild 1
4	2	Grandchild 2
5	1	Child 2
6	NULL	Parent 2
             */

            // pull everything from database 2
            var flatRows = GetCategoryRows();
            return BuildTree(flatRows, null);

            // pull everything from database, build hierarchy, etc.
            var treeData = new List<TreeNode>
            {
                new TreeNode { text = "Parent 1", nodes = new List<TreeNode> {
                    new TreeNode { text = "Child 1"/*,
                        nodes = new List<TreeNode>()
                        {
                            new TreeNode { text = "child of Child 1" },
                        },*/
                    },
                    new TreeNode { text = "Child 2" }
                }},
                new TreeNode { text = "Parent 2" }
            };
            return treeData;
        }

        public class CategoryRow
        {
            public int ID { get; set; }
            public int? ParentID { get; set; }
            public string Name { get; set; }
            /// <summary>
            /// used only for the "load on expand" version
            /// </summary>
            public bool HasChildren { get; set; }
        };

        private static List<CategoryRow> GetCategoryRowsByParent(int? parentId)
        {
            try
            {
                var rows = new List<CategoryRow>();
                string connStr = "server=.; initial catalog=DataSetReport; User ID=sa; Password=M-anager98;";
                //System.Configuration.ConfigurationManager
                //.ConnectionStrings["YourConnStringName"].ConnectionString;

                string sql = $@"SELECT c1.ID, c1.ParentID, c1.Name,
                CASE WHEN EXISTS (
                    SELECT 1 FROM Categories c2 WHERE c2.ParentID = c1.ID
                ) THEN 1 ELSE 0 END AS HasChildren
                FROM Categories c1
                WHERE ({(null == parentId ? "c1.ParentId IS NULL" : $"c1.ParentId = {parentId}")})";
                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rows.Add(new CategoryRow
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                ParentID = reader.IsDBNull(reader.GetOrdinal("ParentID"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("ParentID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                HasChildren = reader.GetInt32(reader.GetOrdinal("HasChildren")) == 1
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<TreeNode> GetRootNodes()
        {
            var rows = GetCategoryRowsByParent(null);
            var ret = BuildLevel(rows, null);
            return ret;
        }

        /// <summary>
        /// "load on expand" version
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<TreeNode> GetChildNodes(int parentId)
        {
            var allRows = GetCategoryRowsByParent(parentId);
            var ret = BuildLevel(allRows, parentId);
            return ret;
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

        private static List<TreeNode> BuildLevel(List<CategoryRow> rows, int? parentId)
        {
            var ret =rows
                .Where(r => r.ParentID == parentId)
                .Select(r => new TreeNode
                {
                    id = r.ID,
                    text = r.Name,

                    nodes = r.HasChildren ?
                    new List<TreeNode> { new TreeNode { text = "Loading...", id = -1 } }
                    : null,
                    /// <summary>
                    /// used only for the "load on expand" version
                    /// </summary>
                    //lazyLoad = r.HasChildren
                })
                .ToList();
            return ret;
        }
        private static List<TreeNode> BuildTree(List<CategoryRow> allRows, int? parentId)
        {
            return allRows
                .Where(r => r.ParentID == parentId)
                .Select(r => new TreeNode
                {
                    text = r.Name,
                    nodes = BuildTree(allRows, r.ID) // recurse for children
                })
                .ToList();
        }

        public class TreeNode
        {
            public string text { get; set; }
            public List<TreeNode> nodes { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }          // needed so JS knows which node to expand
            public bool lazyLoad { get; set; } = true;
        }

        // Example C# method that returns a DataTable with our data
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