<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RdlcReportViewerBranchWise.aspx.cs" Inherits="ERP.WebUI.ReportViewer.RdlcReportViewerBranchWise" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            
            <rsweb:ReportViewer Height="800" Width="100%" ID="ReportViewer1" runat="server">
            </rsweb:ReportViewer>
            
        </div>
    </form>
</body>
</html>
