﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

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
    private SQLMethod sqlMethod = new SQLMethod();
    private GetMethod gm = new GetMethod();

    public WebService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    // 用於測試方法，Commit前記得回復原狀
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void HelloWorld()
    {
        
    }

    // [WebMethod]要加在方法上方
    // [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    // 方法名稱直白，單字第一個字需大寫

    // Main Method

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SignIn(string Account, string Password)
    {
        string identify = "";
        string memberID = "";
        string ip = "";
        string msg = "";
        string jsonStr = "";
        bool isSign = false;
        bool isIdentify = false;
        bool isLog = false;
        sql = "select MemberID,Password from Member where Account = '" + Account + "'";
        JObject job = JObject.Parse(sqlMethod.SignSelect(sql, "MemberID;Password"));
        if (job["Password"].ToString().Equals(Password))
        {
            isSign = true;
            identify = gm.getUUID();
            memberID = job["MemberID"].ToString();
            ip = gm.getIpAddress();
            msg = "Sign success";
        }
        else
        {
            msg = "Account or password was wrong, please try again.";
        }
        if (isSign)
        {
            // Update Member table's identify
            sql = "update Member set Identify = '" + identify + "' where (MemberID = " + memberID + ")";
            job = JObject.Parse(sqlMethod.Update(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
            {
                isIdentify = true;
            }
            
            // insert SignLog table
            sql = "insert into SignLog(MemberID,Account,SignTime,Identify,IP) values('" + memberID + "','" 
                + Account + "','" + DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss") + "','" + identify + "','" + ip + "')";
            job = JObject.Parse(sqlMethod.Update(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
            {
                isLog = true;
            }
        }
        if (isSign && isIdentify && isLog)
        {
            jsonStr = gm.getJsonArray("stage;message;uuid", isSign.ToString() + ";" + msg + ";" + identify);
            return jsonStr;
        }
        else
        {
            jsonStr = gm.getJsonArray("stage;message;uuid", false.ToString() + ";" + msg + ";null");
            return jsonStr;
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProduct(string CompanyID, string CompanyName, string ProductName, string Type, string Introduction,
                                string AdditionalValue, string Origin, string Image, string PackagingDate, string Verification,
                                string ValidityPeriod, string ValidityNumber)
    {
        string id = "null";
        string qr = "null";
        QRCodeMethod qrcm = new QRCodeMethod();

        sql = "insert into Product(CompanyID,CompanyName,ProductName,Type,Introduction,AdditionalValue,Origin," +
                "Image,PackagingDate,Verification,ValidityPeriod,ValidityNumber) values ('" + CompanyID + "','" +
                CompanyName + "','" + ProductName + "','" + Type + "','" + Introduction + "','" + AdditionalValue + "','" +
                Origin + "','" + Image + "','" + PackagingDate + "','" + Verification + "','" + ValidityPeriod + "','" + ValidityNumber +
                "');SELECT SCOPE_IDENTITY()";
        JObject job = gm.getJsonResult(sqlMethod.InsertSelect(sql));
        id = job["ProductID"].ToString();
        qr = qrcm.GetQRCode(id);
        sql = "update Product set QRCode = '" + qr + "' where ProductID = '" + id + "'";
        return sqlMethod.Update(sql);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ResetPassword(string Identify, string OldPassword, string NewPassword)
    {
        sql = "select MemberID,Password from Member where Identify = '" + Identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.SelectSingle(sql, "MemberID;Password", 2));
        string id = job["MemberID"].ToString();
        if (job["Password"].ToString().Equals(OldPassword))
        {
            sql = "update Member set Password = '" + NewPassword + "' where MemberID = '" + id + "'";
            return sqlMethod.Update(sql);
        }
        else
        {
            return gm.getStageJson(false, "Password is wrong");
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetIntroduction(string input)
    {
        int id = 0;
        string jsonStr = "";
        sql = @"select Attributes,BigItem,SmallItem,ProductName,Period,PDF from Introduction"; // SQL語法
        if (input.Equals("ALL"))
        {
            JObject job = JObject.Parse(sqlMethod.SelectSingle(sql, "Attributes;BigItem;SmallItem;ProductName;Period;PDF", 6));
            jsonStr = JsonConvert.SerializeObject(job, Formatting.None);
            return jsonStr.Replace(@"\r\n", string.Empty);//移除那些特殊字元
        }
        else
        {
            input = Regex.Replace(input, "[^0-9]", "");//移除非數字的字元
            if (input.Equals(""))
            {
                return gm.getStageJson(false, "Input Number");
            }
            else
            {
                id = int.Parse(input);
            }
            sql += @" where IntroductionID=" + id;
            jsonStr = sqlMethod.Select(sql);
            return jsonStr.Replace(@"\r\n", string.Empty);//移除那些特殊字元
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMemberInfo(string input)
    {
        int id;
        input = Regex.Replace(input, "[^0-9]", "");//移除非數字的字元
        if (input.Equals(""))
        {
            return gm.getStageJson(false, "Input Number");
        }
        else
        {
            id = int.Parse(input);
        }
        sql = @"select Account,FirstName,LastName,Phone,Email,CompanyName,Address,Access from Member where MemberID=" + id; // SQL語法
        JObject job = JObject.Parse(sqlMethod.SelectSingle(sql, "Account;FirstName;LastName;Phone;Email;CompanyName;Address;Access", 8));
        return JsonConvert.SerializeObject(job, Formatting.None);
    }


    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewMember(string Account, string Password, string FirstName, string LastName, string Phone, string Email,
                            string CompanyName, string Address, string Access)
    {
        sql = "insert into Member(Account, Password, FirstName, LastName, Phone, Email, CompanyName, Address, Access) " +
                  "values('" + Account + "','" + Password + "','" + FirstName + "','" + LastName + "','" + Phone + "','" +
                  Email + "','" + CompanyName + "','" + Address + "','" + Access + "')";

        return sqlMethod.Insert(sql);
    }

}