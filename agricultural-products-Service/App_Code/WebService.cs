using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using QRCoder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;


/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://140.127.22.4/AgriculturalProducts/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{

    private string strdbcon = "server=140.127.22.4;database=AgriculturalProducts;uid=CCBDA;pwd=CCBDA";
    private SqlConnection objcon;
    private SqlCommand sqlcmd;
    private string sql;

    public WebService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string HelloWorld(string d)
    {
        return new JavaScriptSerializer().Serialize(getIpAddress());
    }

    // [WebMethod]要加在方法上方
    // [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    // 方法名稱直白，單字第一個字需大寫
    // 範例如下

    // 測試資料庫連接
    [WebMethod]
    public string TestDataBaseContact ()
    {
        string ReturnContant = ""; // 回傳資料字串變數
        try
        {
            objcon = new SqlConnection(strdbcon); // 建立連接
            objcon.Open(); // 開啟連接
            //sql = "select * from Test"; // SQL語法
            sql = "insert into Test(ContentText) values('test111');SELECT SCOPE_IDENTITY()";
            sqlcmd = new SqlCommand(sql, objcon); // 建立SQL命令對象
            // 只需修改以下部分---------------------------------------------------------------------------------------
            // 取得回傳值(查詢、修改、刪除使用此語法)，新增語法使用 sqlcmd.ExecuteNonQuery();
            SqlDataReader dr = sqlcmd.ExecuteReader();
            if (dr.IsClosed == false) // 確認資料庫開啟
            {
                while (dr.Read()) // 讀取資料
                {
                    ReturnContant += dr[0];
                    //ReturnContant += dr[0] + "," + dr[1] + ";"; // 同一筆資料不同欄位用逗號隔開，不同資料用分號隔開
                }
                dr.Close(); // 停止讀取資料
                objcon.Close(); // 關閉連接
            }
            // ------------------------------------------------------------------------------------------------------

            objcon.Close();

        }
        catch (Exception ex)
        {
            //Response.Write(ex.Message);
        }
        return ReturnContant; // 回傳資料
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string TestDataBaseContactJSON()
    {
        string ReturnContant = ""; // 回傳資料字串變數
        try
        {
            objcon = new SqlConnection(strdbcon); // 建立連接
            objcon.Open(); // 開啟連接
            sql = "select * from Test where ID = 1 "; // SQL語法
            sqlcmd = new SqlCommand(sql, objcon); // 建立SQL命令對象
            // 只需修改以下部分---------------------------------------------------------------------------------------
            // 取得回傳值(查詢、修改、刪除使用此語法)，新增語法使用 sqlcmd.ExecuteNonQuery();
            SqlDataReader dr = sqlcmd.ExecuteReader();
            if (dr.IsClosed == false) // 確認資料庫開啟
            {
                //宣告一個DataTable等等用來存放資料庫裡撈出來的資料
                DataTable table = new DataTable();
                //把剛剛撈到的資料塞進table裡面
                table.Load(dr);
                //這行是利用Json.net直接把table轉成Json
                ReturnContant = JsonConvert.SerializeObject(table, Formatting.None);
                dr.Close(); // 停止讀取資料
                objcon.Close(); // 關閉連接
            }
            // ------------------------------------------------------------------------------------------------------
        }
        catch (Exception ex)
        {
            //Response.Write(ex.Message);
        }
        //return new JavaScriptSerializer().Serialize(ReturnContant);
        return ReturnContant;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SignIn (string Account, string Password)
    {
        string ipAddress = getIpAddress();
        string identify = System.Guid.NewGuid().ToString();
        string memberID = "";
        bool isSign = false;
        bool isIdentify = false;
        bool isLog = false;
        try
        {
            objcon = new SqlConnection(strdbcon);
            objcon.Open();
            sql = "select MemberID,Account,Password from Member where Account = @account";
            sqlcmd = new SqlCommand(sql, objcon);
            sqlcmd.Parameters.Add("@account", SqlDbType.NVarChar).Value = Account;
            SqlDataReader dr = sqlcmd.ExecuteReader();
            if (dr.IsClosed == false)
            {
                dr.Read();
                if ( dr[2].ToString().Equals(Password) )
                {
                    isSign = true;
                    memberID = dr[0].ToString();
                }
                dr.Close();
                objcon.Close();
            }
            isIdentify = setIdentify(Account, identify);
            isLog = insertSignLog(memberID, Account, identify, ipAddress);

            if(isSign && isIdentify && isLog)
            {
                return getJson("Identify", identify);
            }
            else
            {
                return getBoolJson(false);
            }
        }
        catch (Exception ex)
        {
            //Response.Write(ex.Message);
            return getBoolJson(false);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProduct(string CompanyID, string CompanyName, string ProductName, string Type, string Introduction,
                                string AdditionalValue, string Origin, string Image, string PackagingDate, string Verification,
                                string ValidityPeriod, string ValidityNumber)
    {
        string id = "null";
        string qr = "http://140.127.22.4/XXXXXX/";

        try
        {
            objcon = new SqlConnection(strdbcon); // 建立連接
            objcon.Open(); // 開啟連接
            sql = "insert into Product(CompanyID,CompanyName,ProductName,Type,Introduction,AdditionalValue,Origin," +
                "Image,PackagingDate,Verification,ValidityPeriod,ValidityNumber) values ('" + CompanyID + "','" + 
                CompanyName + "','" + ProductName + "','" + Type + "','" + Introduction + "','" + AdditionalValue + "','" +
                Origin + "','" + Image + "','" + PackagingDate + "','" + Verification + "','" + ValidityPeriod + "','" + ValidityNumber +
                "');SELECT SCOPE_IDENTITY()"; // SQL語法
            sqlcmd = new SqlCommand(sql, objcon); // 建立SQL命令對象
            SqlDataReader dr = sqlcmd.ExecuteReader();
            if (dr.IsClosed == false) // 確認資料庫開啟
            {
                dr.Read();
                id = dr[0].ToString();
                dr.Close(); // 停止讀取資料
                objcon.Close(); // 關閉連接
            }
            
            qr += getQRCode(id);
            if (!qr.Equals("null"))
            {
                objcon.Open();
                sql = "update Product set QRCode = '" + qr +"' where ProductID = '" + id + "'";
                sqlcmd = new SqlCommand(sql, objcon);
                sqlcmd.ExecuteNonQuery();
                objcon.Close();
            }

            return getBoolJson(true);
        }
        catch (Exception ex)
        {
            //Response.Write(ex.Message);
            return getBoolJson(false);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ResetPassword(string Identify, string OldPassword, string NewPassword)
    {
        try
        {
            objcon = new SqlConnection(strdbcon);
            objcon.Open();
            sql = "select MemberID,Password from Member where Identify = @identify";
            sqlcmd = new SqlCommand(sql, objcon);
            sqlcmd.Parameters.Add("@identify", SqlDbType.NVarChar).Value = Identify;
            SqlDataReader dr = sqlcmd.ExecuteReader();
            if (dr.IsClosed == false)
            {
                dr.Read();
                if (dr[1].ToString().Equals(OldPassword))
                {
                    return getBoolJson(updatePassword(dr[0].ToString(), NewPassword));
                }
                else
                {
                    return getBoolJson(false);
                }
                dr.Close();
                objcon.Close();
            }
            else
            {
                return getBoolJson(false);
            }
        }
        catch (Exception ex)
        {
            //Response.Write(ex.Message);
            return getBoolJson(false);
        }
    }

    // Private Method
    private string getBoolJson(bool input)
    {
        JObject job = JObject.Parse(@"{""stage"": """+ input.ToString() + @"""}");
        return JsonConvert.SerializeObject(job, Formatting.None);
    }

    private string getJson(string name, string input)
    {
        JObject job = JObject.Parse(@"{""" + name + @""": """ + input.ToString() + @"""}");
        return JsonConvert.SerializeObject(job, Formatting.None);
    }

    // NewProduct
    private string getQRCode(string data)
    {
        //string savePath = @"D:\\web\\PlatformAPI\\QRCode\\";
        string savePath = @"E:\\Git Project\\NPUST_sales-of-agricultural-products-Service\\agricultural-products-Service\\QRCode\\";
        string saveName = DateTime.Now.ToString("yyyyMMdd") + System.Guid.NewGuid().ToString() + ".jpg";
        string returnURL = savePath + saveName;
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new QRCode(qrCodeData);
        Bitmap qrCodeImage = qrCode.GetGraphic(20);
        System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
        imgBarCode.Height = 150;
        imgBarCode.Width = 150;
        using (Bitmap bitMap = qrCode.GetGraphic(20))
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = ms.ToArray();
                try
                {
                    MemoryStream memoryStream = new MemoryStream(byteImage);
                    FileStream fileStream = new FileStream(savePath + saveName, FileMode.Create);
                    memoryStream.WriteTo(fileStream);
                    memoryStream.Close();
                    fileStream.Close();
                    fileStream = null;
                    memoryStream = null;
                    return returnURL;
                }
                catch (Exception ex)
                {
                    //Response.Write(ex.Message);
                    return "null";
                }
            }
        }
    }

    // SignIn
    private string getIpAddress()
    {
        //登入ip
        string strIpAddr = string.Empty;

        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null || HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf("unknown") > 0)
        {
            strIpAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
        else if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(",") > 0)
        {
            strIpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Substring(1, HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(",") - 1);
        }
        else if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(";") > 0)
        {
            strIpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Substring(1, HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(";") - 1);
        }
        else
        {
            strIpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        }
        return strIpAddr; ;
    }

    // SignIn
    private bool setIdentify(string account, string identify)
    {
        try
        {
            objcon = new SqlConnection(strdbcon);
            objcon.Open();
            sql = "update Member set Identify = '" + identify + "' where Account = '" + account + "'";
            sqlcmd = new SqlCommand(sql, objcon);
            sqlcmd.ExecuteNonQuery();
            objcon.Close();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    // SignIn
    private bool insertSignLog(string MemberID, string Account, string Identify, string Ip)
    {
        try
        {
            objcon = new SqlConnection(strdbcon); // 建立連接
            objcon.Open(); // 開啟連接
            sql = "insert into SignLog(MemberID,Account,SignTime,Identify,IP) values('" +
                MemberID + "','" + Account + "','" + DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss") +
                "','" +Identify + "','" + Ip +"')";
            sqlcmd = new SqlCommand(sql, objcon); // 建立SQL命令對象
            sqlcmd.ExecuteNonQuery();
            objcon.Close(); // 關閉連接
            return true;
        }
        catch (Exception ex)
        {
            //Response.Write(ex.Message);
            return false;
        }
    }

    // ResetPassword
    private bool updatePassword(string MemberID, string NewPassword)
    {
        try
        {
            objcon = new SqlConnection(strdbcon);
            objcon.Open();
            sql = "update Member set Password = @password where MemberID = @memberID";
            sqlcmd = new SqlCommand(sql, objcon);
            sqlcmd.Parameters.Add("@memberID", SqlDbType.NVarChar).Value = MemberID;
            sqlcmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = NewPassword;
            sqlcmd.ExecuteNonQuery();
            objcon.Close();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}