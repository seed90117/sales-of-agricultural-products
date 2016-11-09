using System;
using System.Web.Script.Services;
using System.Web.Services;
using System.Data.SqlClient;
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
    private MainMethod main = new MainMethod();
    private JObject job;

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


    // JSON Method
    
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SignInJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string account = job["Account"].ToString();
            string password = job["Password"].ToString();
            if (!account.Equals("") && !password.Equals(""))
                return main.SignIn(account, password);
            else
                return gm.getStageJson(false, "Account or password can't be null.");
        }
        catch (Exception ex)
        {
            return gm.getStageJson(false, "Json data can't be null.");
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProductJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string companyID = job["CompanyID"].ToString();
            string companyName = job["CompanyName"].ToString();
            string productName = job["ProductName"].ToString();
            string type = job["Type"].ToString();
            string introduction = job["Introduction"].ToString();
            string additionalValue = job["AdditionalValue"].ToString();
            string origin = job["Origin"].ToString();
            string image = job["Image"].ToString();
            string packagingDate = job["PackagingDate"].ToString();
            string verification = job["Verification"].ToString();
            string validityPeriod = job["ValidityPeriod"].ToString();
            string validityNumber = job["ValidityNumber"].ToString();
            if (!companyID.Equals("") && !companyName.Equals("") && !productName.Equals("") && !type.Equals("") && !introduction.Equals("") &&
                !additionalValue.Equals("") && !origin.Equals("") && !image.Equals("") && !packagingDate.Equals("") && !verification.Equals("") &&
                !validityPeriod.Equals("") && !validityNumber.Equals(""))
                return main.NewProduct(companyID, companyName, productName, type, introduction, additionalValue, origin, image, packagingDate, verification, validityPeriod, validityPeriod);
            else
                return gm.getStageJson(false, "Data can't be null.");
        }
        catch (Exception ex)
        {
            return gm.getStageJson(false, "Json data can't be null");
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ResetPasswordJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            string oldPassword = job["OldPassword"].ToString();
            string newPassword = job["NewPassword"].ToString();
            if (!identify.Equals("") && !oldPassword.Equals("") && !newPassword.Equals(""))
                return main.ResetPassword(identify, oldPassword, newPassword);
            else
                return gm.getStageJson(false, "Data can't be null.");
        }
        catch (Exception ex)
        {
            return gm.getStageJson(false, "Json data can't be null.");
        }
    }
    
    // 待確認
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetIntroductionJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            string oldPassword = job["OldPassword"].ToString();
            string newPassword = job["NewPassword"].ToString();
            if (!identify.Equals("") && !oldPassword.Equals("") && !newPassword.Equals(""))
                return main.ResetPassword(identify, oldPassword, newPassword);
            else
                return gm.getStageJson(false, "Data can't be null.");
        }
        catch (Exception ex)
        {
            return gm.getStageJson(false, "Json data can't be null.");
        }
    }

    // 待確認
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMemberInfoJson(string input)
    {
        int id;
        input = Regex.Replace(input, "[^0-9]", "");//移除非數字的字元

        if (input.Equals(""))
            return gm.getStageJson(false, "Input Number");
        else
            id = int.Parse(input);

        sql = @"select Account,FirstName,LastName,Phone,Email,CompanyName,Address,Access from Member where MemberID=" + id; // SQL語法
        JObject job = JObject.Parse(sqlMethod.SelectSingle(sql, "Account;FirstName;LastName;Phone;Email;CompanyName;Address;Access"));
        return JsonConvert.SerializeObject(job, Formatting.None);
    }


    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewMemberJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string account = job["Account"].ToString();
            string password = job["Password"].ToString();
            string firstName = job["FirstName"].ToString();
            string lastName = job["LastName"].ToString();
            string phone = job["Phone"].ToString();
            string email = job["Email"].ToString();
            string companyName = job["CompanyName"].ToString();
            string address = job["Address"].ToString();
            string access = job["Access"].ToString();
            if (!account.Equals("") && !password.Equals("") && !firstName.Equals("") && 
                !lastName.Equals("") && !phone.Equals("") && !email.Equals("") && 
                !companyName.Equals("") && !address.Equals("") && !access.Equals(""))
                return main.NewMember(account, password, firstName, lastName, phone, email, companyName, address, access);
            else
                return gm.getStageJson(false, "Data can't be null.");
        }
        catch (Exception ex)
        {
            return gm.getStageJson(false, "Json data can't be null.");
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewRecordJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            string productID = job["ProductID"].ToString();
            string type = job["Type"].ToString();
            string action = job["Action"].ToString();
            string note = job["Note"].ToString();
            if (!identify.Equals("") && !productID.Equals("") && !type.Equals("") && !action.Equals("") && !note.Equals(""))
                return main.NewRecord(identify, productID, type, action, note);
            else
                return gm.getStageJson(false, "Data can't be null.");
        }
        catch (Exception ex)
        {
            return gm.getStageJson(false, "Json data can't be null.");
        }
    }


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
            msg = "Account or password was wrong, please try again.";

        if (isSign)
        {
            // Update Member table's identify
            sql = "update Member set Identify = '" + identify + "' where (MemberID = " + memberID + ")";
            job = JObject.Parse(sqlMethod.Update(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
                isIdentify = true;

            // insert SignLog table
            sql = "insert into SignLog(MemberID,Account,SignTime,Identify,IP) values('" + memberID + "','"
                + Account + "','" + gm.getCurrentDate() + "','" + identify + "','" + ip + "')";
            job = JObject.Parse(sqlMethod.Update(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
                isLog = true;
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
        JObject job = gm.getJsonResult(sqlMethod.SelectSingle(sql, "MemberID;Password"));
        string id = job["MemberID"].ToString();
        if (job["Password"].ToString().Equals(OldPassword))
        {
            sql = "update Member set Password = '" + NewPassword + "' where MemberID = '" + id + "'";
            return sqlMethod.Update(sql);
        }
        else
            return gm.getStageJson(false, "Password is wrong");
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
            JObject job = JObject.Parse(sqlMethod.SelectSingle(sql, "Attributes;BigItem;SmallItem;ProductName;Period;PDF"));
            jsonStr = JsonConvert.SerializeObject(job, Formatting.None);
            return jsonStr.Replace(@"\r\n", string.Empty);//移除那些特殊字元
        }
        else
        {
            input = Regex.Replace(input, "[^0-9]", "");//移除非數字的字元

            if (input.Equals(""))
                return gm.getStageJson(false, "Input Number");
            else
                id = int.Parse(input);

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
            return gm.getStageJson(false, "Input Number");
        else
            id = int.Parse(input);

        sql = @"select Account,FirstName,LastName,Phone,Email,CompanyName,Address,Access from Member where MemberID=" + id; // SQL語法
        JObject job = JObject.Parse(sqlMethod.SelectSingle(sql, "Account;FirstName;LastName;Phone;Email;CompanyName;Address;Access"));
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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewRecord(string identify, string productID, string type, string action, string note)
    {
        string memberID = "";
        string creator = "";
        string productName = "";
        bool isMember = false;
        bool isProductName = false;

        // Get memberID and creator
        sql = "select MemberID, FirstName, LastName from Member where Identify = '" + identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.SelectSingle(sql, "MemberID;FirstName;LastName"));
        memberID = job["MemberID"].ToString();
        creator = job["LastName"].ToString() + job["FirstName"].ToString();
        if (!memberID.Equals("") && !creator.Equals(""))
            isMember = true;

        // Get ProductName
        sql = "select ProductName from Product where ProductID = '" + productID + "'";
        job = gm.getJsonResult(sqlMethod.SelectSingle(sql, "ProductName"));
        productName = job["ProductName"].ToString();
        if (!productName.Equals(""))
            isProductName = true;

        // Insert record data
        if (isProductName && isMember)
        {
            sql = "insert into Record(MemberID, Creator, ProductID, ProductName, Date, Type, Action, Note) values('" + memberID + "','" +
                creator + "','" + productID + "','" + productName + "','" + gm.getCurrentDate() + "','" + type + "','" + action + "','" +
                note + "')";
            return sqlMethod.Insert(sql);
        }
        else
        {
            if (!isMember)
                return gm.getStageJson(false, "Member information is wrong.");
            else if (!isProductName)
                return gm.getStageJson(false, "ProductID is wrong.");
            else
                return gm.getStageJson(false, "Inserting record is wrong.");
        }
    }
    
}