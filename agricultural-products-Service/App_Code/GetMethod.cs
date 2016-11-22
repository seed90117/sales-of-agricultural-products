using System;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IO;

/// <summary>
/// Summary description for GetMethod
/// </summary>
public class GetMethod
{
    // 回傳結果使用，輸入狀態與訊息
    public string getStageJson(bool stage, string msg)
    {
        ReturnMsg reMsg = new ReturnMsg();
        reMsg.stage = stage;
        reMsg.message = msg;
        return JsonConvert.SerializeObject(reMsg);
    }

    // 自訂單一JSON值
    public string getJson(string name, string input)
    {
        JObject job = JObject.Parse(@"{""" + name + @""": """ + input.ToString() + @"""}");
        return JsonConvert.SerializeObject(job, Formatting.None);
    }

    // 輸入單一資料JSON字串，取得JObject物件
    public JObject getJsonResult(string jsonStr)
    {
        return JsonConvert.DeserializeObject<JObject>(jsonStr);
    }

    // 輸入多資料JSON字串，取得JArray物件
    public JArray getJsonArrayResult(string jsonStr)
    {
        return JsonConvert.DeserializeObject<JArray>(jsonStr);
    }

    // 輸入單一資料JSON字串，取得JSON字串
    public string getJsonSingleResult(string jsonStr)
    {
        JArray jArray = getJsonArrayResult(jsonStr);
        return jArray[0].ToString();
    }

    // 輸入陣列型態單一資料JSON字串，取得JObject物件
    public JObject getJsonObjectResult(string jsonStr)
    {
        JArray jArray = getJsonArrayResult(jsonStr);
        return getJsonResult(jArray[0].ToString());
    }

    // 自訂多資料JSON值，Key與Values內各筆資料用";"隔開
    public string getJsonArray(string nameArray, string inputArray)
    {
        string[] name = nameArray.Split(';');
        string[] input = inputArray.Split(';');
        string json = "{";
        for (int i=0; i<name.Length; i++)
        {
            json += @"""" + name[i] + @""": """ + input[i] + @"""";
            if (i == name.Length - 1)
            {
                json += "}";
            }
            else
            {
                json += ",";
            }
        }
        return json;
    }

    // 自訂多資料JSON值，Key與Values(雙引號需自行輸入)內各筆資料用";"隔開
    public string getJsonItemArray(string nameArray, string inputArray)
    {
        string[] name = nameArray.Split(';');
        string[] input = inputArray.Split(';');
        string json = "{";
        for (int i = 0; i < name.Length; i++)
        {
            json += @"""" + name[i] + @""": " + input[i];
            if (i == name.Length - 1)
            {
                json += "}";
            }
            else
            {
                json += ",";
            }
        }
        return json;
    }

    // 取得客戶端IP位址
    public string getIpAddress()
    {
        //登入ip
        string strIpAddr = string.Empty;

        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null || HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf("unknown") > 0)
        {
            strIpAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
        else if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(",") > 0)
        {
            strIpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Substring(1, HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(",") - 1);
        }
        else if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(";") > 0)
        {
            strIpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Substring(1, HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(";") - 1);
        }
        else
        {
            strIpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        }
        return strIpAddr; ;
    }

    // 取得唯一碼
    public string getUUID()
    {
        return System.Guid.NewGuid().ToString().Replace("-","");
    }

    // 取得現在時間，範例2016/12/30-23:59:59
    public string getCurrentDate()
    {
        return DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss");
    }

    // 上傳圖片
    public string upload(string file, string fileName, string type)
    {
        string savePath = "";
        string saveName = "";
        string serverPath = "";
        if (type.Equals(""))
        {
            savePath = HttpContext.Current.Server.MapPath("~/Other/");
            serverPath = "http://140.127.22.4/PlatformAPI/Other/";

        }
        else
        {
            savePath = HttpContext.Current.Server.MapPath("~/" + type + "/");
            serverPath = "http://140.127.22.4/PlatformAPI/" + type + "/";
        }
        string[] fileType = fileName.Split('.');
        saveName = getUUID() + "." + fileType[fileType.Length-1];
        string returnURL = serverPath + saveName;
        byte[] bt = Convert.FromBase64String(file);
        MemoryStream memoryStream = null;
        FileStream fileStream = null;
        try
        {
            memoryStream = new MemoryStream(bt);
            fileStream = new FileStream(savePath+saveName, FileMode.Create);
            memoryStream.WriteTo(fileStream);
            return returnURL;
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return "";
        }
        finally
        {
            memoryStream.Close();
            fileStream.Close();
            fileStream = null;
            memoryStream = null;
        }
    }

    // 上傳圖片類型判斷
    public string getImageType(string type)
    {
        switch (type)
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
            default:
                type = "";
                break;
        }
        return type;
    }

    // 會員權限類型判斷
    public string getMemberAccess(string type)
    {
        switch (type)
        {
            case "A":
                type = "ㄍㄌㄓ";
                break;
            case "E":
                type = "ㄔㄕ";
                break;
            case "C":
                type = "ㄍㄎ";
                break;
            default:
                type = "";
                break;
        }
        return type;
    }

    // 產品狀態判斷
    public string getProductStage(string type)
    {
        switch (type)
        {
            case "S":
                type = "Sales";
                break;
            case "P":
                type = "Prepare";
                break;
            case "D":
                type = "Drop";
                break;
            default:
                type = "Prepare";
                break;
        }
        return type;
    }
}