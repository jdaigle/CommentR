﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Bootstrapper;

namespace CommentR
{
    public class HTTPReferrerValidation : IApplicationStartup
    {
        static List<string> referrerHostWhitelist = new List<string>()
            {
                "localhost",
                "josephdaigle.me",
                "commentr.azurewebsites.net",
            };

        public void Initialize(IPipelines pipelines)
        {
            pipelines.BeforeRequest += (NancyContext ctx) =>
            {
                if (string.IsNullOrWhiteSpace(ctx.Request.Headers.Referrer))
                {
                    return "Invalid Referrer";
                }
                var referrer = new Url(ctx.Request.Headers.Referrer);
                if (!referrerHostWhitelist.Any(x => string.Equals(x, referrer.HostName, System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    return "Invalid Referrer";
                }
                return null;
            };
        }
    }
}