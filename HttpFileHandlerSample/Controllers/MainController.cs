using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;

using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace HttpFileHandlerSample.Controllers
{
    public class MainController : ApiController
    {
        [HttpPost]
        public IHttpActionResult getAllDetails()
        {
            var isMultiForm = Request.Content.IsMimeMultipartContent();

//            var result = await Request.Content.ReadAsMultipartAsync();

//            var requestJson = await result.Contents[0].ReadAsStringAsync();
//            var request = JsonConvert.DeserializeObject<MyRequestType>(requestJson);

//            if (result.Contents.Count > 1)
//            {
//                var fileByteArray = await result.Contents[1].ReadAsByteArrayAsync();
//    ...
//}
            var contentRequest = Request.Content;
            if (isMultiForm)
            {
                var provider = new MultipartFormDataStreamProvider(HttpContext.Current.Server.MapPath("~/App_Data"));
                // var multiProvider = provider.ReadAsMultipartAsync().Result;
                var multiStreamProvider = Request.Content.ReadAsMultipartAsync(provider).Result;
                var multiStreamProvider1 = Request.Content.ReadAsMultipartAsync().Result;
                
                for (int i =0; i< multiStreamProvider1.Contents.Count;i++)
                {
                    var content = multiStreamProvider1.Contents[i].IsFormData();
                }


                var formData = multiStreamProvider.FormData;
                var fileData = multiStreamProvider.FileData;
                foreach (var form in formData)
                {
                    textWrite(form.ToString() + "-" + formData[form.ToString()]);
                }

                foreach (MultipartFileData file in fileData)
                {
                    textWrite(file.LocalFileName);
                }
               
               
                
            }
            else
            {
                return BadRequest("Not a valid form data content");
            }
            return Ok(isMultiForm);
        }

        private void textWrite(string text)
        {
            var path = HttpContext.Current.Server.MapPath("~/log");
            var pathExists = Directory.Exists(path);
            if (!pathExists) Directory.CreateDirectory(path);

            using (StreamWriter sf = new StreamWriter(path + "\\" + DateTime.Now.ToString("yyyyMMdd.txt"), true))
            {
                sf.WriteLine(text);
            }
        }

    }
}
