using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LoadStaticContentThroughHandlers.Handlers
{
    public class StaticContentHandler : IHttpHandler
    {
        private static Dictionary<string, string> _extensionToContentTypeDict = new Dictionary<string, string>()
        {
            {".js","text/javascript" },
            {".css","text/css" },
            { ".jpg","image/jpeg" },
            {".png","image/png" },
             {".gif","image/gif" }
        };

        private const int NotFound = 404;
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            string filePath = context.Server.MapPath(context.Request.Url.AbsolutePath);

            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = NotFound;
               
            }
            else
            {
                context.Response.ContentType = GetByExtension(filePath);
                context.Response.WriteFile(filePath);
            }

        }

        private string GetByExtension(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            return _extensionToContentTypeDict[extension];

        }
    }
}
