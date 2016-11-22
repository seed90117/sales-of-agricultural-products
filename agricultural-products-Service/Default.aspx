<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
    <title></title>
    <script type="text/javascript" src="jquery.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <script>
                function Post() {
                    /*$.ajax({
                        type: 'post',
                        url: "http://140.127.22.4/test/webservice.asmx/HelloWorld",
                        //要傳過去的值
                        //data: 'name=' + 變數1 + '&Text=' + 變數2,
                        //成功接收的function
                        success: function (oXml) {
                            //先將xml轉成jQuery物件
                            var x = $(oXml);
                            //用find和text來做擷取
                            alert(x.find("string").text());
                        },
                        error: function () { alert('ajax failed'); }
                    });*/

                    $.post("http://140.127.22.4/test/webservice.asmx/HelloWorld",
                    { name: 1, text: 2 },
                    function (data) {
                    var x = $(data);
                    alert(x.find("string").text());
            }, "xml"
        );
                }</script>
            <input id="Button1" type="button" value="Post" onclick="Post();" />
    </div>
        <asp:FileUpload ID="FileUpload1" runat="server"/>
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Button" />
        <br />
        <br />
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
    </form>
</body>
</html>
