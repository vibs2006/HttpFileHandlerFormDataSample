using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace HttpFileHandlerSample
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Handler1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            DeleteAllFiles(context);

            List<string> str = new List<string>();

            var str1 = new StringBuilder();
            context.Response.ContentType = "text/html";
            str1.AppendLine("Total Number of files are " + context.Request.Files.Count + "<br />");
            str1.AppendLine("Total Number of Elemens are " + context.Request.Form.Count + "<br />");
            
            var files = context.Request.Files;
            var path = context.Server.MapPath("~/upload");

            if (!Directory.Exists(path))
            Directory.CreateDirectory(path);


            for (int i=0;i<=files.Count-1;i++)
            {
                HttpPostedFile postedFile = files[i];
                postedFile.SaveAs(path + "\\" + postedFile.FileName);
            }

            var Name = context.Request.Form;
            foreach (var item in Name)
            {
                str1.AppendLine(item.ToString() + " - " + Name[item.ToString()] + "<br />");
            }

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var allFiles = dirInfo.EnumerateFiles("*").OrderByDescending( x => x.CreationTime);
            str1.AppendLine("<fieldset><legend>List of Files Uploaded</legend>");
            foreach (var file in allFiles)
            {
                
                str1.AppendLine("<a href='/upload/"+file.Name+"'>" + file.Name + "</a>" + "<br />");
                
            }
            str1.AppendLine("</fieldset>");

            context.Response.Write(str1.ToString());
        }

        public void DeleteAllFiles(HttpContext context)
        {
            var path = context.Server.MapPath("~/upload");
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var allFiles = dirInfo.EnumerateFiles("*").OrderByDescending(x => x.CreationTime);
            
            foreach (var file in allFiles)
            {
                File.Delete(file.FullName);

            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}