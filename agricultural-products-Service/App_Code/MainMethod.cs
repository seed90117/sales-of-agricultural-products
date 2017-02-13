using Newtonsoft.Json.Linq;
using System.Net.Mail;

/// <summary>
/// Summary description for MainMethod
/// </summary>
public class MainMethod
{
    private string sql;
    private SQLMethod sqlMethod = new SQLMethod();
    private GetMethod gm = new GetMethod();
    private Message msg = new Message();

    public string test(string file)
    {
        //sql = "select TOP(8) ProductName from Product ORDER BY NEWID()";
        sql = "select Item,Introduction from Type where Item in ('瓜果類','大漿果','果菜類')";
        return sqlMethod.Select(sql);
        //sql = "insert into Test (ContentText,Date) values ('" + file + "','" + gm.getCurrentDate() + "')";
        //return sqlMethod.Insert(sql);

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
            job = gm.getJsonObjectResult(job["message"].ToString());
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
            job = gm.getJsonObjectResult(job["message"].ToString());
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
        if (gm.isEmail(email))
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
        else
            return gm.getStageJson(false, msg.emailError_cht);
    }

    public string NewHeadShot(string identify, string fileName, string image) // By Kevin Yen
    {
        sql = "select MemberID from Member where Identify = '" + identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            job = gm.getJsonObjectResult(job["message"].ToString());
            string memeberID = job["MemberID"].ToString();
            string imageUrl = gm.upload(image, fileName, "Member");
            if (!imageUrl.Equals(""))
            {
                sql = "update Farm set Mugshot = '" + imageUrl + "' where MemberID = '" + memeberID + "'";
                return sqlMethod.Update(sql);
            }
            else
                return gm.getStageJson(false, msg.uploadFail_cht); // 上傳失敗
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
            job = gm.getJsonObjectResult(job["message"].ToString());
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
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            job = gm.getJsonObjectResult(job["message"].ToString());
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
        if (gm.isEmail(email))
            return gm.getStageJson(true, msg.success);
        else
            return gm.getStageJson(false, msg.emailError_cht);
    }


    // Product
    public string NewProduct(string identify, string farmID, string productName, string typeBig, string typeSmall,
                            string introduction, string additionalValue, string origin, string price, string amount,
                            string packagingDate, string validityPeriod, string verificationID, string stage) // By Kevin Yen //資料庫設定變更20170206
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
            jObject = gm.getJsonObjectResult(jObject["message"].ToString());
            farmName = jObject["FarmName"].ToString();
            sql = "insert into Product(FarmID,FarmName,ProductName,TypeBig,TypeSmall,Introduction,AdditionalValue,Origin," +
                    "Price,Amount,PackagingDate,ValidityPeriod,VerificationID,Stage,OrderAmount) values " +
                    "('" + farmID + "','" + farmName + "','" + productName + "','" + typeBig + "','" + typeSmall + "','" + introduction +
                    "','" + additionalValue + "','" + origin + "','" + price + "','" + amount + "','" + packagingDate + "','" +
                    validityPeriod + "','" + verificationID + "','" + productStage + "',0);SELECT SCOPE_IDENTITY()";
            jObject = gm.getJsonResult(sqlMethod.Select(sql));
            if (jObject["stage"].ToString().Equals(true.ToString()))
            {
                jObject = gm.getJsonObjectResult(jObject["message"].ToString());
                id = jObject["Column1"].ToString();
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

    public string GetProduct(string productID) // By Kevin Yen        by chc 20170207
    {//OrderAmount, Stage 這兩個欄位是否使用到，再考慮
        sql = "select ProductName, Price, Amount, PackagingDate, CertificationClassification.CertificationClassification, Certification.CertificationUnit, Certification.CertificationUnit2, Verification.VerificationUnit,Verification.ImgUrl, QRCode, OrderAmount, Stage from Product"
            + " INNER JOIN CertificationClassification ON Product.CertificationClassificationID=CertificationClassification.CertificationClassificationID"
            + " INNER JOIN Certification ON Product.CertificationID=Certification.CertificationID"
            + " INNER JOIN Verification ON Product.VerificationID=Verification.VerificationID";
        if (!productID.Equals("ALL"))
        {
            sql += " where ProductID = '" + productID + "'";
        }
        return sqlMethod.Select(sql);
    }

    public string GetProductKey(string bigItem, string smallItem, string value, string p, string h) // By Kevin Yen
    {
        string[] id = null;
        string[] name = null;
        string[] price = null;
        string[] image = null;
        JArray jarray = null;
        JObject job = null;
        string json = gm.getStageJson(false, msg.noData_cht);
        bool isProduct = false;
        bool isValue = false;
        bool isImage = false;

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

        if (!p.Equals(""))
        {
            if (p.Equals("H"))
                sql += " order by Price desc";
            else if (p.Equals("L"))
                sql += " order by Price asc";
        }
        else if (!h.Equals(""))
        {
            if (h.Equals("H"))
                sql += " order by OrderAmount desc";
            else if (h.Equals("L"))
                sql += " order by OrderAmount asc";
        }
        else
            sql += " order by ProductID";

        // 暫存商品表單，ProductID與ProductName
        job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            jarray = gm.getJsonArrayResult(job["message"].ToString());
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
            sql = "select ProductID,ImageUrl from ProductImage where Type ='Main' AND (";
            for (int i = 0; i < id.Length; i++)
            {
                sql += "ProductID = '" + id[i] + "'";
                if (i < id.Length - 1)
                    sql += " OR ";
                else
                    sql += ")";
            }
            job = gm.getJsonResult(sqlMethod.Select(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
            {
                jarray = gm.getJsonArrayResult(job["message"].ToString());
                image = new string[jarray.Count];
                string[] productID = new string[jarray.Count];
                for (int i = 0; i < jarray.Count; i++)
                {
                    job = gm.getJsonResult(jarray[i].ToString());
                    image[i] = job["ImageUrl"].ToString();
                    productID[i] = job["ProductID"].ToString();
                }
                image = gm.selectOrder(id, productID, image);
                isImage = true;
            }



            // 輸出JSON，欄位ProductID, ProductName, Image
            if (isImage && isProduct)
            {
                string rejson = "[";
                for (int i = 0; i < id.Length; i++)
                {
                    rejson += gm.getJsonArray("ProductID;ProductName;Price;Image", id[i] + ";" + name[i] + ";" + price[i] + ";" + image[i]);
                    if (i < id.Length - 1)
                        rejson += ",";
                }
                rejson += "]";
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

    public string GetRandomProduct() // By Kevin Yen
    {
        int item = 8;
        string[] productID = null;
        string[] productName = null;
        string[] imageUrl = null;
        bool isProduct = false;
        bool isImage = false;
        sql = "select TOP(" + item + ") ProductID,ProductName from Product ORDER BY NEWID()";
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            JArray jarray = gm.getJsonArrayResult(job["message"].ToString());
            productID = new string[jarray.Count];
            productName = new string[jarray.Count];
            for (int i = 0; i < jarray.Count; i++)
            {
                job = gm.getJsonResult(jarray[i].ToString());
                productID[i] = job["ProductID"].ToString();
                productName[i] = job["ProductName"].ToString();
            }
            jarray = null;
            isProduct = true;
        }
        if (isProduct)
        {
            string[] id = null;
            sql = "select ProductID,ImageUrl from ProductImage where Type = 'Main' AND ProductID in (";
            for (int i = 0; i < productID.Length; i++)
            {
                sql += "'" + productID[i] + "'";
                if (i < productID.Length - 1)
                    sql += ",";
                else
                    sql += ")";
            }
            job = gm.getJsonResult(sqlMethod.Select(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
            {
                JArray jarray = gm.getJsonArrayResult(job["message"].ToString());
                imageUrl = new string[jarray.Count];
                id = new string[jarray.Count];
                for (int i = 0; i < jarray.Count; i++)
                {
                    job = gm.getJsonResult(jarray[i].ToString());
                    id[i] = job["ProductID"].ToString();
                    imageUrl[i] = job["ImageUrl"].ToString();
                }
                imageUrl = gm.selectOrder(productID, id, imageUrl);
                isImage = true;
            }
        }
        if (isImage && isProduct)
        {
            string reJson = "[";
            for (int i = 0; i < productID.Length; i++)
            {
                reJson += gm.getJsonArray("ProductID;ProductName;ImageUrl", productID[i] + ";" + productName[i] + ";" + imageUrl[i]);
                if (i < productID.Length - 1)
                    reJson += ",";
                else
                    reJson += "]";
            }
            return gm.getStageJson(true, reJson);
        }
        else
            return gm.getStageJson(false, msg.noData_cht);
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
            JArray jArray = gm.getJsonArrayResult(jObject["message"].ToString());
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
                    smallItem[i] = jObject["message"].ToString();
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

    public string GetProductTypeItem() // By Kevin Yen
    {
        string jsonStr = gm.getStageJson(false, msg.noData_cht);
        string[] bigItem = null;
        string[] smallItem = null;
        string[] smallItemValue = null;
        bool isBigItem = false;
        bool isSmallItem = false;

        // 取得BigItem，陣列大小4
        sql = "select distinct BigItem from Introduction";
        JObject jObject = gm.getJsonResult(sqlMethod.Select(sql));
        if (jObject["stage"].ToString().Equals(true.ToString()))
        {
            JArray jArray = gm.getJsonArrayResult(jObject["message"].ToString());
            bigItem = new string[jArray.Count];
            for (int i = 0; i < jArray.Count; i++)
            {
                jObject = (JObject)jArray[i];
                bigItem[i] = jObject["BigItem"].ToString();
            }
            isBigItem = true;
        }


        // 取得SmallItem，陣列大小4
        if (isBigItem)
        {
            smallItem = new string[bigItem.Length];
            for (int i = 0; i < bigItem.Length; i++)
            {
                sql = "select distinct SmallItem from Introduction where BigItem ='" + bigItem[i] + "'";
                jObject = gm.getJsonResult(sqlMethod.Select(sql));
                if (jObject["stage"].ToString().Equals(true.ToString()))
                {
                    smallItem[i] = jObject["message"].ToString();
                }
            }
            isSmallItem = true;
        }


        // 取得Introduction,Product並輸出Json
        if (isSmallItem)
        {
            string reJson = "[";
            smallItemValue = new string[bigItem.Length];
            for (int i = 0; i < smallItem.Length; i++) // BigItem size 4
            {
                JArray jArray = gm.getJsonArrayResult(smallItem[i]);
                smallItemValue[i] = "[";
                for (int j = 0; j < jArray.Count; j++)
                {
                    JObject job = gm.getJsonResult(jArray[j].ToString());
                    // 取得Introduction
                    string tmpSmallItem = job["SmallItem"].ToString(); ;
                    string tmpIntroduction = "";
                    string tmpProduct = "";
                    sql = "select Introduction from Type where Item = '" + tmpSmallItem + "'";

                    jObject = gm.getJsonResult(sqlMethod.Select(sql));
                    if (jObject["stage"].ToString().Equals(true.ToString()))
                    {
                        jObject = gm.getJsonObjectResult(jObject["message"].ToString());
                        tmpIntroduction = jObject["Introduction"].ToString();
                    }

                    // 取得ProductID
                    string tmpProductArray = "(";
                    sql = "select TOP(3) ProductID from Product where TypeSmall = '" + tmpSmallItem + "' order by OrderAmount desc";
                    jObject = gm.getJsonResult(sqlMethod.Select(sql));
                    if (jObject["stage"].ToString().Equals(true.ToString()))
                    {
                        JArray jAr = gm.getJsonArrayResult(jObject["message"].ToString());
                        for (int k = 0; k < jAr.Count; k++)
                        {
                            jObject = (JObject)jAr[k];
                            tmpProductArray += jObject["ProductID"].ToString();
                            if (k < jAr.Count - 1)
                                tmpProductArray += ",";
                            else
                                tmpProductArray += ")";
                        }
                    }

                    // 取得ProductID, ImageUrl
                    if (!tmpProductArray.Equals("("))
                    {
                        if (j > 0)
                            smallItemValue[i] += ",";
                        sql = "select ProductID, ImageUrl from ProductImage where Type = 'Main' AND ProductID in" + tmpProductArray;
                        jObject = gm.getJsonResult(sqlMethod.Select(sql));
                        if (jObject["stage"].ToString().Equals(true.ToString()))
                        {
                            tmpProduct = jObject["message"].ToString();
                        }


                        // 放入BigItem Json
                        smallItemValue[i] += gm.getJsonItemArray("SmallItem;Introduction;Product", @"""" + tmpSmallItem + @"""" +
                                                                  ";" + @"""" + tmpIntroduction + @"""" + ";" + tmpProduct);
                    }
                }
                smallItemValue[i] += "]";
                reJson += gm.getJsonItemArray("BigItem;SmallItem", @"""" + bigItem[i] + @"""" + ";" + smallItemValue[i]);
                if (i < smallItem.Length - 1)
                    reJson += ",";
                else
                    reJson += "]";
            }
            return gm.getStageJson(true, reJson);
        }
        else
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
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            job = gm.getJsonObjectResult(job["message"].ToString());
            memberID = job["MemberID"].ToString();
            creator = job["LastName"].ToString() + job["FirstName"].ToString();
            if (!memberID.Equals("") && !creator.Equals(""))
                isMember = true;
        }


        // Get ProductName
        sql = "select ProductName from Product where ProductID = '" + productID + "'";
        job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            job = gm.getJsonObjectResult(job["message"].ToString());
            productName = job["ProductName"].ToString();
            if (!productName.Equals(""))
                isProductName = true;
        }


        // Insert record data//資料庫設定變更20170206
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

    public string GetRecord(string id) //Huan-Chieh Chen
    {
        sql = "select TOP (1) RecordID, Creator, Origin, Package, CultivationID, YearNumber, CultivationRegion, CultivationArea from Record where ProductID = '" + id + "' order by RecordID desc";
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

        sql = "select Email , (LastName + ' ' + FirstName) As Name, Phone , Address from Member where Identify = '" + identify + "'";
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            job = gm.getJsonObjectResult(job["message"].ToString());
            account = job["Email"].ToString();
            name = job["Name"].ToString();
            phone = job["Phone"].ToString();
            address = job["Address"].ToString();
            if (!account.Equals("") && !name.Equals("") && !phone.Equals("") && !address.Equals(""))
                memberInfo = true;
        }

        if (memberInfo)
        {
            sql = "select ProductName,Amount,Price,OrderAmount  from Product where ProductID =" + productID;
            job = gm.getJsonResult(sqlMethod.Select(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
            {
                job = gm.getJsonObjectResult(job["message"].ToString());
                productName = job["ProductName"].ToString();
                productAmount = int.Parse(job["Amount"].ToString());
                orderAmount = int.Parse(job["OrderAmount"].ToString());
                price = job["Price"].ToString();
                if (!productName.Equals("") && !price.Equals("") && !productAmount.Equals(""))
                    productInfo = true;
            }
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


    //E-mail
    public string EMailCheck(string MailTo)//Huan-Chieh Chen
    {
        string memberID = "", identify = "";
        bool NeedInsert = false;
        System.DateTime time = System.DateTime.Now;
        sql = "select MemberID from Member where Email = '" + MailTo + "'";
        JObject job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            job = gm.getJsonObjectResult(job["message"].ToString());
            memberID = job["MemberID"].ToString();
        }
        else
        {
            return gm.getStageJson(false, msg.EmailFail);
        }
        sql = "select TOP (1) Identify, Time from ForgetPassword where MemberID = " + memberID + " order by Time desc";
        job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            job = gm.getJsonObjectResult(job["message"].ToString());
            time = System.Convert.ToDateTime((job["Time"].ToString()));
            if (System.DateTime.Compare(System.DateTime.Now.AddHours(-1), time) > 0)//舊認證信超過1小時，寄出新的信
            {
                NeedInsert = true;
            }
            else
            {
                identify = job["Identify"].ToString();
            }
        }
        else
        {
            NeedInsert = true;
        }

        if (NeedInsert)
        {
            identify = gm.getUUID();
            sql = "insert into ForgetPassword(MemberID, Identify, Time) values('" + memberID + "','" + identify + "','" + gm.getCurrentDate() + "')";
            sqlMethod.Insert(sql);
        }

        string MailFrom = "NPUSTProducePlatform@NPUSTProducePlatform.com.tw";//此網站的寄信人地址
        string MailSub = "NPUSTProducePlatform忘記密碼認證信";//信件主旨
        bool isBodyHtml = true;
        //信件內容 在此處設定內容 由於isBodyHtml是ture 那這裡的語法將會是html)
        string MailBody = "忘記密碼確認信<br />" +
            "點選以下連結可以前往變更密碼<br />" +
            "http://140.127.22.4/NPUSTProducePlatform/view/哪個網頁" + "?MemberID=" + memberID + "<br />" +
            "認證碼： " + identify + "<br>" +
            "若最近沒有使用忘記密碼，請無視此信件";
        string smtpServer = "140.127.22.4";// 寄信smtp server
        int smtpPort = 25;// 寄信smtp server的Port，預設25
        try
        {
            MailMessage mms = new MailMessage();//建立MailMessage物件
            mms.From = new MailAddress(MailFrom);//指定一位寄信人MailAddress
            mms.Subject = MailSub;//信件主旨
            mms.Body = MailBody;//信件內容
            mms.IsBodyHtml = isBodyHtml;//信件內容 是否採用Html格式
            mms.To.Add(new MailAddress(MailTo.Trim()));
            using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))//smtp_server
            {
                client.Send(mms);//寄出一封信
            }//end using
            return gm.getStageJson(true, msg.EmailSuccess);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            return gm.getStageJson(false, msg.EmailFail);
        }
    }

    //E-mail
    public string EMailResetPassword(string Identify,string NewPassword)//Huan-Chieh Chen
    {
        if (!Identify.Equals("")&&!NewPassword.Equals(""))
        {
            System.DateTime time;
            sql = "select ForgetPassID,MemberID,Time from ForgetPassword where Identify = '" + Identify + "'";
            JObject job = gm.getJsonResult(sqlMethod.Select(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
            {
                job = gm.getJsonObjectResult(job["message"].ToString());
                time = System.Convert.ToDateTime((job["Time"].ToString()));
                if (System.DateTime.Compare(System.DateTime.Now.AddHours(-1), time) < 0)//信寄出還沒超過1小時，超過1小時就不給更改密碼
                {
                    string forgetpassid = job["ForgetPassID"].ToString();
                    string memberid = job["MemberID"].ToString();
                    sql = "update Member set Password = '" + NewPassword + "' where MemberID = " + memberid;
                    sqlMethod.Update(sql);
                    sql = "update ForgetPassword set Identify = '' where ForgetPassID = " + forgetpassid;
                    return sqlMethod.Update(sql);
                }else
                {
                    return gm.getStageJson(false, msg.EMailResetPasswordFail);
                }
            }
            else
                return gm.getStageJson(false, msg.dataError_cht);
        }
        else
        {
            return gm.getStageJson(false, msg.dataError_cht);
        }
    }

    //支付方式
    public string GetMemberPaymentMethod(string MemberID) //Huan-Chieh Chen
    {
        sql = "select RemittanceAccount,PickupAddress from Member where MemberID = " + MemberID + " and Access = '" + gm.getMemberAccess("E") + "'";
        return sqlMethod.Select(sql);
    }

    public string GetProductKey2(string bigItem, string smallItem, string ValiditySpecies, string value, string p, string h) // By Kevin Yen Huan-Chieh Chen
    {
        string[] id = null;
        string[] name = null;
        string[] price = null;
        string[] image = null;
        JArray jarray = null;
        JObject job = null;
        string json = gm.getStageJson(false, msg.noData_cht);
        bool isProduct = false;
        bool isImage = false;

        // 取得商品表單內資料
        sql = "select ProductID,ProductName,Price from Product";
        if (!bigItem.Equals("")|| !smallItem.Equals("") || !value.Equals("")|| !ValiditySpecies.Equals(""))
        {
            sql += " where";
            if(!bigItem.Equals(""))
                sql += " TypeBig = '" + bigItem + "'";
            if (!smallItem.Equals(""))
            {
                if(!bigItem.Equals(""))
                    sql += " AND";
                sql += " TypeSmall = '" + smallItem + "'";
            }
            if (!ValiditySpecies.Equals(""))
            {
                if (!bigItem.Equals(""))
                    sql += " AND";
                sql += " ValiditySpecies = '" + ValiditySpecies + "'";
            }
                
            if (!value.Equals(""))
            {
                if (!bigItem.Equals(""))
                    sql += " AND";
                sql += " (";
                for (int i = 0; i < value.Length; i++)
                {
                    sql += "ProductName like '%" + value[i].ToString() + "%'";
                    if (i < value.Length - 1)
                        sql += " OR ";
                }
                sql += ")";
            }
        }

        if (!p.Equals(""))
        {
            if (p.Equals("H"))
                sql += " order by Price desc";
            else if (p.Equals("L"))
                sql += " order by Price asc";
        }
        else if (!h.Equals(""))
        {
            if (h.Equals("H"))
                sql += " order by OrderAmount desc";
            else if (h.Equals("L"))
                sql += " order by OrderAmount asc";
        }
        else
            sql += " order by ProductID";

        // 暫存商品表單，ProductID與ProductName
        job = gm.getJsonResult(sqlMethod.Select(sql));
        if (job["stage"].ToString().Equals(true.ToString()))
        {
            jarray = gm.getJsonArrayResult(job["message"].ToString());
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
            sql = "select ProductID,ImageUrl from ProductImage where Type ='Main' AND (";
            for (int i = 0; i < id.Length; i++)
            {
                sql += "ProductID = '" + id[i] + "'";
                if (i < id.Length - 1)
                    sql += " OR ";
                else
                    sql += ")";
            }
            job = gm.getJsonResult(sqlMethod.Select(sql));
            if (job["stage"].ToString().Equals(true.ToString()))
            {
                jarray = gm.getJsonArrayResult(job["message"].ToString());
                image = new string[jarray.Count];
                string[] productID = new string[jarray.Count];
                for (int i = 0; i < jarray.Count; i++)
                {
                    job = gm.getJsonResult(jarray[i].ToString());
                    image[i] = job["ImageUrl"].ToString();
                    productID[i] = job["ProductID"].ToString();
                }
                image = gm.selectOrder(id, productID, image);
                isImage = true;
            }



            // 輸出JSON，欄位ProductID, ProductName, Image
            if (isImage && isProduct)
            {
                string rejson = "[";
                for (int i = 0; i < id.Length; i++)
                {
                    rejson += gm.getJsonArray("ProductID;ProductName;Price;Image", id[i] + ";" + name[i] + ";" + price[i] + ";" + image[i]);
                    if (i < id.Length - 1)
                        rejson += ",";
                }
                rejson += "]";
                json = gm.getStageJson(true, rejson);
            }

        }

        return json;
    }

    //取得指定商品的圖片
    public string GetProductImage(string id) //Huan-Chieh Chen
    {
        sql = "select ImageUrl,Type from ProductImage where ProductID=" + id;
        return sqlMethod.Select(sql);
    }

    //取得指定商品介紹
    public string GetProductIntroduce(string id) //Huan-Chieh Chen
    {
        sql = "select TOP (1) Introduction, PackageSpecification, Date, Happy,Transportation from ProductIntroduce where ProductID=" + id + " order by ProductIntroduceID desc";
        return sqlMethod.Select(sql);
    }

    //取得指定商品履歷
    public string GetRecordOperation(string id) //Huan-Chieh Chen
    {
        sql = "select Date, Type, Action, Note from RecordOperation where RecordID =" + id;
        return sqlMethod.Select(sql);
    }
}


