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
