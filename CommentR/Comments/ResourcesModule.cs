using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Nancy;

namespace CommentR.Comments
{
    public class ResourcesModule : NancyModule
    {
        public ResourcesModule(IRootPathProvider rootPath)
        {
            // TODO: implement caching
            Get["/js"] = _ =>
            {
                return new Response()
                {
                    Contents = x => RenderExternalJavascript(x, rootPath),
                    ContentType = "application/javascript",
                };
            };
        }

        public static void RenderExternalJavascript(Stream responseStream, IRootPathProvider rootPath)
        {
            using (var js = new FileStream(Path.Combine(rootPath.GetRootPath(), "content/external.js"), FileMode.Open))
            {
                js.CopyTo(responseStream);
            }
        }
    }
}