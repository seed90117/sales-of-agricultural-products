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
    public string HelloWorld(string file)
    {
        return main.test(file);
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
            return gm.getStageJson(false, msg.fail);
        }
    }

    // [WebMethod]要加在方法上方
    // [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    // 方法名稱直白，單字第一個字需大寫


    // JSON Method
    // SignIn
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
            return gm.getStageJson(false, msg.signError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SignOutJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            if (!identify.Equals(""))
                return main.SignOut(identify);
            else
                return gm.getStageJson(false, msg.signNullError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.fail);
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
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.noData_cht);
        }
    }

    // Member
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
            string email = job["Email"].ToString();
            string address = job["Address"].ToString();
            if (!password.Equals("") && !firstName.Equals("") && !lastName.Equals("") && !email.Equals(""))
                return main.NewMember(email, password, firstName, lastName, phone, address);
            else
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
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
            {
                if (image.Equals(""))
                    return gm.getStageJson(false, msg.unSelectFile_cht);
                else
                    return gm.getStageJson(false, msg.dataError_cht);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
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
            {
                if (video.Equals(""))
                    return gm.getStageJson(false, msg.unSelectFile_cht);
                else
                    return gm.getStageJson(false, msg.dataError_cht);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
        }
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
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.noData_cht);
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
            return gm.getStageJson(false, msg.noData_cht);
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
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.noData_cht);
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
                return main.UpdateMemberInfo(idnetify, column, value);
            else
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
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
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
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
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
        }
    }


    // Product
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProductJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            string farmID = job["FarmID"].ToString();
            string productName = job["ProductName"].ToString();
            string typeBig = job["TypeBig"].ToString();
            string typeSmall = job["TypeSmall"].ToString();
            string introduction = job["Introduction"].ToString();
            string additionalValue = job["AdditionalValue"].ToString();
            string origin = job["Origin"].ToString();
            string price = job["Price"].ToString();
            string amount = job["Amount"].ToString();
            string packagingDate = job["PackagingDate"].ToString();
            string validityPeriod = job["ValidityPeriod"].ToString();
            string verificationID = job["ValidityID"].ToString();
            string stage = job["Stage"].ToString();
            if (!identify.Equals("") && !farmID.Equals("") && !productName.Equals("") && !typeBig.Equals("") && !typeSmall.Equals("") &&
                !introduction.Equals("") && !additionalValue.Equals("") && !origin.Equals("") && !price.Equals("") && !amount.Equals("") &&
                !packagingDate.Equals("") && !validityPeriod.Equals("") && !verificationID.Equals(""))
                return main.NewProduct(identify, farmID, productName, typeBig, typeSmall, introduction, additionalValue, origin,
                    price, amount, packagingDate, validityPeriod, verificationID, stage);
            else
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProduct2Json(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            string productName = job["ProductName"].ToString();
            string typeBig = job["TypeBig"].ToString();
            string price = job["Price"].ToString();
            string paymentmethod = job["PaymentMethod"].ToString();
            string shipmentsmethod = job["ShipmentsMethod"].ToString();
            string location = job["Location"].ToString();
            string amount = job["Amount"].ToString();
            string specification = job["Specification"].ToString();
            string imageBig = job["ImageBig"].ToString();
            string imageSmall = job["ImageSmall"].ToString();
            string introduction = job["Introduction"].ToString();

            if (!identify.Equals("") && !productName.Equals("") && !typeBig.Equals("") && !price.Equals("") && !paymentmethod.Equals("") &&
                !shipmentsmethod.Equals("") && !location.Equals("") && !amount.Equals("") && !specification.Equals("") && !imageBig.Equals("") &&
                !imageSmall.Equals("") && !introduction.Equals(""))
                return main.NewProduct2(identify, productName, typeBig, price, paymentmethod, shipmentsmethod, location, amount, specification,
            imageBig, imageSmall, introduction);
            else
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
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
            {
                if (image.Equals(""))
                    return gm.getStageJson(false, msg.unSelectFile_cht);
                else
                    return gm.getStageJson(false, msg.dataError_cht);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProductFileJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string productID = job["ProductID"].ToString();
            string fileName = job["FileName"].ToString();
            string file = job["File"].ToString();
            if (!productID.Equals("") && !fileName.Equals("") && !file.Equals(""))
                return main.NewProductFile(productID, fileName, file);
            else
            {
                if (file.Equals(""))
                    return gm.getStageJson(false, msg.unSelectFile_cht);
                else
                    return gm.getStageJson(false, msg.dataError_cht);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
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
            return gm.getStageJson(false, msg.noData_cht);
        }
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
            string CCID = job["CCID"].ToString();
            string orderPrice = job["Price"].ToString();
            string orderHot = job["Hot"].ToString();
            return main.GetProductKey(bigItem, smallItem, value, CCID, orderPrice, orderHot);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.noData_cht);
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
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.noData_cht);
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
    public string GetRandomProductJson()
    {
        return main.GetRandomProduct();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductTypeJson()
    {
        return main.GetProductType();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductTypeItemJson()
    {
        return main.GetProductTypeItem();
    }


    // Record
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
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
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
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.noData_cht);
        }
    }


    // Order
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
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
        }
    }


    // Cooperation
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
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.dataError_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetCooperationJson()
    {
        return main.GetCooperation();
    }


    // Introduction
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetIntroductionJson()
    {
        return main.GetIntroduction();
    }

    //E-mail
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string EMailCheckJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string mailto = job["MailTo"].ToString();
            if (!mailto.Equals(""))
                return main.EMailCheck(mailto);
            else
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.noData_cht);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string EMailResetPasswordJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string identify = job["Identify"].ToString();
            string newpassword = job["NewPassword"].ToString();
            if (!identify.Equals("") && !newpassword.Equals(""))
                return main.EMailResetPassword(identify, newpassword);
            else
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.noData_cht);
        }
    }

    //顯示農夫匯款帳號或地址
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMemberPaymentMethodJson(string inputJsonStr)
    {
        job = gm.getJsonResult(inputJsonStr);
        try
        {
            string MemberID = job["MemberID"].ToString();
            if (!MemberID.Equals(""))
                return main.GetMemberPaymentMethod(MemberID);
            else
                return gm.getStageJson(false, msg.dataError_cht);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.noData_cht);
        }
    }


    // Main Method------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // SignIn
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SignIn(string Account, string Password)
    {
        return main.SignIn(Account, Password);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SignOut(string identify)
    {
        return main.SignOut(identify);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetSignLog(string Identify)
    {
        return main.GetSignLog(Identify);
    }


    // Member
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewMember(string Email, string Password, string FirstName, string LastName, string Phone, string Address)
    {
        return main.NewMember(Email, Password, FirstName, LastName, Phone, Address);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewFarmMember(string Email, string Password, string FirstName, string LastName, string Phone, string Address, string Birthday, string SSNs, string FarmName, string IdentifyID, string FarmArea, string CultivateArea, string ExpectTime, string ExpectVolume)
    {
        return main.NewFarmMember(MemberID, FarmName, IdentifyID, FarmArea, CultivateArea, ExpectTime, ExpectVolume);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewMemberImage(string Identify, string FileName, string Image)
    {
        return main.NewHeadShot(Identify, FileName, Image);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewVideo(string Identify, string Name, string FileName, string Video)
    {
        return main.NewVideo(Identify, Name, FileName, Video);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMemberInfo(string Identify)
    {
        return main.GetMemberInfo(Identify);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMember(string Access)
    {
        return main.GetMember(Access);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetVideo(string ProductID)
    {
        return main.GetVideo(ProductID);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string UpdateMemberInfo(string Identify, string Column, string Value)
    {
        return main.UpdateMemberInfo(Identify, Column, Value);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ResetPassword(string Identify, string OldPassword, string NewPassword)
    {
        return main.ResetPassword(Identify, OldPassword, NewPassword);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string IsEmail(string Email)
    {
        return main.IsEmail(Email);
    }


    // Product
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProduct(string identify, string farmID, string ProductName, string TypeBig, string TypeSmall, string Introduction,
                                string AdditionalValue, string Origin, string Price, string Amount, string PackagingDate,
                                string ValidityPeriod, string VerificationID, string Stage)
    {
        return main.NewProduct(identify, farmID, ProductName, TypeBig, TypeSmall, Introduction, AdditionalValue, Origin,
                    Price, Amount, PackagingDate, ValidityPeriod, VerificationID, Stage);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProduct2(string identify, string ProductName, string TypeBig, string Price, string PaymentMethod, string ShipmentsMethod, string Location, string Amount, string Specification,
        string ImageBig, string ImageSmall, string Introduction)
    {
        return main.NewProduct2(identify, ProductName, TypeBig, Price, PaymentMethod, ShipmentsMethod, Location, Amount, Specification,
            ImageBig, ImageSmall, Introduction);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProductImage(string ProductID, string ImageType, string FileName, string Image)
    {
        return main.NewProductImage(ProductID, ImageType, FileName, Image);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewProductFile(string ProductID, string FileName, string File)
    {
        return main.NewProductFile(ProductID, FileName, File);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProduct(string ProductID)
    {
        if (ProductID.Equals(""))
            return main.GetProduct("ALL");
        else
            return main.GetProduct(ProductID);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductKey(string BigItem, string SmallItem, string Value, string CCID, string Price, string Hot)
    {
        return main.GetProductKey(BigItem, SmallItem, Value, CCID, Price, Hot);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductColumn(string Column, string Value)
    {
        return main.GetProductColumn(Column, Value);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetTopHotProduct()
    {
        return main.GetTopHotProduct();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRandomProduct()
    {
        return main.GetRandomProduct();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductType()
    {
        return main.GetProductType();
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductTypeItem()
    {
        return main.GetProductTypeItem();
    }
    

    // Record
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewRecord(string Identify, string ProductID, string Type, string Action, string Note)
    {
        return main.NewRecord(Identify, ProductID, Type, Action, Note);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRecord(string ProductID)
    {
        return main.GetRecord(ProductID);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetRecordOperation(string RecordID)
    {
        return main.GetRecordOperation(RecordID);
    }

    // Order
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewOrder(string Identify, string ProductID, string Amount, string Delivery, string Shipment, string Note)
    {
        return main.NewProductOrder(Identify, ProductID, Amount, Delivery, Shipment, Note);
    }


    // Cooperation
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string NewCooperationImage(string CompanyName, string CompanyUrl, string FileName, string Image)
    {
        return main.NewCooperationImage(CompanyName, CompanyUrl, FileName, Image);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetCooperation()
    {
        return main.GetCooperation();
    }


    // Introduction
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetIntroduction()
    {
        return main.GetIntroduction();
    }


    //E-mail
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string EMailCheck(string MailTo)
    {
        return main.EMailCheck(MailTo);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string EMailResetPassword(string Identify, string NewPassword)
    {
        return main.EMailResetPassword(Identify, NewPassword);
    }

    //顯示農夫匯款帳號或地址
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMemberdPaymentMethod(string MemberID)
    {
        return main.GetMemberPaymentMethod(MemberID);
    }

    //取得指定商品的圖片
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductImage(string ProductID)
    {
        return main.GetProductImage(ProductID);
    }

    //取得指定商品介紹
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetProductIntroduce(string ProductID)
    {
        return main.GetProductIntroduce(ProductID);
    }

    //忘記密碼確認信箱
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ForgetPasswordCheckEMail(string MailTo)
    {
        return main.ForgetPasswordCheckEMail(MailTo);
    }

    //忘記密碼寄信部分
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ForgetPasswordEMail(string MailTo)
    {
        return main.ForgetPasswordEMail(MailTo);
    }
}