using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QRCoder;
using System.Drawing;
using System.IO;

/// <summary>
/// Summary description for QRCode
/// </summary>
public class QRCodeMethod
{
    public string GetQRCode(string data)
    {
        GetMethod gm = new GetMethod();
        string savePath = HttpContext.Current.Server.MapPath("~/QRCode/");
        string saveName = DateTime.Now.ToString("yyyyMMdd") + gm.getUUID() + ".jpg";
        string serverPath = "http://140.127.22.4/PlatformAPI/QRCode/";
        string returnURL = serverPath + saveName;
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new QRCode(qrCodeData);
        Bitmap qrCodeImage = qrCode.GetGraphic(20);
        System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
        imgBarCode.Height = 150;
        imgBarCode.Width = 150;
        using (Bitmap bitMap = qrCode.GetGraphic(20))
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = ms.ToArray();
                try
                {
                    MemoryStream memoryStream = new MemoryStream(byteImage);
                    FileStream fileStream = new FileStream(savePath + saveName, FileMode.Create);
                    memoryStream.WriteTo(fileStream);
                    memoryStream.Close();
                    fileStream.Close();
                    fileStream = null;
                    memoryStream = null;
                    return returnURL;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "null";
                }
            }
        }
    }
}