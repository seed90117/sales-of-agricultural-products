using Newtonsoft.Json.Linq;


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
        string access = "";
        string ip = "";
        string reMsg = "";
        string jsonStr = "";
        bool isSign = false;
        bool isIdentify = false;
        bool isLog = false;
        sql = "select MemberID,Email,Password from Member where Email = '" + account + "'";
        JObject job = gm.getJsonObjectResult(sqlMethod.Select(sql));
        if (job["Password"].ToString().Equals(password))
        {
            isSign = true;
            identify = gm.getUUID();
            memberID = job["MemberID"].ToString();
            access = job["Email"].ToString();
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
            jsonStr = gm.getJsonArray("stage;message;UUID;Access", isSign.ToString() + ";" + reMsg + ";" + identify + ";" + access);
            return jsonStr;
        }
        else
        {
            jsonStr = gm.getStageJson(false, reMsg);
            return jsonStr;
        }
    }

    public string NewProduct(string companyID, string companyName, string productName, string type, string introduction,
                                string additionalValue, string origin, string packagingDate, string verification,
                                string validityPeriod, string validityNumber, string amount, string price, string stage) // By Kevin Yen
    {
        string id = "null";
        string qr = "null";
        string productStage = "Prepare";
        QRCodeMethod qrcm = new QRCodeMethod();

        if (stage.Equals("S"))
            productStage = "Sales";
        if (stage.Equals("P"))
            productStage = "Prepare";
        if (stage.Equals("D"))
            productStage = "Drop";

        sql = "insert into Product(CompanyID,CompanyName,ProductName,Type,Introduction,AdditionalValue,Origin," +
                "PackagingDate,Verification,ValidityPeriod,ValidityNumber,Amount,Price,OrderAmount,Stage) values " +
                "('" + companyID + "','" + companyName + "','" + productName + "','" + type + "','" + introduction + 
                "','" + additionalValue + "','" + origin + "','" + packagingDate + "','" + verification + "','" + 
                validityPeriod + "','" + validityNumber + "'," + amount + "," + price + ",0,'" + productStage + ");SELECT SCOPE_IDENTITY()";

        JObject job = gm.getJsonResult(sqlMethod.InsertSelect(sql));
        id = job["ProductID"].ToString();
        qr = qrcm.GetQRCode(id);
        sql = "update Product set QRCode = '" + qr + "' where ProductID = '" + id + "'";
        return sqlMethod.Update(sql);
    }

    public string ResetPassword(string identify, string oldPassword, string newPassword) // By Kevin Yen
    {
        sql = "select MemberID,Password from Member where Identify = '" + identify + "'";
        JObject job = gm.getJsonObjectResult(sqlMethod.Select(sql));
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
        return gm.getJsonSingleResult(sqlMethod.Select(sql));
    }

    public string NewMember(string email, string password, string firstName, string lastName, string phone, 
        string fax, string cellPhone, string address) // By Wei-Min Zhang
    {
        sql = "select Email from Member where Email = '" + email + "'";
        if (sqlMethod.Select(sql).Equals(gm.getStageJson(false, msg.noData_cht)))
        {
            sql = "insert into Member(Email, Password, FirstName, LastName, Phone, Fax, CellPhone, Address, Access) " +
                  "values('" + email + "','" + password + "','" + firstName + "','" + lastName + "','" + phone + "','" +
                  fax + "','" + cellPhone + "','" + address + "','" + gm.getMemberAccess("C") + "')";
            return sqlMethod.Insert(sql);
        }
        else
            return gm.getStageJson(false, msg.emailRepeat);
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
        JObject job = gm.getJsonObjectResult(sqlMethod.Select(sql));
        memberID = job["MemberID"].ToString();
        creator = job["LastName"].ToString() + job["FirstName"].ToString();
        if (!memberID.Equals("") && !creator.Equals(""))
            isMember = true;

        // Get ProductName
        sql = "select ProductName from Product where ProductID = '" + productID + "'";
        job = gm.getJsonObjectResult(sqlMethod.Select(sql));
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
        string returnJson = "";
        string account = "";
        string name = "";
        string phone = "";
        string address = "";
        string productName = "";
        string price = "";
        int productAmount = -1;
        int orderAmount = -1;
        int totalPrice = 0;
        bool amountCheck = false;
        bool memberInfo = false;
        bool productInfo = false;
        bool checkOrder = false;

        sql = "select Account , (LastName + ' ' + FirstName) As Name, Phone , Address from Member where Identify = '" + identify +"'";
        JObject job = gm.getJsonObjectResult(sqlMethod.Select(sql));
        account = job["Account"].ToString();
        name = job["Name"].ToString();
        phone = job["Phone"].ToString();
        address = job["Address"].ToString();
        if (!account.Equals("") && !name.Equals("") && !phone.Equals("") && !address.Equals(""))
            memberInfo = true;

        sql = "select ProductName,Amount,Price,OrderAmount  from Product where ProductID =" + productID;
        job = gm.getJsonObjectResult(sqlMethod.Select(sql));
        productName = job["ProductName"].ToString();
        productAmount = int.Parse(job["Amount"].ToString());
        orderAmount = int.Parse(job["OrderAmount"].ToString());
        price = job["Price"].ToString();

        if (!productName.Equals("") && !price.Equals(""))
            productInfo = true;

        if (productAmount >= int.Parse(amount) && productAmount > 0)
            amountCheck = true;

        totalPrice = (int.Parse(price) * int.Parse(amount)) + int.Parse(shipment);

        if (memberInfo && productInfo && amountCheck)
        {
            sql = "Insert into ProductOrder (MemberAccount, MemberName, MemberPhone, MemberAddress, ProductName,Amount," +
                " Price, Delivery, Shipment, TotalPrice, Note) values ('" + account + "','" + name + "','" + phone + 
                "','" + address + "','" + productName + "','" + amount + "','" + price + "','" + delivery + "','" + 
                shipment + "','" + totalPrice.ToString() + "','" + note + "') ";
            returnJson = sqlMethod.Insert(sql);
            checkOrder = true;
        }
        else
        {
            if (!memberInfo)
                return gm.getStageJson(false, msg.memberInfoError_cht);
            if (!productInfo)
                return gm.getStageJson(false, msg.productInfoError_cht);
            if (!amountCheck)
                return gm.getStageJson(false, msg.amountError_cht);
        }

        if (checkOrder)
        {
            orderAmount++;
            productAmount -= int.Parse(amount);
            sql = "update Product set Amount = " + productAmount + ", OrderAmount = " + orderAmount + " where ProductID = " + productID;
            sqlMethod.Update(sql);
            return returnJson;
        }
        else
        {
            return gm.getStageJson(false, msg.productAmountError_cht);
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
            sql = "select MemberID, Account, FirstName, LastName, Phone, Email, CompanyName, Address, Access from Member where Access = '" + gm.getMemberAccess(access) + "'";
        }
        return sqlMethod.Select(sql);
    }

    public string GetProduct(string productID) // By Kevin Yen
    {
        if (productID.Equals("ALL"))
        {
            sql = "select * from Product";
            return sqlMethod.Select(sql);
        }
        else
        {
            sql = "select * from Product where ProductID = '" + productID + "'";
            return gm.getJsonSingleResult(sqlMethod.Select(sql));
        }
    }

    public string GetProductKey(string type, string value) // By Kevin Yen
    {
        // 取得商品表單內資料
        if (type.Equals("ALL"))
            sql = "select ProductID,ProductName from Product where ";
        else
            sql = "select ProductID,ProductName from Product where Tpye = '" + type + "' AND (";
        for (int i=0; i< value.Length; i++)
        {
            sql += "ProductName like '%" + value[i].ToString() + "%'";
            if (i < value.Length - 1)
                sql += " OR ";
        }
        if (!type.Equals("ALL"))
            sql += ")";

        // 暫存商品表單，ProductID與ProductName
        JArray jarray = gm.getJsonArrayResult(sqlMethod.Select(sql));
        string[] id = new string[jarray.Count];
        string[] name = new string[jarray.Count];
        for (int i = 0; i < jarray.Count; i++)
        {
            JObject job = gm.getJsonResult(jarray[i].ToString());
            id[i] = job["ProductID"].ToString();
            name[i] = job["ProductName"].ToString();
        }
        jarray = null;

        // 取得商品圖片表單內資料
        sql = "select ImageUrl from ProductImage where Type ='Main' AND (";
        for (int i = 0; i < id.Length; i++)
        {
            sql += "ProductID = '" + id[i] + "'";
            if (i < id.Length - 1)
                sql += " OR ";
            else
                sql += ")";
        }

        // 輸出JSON，欄位ProductID, ProductName, Image
        jarray = gm.getJsonArrayResult(sqlMethod.Select(sql));
        string json = "[";
        for (int i = 0; i < id.Length; i++)
        {
            JObject job = gm.getJsonResult(jarray[i].ToString());
            json += gm.getJsonArray("ProductID;ProductName;Image", id[i] + ";" + name[i] + ";" + job["ImageUrl"].ToString());
            if (i < id.Length - 1)
                json += ",";
            else
                json += "]";
        }

        return json;
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
        JObject job = gm.getJsonObjectResult(sqlMethod.Select(sql));
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

    public string NewProductImage(string productID, string imageType,string fileName, string image) // By Kevin Yen
    {
        string imageUrl = gm.upload(image, fileName, "Product");
        string type = gm.getImageType(imageType);

        if (!image.Equals("") && !type.Equals(""))
        {
            sql = "insert into ProductImage(ProductID, Type, ImageUrl) values('" + productID + "','" + type + "','"+ imageUrl +"')";
            return sqlMethod.Insert(sql);
        }
        else
            return gm.getStageJson(false, msg.uploadFail_cht);
    }

    public string NewProductVideo(string identify, string name, string fileName, string video) // By Kevin Yen
    {
        sql = "select MemberID from Member where Identify ='" + identify + "'";
        JObject job = gm.getJsonObjectResult(sqlMethod.Select(sql));
        string memberID = job["MemberID"].ToString();
        string videoUrl = gm.upload(video, fileName, "Video");
        
        if (!memberID.Equals("") && !videoUrl.Equals(""))
        {
            sql = "insert into Video(MemberID, Name, Url) values('" + memberID + "','" + name + "','" + video + "')";
            return sqlMethod.Insert(sql);
        }
        else
            return gm.getStageJson(false, msg.uploadFail_cht);
    }

    public string NewHeadShot(string identify, string fileName, string image) // By Kevin Yen
    {
        string imageUrl = gm.upload(image, fileName, "Member");
        if (!imageUrl.Equals(""))
        {
            sql = "update Member set Image = '" + imageUrl + "' where Identify = '" + identify + "'";
            return sqlMethod.Update(sql);
        }
        else
            return gm.getStageJson(false, msg.uploadFail_cht);
    }

    public string GetTopHotProduct() // By Kevin Yen
    {
        int item = 8;
        string sqlStr = "";
        sql = "select top(" + item.ToString() + ") ProductID from Product order by OrderAmount desc";
        JArray jArray = gm.getJsonArrayResult(sqlMethod.Select(sql));

        for(int i = 0;i < jArray.Count; i++)
        {
            JObject jObject = gm.getJsonResult(jArray[i].ToString());
            sqlStr += "ProductID = " + jObject["ProductID"].ToString();
            if (i < jArray.Count - 1)
                sqlStr += " OR ";
        }

        sql = "select ProductID,ImageUrl from ProductImage where Type = 'Main' AND (" + sqlStr + ")";
        return sqlMethod.Select(sql);
    }

    public string GetCooperation() // By Kevin Yen
    {
        sql = "select CompanyUrl,ImageUrl from Cooperation";
        return sqlMethod.Select(sql);
    }

    public string GetVideo(string productID) // By Kevin Yen
    {
        sql = "select Name,Url from Video where ProductID = '" + productID + "'";
        return sqlMethod.Select(sql);
    }

    public string NewCooperationImage(string companyName, string companyUrl, string fileName, string image) // By Kevin Yen
    {
        string imageUrl = gm.upload(image, fileName, "Company");
        if (!image.Equals(""))
        {
            sql = "insert into Cooperation(CompanyName, CompanyUrl, ImageUrl) values('" + companyName + "','" + companyUrl + "','" + imageUrl + "')";
            return sqlMethod.Insert(sql);
        }
        else
            return gm.getStageJson(false, msg.uploadFail_cht);
    }
}


