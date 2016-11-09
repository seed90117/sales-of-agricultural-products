using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

/// <summary>
/// Summary description for SQLMethod
/// </summary>
public class SQLMethod
{
    private static string strdbcon = "server=140.127.22.4;database=AgriculturalProducts;uid=CCBDA;pwd=CCBDA";
    private SqlConnection objcon = new SqlConnection(strdbcon);
    private SqlCommand sqlcmd;

    public SQLMethod()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    // Simple insert method
    public string Insert(string sql)
    {
        GetMethod gm = new GetMethod();
        if (!sql.Equals(""))
        {
            try
            {
                objcon.Open(); // 開啟連接
                sqlcmd = new SqlCommand(sql, objcon); // 建立SQL命令對象
                sqlcmd.ExecuteNonQuery();
                return gm.getStageJson(true, "Success");
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
                return gm.getStageJson(false, "Fail");
            }
            finally
            {
                objcon.Close(); // 關閉連接
            }
        }
        else
        {
            return gm.getStageJson(false, "Null");
        }
    }

    // Simple select method for two or many data
    public string Select(string sql)
    {
        GetMethod gm = new GetMethod();
        if (!sql.Equals(""))
        {
            try
            {
                objcon.Open();
                sqlcmd = new SqlCommand(sql, objcon);
                SqlDataReader dr = sqlcmd.ExecuteReader();
                if (dr.IsClosed == false)
                {
                    DataTable table = new DataTable();
                    table.Load(dr);
                    //這行是利用Json.net直接把table轉成Json
                    return JsonConvert.SerializeObject(table, Formatting.None);
                    dr.Close(); // 停止讀取資料
                }
                else
                {
                    return gm.getStageJson(false, "Contact");
                }
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
                return gm.getStageJson(false, "Fail");
            }
            finally
            {
                objcon.Close(); // 關閉連接
            }
        }
        else
        {
            return gm.getStageJson(false, "Null");
        }
    }

    // Simple select method for one data, return{"Result" : "Value"}
    public string SelectSingle(string sql, string returnName, int count)
    {
        GetMethod gm = new GetMethod();
        if (!sql.Equals(""))
        {
            try
            {
                objcon.Open();
                sqlcmd = new SqlCommand(sql, objcon);
                SqlDataReader dr = sqlcmd.ExecuteReader();
                if (dr.IsClosed == false)
                {
                    dr.Read();
                    string drStr = "";
                    for (int i = 0; i < count; i++)
                    {
                        drStr += dr[i].ToString();
                        if (i < count-1)
                        {
                            drStr += ";";
                        }
                    }
                    return gm.getJsonArray(returnName, drStr);
                    dr.Close(); // 停止讀取資料
                }
                else
                {
                    return gm.getStageJson(false, "Contact");
                }
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
                return gm.getStageJson(false, "Fail");
            }
            finally
            {
                objcon.Close(); // 關閉連接
            }
        }
        else
        {
            return gm.getStageJson(false, "Null");
        }
    }

    // Simple update method
    public string Update(string sql)
    {
        GetMethod gm = new GetMethod();
        if (!sql.Equals(""))
        {
            try
            {
                objcon.Open();
                sqlcmd = new SqlCommand(sql, objcon);
                sqlcmd.ExecuteNonQuery();
                
                return gm.getStageJson(true, "Success");
            }
            catch (Exception ex)
            {
                return gm.getStageJson(false, "Fail");
            }
            finally
            {
                objcon.Close();
            }
        }
        else
        {
            return gm.getStageJson(false, "Null");
        }
    }
    
    // SignSelect
    public string SignSelect(string sql, string returnNameArray)
    {
        GetMethod gm = new GetMethod();
        if (!sql.Equals(""))
        {
            try
            {
                objcon.Open();
                sqlcmd = new SqlCommand(sql, objcon);
                SqlDataReader dr = sqlcmd.ExecuteReader();
                if (dr.IsClosed == false)
                {
                    dr.Read();
                    return gm.getJsonArray(returnNameArray, dr[0].ToString() + ";" + dr[1].ToString());
                    dr.Close(); // 停止讀取資料
                }
                else
                {
                    return gm.getStageJson(false, "Contact");
                }
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
                return gm.getStageJson(false, "Fail");
            }
            finally
            {
                objcon.Close(); // 關閉連接
            }
        }
        else
        {
            return gm.getStageJson(false, "Null");
        }
    }

    // Insert and return new ID
    public string InsertSelect(string sql)
    {
        GetMethod gm = new GetMethod();
        if (!sql.Equals(""))
        {
            try
            {
                objcon.Open();
                sqlcmd = new SqlCommand(sql, objcon);
                SqlDataReader dr = sqlcmd.ExecuteReader();
                if (dr.IsClosed == false)
                {
                    dr.Read();
                    return gm.getJson("ProductID", dr[0].ToString());
                    dr.Close(); // 停止讀取資料
                }
                else
                {
                    return gm.getStageJson(false, "Contact");
                }
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
                return gm.getStageJson(false, "Fail");
            }
            finally
            {
                objcon.Close(); // 關閉連接
            }
        }
        else
        {
            return gm.getStageJson(false, "Null");
        }
    }

    /*
    // SignIn
    private bool insertSignLog(string MemberID, string Account, string Identify, string Ip)
    {
        try
        {
            objcon = new SqlConnection(strdbcon); // 建立連接
            objcon.Open(); // 開啟連接
            sql = "insert into SignLog(MemberID,Account,SignTime,Identify,IP) values('" +
                MemberID + "','" + Account + "','" + DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss") +
                "','" + Identify + "','" + Ip + "')";
            sqlcmd = new SqlCommand(sql, objcon); // 建立SQL命令對象
            sqlcmd.ExecuteNonQuery();
            objcon.Close(); // 關閉連接
            return true;
        }
        catch (Exception ex)
        {
            //Response.Write(ex.Message);
            return false;
        }
    }*/
}