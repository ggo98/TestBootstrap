using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestBootstrap
{
    public partial class Default2 : System.Web.UI.Page
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