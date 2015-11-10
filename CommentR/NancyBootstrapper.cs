using Nancy;
using Nancy.Conventions;
using Nancy.Diagnostics;

namespace CommentR
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);
            conventions.StaticContentsConventions.AddDirectory("/test");
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                return new DiagnosticsConfiguration
                {
                    Password = @"password"
                };
            }
        }
    }
}