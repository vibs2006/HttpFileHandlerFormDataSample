using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Web.Http;

namespace HttpFileHandlerSample.Controllers
{
    public class MainController : ApiController
    {
        [HttpPost]
        public IHttpActionResult getAllDetails()
        {
            var isMultiForm = Request.Content.IsMimeMultipartContent();

            //            Fetch JSON Data and File in the same Post (JSON at Index 0 and File in at Index 1
            //            var result = await Request.Content.ReadAsMultipartAsync();
            //            var requestJson = await result.Contents[0].ReadAsStringAsync();
            //            var request = JsonConvert.DeserializeObject<MyRequestType>(requestJson);
            //            if (result.Contents.Count > 1)
            //            {
            //                var fileByteArray = await result.Contents[1].ReadAsByteArrayAsync();
            //    
            //}
            //var contentRequest = Request.Content;
            if (isMultiForm)
            {
                //var provider = new MultipartFormDataStreamProvider(HttpContext.Current.Server.MapPath("~/App_Data"));
                // var multiProvider = provider.ReadAsMultipartAsync().Result;
                //var multiStreamProvider = Request.Content.ReadAsMultipartAsync(provider).Result;

                var objMultiPartMemoryStreamProvider = Request.Content.ReadAsMultipartAsync().Result;
                var multiPartContents = objMultiPartMemoryStreamProvider.Contents;

                for (int i = 0; i < multiPartContents.Count; i++)
                {
                    var contentDispositionHeaderDetails = multiPartContents[i].Headers.ContentDisposition;
                    var httpContent = multiPartContents[i];
                    //textWrite(contentDispositionHeaderDetails.FileName + " - " + contentDispositionHeaderDetails.Name);
                    //Checking if Content is a FormData Content is a file type
                    //if (string.IsNullOrEmpty(contentDispositionHeaderDetails.FileName) && string.IsNullOrEmpty(contentDispositionHeaderDetails.Name))
                    //{
                    //    textWrite(contentDispositionHeaderDetails.FileName + " - " + contentDispositionHeaderDetails.Name);
                    //}

                    //If FileName is empty then it means its a form data collection
                    if (string.IsNullOrEmpty(contentDispositionHeaderDetails.FileName))
                    {
                        var contentBody = httpContent.ReadAsStringAsync().Result;
                        textWrite(contentBody);
                    }
                    else
                    {
                        Stream fileStream = httpContent.ReadAsStreamAsync().Result;
                        var uploadPath = HttpContext.Current.Server.MapPath("~/upload/");
                        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                        using (FileStream fs = new FileStream(uploadPath+DateTime.Now.ToString("yyyyMMddHHmmss"), FileMode.Append))
                        {
                            fileStream.CopyTo(fs);
                        }


                        //var file = File.Create(uploadPath + DateTime.Now.ToString("yyyyMMddHHmmss"));
                        //fileStream.CopyTo(file);
                    }



                    

                }
                //var formData = multiStreamProvider.FormData;
                //var fileData = multiStreamProvider.FileData;
                //foreach (var form in formData)
                //{
                //    textWrite(form.ToString() + "-" + formData[form.ToString()]);
                //}

                //foreach (MultipartFileData file in fileData)
                //{
                //    textWrite(file.LocalFileName);
                //}



                //}
                //else
                //{
                //    return BadRequest("Not a valid form data content");
                //}
                
            }
            return Ok(isMultiForm);
        }

        private void textWrite(string text)
        {
            var path = HttpContext.Current.Server.MapPath("~/log");
            var pathExists = Directory.Exists(path);
            if (!pathExists) Directory.CreateDirectory(path);

            using (StreamWriter sf = new StreamWriter(path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", true))
            {
                sf.WriteLine(text);
            }
        }

    }
}
