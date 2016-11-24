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

    public string test()
    {
        sql = "select * from SignLog where SignInTime <'2016-11-24 23:50:54.5054'";
        return sqlMethod.Select(sql);
    }

    // 方法名稱直白，單字第一個字需大寫
    // 變數第一個單字須小寫

   
    // Sign
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
        sql = "select MemberID,Password,Access from Member where Email = '" + account + "'";
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            string message = job["message"].ToString();
            job = gm.getJsonObjectResult(message);
            if (job["Password"].ToString().Equals(password))
            {
                isSign = true;
                identify = gm.getUUID();
                memberID = job["MemberID"].ToString();
                access = job["Access"].ToString();
                ip = gm.getIpAddress();
                reMsg = msg.success;
            }
            else
                reMsg = msg.signError_cht;// 帳號密碼錯誤
        }
        else
            reMsg = msg.signError_cht; // 帳號密碼錯誤

        if (isSign)
        {
            // Update Member table's identify
            sql = "update Member set Identify = '" + identify + "' where (MemberID = " + memberID + ")";
            job = JObject.Parse(sqlMethod.Update(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
                isIdentify = true;

            // insert SignLog table
            sql = "insert into SignLog(MemberID,Account,SignInTime,Identify,IP) values('" + memberID + "','"
                + account + "','" + gm.getCurrentDate() + "','" + identify + "','" + ip + "')";
            job = gm.getJsonResult(sqlMethod.Insert(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
                isLog = true;
        }
        if (isSign && isIdentify && isLog)
        {
            jsonStr = gm.getStageJson(true, gm.getJsonArray("Identify;Access", identify + ";" + access));
            return jsonStr;
        }
        else
        {
            if (!isLog)
                reMsg = msg.signError_cht; // 帳號密碼錯誤
            jsonStr = gm.getStageJson(false, reMsg);
            return jsonStr;
        }
    }

    public string SignOut(string identify) // By Kevin Yen
    {
        sql = "update Member set Identify = '' where Identify = '" + identify + "'";
        JObject jObject = gm.getJsonResult(sqlMethod.Update(sql));
        if (jObject["stage"].ToString().Equals(true.ToString()))
        {
            sql = "update SignLog set SignOutTime = '" + gm.getCurrentDate() + "' where Identify = '" + identify + "'";
            return sqlMethod.Update(sql);
        }
        else
            return gm.getStageJson(false, msg.fail);
    }

    public string GetSignLog(string identify) // By Kevin Yen
    {
        string memberID = "";
        bool isGetID = false;
        sql = "select MemberID from SignLog where Identify = '" + identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            string message = job["message"].ToString();
            job = gm.getJsonObjectResult(message);
            memberID = job["MemberID"].ToString();
            if (!memberID.Equals(""))
                isGetID = true;
        }

        if (isGetID)
        {
            sql = "select * from SignLog where MemberID = '" + memberID + "'";
            return sqlMethod.Select(sql);
        }
        else
            return gm.getStageJson(false, msg.noData_cht); // 無此資料

    }


    // Member
    public string NewMember(string email, string password, string firstName, string lastName, string phone, string address) // By Wei-Min Zhang
    {
        sql = "select Email from Member where Email = '" + email + "'";
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(false.ToString()))
        {
            sql = "insert into Member(Email, Password, FirstName, LastName, Phone, Address, Access) " +
                  "values('" + email + "','" + password + "','" + firstName + "','" + lastName + "','" + phone + "','" +
                  address + "','" + gm.getMemberAccess("C") + "')";
            return sqlMethod.Insert(sql);
        }
        else
            return gm.getStageJson(false, msg.emailRepeat_cht);
    }

    public string NewHeadShot(string identify, string fileName, string image) // By Kevin Yen
    {
        string imageUrl = gm.upload(image, fileName, "Member");
        if (!imageUrl.Equals(""))
        {
            sql = "update Farm set Mugshot = '" + imageUrl + "' where Identify = '" + identify + "'";
            return sqlMethod.Update(sql);
        }
        else
            return gm.getStageJson(false, msg.uploadFail_cht); // 上傳失敗
    }

    public string NewVideo(string identify, string name, string fileName, string video) // By Kevin Yen
    {
        string memberID = "";
        string videoUrl = "";
        sql = "select MemberID from Member where Identify ='" + identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            string message = job["message"].ToString();
            job = gm.getJsonObjectResult(message);
            memberID = job["MemberID"].ToString();
            videoUrl = gm.upload(video, fileName, "Video");
        }

        if (!memberID.Equals("") && !videoUrl.Equals(""))
        {
            sql = "insert into Video(MemberID, Name, Url) values('" + memberID + "','" + name + "','" + video + "')";
            return sqlMethod.Insert(sql);
        }
        else
            return gm.getStageJson(false, msg.uploadFail_cht); // 上傳失敗
    }

    public string GetMemberInfo(string identify) //Huan-Chieh Chen
    {
        sql = "select Email,FirstName,LastName,Phone,Address,Access from Member where Identify = '" + identify + "'";
        return sqlMethod.Select(sql);
    }

    public string GetMember(string access) // By Kevin Yen
    {
        if (access.Equals("ALL"))
        {
            sql = "select MemberID,Email,FirstName,LastName,Phone,Address,Access from Member";
        }
        else
        {
            sql = "select MemberID,Email,FirstName,LastName,Phone,Address,Access from Member where Access = '" + gm.getMemberAccess(access) + "'";
        }
        return sqlMethod.Select(sql);
    }

    public string GetVideo(string memberID) // By Kevin Yen
    {
        sql = "select Name,Url from Video where MemberID = '" + memberID + "'";
        return sqlMethod.Select(sql);
    }

    public string UpdateMemberInfo(string identify, string column, string value) //Huan-Chieh Chen
    {
        string[] columnArray = column.Split(';');
        string[] valueArray = value.Split(';');
        if (columnArray.Length > 0 && valueArray.Length > 0 && columnArray.Length == valueArray.Length)
        {
            if (!identify.Equals(""))
            {
                sql = "update Member set ";
                for (int i = 0; i < columnArray.Length; i++)
                {
                    sql += columnArray[i] + " = '" + valueArray[i] + "'";
                    if (i < columnArray.Length - 1)
                    {
                        sql += " , ";
                    }
                }
                sql += " where Identify = '" + identify + "'";
                return sqlMethod.Update(sql);
            }
            else
            {
                return gm.getStageJson(false, msg.dataError_cht);
            }
        }
        else
        {
            return gm.getStageJson(false, msg.dataError_cht);
        }
    }

    public string ResetPassword(string identify, string oldPassword, string newPassword) // By Kevin Yen
    {
        sql = "select MemberID,Password from Member where Identify = '" + identify + "'";
        JObject job = gm.getJsonObjectResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            string message = job["message"].ToString();
            job = gm.getJsonResult(message);
            string id = job["MemberID"].ToString();
            if (job["Password"].ToString().Equals(oldPassword))
            {
                sql = "update Member set Password = '" + newPassword + "' where MemberID = '" + id + "'";
                return sqlMethod.Update(sql);
            }
            else
                return gm.getStageJson(false, msg.passwordError_cht);
        }
        else
            return gm.getStageJson(false, msg.passwordError_cht);
    }

    public string IsEmail(string email) // By Kevin Yen
    {
        bool isemail = Regex.IsMatch(email, @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
              @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        if (isemail)
            return gm.getStageJson(true, msg.success);
        else
            return gm.getStageJson(false, msg.emailError_cht);
    }


    // Product
    public string NewProduct(string identify, string farmID, string productName, string typeBig, string typeSmall, 
                            string introduction, string additionalValue, string origin, string price, string amount, 
                            string packagingDate, string validityPeriod, string verificationID, string stage) // By Kevin Yen
    {
        string id = "null";
        string qr = "null";
        string farmName = "";
        string productStage = gm.getProductStage(stage);
        QRCodeMethod qrcm = new QRCodeMethod();
        sql = "select FarmName from Farm where FarmID = '" + farmID + "'";
        JObject jObject = gm.getJsonResult(sqlMethod.Select(sql));
        if (jObject["stage"].ToString().Equals(true.ToString()))
        {
            string message = jObject["message"].ToString();
            jObject = gm.getJsonObjectResult(message);
            farmName = jObject["FarmName"].ToString();
            sql = "insert into Product(FarmID,FarmName,ProductName,TypeBig,TypeSmall,Introduction,AdditionalValue,Origin," +
                    "Price,Amount,PackagingDate,ValidityPeriod,ValidityID,Stage,OrderAmount) values " +
                    "('" + farmID + "','" + farmName + "','" + productName + "','" + typeBig + "','" + typeSmall + "','" + introduction +
                    "','" + additionalValue + "','" + origin + "','" + price + "','" + amount + "','" + packagingDate + "','" +
                    validityPeriod + "','" + verificationID + "','" + productStage + "',0);SELECT SCOPE_IDENTITY()";
            jObject = gm.getJsonResult(sqlMethod.Select(sql));
            if (jObject["stage"].ToString().Equals(true.ToString()))
            {
                message = jObject["message"].ToString();
                jObject = gm.getJsonObjectResult(message);
                id = jObject["ProductID"].ToString();
                qr = qrcm.GetQRCode(id);
                sql = "update Product set QRCode = '" + qr + "' where ProductID = '" + id + "'";
                return sqlMethod.Update(sql);
            }
            else
                return gm.getStageJson(false, msg.dataError_cht);
        }
        else
            return gm.getStageJson(false, msg.dataError_cht);
        
    }

    public string NewProductImage(string productID, string imageType, string fileName, string image) // By Kevin Yen
    {
        string imageUrl = gm.upload(image, fileName, "Product");
        string type = gm.getImageType(imageType);

        if (!image.Equals("") && !type.Equals(""))
        {
            sql = "insert into ProductImage(ProductID, Type, ImageUrl) values('" + productID + "','" + type + "','" + imageUrl + "')";
            return sqlMethod.Insert(sql);
        }
        else
            return gm.getStageJson(false, msg.uploadFail_cht);
    }

    public string NewProductFile(string productID, string fileName, string file) // By Kevin Yen
    {
        string fileUrl = gm.upload(file, fileName, "File");
        string[] name = fileName.Split('.');
        fileName = name[0];

        if (!fileName.Equals("") && !fileUrl.Equals(""))
        {
            sql = "insert into ProductFile (ProductID,FileName,FileUrl) values ('" + productID + "','" + fileName + "','" + fileUrl + "')";
            return sqlMethod.Insert(sql);
        }
        else
            return gm.getStageJson(false, msg.uploadFail_cht);
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
            return sqlMethod.Select(sql);
        }
    }

    public string GetProductKey(string bigItem, string smallItem, string value) // By Kevin Yen
    {
        string[] id = null;
        string[] name = null;
        string[] price = null;
        JArray jarray = null;
        JObject job = null;
        string json = gm.getStageJson(false, msg.noData_cht);
        bool isProduct = false;
        bool isValue = false;

        // 取得商品表單內資料
        if (bigItem.Equals("") && smallItem.Equals("") && value.Equals(""))
            sql = "select ProductID,ProductName,Price from Product";
        else if (bigItem.Equals("") && smallItem.Equals(""))
        {
            sql = "select ProductID,ProductName,Price from Product where (";
            isValue = true;
        }
        else if (smallItem.Equals("") && value.Equals(""))
            sql = "select ProductID,ProductName,Price from Product where TypeBig = '" + bigItem + "'";
        else if (smallItem.Equals(""))
        {
            sql = "select ProductID,ProductName,Price from Product where TypeBig = '" + bigItem + "' AND (";
            isValue = true;
        }
        else if (value.Equals(""))
            sql = "select ProductID,ProductName,Price from Product where TypeBig = '" + bigItem + "' AND TypeSmall = '" + smallItem + "'";
        else
        {
            sql = "select ProductID,ProductName,Price from Product where TypeBig = '" + bigItem + "' AND TypeSmall = '" + smallItem + "' AND (";
            isValue = true;
        }

        if (isValue)
        {
            for (int i = 0; i < value.Length; i++)
            {
                sql += "ProductName like '%" + value[i].ToString() + "%'";
                if (i < value.Length - 1)
                    sql += " OR ";
            }
            sql += ")";
        }

        // 暫存商品表單，ProductID與ProductName
        job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            string message = job["message"].ToString();
            jarray = gm.getJsonArrayResult(message);
            id = new string[jarray.Count];
            name = new string[jarray.Count];
            price = new string[jarray.Count];
            for (int i = 0; i < jarray.Count; i++)
            {
                job = gm.getJsonResult(jarray[i].ToString());
                id[i] = job["ProductID"].ToString();
                name[i] = job["ProductName"].ToString();
                price[i] = job["Price"].ToString();
            }
            jarray = null;
            isProduct = true;
        }
        

        // 取得商品圖片表單內資料
        if (isProduct)
        {
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
            job = gm.getJsonResult(sqlMethod.Select(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
            {
                string message = job["message"].ToString();
                jarray = gm.getJsonArrayResult(message);
                string rejson = "[";
                for (int i = 0; i < id.Length; i++)
                {
                    job = gm.getJsonResult(jarray[i].ToString());
                    rejson += gm.getJsonArray("ProductID;ProductName;Price;Image", id[i] + ";" + name[i] + ";" + price[i] + ";" + job["ImageUrl"].ToString());
                    if (i < id.Length - 1)
                        rejson += ",";
                    else
                        rejson += "]";
                }
                json = gm.getStageJson(true, rejson);
            }
        }

        return json;
    }

    public string GetProductColumn(string column, string value) //Huan-Chieh Chen
    {
        sql = "select * from Product where " + column + " = '" + value + "'";
        return sqlMethod.Select(sql);
    }

    public string GetTopHotProduct() // By Kevin Yen
    {
        int item = 7;
        sql = "select ProductID,ImageUrl from ProductImage where Type = 'Main' AND ProductID in (select top(" + item.ToString() + ") ProductID from Product order by OrderAmount desc)";
        return sqlMethod.Select(sql);
    }

    public string GetProductType() // By Kevin Yen
    {
        string jsonStr = gm.getStageJson(false, msg.noData_cht);
        string[] bigItem = null;
        string[] smallItem = null;
        bool isBigItem = false;
        bool isSmallItem = false;
        // 取得BigItem
        sql = "select distinct BigItem from Introduction";
        JObject jObject = gm.getJsonResult(sqlMethod.Select(sql));
        if (jObject["stage"].ToString().Equals(true.ToString()))
        {
            string message = jObject["message"].ToString();
            JArray jArray = gm.getJsonArrayResult(message);
            bigItem = new string[jArray.Count];
            for (int i = 0; i < jArray.Count; i++)
            {
                jObject = (JObject)jArray[i];
                bigItem[i] = jObject["BigItem"].ToString();
            }
            isBigItem = true;
        }
        

        // 取得SmallItem
        if (isBigItem)
        {
            smallItem = new string[bigItem.Length];
            for (int i = 0; i < bigItem.Length; i++)
            {
                sql = "select distinct SmallItem from Introduction where BigItem ='" + bigItem[i] + "'";
                jObject = gm.getJsonResult(sqlMethod.Select(sql));
                if (jObject["stage"].ToString().Equals(true.ToString()))
                {
                    string message = jObject["message"].ToString();
                    jObject = gm.getJsonObjectResult(message);
                    smallItem[i] = sqlMethod.Select(sql);
                }
            }
            isSmallItem = true;
        }

        // 合併Json
        if (isSmallItem)
        {
            string reJsonStr = "[";
            for (int i = 0; i < bigItem.Length; i++)
            {
                reJsonStr += gm.getJsonItemArray("BigItem;SmallItem", @"""" + bigItem[i] + @"""" + ";" + smallItem[i]);
                if (i < bigItem.Length - 1)
                    reJsonStr += ",";
                else
                    reJsonStr += "]";
            }
            jsonStr = gm.getStageJson(true, reJsonStr);
        }
        return jsonStr;
    }


    // Record
    public string NewRecord(string identify, string productID, string type, string action, string note) // By Kevin Yen
    {
        string memberID = "";
        string creator = "";
        string productName = "";
        bool isMember = false;
        bool isProductName = false;

        // Get memberID and creator
        sql = "select MemberID, FirstName, LastName from Member where Identify = '" + identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(false.ToString()))
        {
            string message = job["message"].ToString();
            job = gm.getJsonObjectResult(message);
            memberID = job["MemberID"].ToString();
            creator = job["LastName"].ToString() + job["FirstName"].ToString();
            if (!memberID.Equals("") && !creator.Equals(""))
                isMember = true;
        }


        // Get ProductName
        sql = "select ProductName from Product where ProductID = '" + productID + "'";
        job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(false.ToString()))
        {
            string message = job["message"].ToString();
            job = gm.getJsonObjectResult(message);
            productName = job["ProductName"].ToString();
            if (!productName.Equals(""))
                isProductName = true;
        }
        

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
            return gm.getStageJson(false, msg.dataError_cht);
        }
    }

    public string GetRecord(string productID) //Huan-Chieh Chen
    {
        sql = "select * from Record where ProductID = '" + productID + "'";
        return sqlMethod.Select(sql);
    }


    // Order
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
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(false.ToString()))
        {
            string message = job["message"].ToString();
            job = gm.getJsonObjectResult(message);
            account = job["Account"].ToString();
            name = job["Name"].ToString();
            phone = job["Phone"].ToString();
            address = job["Address"].ToString();
            if (!account.Equals("") && !name.Equals("") && !phone.Equals("") && !address.Equals(""))
                memberInfo = true;
        }
        
        sql = "select ProductName,Amount,Price,OrderAmount  from Product where ProductID =" + productID;
        job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(false.ToString()))
        {
            string message = job["message"].ToString();
            job = gm.getJsonObjectResult(message);
            productName = job["ProductName"].ToString();
            productAmount = int.Parse(job["Amount"].ToString());
            orderAmount = int.Parse(job["OrderAmount"].ToString());
            price = job["Price"].ToString();
            if (!productName.Equals("") && !price.Equals("") && !productAmount.Equals(""))
                productInfo = true;
        }

        if (productInfo)
        {
            if (productAmount >= int.Parse(amount) && productAmount > 0)
                amountCheck = true;
            totalPrice = (int.Parse(price) * int.Parse(amount)) + int.Parse(shipment);
        }

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
            return gm.getStageJson(false, msg.dataError_cht);
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


    // Cooperation
    public string GetCooperation() // By Kevin Yen
    {
        sql = "select WebsiteUrl,ImageUrl from Cooperation";
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


    // Introduction
    public string GetIntroduction() //Huan-Chieh Chen
    {
        sql = "select * from Introduction";
        return sqlMethod.Select(sql);
    }
}


