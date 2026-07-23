<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestBootstrap.Default" %>

<!DOCTYPE html>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bootstrap Grid Demo</title>
    <!-- Bootstrap 5 CSS (CDN) -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        /* Just for visual clarity - so you can see the grid boxes */
        .demo-box {
            background-color: #e9ecef;
            border: 1px solid #ced4da;
            padding: 15px;
            text-align: center;
            border-radius: 4px;
        }
        .demo-box.highlight {
            background-color: #0d6efd;
            color: white;
            border-color: #0d6efd;
        }
    </style>
</head>
<body>

    <div class="container mt-5">
        <h1 class="text-center mb-4">Bootstrap Grid Example</h1>

        <!-- ROW 1: Three Equal Columns -->
        <h5>1. 3 Equal Columns (stack on mobile)</h5>
        <div class="row">
            <div class="col-md-4">
                <div class="demo-box">Column 1 (col-md-4)</div>
            </div>
            <div class="col-md-4">
                <div class="demo-box">Column 2 (col-md-4)</div>
            </div>
            <div class="col-md-4">
                <div class="demo-box">Column 3 (col-md-4)</div>
            </div>
        </div>

        <hr class="my-4">

        <!-- ROW 2: Unequal Columns (Sidebar layout) -->
        <h5>2. Unequal Columns (8/4 split)</h5>
        <div class="row">
            <div class="col-md-8">
                <div class="demo-box highlight">Main Content (col-md-8)</div>
            </div>
            <div class="col-md-4">
                <div class="demo-box">Sidebar (col-md-4)</div>
            </div>
        </div>

        <hr class="my-4">

        <!-- ROW 3: Automatic equal widths -->
        <h5>3. Auto-Layout (Equal width, no numbers needed)</h5>
        <div class="row">
            <div class="col">
                <div class="demo-box">Auto Col 1</div>
            </div>
            <div class="col">
                <div class="demo-box">Auto Col 2</div>
            </div>
            <div class="col">
                <div class="demo-box">Auto Col 3</div>
            </div>
            <div class="col">
                <div class="demo-box">Auto Col 4</div>
            </div>
        </div>
    </div>

    <!-- Bootstrap 5 JavaScript Bundle (optional, but needed for menus/modals) -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>