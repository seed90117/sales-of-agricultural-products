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
        return main.GetProductType();
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
            string amount = job["Amount"].ToString();
            string price = job["Price"].ToString();
            string stage = job["Stage"].ToString();
            if (!companyID.Equals("") && !companyName.Equals("") && !productName.Equals("") && !type.Equals("") && !introduction.Equals("") &&
                !additionalValue.Equals("") && !origin.Equals("") && !image.Equals("") && !packagingDate.Equals("") && !verification.Equals("") &&
                !validityPeriod.Equals("") && !validityNumber.Equals("") && !price.Equals(""))
                return main.NewProduct(companyID, companyName, productName, type, introduction, additionalValue, origin, packagingDate, 
                                        verification, validityPeriod, validityPeriod, amount, price, stage);
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
            string password = job["Password"].ToString();
            string firstName = job["FirstName"].ToString();
            string lastName = job["LastName"].ToString();
            string phone = job["Phone"].ToString();
            string fax = job["Fax"].ToString();
            string cellPhone = job["CellPhone"].ToString();
            string email = job["Email"].ToString();
            string address = job["Address"].ToString();
            if (!password.Equals("") && !firstName.Equals("") && !lastName.Equals("") && !email.Equals(""))
                return main.NewMember(email, password, firstName, lastName, phone, fax, cellPhone,address);
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
            string idnetify = job["Identify"].ToString();
            string column = job["Column"].ToString();
            string value = job["Value"].ToString();
            if (!column.Equals("") && !value.Equals(""))
                return main.UpdateMemberInfo(idnetify,column, value);
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
            if (access.Equals(""))
                return main.GetMember("ALL");
            else
                return main.GetMember(access);
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
            if (productID.Equals(""))
                return main.GetProduct("ALL");
            else
                return main.GetProduct(productID);
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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProductImageJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string productID = job["ProductID"].ToString();
            string imageType = job["ImageType"].ToString();
            string fileName = job["FileName"].ToString();
            string image = job["Image"].ToString();
            if (!productID.Equals("") && !imageType.Equals("") && !fileName.Equals("") && !image.Equals(""))
                return main.NewProductImage(productID, imageType, fileName, image);
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
    public string GetTopHotProductJson()
    {
        return main.GetTopHotProduct();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetCooperationJson()
    {
        return main.GetCooperation();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductKeyJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string bigItem = job["BigItem"].ToString();
            string smallItem = job["SmallItem"].ToString();
            string value = job["Value"].ToString();
            if (!bigItem.Equals("") && !smallItem.Equals("") && !value.Equals(""))
                return main.GetProductKey(bigItem, smallItem, value);
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
    public string GetVideoJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string productID = job["ProductID"].ToString();
            if (!productID.Equals(""))
                return main.GetVideo(productID);
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
    public string NewVideoJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            string name = job["Name"].ToString();
            string fileName = job["FileName"].ToString();
            string video = job["Video"].ToString();
            if (!identify.Equals("") && !name.Equals("") && !fileName.Equals("") && !video.Equals(""))
                return main.NewVideo(identify, name, fileName, video);
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
    public string NewMemberImageJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            string fileName = job["FileName"].ToString();
            string image = job["Image"].ToString();
            if (!identify.Equals("") && !fileName.Equals("") && !image.Equals(""))
                return main.NewHeadShot(identify, fileName, image);
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
    public string NewCooperationImageJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string companyName = job["CompanyName"].ToString();
            string companyUrl = job["CompanyUrl"].ToString();
            string fileName = job["FileName"].ToString();
            string image = job["Image"].ToString();
            if (!companyName.Equals("") && !companyUrl.Equals("") && !fileName.Equals("") && !image.Equals(""))
                return main.NewCooperationImage(companyName, companyUrl, fileName, image);
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
    public string IsEmailJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string email = job["Email"].ToString();
            if (!email.Equals(""))
                return main.IsEmail(email);
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
    public string GetProductTypeJson()
    {
        return main.GetProductType();
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
                                string ValidityPeriod, string ValidityNumber, string Amount, string Price, string Stage)
    {
        return main.NewProduct(CompanyID, CompanyName, ProductName, Type, Introduction, AdditionalValue, Origin, 
                               PackagingDate, Verification, ValidityPeriod, ValidityNumber, Amount, Price, Stage);
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
    public string NewMember(string Email, string Password, string FirstName, string LastName, string Phone, string fax, string cellPhone, string Address)
    {
        return main.NewMember(Email, Password, FirstName, LastName, Phone, fax, cellPhone, Address);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewRecord(string Identify, string ProductID, string Type, string Action, string Note)
    {
        return main.NewRecord(Identify, ProductID, Type, Action, Note);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string UpdateMemberInfo(string Identify, string Column, string Value)
    {
        return main.UpdateMemberInfo(Identify, Column, Value);
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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProductImage(string ProductID, string ImageType,string FileName, string Image)
    {
        return main.NewProductImage(ProductID, ImageType,  FileName, Image);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetTopHotProduct()
    {
        return main.GetTopHotProduct();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetCooperation()
    {
        return main.GetCooperation();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductKey(string BigItem, string SmallItem, string Value)
    {
        return main.GetProductKey(BigItem, SmallItem, Value);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetVideo(string ProductID)
    {
        return main.GetVideo(ProductID);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewVideo(string Identify, string Name, string FileName, string Video)
    {
        return main.NewVideo(Identify, Name, FileName, Video);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewMemberImage(string Identify, string FileName, string Image)
    {
        return main.NewHeadShot(Identify, FileName, Image);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewCooperationImage(string CompanyName, string CompanyUrl, string FileName, string Image)
    {
        return main.NewCooperationImage(CompanyName, CompanyUrl, FileName, Image);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string IsEmail(string Email)
    {
        return main.IsEmail(Email);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductType()
    {
        return main.GetProductType();
    }
}