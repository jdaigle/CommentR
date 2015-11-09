using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    Contents = x => RenderJavascript(x, rootPath),
                    ContentType = "application/javascript",
                };
            };

            Get["/css"] = _ =>
            {
                return new Response()
                {
                    Contents = x => RenderCSS(x, rootPath),
                    ContentType = "text/css",
                };
            };
        }

        public static void RenderJavascript(Stream responseStream, IRootPathProvider rootPath)
        {
            using (var js = new FileStream(Path.Combine(rootPath.GetRootPath(), "content/blog.js"), FileMode.Open))
            {
                js.CopyTo(responseStream);
            }
        }

        public static void RenderCSS(Stream responseStream, IRootPathProvider rootPath)
        {
            using (var js = new FileStream(Path.Combine(rootPath.GetRootPath(), "content/blog.css"), FileMode.Open))
            {
                js.CopyTo(responseStream);
            }
        }
    }
}