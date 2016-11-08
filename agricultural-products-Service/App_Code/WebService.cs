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
using System.Web.Script.Serialization;


/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://127.0.0.1/AgriculturalProducts/")]
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
        return new JavaScriptSerializer().Serialize("Hello World");
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
            sql = "select * from Test"; // SQL語法
            sqlcmd = new SqlCommand(sql, objcon); // 建立SQL命令對象

            // 只需修改以下部分---------------------------------------------------------------------------------------
            // 取得回傳值(查詢、修改、刪除使用此語法)，新增語法使用 sqlcmd.ExecuteNonQuery();
            SqlDataReader dr = sqlcmd.ExecuteReader();
            if (dr.IsClosed == false) // 確認資料庫開啟
            {
                while (dr.Read()) // 讀取資料
                {
                    ReturnContant += dr[0] + "," + dr[1] + ";"; // 同一筆資料不同欄位用逗號隔開，不同資料用分號隔開
                }
                dr.Close(); // 停止讀取資料
                objcon.Close(); // 關閉連接
            }
            // ------------------------------------------------------------------------------------------------------

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
            sql = "select * from Test"; // SQL語法
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

    // 登入方法，密碼正確回傳"True"，錯誤則回傳錯誤訊息
    [WebMethod]
    public string LoginSystem (string Account, string Password)
    {
        string returnContant = "";
        try
        {
            objcon = new SqlConnection(strdbcon);
            objcon.Open();
            sql = "select Account,Password from Member where Account = @account";
            sqlcmd = new SqlCommand(sql, objcon);
            sqlcmd.Parameters.Add("@account", SqlDbType.NVarChar).Value = Account;
            SqlDataReader dr = sqlcmd.ExecuteReader();
            if (dr.IsClosed == false)
            {
                while (dr.Read())
                {
                    if ( dr[1].ToString().Equals(Password) )
                    {
                        returnContant = "True";
                    }
                    else
                    {
                        returnContant = "Account or password is wrong";
                    }
                }
                dr.Close();
                objcon.Close();
            }
            else
            {
                returnContant = "Account doesn't exist.";
            }
            

        }
        catch (Exception ex)
        {
            //Response.Write(ex.Message);
        }
        return returnContant;
    }

    [WebMethod]
    public void getQRCode(string data)
    {
        string savePath = @"E:\\Git Project\\NPUST_sales-of-agricultural-products-Service\\agricultural-products-Service\\QRCode\\";
        string saveName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
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
                }
                catch (Exception ex)
                {
                    //Response.Write(ex.Message);
                }
            }
        }
    }
}