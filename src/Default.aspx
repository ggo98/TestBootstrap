<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestBootstrap.Default2" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Product List</title>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <!-- Bootstrap 5 CSS (CDN) -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

    <!-- bootstrap-treeview -->
    <link href="Content/bootstrap-treeview.min.css" rel="stylesheet" />
    <script src="Scripts/jquery-2.1.4.js"></script>
    <script src="Scripts/bootstrap-treeview.js"></script>

    <script>
        $(document).ready(function () {
            //alert("");
            // Define the tree data
            var treeData = [
                {
                    text: "Parent 1",
                    nodes: [
                        {
                            text: "Child 1",
                            nodes: [
                                { text: "Grandchild 1" },
                                { text: "Grandchild 2" }
                            ]
                        },
                        { text: "Child 2" }
                    ]
                },
                {
                    text: "Parent 2"
                },
                {
                    text: "Parent 3"
                }
            ];

            // Initialize the treeview
            $('#tree').treeview({
                data: treeData,
                levels: 2,          // Expand up to 2 levels by default
                color: "#428bca",   // Optional: text color
                expandIcon: 'fas fa-chevron-right',
                collapseIcon: 'fas fa-chevron-down'
            //    expandIcon: 'glyphicon glyphicon-chevron-right',
            //        collapseIcon: 'glyphicon glyphicon-chevron-down'
            //    expandIcon: 'glyphicon glyphicon-plus',
            //    collapseIcon: 'glyphicon glyphicon-minus'
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-4">
            <h2 class="mb-3">Product Inventory (HTML Table)</h2>

            <!-- 
                THE BOOTSTRAP HTML TABLE 
                The <asp:Repeater> will generate the <tr> rows for us.
            -->
            <div class="table-responsive">
                <table class="table table-striped table-hover table-bordered">
                    <thead class="table-dark">
                        <tr>
                            <th>ID</th>
                            <th>Product Name</th>
                            <th>Category</th>
                            <th class="text-end">Price</th>
                        </tr>
                    </thead>
                    <tbody>
                        <!-- The Repeater control binds to our C# data -->
                        <asp:Repeater ID="rptProducts" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("ID") %></td>
                                    <td><%# Eval("Name") %></td>
                                    <td>
                                        <span class="badge bg-secondary"><%# Eval("Category") %></span>
                                    </td>
                                    <td class="text-end fw-bold"><%# Eval("Price", "{0:C}") %></td>
                                </tr>
                            </ItemTemplate>
<%--                            <!-- Optional: Show a message if the data source is empty -->
                            <EmptyDataTemplate>
                                <tr>
                                    <td colspan="4" class="text-center text-muted">No products found.</td>
                                </tr>
                            </EmptyDataTemplate>--%>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>
        <hr />
        <div class="container mt-4">
            <h2 class="mb-3">Product Inventory (GridView)</h2>

            <!-- 
                THE BOOTSTRAP HTML TABLE 
                The <asp:Repeater> will generate the <tr> rows for us.
            -->
            <div class="gridview-responsive">
                <asp:GridView ID="gvProducts" runat="server" 
                AutoGenerateColumns="true" 
                CssClass="table table-striped table-hover table-bordered"
                GridLines="None" 
                BorderStyle="None" />
            </div>
        </div>
        <hr />
        <div class="container mt-4">
            <h2>Simple Treeview</h2>
            <!-- The container where the tree will render -->
            <div id="tree"></div>
        </div>
    </form>
</body>
</html>