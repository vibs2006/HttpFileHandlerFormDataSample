﻿using System;
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
using System.Web.Routing;

namespace HttpFileHandlerSample.Controllers
{
    public class MainController : ApiController
    {
        [HttpGet]
        public IHttpActionResult ok()
        {
            return Ok("vibs");
        }

        /// <summary>
        /// Accept Large File Uploads as it use Bufferless Input Stream Internall
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("~/api/main/uploadLargeFormData")]
        public async Task<IHttpActionResult> uploadLargeFormData()
        {
            
            Stream rawStream =
                HttpContext.Current.Request.GetBufferlessInputStream(true);

            var streamContent = new StreamContent(rawStream);
            foreach (var header in Request.Content.Headers)
            {
                streamContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var provider = new MultipartFormDataStreamProvider(HttpContext.Current.Server.MapPath("~/App_Data"));


            var multipartFormContents = await streamContent.ReadAsMultipartAsync(provider);

            var files = multipartFormContents.FileData;

            var path = HttpContext.Current.Server.MapPath("~/upload");

            foreach (var file in files)
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                //FileInfo fi = new FileInfo(file.LocalFileName);
                var localFileNameWithoutQuotes = file.Headers.ContentDisposition.FileName.Replace("\"", "");
                
                //File has to be moved from App_Data otherwise it'll remain there foreever. If this fails then try File.Copy followed or File.Delete of the original file.                                
                var newFileNameWithPath = Path.Combine(path, localFileNameWithoutQuotes);

                //Checking if File Exists on the upload file path location
                if (File.Exists(newFileNameWithPath))
                {
                    File.GetAccessControl(newFileNameWithPath);
                    File.Delete(newFileNameWithPath);
                }
                File.Copy(file.LocalFileName, newFileNameWithPath);
            }

            foreach (var formValue in multipartFormContents.FormData)
            {
                textWrite($"{formValue} - {multipartFormContents.FormData[formValue.ToString()]}");
            }

            //Cleaning File Data files
            foreach (var file in files)
            {
                File.Delete(file.LocalFileName);
            }

            //Stream reqStream = Request.Content.ReadAsStreamAsync().Result;
            //MemoryStream tempStream = new MemoryStream();
            //reqStream.CopyTo(tempStream);

            //tempStream.Seek(0, SeekOrigin.End);
            //StreamWriter writer = new StreamWriter(tempStream);
            //writer.WriteLine();
            //writer.Flush();
            //tempStream.Position = 0;

            //StreamContent streamContent = new StreamContent(tempStream);
            //foreach (var header in Request.Content.Headers)
            //{
            //    streamContent.Headers.Add(header.Key, header.Value);
            //}

            //// Read the form data and return an async task.
            //await streamContent.ReadAsMultipartAsync(provider);

            

            return Ok("Success");
        }

        [HttpPost]        
        [Route("~/api/main/AcceptAllDetails")]
        public async Task<IHttpActionResult> AcceptAllDetails()
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

                var objMultiPartMemoryStreamProvider = await Request.Content.ReadAsMultipartAsync();
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
                        var contentBody = await httpContent.ReadAsStringAsync();
                        textWrite(contentBody);
                    }
                    else
                    {
                        

                        //byte[] buffer = new byte[2048];
                        //var buffer = await httpContent.ReadAsByteArrayAsync();

                        //HttpContext.Current.Request.GetBufferlessInputStream()

                        //var content = await new StreamContent(objMultiPartMemoryStreamProvider.Contents[i].ReadAsStreamAsync(),)

                        // var stream = await httpContent.ReadAsStreamAsync()
                        //Stream fileStream = await httpContent.ReadAsStreamAsync();
                        //var uploadPath = HttpContext.Current.Server.MapPath("~/upload");
                        //if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                        //var fileNameWithPath = "/" + uploadPath + DateTime.Now.ToString("yyyyMMddHHmmss");

                        //File.WriteAllBytes(fileNameWithPath, buffer);
                        //using (FileStream fs = new FileStream(uploadPath+DateTime.Now.ToString("yyyyMMddHHmmss"), FileMode.Append))
                        //{
                        //    await fileStream.CopyToAsync(fs);
                        //}


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
