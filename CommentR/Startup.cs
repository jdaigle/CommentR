using CommentR;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Nancy;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace CommentR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            StaticConfiguration.EnableRequestTracing = true;

            app.UseNancy(new Nancy.Owin.NancyOptions()
            {
                Bootstrapper = new NancyBootstrapper(),
                //PerformPassThrough = c => { return c.Response.StatusCode == HttpStatusCode.NotFound; },
            });
            app.UseStageMarker(PipelineStage.MapHandler);
        }
    }
}