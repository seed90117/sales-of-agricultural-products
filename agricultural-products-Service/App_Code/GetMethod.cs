using System;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Summary description for GetMethod
/// </summary>
public class GetMethod
{

    public string getStageJson(bool stage, string msg)
    {
        ReturnMsg reMsg = new ReturnMsg();
        reMsg.stage = stage;
        reMsg.msg = msg;
        return JsonConvert.SerializeObject(reMsg);
    }

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

    public JObject getJsonResult(string jsonStr)
    {
        return JsonConvert.DeserializeObject<JObject>(jsonStr);
    }

    public string getJson(string name, string input)
    {
        JObject job = JObject.Parse(@"{""" + name + @""": """ + input.ToString() + @"""}");
        return JsonConvert.SerializeObject(job, Formatting.None);
    }

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

    public string getUUID()
    {
        return System.Guid.NewGuid().ToString();
    }

    public string getCurrentDate()
    {
        return DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss");
    }
}