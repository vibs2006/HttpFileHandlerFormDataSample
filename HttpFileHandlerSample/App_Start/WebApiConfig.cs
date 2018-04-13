using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HttpFileHandlerSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Use following if you want to define via ActionName (for multiple web api methods)
            //config.Routes.MapHttpRoute(
            //    name: "DefaultActionControllerApi", 
            //        routeTemplate : "api/{controller}/{action}",
            //        defaults: null                
            //    );
        }
    }
}
