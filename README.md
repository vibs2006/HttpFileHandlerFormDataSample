## Upload Form Multipart Data to an ASP.NET File Handler using C#

Two approaches are defined in this example. 

1. Upload mixed data (Form Fields and Files) using browser postback (input type is **Submit**)
2. Upload mixed data (Form Fields and Files) using browser postback (input type is **Button**)

#### Posting Multipart Formdata (inclusive of form fields and files) using AJAX Method of JQuery
```js
  var form = $("#frmData");
  var params = form.serializeArray();

  var files = $("#File1")[0].files;

  if (typeof (FormData) == "undefined") alert("Please update to latest browser");

  var customFormData = new FormData();

  //Populating all files in custom form data
  for (var i=0; i < files.length; i++)
  {
      customFormData.append(files[i].name, files[i]);
  }

  //Populating all input field names (ensure that you have attribute name in their columns as id doesn't work
  $.each(params, function (index,element) {
      customFormData.append(element.name, element.value);
  });

  var btn = $(this);
  btn.val("Uploading..");
  btn.prop("disabled", true);
  var dynamicDiv = $("#divDynamic");
  dynamicDiv.html("");
  dynamicDiv.attr("class", ""); //Remove all classes
  $.ajax({
      url: "Handler1.ashx",
      method: "post",
      contentType: false,
      processData: false,
      data: customFormData,
      success: function (successData) {
          dynamicDiv.html(successData);
          alert("Upload Completed");
          dynamicDiv.addClass("success");
      },
      error: function (errorData) {
          console.log(errorData);
          alert("Error in uploading");
          $("#divDynamic").html(errorData.responseText);
          dynamicDiv.addClass("fail");
      },
      complete: function () {
          btn.val("submit without postback");
          btn.prop("disabled", false);
      }
  });
```


#### Example of a sample ASP.NET C# Filehandler handing Accept Request of Multipart Formdata
```cs
public void ProcessRequest(HttpContext context)
  {
      if (context.Request.Form.Count == 0 && context.Request.Files.Count == 0)
      {
          throw new Exception("This file is supposed to be accessed only via a webpage having <form> object");
      }

      var path = context.Server.MapPath("~/upload");

      if (!Directory.Exists(path))
      {
          Directory.CreateDirectory(path);
      }
      else
      {
          DeleteAllFiles(context,path);
      }

      //Simulating delay. Remove Thread.sleep method for production
      System.Threading.Thread.Sleep(2500); //2.5 Seconds

      List<string> str = new List<string>();

      var str1 = new StringBuilder();
      context.Response.ContentType = "text/html";
      str1.AppendLine("<fieldset><legend>Multipart Formdata Statistics</legend>");
      str1.AppendLine("Total Number of uploaded Files are " + context.Request.Files.Count + "<br />");
      str1.AppendLine("Total Number of Elements are " + context.Request.Form.Count + "<br />");
      str1.AppendLine("</fieldset>");
      var files = context.Request.Files;


      //Saving All Files from Files collection
      for (int i=0;i<files.Count;i++) //Notice that index starts from ZERO!
      {
          HttpPostedFile postedFile = files[i];
          string fileName = Path.GetFileName(postedFile.FileName);
          postedFile.SaveAs(path + "\\" + fileName);
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
  public void DeleteAllFiles(HttpContext context, string path)
  {
      //throw new Exception("Custom ExceptionCalled"); Test for Failture                       
      DirectoryInfo dirInfo = new DirectoryInfo(path);
      var allFiles = dirInfo.EnumerateFiles("*");            
      foreach (var file in allFiles)
      {
          File.Delete(file.FullName);
      }
  }
```

