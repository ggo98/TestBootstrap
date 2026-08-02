<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DdiBrowser.aspx.cs" Inherits="TestBootstrap.DdiBrowser" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Product List</title>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.css" />

    <style>
        .custom-expand-icon::before {
        content: "\2717";   /* ▉ - Unicode escape, CSS syntax */
        font-family: inherit; /* or a specific font if the glyph needs one */
    </style>

    <script src="Scripts/jquery-3.7.1.js"></script>

    <!-- Bootstrap 5 CSS (CDN) -->
<%--    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />--%>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.css" rel="stylesheet" />

    <!-- bootstrap-treeview (from https://github.com/patternfly/patternfly-bootstrap-treeview)-->
    <link href="css/bootstrap-treeview.css" rel="stylesheet" />
    <script src="js/bootstrap-treeview.js"></script>

    <script>
        $(document).ready(function () {
            //alert("");
            //return;

            // "load on expand" version
            loadRootTree();
            return;
        });

        function TEST(s) {
            //alert(s);
            return s;
        }

        function loadRootTree() {
            $.ajax({
                type: "POST",
                url: "DdiBrowser.aspx/GetRootNodes",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $('#tree').treeview({
                        data: response.d,
                        levels: 1,
                        color: "#428bca",
                        showImage: true,
                        //
                        //expandIcon: 'fa-angle-right',
                        // "good" combination / can't commented even if we use custom icons, because no item will be expandable
                        collapseIcon: 'fa fa-angle-down',
                        expandIcon: 'fas fa-chevron-right',
                        //
                        //collapseIcon: 'fas fa-chevron-down',
                        emptyIcon: 'custom-expand-icon',
                        lazyLoad: function (node, render) {
                            $.ajax({
                                type: "POST",
                                url: "DdiBrowser.aspx/GetChildrenNodes",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: TEST(JSON.stringify({ parentId: node.id, "delimitedFlags": node.delimitedFlags })),
                                success: function (childResponse) {
                                    render(childResponse.d); // at this point, render is a parameter that holds a reference to the internal callback function bootstrap-treeview provides as part of its lazyLoad feature.
                                },
                                error: function (xhr) {
                                    console.error("Failed to load children:", xhr.responseText);
                                }
                            });
                        }
                    });
                },
                error: function (xhr) {
                    console.error("Failed to load root nodes:", xhr.responseText);
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-4">
            <div class="container mt-4">
                <h2>Simple Treeview</h2>
                <!-- The container where the tree will render -->
                <div id="tree"></div>
            </div>
        </div>
    </form>
</body>
</html>