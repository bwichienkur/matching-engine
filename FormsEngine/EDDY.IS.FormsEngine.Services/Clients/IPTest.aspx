<%@ Page Language="C#" AutoEventWireup="true"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%
                string ipAddress = Request.UserHostAddress;

                if (!String.IsNullOrWhiteSpace(Request.ServerVariables["HTTP_VIA"]))
                {
                    if (!String.IsNullOrWhiteSpace(Request.ServerVariables["HTTP_TRUE_CLIENT_IP"]))
                    {
                        ipAddress = Request.ServerVariables["HTTP_TRUE_CLIENT_IP"];
                    }
                    else if (!String.IsNullOrWhiteSpace(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) && Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToLower() != "unknown")
                    {
                        ipAddress = !Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Contains(",") ? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last();
                    }
                }
                else if (!String.IsNullOrWhiteSpace(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) && Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToLower() != "unknown")
                {
                    ipAddress = !Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Contains(",") ? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last();
                }
                else if (!String.IsNullOrWhiteSpace(Request.ServerVariables["REMOTE_ADDR"]))
                {
                    ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                }

                ltlIP.Text = "IP Address detected:" + ipAddress;

                %>

              <asp:Literal ID="ltlIP" runat="server"></asp:Literal>
        </div>
    </form>
</body>
</html>
