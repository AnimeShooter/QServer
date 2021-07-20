using Nancy;
using Nancy.Bootstrapper;
using Nancy.ErrorHandling;
using Nancy.LeakyBucket;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.External.HTTP.Nancy
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            LeakyBucketRateLimiter.Enable(pipelines, new LeakyBucketRateLimiterConfiguration
            {
                MaxNumberOfRequests = 12,
                RefreshRate = TimeSpan.FromSeconds(60)
            });

            base.ApplicationStartup(container, pipelines);
        }

#if DEBUG
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            // CORS Enabled for DEBUG (server should use nginx)
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type")
                    .WithHeader("Access-Control-Allow-Headers", "*");
            });
        }
#endif

    }

#if !DEBUG
    public class MyStatusHandler : IStatusCodeHandler
    {
        public bool HandlesStatusCode(global::Nancy.HttpStatusCode statusCode, NancyContext context)
        {
            return true;
        }

        public void Handle(global::Nancy.HttpStatusCode statusCode, NancyContext context)
        {
            return;
        }
    }
#endif


}
