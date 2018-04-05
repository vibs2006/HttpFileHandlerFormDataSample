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
            List<string> str = new List<string>();

            var str1 = new StringBuilder();
            context.Response.ContentType = "text/plain";
            str1.AppendLine("Total Number of files are " + context.Request.Files.Count);
            str1.AppendLine("Total Number of Elemens are " + context.Request.Form.Count);
            
            var files = context.Request.Files;
            var path = context.Server.MapPath("~/upload");

            if (!Directory.Exists(path))
            Directory.CreateDirectory(path);


            foreach (var file in files)
            {
                HttpPostedFile postFIle = files[file.ToString()];
                postFIle.SaveAs(path + "\\" + postFIle.FileName);
            }

            


            var Name = context.Request.Form;
            foreach (var item in Name)
            {
                str1.AppendLine(item.ToString() + " - " + Name[item.ToString()]);
            }
            context.Response.Write(str1.ToString());
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