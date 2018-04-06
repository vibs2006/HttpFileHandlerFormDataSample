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

            //Simulating delay. Remove Thread.sleep method for production
            System.Threading.Thread.Sleep(4000); //4 Seconds

            List<string> str = new List<string>();

            var str1 = new StringBuilder();
            context.Response.ContentType = "text/html";
            str1.AppendLine("<fieldset><legend>Multipart Formdata Statistics</legend>");
            str1.AppendLine("Total Number of uploaded Files are " + context.Request.Files.Count + "<br />");
            str1.AppendLine("Total Number of Elements are " + context.Request.Form.Count + "<br />");
            str1.AppendLine("</fieldset>");
            var files = context.Request.Files;
            var path = context.Server.MapPath("~/upload");

            if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

            //Saving All Files from Files collection
            for (int i=0;i<files.Count;i++) //Notice that index starts from ZERO!
            {
                HttpPostedFile postedFile = files[i];
                postedFile.SaveAs(path + "\\" + postedFile.FileName);
            }
            

            var Name = context.Request.Form;
            str1.AppendLine("<fieldset><legend>Name of Input Text Fields with 'name' attribute present</legend>");
            str1.AppendLine("<b>Attribute Name - Attribute Value</b><br />");
            foreach (var item in Name)
            {
                str1.AppendLine(item.ToString() + " - " + Name[item.ToString()] + "<br />");
            }
            str1.AppendLine("</fieldset>");

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var allFiles = dirInfo.EnumerateFiles("*").OrderByDescending( x => x.CreationTime);
            str1.AppendLine("<fieldset><legend>List of Files Uploaded</legend>");
            foreach (var file in allFiles)
            {                
                str1.AppendLine("<a href='/upload/"+file.Name+"' target='_blank'>" + file.Name + "</a>" + "<br />");                
            }
            str1.AppendLine("</fieldset>");

            context.Response.Write(str1.ToString());
        }

        /// <summary>
        /// Deleting all previous files before sending new request so that our state is clean for each example
        /// </summary>
        /// <param name="context"></param>
        public void DeleteAllFiles(HttpContext context)
        {
            //throw new Exception("Custom ExceptionCalled"); Test for Failture
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