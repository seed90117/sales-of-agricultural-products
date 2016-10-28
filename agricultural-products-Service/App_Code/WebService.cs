using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Data.SqlClient;
using System.Data;


/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://140.127.22.4/AgriculturalProducts/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{

    string strdbcon = "server=140.127.22.4;database=AgriculturalProducts;uid=CCBDA;pwd=CCBDA";
    SqlConnection objcon;
    SqlCommand sqlcmd;
    string sql;

    public WebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld()
    {
        return "Hello World";
    }

    // [WebMethod]要加在方法上方
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

    // 登入方法，密碼正確回傳"True"，錯誤則回傳錯誤訊息
    [WebMethod]
    public string LoginSystem (string Account, string Password)
    {
        string ReturnContant = "";
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
                        ReturnContant = "True";
                    }
                    else
                    {
                        ReturnContant = "Account or password is wrong";
                    }
                }
                dr.Close();
                objcon.Close();
            }
            else
            {
                ReturnContant = "Account doesn't exist.";
            }
            

        }
        catch (Exception ex)
        {
            //Response.Write(ex.Message);
        }
        return ReturnContant;
    }

}
