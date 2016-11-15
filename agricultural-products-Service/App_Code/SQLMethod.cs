using System;
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
    private GetMethod gm = new GetMethod();
    private Message msg = new Message();

    public SQLMethod()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    // Simple insert method
    public string Insert(string sql)
    {
        if (!sql.Equals(""))
        {
            try
            {
                objcon.Open(); // 開啟連接
                sqlcmd = new SqlCommand(sql, objcon); // 建立SQL命令對象
                sqlcmd.ExecuteNonQuery();
                return gm.getStageJson(true, msg.success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return gm.getStageJson(false, msg.contactError_cht);
            }
            finally
            {
                objcon.Close(); // 關閉連接
            }
        }
        else
        {
            return gm.getStageJson(false, msg.sqlError_cht);
        }
    }

    // Simple select method, two or many database data were returned
    public string Select(string sql)
    {
        if (!sql.Equals(""))
        {
            SqlDataReader dr = null;
            string data = "";
            try
            {
                objcon.Open();
                sqlcmd = new SqlCommand(sql, objcon);
                dr = sqlcmd.ExecuteReader();
                if (dr.IsClosed == false)
                {
                    DataTable table = new DataTable();
                    table.Load(dr);
                    //這行是利用Json.net直接把table轉成Json
                    data = JsonConvert.SerializeObject(table, Formatting.None);
                    if (!data.Equals("[]"))
                        return data;
                    else
                        return gm.getStageJson(false, msg.noData_cht);
                }
                else
                {
                    return gm.getStageJson(false, msg.contactError_cht);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return gm.getStageJson(false, msg.error_cht);
            }
            finally
            {
                dr.Close(); // 停止讀取資料
                objcon.Close(); // 關閉連接
            }
        }
        else
        {
            return gm.getStageJson(false, msg.sqlError_cht);
        }
    }

    // Simple update method
    public string Update(string sql)
    {
        if (!sql.Equals(""))
        {
            try
            {
                objcon.Open();
                sqlcmd = new SqlCommand(sql, objcon);
                sqlcmd.ExecuteNonQuery();
                return gm.getStageJson(true, msg.success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return gm.getStageJson(false, msg.error_cht);
            }
            finally
            {
                objcon.Close();
            }
        }
        else
        {
            return gm.getStageJson(false, msg.sqlError_cht);
        }
    }

    // Insert and return new ID
    public string InsertSelect(string sql)
    {
        if (!sql.Equals(""))
        {
            SqlDataReader dr = null;
            try
            {
                objcon.Open();
                sqlcmd = new SqlCommand(sql, objcon);
                dr = sqlcmd.ExecuteReader();
                if (dr.IsClosed == false)
                {
                    if (dr.Read())
                    {
                        string productID = dr[0].ToString();
                        return gm.getJson("ProductID", productID);
                    }
                    else
                        return gm.getStageJson(false, msg.noData_cht);
                }
                else
                {
                    return gm.getStageJson(false, msg.contactError_cht);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return gm.getStageJson(false, msg.error_cht);
            }
            finally
            {
                dr.Close(); // 停止讀取資料
                objcon.Close(); // 關閉連接
            }
        }
        else
        {
            return gm.getStageJson(false, msg.sqlError_cht);
        }
    }

}