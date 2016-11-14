using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for MainMethod
/// </summary>
public class MainMethod
{
    private string sql;
    private SQLMethod sqlMethod = new SQLMethod();
    private GetMethod gm = new GetMethod();
    private Message msg = new Message();

    // 方法名稱直白，單字第一個字需大寫
    // 變數第一個單字須小寫

    public string SignIn(string account, string password) // By Kevin Yen
    {
        string identify = "";
        string memberID = "";
        string ip = "";
        string reMsg = "";
        string jsonStr = "";
        bool isSign = false;
        bool isIdentify = false;
        bool isLog = false;
        sql = "select MemberID,Password from Member where Account = '" + account + "'";
        JObject job = gm.getJsonResult(sqlMethod.SignSelect(sql, "MemberID;Password"));
        if (job["Password"].ToString().Equals(password))
        {
            isSign = true;
            identify = gm.getUUID();
            memberID = job["MemberID"].ToString();
            ip = gm.getIpAddress();
            reMsg = msg.success;
        }
        else
            reMsg = msg.signError_cht;

        if (isSign)
        {
            // Update Member table's identify
            sql = "update Member set Identify = '" + identify + "' where (MemberID = " + memberID + ")";
            job = JObject.Parse(sqlMethod.Update(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
                isIdentify = true;

            // insert SignLog table
            sql = "insert into SignLog(MemberID,Account,SignTime,Identify,IP) values('" + memberID + "','"
                + account + "','" + gm.getCurrentDate() + "','" + identify + "','" + ip + "')";
            job = JObject.Parse(sqlMethod.Update(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
                isLog = true;
        }
        if (isSign && isIdentify && isLog)
        {
            jsonStr = gm.getJsonArray("stage;message;uuid", isSign.ToString() + ";" + reMsg + ";" + identify);
            return jsonStr;
        }
        else
        {
            jsonStr = gm.getJsonArray("stage;message;uuid", false.ToString() + ";" + reMsg + ";null");
            return jsonStr;
        }
    }

    public string NewProduct(string companyID, string companyName, string productName, string type, string introduction,
                                string additionalValue, string origin, string packagingDate, string verification,
                                string validityPeriod, string validityNumber, string price) // By Kevin Yen
    {
        string id = "null";
        string qr = "null";
        QRCodeMethod qrcm = new QRCodeMethod();

        sql = "insert into Product(CompanyID,CompanyName,ProductName,Type,Introduction,AdditionalValue,Origin," +
                "PackagingDate,Verification,ValidityPeriod,ValidityNumber,Price) values ('" + companyID + "','" +
                companyName + "','" + productName + "','" + type + "','" + introduction + "','" + additionalValue + "','" +
                origin + "','" + packagingDate + "','" + verification + "','" + validityPeriod + "','" +
                validityNumber + "'," + price + ");SELECT SCOPE_IDENTITY()";
        JObject job = gm.getJsonResult(sqlMethod.InsertSelect(sql));
        id = job["ProductID"].ToString();
        qr = qrcm.GetQRCode(id);
        sql = "update Product set QRCode = '" + qr + "' where ProductID = '" + id + "'";
        return sqlMethod.Update(sql);
    }

    public string ResetPassword(string identify, string oldPassword, string newPassword) // By Kevin Yen
    {
        sql = "select MemberID,Password from Member where Identify = '" + identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.SelectSingle(sql, "MemberID;Password"));
        string id = job["MemberID"].ToString();
        if (job["Password"].ToString().Equals(oldPassword))
        {
            sql = "update Member set Password = '" + newPassword + "' where MemberID = '" + id + "'";
            return sqlMethod.Update(sql);
        }
        else
            return gm.getStageJson(false, msg.passwordError_cht);
    }

    public string GetIntroduction() //Huan-Chieh Chen
    {
        sql = "select * from Introduction";
        return sqlMethod.Select(sql);
    }

    public string GetMemberInfo(string identify) //Huan-Chieh Chen
    {
        sql = "select Account,FirstName,LastName,Phone,Email,CompanyName,Address,Access from Member where Identify = '" + identify + "'";
        return sqlMethod.SelectSingle(sql, "Account;FirstName;LastName;Phone;Email;CompanyName;Address;Access");
    }

    public string NewMember(string account, string password, string firstName, string lastName, string phone, string email,
                            string companyName, string address, string access) // By Wei-Min Zhang
    {
        sql = "insert into Member(Account, Password, FirstName, LastName, Phone, Email, CompanyName, Address, Access) " +
                  "values('" + account + "','" + password + "','" + firstName + "','" + lastName + "','" + phone + "','" +
                  email + "','" + companyName + "','" + address + "','" + access + "')";
        return sqlMethod.Insert(sql);
    }

    public string NewRecord(string identify, string productID, string type, string action, string note) // By Kevin Yen
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

    public string UpdateMemberInfo(string column, string value) //Huan-Chieh Chen
    {
        int IdentifyNumber = -1;
        string[] strSplit1 = column.Split(';');
        string[] strSplit2 = value.Split(';');
        if (strSplit1.Length > 1)
        {
            sql = "update Member set ";
            for (int i = 0; i < strSplit1.Length; i++)
            {
                if (strSplit1[i].Equals("Identify"))
                {
                    IdentifyNumber = i;
                    continue;
                }
                if (i != 0)
                {
                    sql += ", ";
                }
                sql += strSplit1[i] + " = '" + strSplit2[i] + "' ";
            }
            if (IdentifyNumber > -1)
            {
                sql += "where Identify = '" + strSplit2[IdentifyNumber] + "'";
                return sqlMethod.Update(sql);
            }
            else
            {
                return gm.getStageJson(false, msg.identifyError_cht);
            }
        }
        else
        {
            return gm.getStageJson(false, msg.columnError_cht);
        }
    }

    public string NewProductOrder(string identify, string productID, string amount, string delivery, string shipment, string note) // By Wei-Min Zhang
    {
        string account = "";
        string name = "";
        string phone = "";
        string address = "";
        string productName = "";
        string price = "";
        int totalPrice = 0;
        bool Bm = false;
        bool Bp = false;
        sql = "select Account , (LastName + ' ' + FirstName) As Name, Phone , Address from Member where Identify = '" + identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.SelectSingle(sql, "Account;Name;Phone;Address"));
        account = job["Account"].ToString();
        name = job["Name"].ToString();
        phone = job["Phone"].ToString();
        address = job["Address"].ToString();
        if (!account.Equals("") && !name.Equals("") && !phone.Equals("") && !address.Equals(""))
            Bm = true;
        sql = "select ProductName , Price  from Product where ProductID =" + productID + "'";
        job = gm.getJsonResult(sqlMethod.SelectSingle(sql, "ProductName;Price"));
        productName = job["ProductName"].ToString();
        price = job["Price"].ToString();
        if (!productName.Equals("") && !price.Equals(""))
            Bp = true;

        totalPrice = int.Parse(price) + int.Parse(shipment);

        if (Bm && Bp)
        {
            sql = "Insert into ProductOrder (MemberAccount, MemberName, MemberPhone, MemberAddress, ProductName,Amount," +
                " Price, Delivery, Shipment, TotalPrice, Note)" + " values ('" + account + "','" + name + "','" + phone + 
                "','" + address + "','" + productName + "','" + amount + "','" + price + "','" + delivery + "','" + 
                shipment + "','" + totalPrice.ToString() + "','" + note + "') ";
            return sqlMethod.Insert(sql);
        }
        else
        {
            if (!Bm)
                return gm.getStageJson(false, msg.memberInfoError_cht);
            else
                return gm.getStageJson(false, msg.productInfoError_cht);

        }
    }

    public string GetMember(string access) // By Kevin Yen
    {
        if (access.Equals("ALL"))
        {
            sql = "select MemberID, Account, FirstName, LastName, Phone, Email, CompanyName, Address, Access from Member";
        }
        else
        {
            sql = "select MemberID, Account, FirstName, LastName, Phone, Email, CompanyName, Address, Access from Member where Access = '" + access + "'";
        }
        return sqlMethod.Select(sql);
    }

    public string GetProduct(string productID)
    {
        if (productID.Equals("ALL"))
        {
            sql = "select * from Product";
            return sqlMethod.Select(sql);
        }
        else
        {
            sql = "select * from Product where ProductID = '" + productID + "'";
            return sqlMethod.SelectSingle(sql, "CompanyID;CompanyName;ProductName;Type;Introduction;AdditionalValue;Origin;" +
                "Image;PackagingDate;Verification;ValidityPeriod;ValidityNumber;Price");
        }
    }

    public string GetProductColumn(string column, string value) //Huan-Chieh Chen
    {
        sql = "select * from Product where "+ column + " = '" + value + "'";
        return sqlMethod.Select(sql);
    }

    public string GetRecord(string productID) //Huan-Chieh Chen
    {
        sql = "select * from Record where ProductID = '" + productID + "'";
        return sqlMethod.Select(sql); 
    }

    public string GetSignLog(string identify) // By Kevin Yen
    {
        string memberID = "";
        bool isGetID = false;
        sql = "select MemberID from SignLog where Identify = '" + identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.SelectSingle(sql, "MemberID"));
        memberID = job["MemberID"].ToString();
        if (!memberID.Equals(""))
            isGetID = true;

        if (isGetID)
        {
            sql = "select * from SignLog where MemberID = '" + memberID + "'";
            return sqlMethod.Select(sql);
        }
        else
            return gm.getStageJson(false, msg.memberInfoError_cht);

    }

    public string NewProductImage(string productID, string imageType, string image) // By Kevin Yen
    {
        string imageUrl = gm.uploadImage(image);
        string type = "";

        switch (imageUrl)
        {
            case "M":
                type = "Main";
                break;
            case "I":
                type = "Introduction";
                break;
            case "G":
                type = "General";
                break;
        }

        if (!image.Equals("") && !type.Equals(""))
        {
            sql = "insert into ProductImage(ProductID, Type, ImageUrl) values('" + productID + "','" + type + "','"+ imageUrl +"')";
            return sqlMethod.Insert(sql);
        }
        else
            return gm.getStageJson(false, msg.uploadFail_cht);
    }
}


