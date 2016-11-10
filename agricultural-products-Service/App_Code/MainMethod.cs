using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

/// <summary>
/// Summary description for MainMethod
/// </summary>
public class MainMethod
{

    private string strdbcon = "server=140.127.22.4;database=AgriculturalProducts;uid=CCBDA;pwd=CCBDA";
    private SqlConnection objcon;
    private SqlCommand sqlcmd;
    private string sql;
    private SQLMethod sqlMethod = new SQLMethod();
    private GetMethod gm = new GetMethod();

    // 方法名稱直白，單字第一個字需大寫

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

    public string NewProduct(string CompanyID, string CompanyName, string ProductName, string Type, string Introduction,
                                string AdditionalValue, string Origin, string Image, string PackagingDate, string Verification,
                                string ValidityPeriod, string ValidityNumber, string Price)
    {
        string id = "null";
        string qr = "null";
        QRCodeMethod qrcm = new QRCodeMethod();

        sql = "insert into Product(CompanyID,CompanyName,ProductName,Type,Introduction,AdditionalValue,Origin," +
                "Image,PackagingDate,Verification,ValidityPeriod,ValidityNumber,Price) values ('" + CompanyID + "','" +
                CompanyName + "','" + ProductName + "','" + Type + "','" + Introduction + "','" + AdditionalValue + "','" +
                Origin + "','" + Image + "','" + PackagingDate + "','" + Verification + "','" + ValidityPeriod + "','" + 
                ValidityNumber + "'," + Price + ");SELECT SCOPE_IDENTITY()";
        JObject job = gm.getJsonResult(sqlMethod.InsertSelect(sql));
        id = job["ProductID"].ToString();
        qr = qrcm.GetQRCode(id);
        sql = "update Product set QRCode = '" + qr + "' where ProductID = '" + id + "'";
        return sqlMethod.Update(sql);
    }

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

    public string GetIntroduction()
    {
        sql = "select * from Introduction";
        return sqlMethod.Select(sql);
    }

    public string GetMemberInfo(string identify)
    {
        sql = "select Account,FirstName,LastName,Phone,Email,CompanyName,Address,Access from Member where Identify =" + identify;
        return sqlMethod.SelectSingle(sql, "Account;FirstName;LastName;Phone;Email;CompanyName;Address;Access");
    }

    public string NewMember(string Account, string Password, string FirstName, string LastName, string Phone, string Email,
                            string CompanyName, string Address, string Access)
    {
        sql = "insert into Member(Account, Password, FirstName, LastName, Phone, Email, CompanyName, Address, Access) " +
                  "values('" + Account + "','" + Password + "','" + FirstName + "','" + LastName + "','" + Phone + "','" +
                  Email + "','" + CompanyName + "','" + Address + "','" + Access + "')";
        return sqlMethod.Insert(sql);
    }

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

    public string UpdateMemberInfo(string column, string value)
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
                return gm.getStageJson(false, "Identify is not exist");
            }
        }
        else
        {
            return gm.getStageJson(false, "column is not enough");
        }
    }

    public string NewProductOrder(string Identify, string ProductID, string Amount, string Delivery, string Shipment, string Note)
    {
        string Account = "";
        string Name = "";
        string Phone = "";
        string Address = "";
        string ProductName = "";
        string Price = "";
        bool Bm = false;
        bool Bp = false;
        sql = "select Account , (LastName + ' ' + FirstName) As Name, Phone , Address from Member where Identify = '" + Identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.SelectSingle(sql, "Account;Name;Phone;Address"));
        Account = job["Account"].ToString();
        Name = job["Name"].ToString();
        Phone = job["Phone"].ToString();
        Address = job["Address"].ToString();
        if (!Account.Equals("") && !Name.Equals("") && !Phone.Equals("") && !Address.Equals(""))
            Bm = true;
        sql = "select ProductName , Price  from Product where ProductID =" + ProductID;
        job = gm.getJsonResult(sqlMethod.SelectSingle(sql, "ProductName;Price"));
        ProductName = job["ProductName"].ToString();
        Price = job["Price"].ToString();
        if (!ProductName.Equals("") && !Price.Equals(""))
            Bp = true;
        if (Bm && Bp)
        {
            sql = "Insert into ProductOrder ( MemberAccount , MemberName , MemberPhone , MemberAddress , ProductName , Price) values ('" + Account + "','" + Name + "','" + Phone + "','" + Address + "','" + ProductName + "','" + Price + "') ";
            return sqlMethod.Insert(sql);
        }
        else
        {
            if (!Bm)
                return gm.getStageJson(false, "Member information is wrong.");
            else if (!Bp)
                return gm.getStageJson(false, "Product information is wrong.");
            else
                return gm.getStageJson(false, "All wrong.");

        }
    }

    public string GetMember(string Access)
    {
        if (Access.Equals("ALL"))
        {
            sql = "select MemberID, Account, FirstName, LastName, Phone, Email, CompanyName, Address, Access from Member";
        }
        else
        {
            sql = "select MemberID, Account, FirstName, LastName, Phone, Email, CompanyName, Address, Access from Member where Access = '" + Access + "'";
        }
        return sqlMethod.Select(sql);
    }
}