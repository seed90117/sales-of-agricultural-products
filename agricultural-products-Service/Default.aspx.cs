using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       
    }



    protected void Button2_Click(object sender, EventArgs e)
    {
        Label1.Text = FileUpload1.FileName;
        byte[] bt = FileUpload1.FileBytes;
        TextBox1.Text = Convert.ToBase64String(bt);

    }
}