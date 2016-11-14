using System;
using System.Web.Script.Services;
using System.Web.Services;
using Newtonsoft.Json.Linq;

/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://140.127.22.4/AgriculturalProducts/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{
    private GetMethod gm = new GetMethod();
    private MainMethod main = new MainMethod();
    private Message msg = new Message();
    private JObject job;

    public WebService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    // 用於測試方法，Commit前記得回復原狀
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string HelloWorld()
    {
        return "Hello World";
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string TextJsonProduce(string Content)
    {
        return gm.getJson("Content", Content);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string TestJsonInput(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string content = job["Content"].ToString();
            if (!content.Equals(""))
                return gm.getStageJson(true, "Input success, value is " + content);
            else
                return gm.getStageJson(false, "Input fail");
        }
        catch (Exception ex)
        {
            return gm.getStageJson(false, msg.jsonError_cht);
        }
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
                return gm.getStageJson(false, msg.signNullError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
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
            string price = job["Price"].ToString();
            if (!companyID.Equals("") && !companyName.Equals("") && !productName.Equals("") && !type.Equals("") && !introduction.Equals("") &&
                !additionalValue.Equals("") && !origin.Equals("") && !image.Equals("") && !packagingDate.Equals("") && !verification.Equals("") &&
                !validityPeriod.Equals("") && !validityNumber.Equals("") && !price.Equals(""))
                return main.NewProduct(companyID, companyName, productName, type, introduction, additionalValue, origin, image, packagingDate, verification, validityPeriod, validityPeriod, price);
            else
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
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
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
        }
    }
    
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetIntroductionJson()
    {
        return main.GetIntroduction();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMemberInfoJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            if (!identify.Equals(""))
                return main.GetMemberInfo(identify);
            else
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
        }
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
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
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
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string UpdateMemberInfoJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string column = job["Column"].ToString();
            string value = job["Value"].ToString();
            if (!column.Equals("") && !value.Equals(""))
                return main.UpdateMemberInfo(column, value);
            else
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewOrderJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            string productID = job["ProductID"].ToString();
            string amount = job["Amount"].ToString();
            string delivery = job["Delivery"].ToString();
            string shipment = job["Shipment"].ToString();
            string note = job["Note"].ToString();
            if (!identify.Equals("") && !productID.Equals("") && !amount.Equals("") && !delivery.Equals("") && !shipment.Equals("") && !note.Equals(""))
                return main.NewProductOrder(identify, productID, amount, delivery, shipment, note);
            else
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMemberJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string access = job["Access"].ToString();
            if (!access.Equals(""))
                return main.GetMember(access);
            else
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string productID = job["ProductID"].ToString();
            if (!productID.Equals(""))
                return main.GetProduct(productID);
            else
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRecordJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string productID = job["ProductID"].ToString();
            if (!productID.Equals(""))
                return main.GetRecord(productID);
            else
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetSignLogJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            if (!identify.Equals(""))
                return main.GetSignLog(identify);
            else
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductColumnJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string column = job["Column"].ToString();
            string value = job["Value"].ToString();
            if (!column.Equals("") && !value.Equals(""))
                return main.GetProductColumn(column, value);
            else
                return gm.getStageJson(false, msg.inputDataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.jsonError_cht);
        }
    }


    // Main Method
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SignIn(string Account, string Password)
    {
        return main.SignIn(Account, Password);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProduct(string CompanyID, string CompanyName, string ProductName, string Type, string Introduction,
                                string AdditionalValue, string Origin, string Image, string PackagingDate, string Verification,
                                string ValidityPeriod, string ValidityNumber, string Price)
    {
        return main.NewProduct(CompanyID, CompanyName, ProductName, Type, Introduction, AdditionalValue, Origin, 
                               Image, PackagingDate, Verification, ValidityPeriod, ValidityNumber, Price);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ResetPassword(string Identify, string OldPassword, string NewPassword)
    {
        return main.ResetPassword(Identify, OldPassword, NewPassword);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetIntroduction()
    {
        return main.GetIntroduction();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMemberInfo(string Identify)
    {
        return main.GetMemberInfo(Identify);
    }


    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewMember(string Account, string Password, string FirstName, string LastName, string Phone, string Email,
                            string CompanyName, string Address, string Access)
    {
        return main.NewMember(Account, Password, FirstName, LastName, Phone, Email, CompanyName, Address, Access);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewRecord(string Identify, string ProductID, string Type, string Action, string Note)
    {
        return main.NewRecord(Identify, ProductID, Type, Action, Note);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string UpdateMemberInfo(string column, string value)
    {
        return main.UpdateMemberInfo(column, value);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewOrder(string Identify, string ProductID, string Amount, string Delivery, string Shipment, string Note)
    {
        return main.NewProductOrder(Identify, ProductID, Amount, Delivery, Shipment, Note);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMember(string Access)
    {
        return main.GetMember(Access);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProduct(string ProductID)
    {
        return main.GetProduct(ProductID);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRecord(string ProductID)
    {
        return main.GetRecord(ProductID);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetSignLog(string Identify)
    {
        return main.GetSignLog(Identify);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductColumn(string Column, string Value)
    {
        return main.GetProductColumn(Column, Value);
    }
}