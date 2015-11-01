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
        private const int NotModified = 304;

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            var modifiedSince = context.Request.Headers["If-Modified-Since"];
            context.Response.Headers["If-Modified-Since-Eco"] = modifiedSince;
            string filePath = context.Server.MapPath(context.Request.Url.AbsolutePath);
            DateTime modifiedSinceRequest;
          
                
         

            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = NotFound;
               
            }
            else if (DateTime.TryParse(modifiedSince, out modifiedSinceRequest) &&
              !HasResourceModified(modifiedSinceRequest, filePath))
                {
                context.Response.StatusCode = NotModified;
            }
            else
            {
                SetCacheHeaders(context, filePath);
                context.Response.ContentType = GetByExtension(filePath);
                context.Response.WriteFile(filePath);
            }

        }

        private bool HasResourceModified(DateTime modifiedSinceRequest, string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.LastWriteTime > modifiedSinceRequest;
        }

        private static void SetCacheHeaders(HttpContext context, string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            context.Response.Cache.SetCacheability(HttpCacheability.Private);
            context.Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0, 0));
            context.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
            
            context.Response.Cache.SetLastModified(fileInfo.LastWriteTime.AddSeconds(1));
        }

        private string GetByExtension(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            return _extensionToContentTypeDict[extension];

        }
    }
}
