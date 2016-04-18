using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using HtmlAgilityPack;
using Nancy;

namespace CommentR.Comments
{
    public class CommentsModule : NancyModule
    {
        private static string connectionString;

        static CommentsModule()
        {
            connectionString = ConfigurationManager.ConnectionStrings["CommentR"].ConnectionString;
        }

        public CommentsModule()
        {
            Before += HTTPReferrerValidation.ValidateRequest;

            Get["/frame"] = _ =>
            {
                var permalink = (string)this.Request.Query.Permalink;
                if (string.IsNullOrWhiteSpace(permalink))
                {
                    return "'permalink' is a required query string parameter";
                }
                return new FrameModel
                {
                    Permalink = permalink,
                };
            };

            Get["/comments"] = _ =>
            {
                var permalink = (string)this.Request.Query.Permalink;
                if (string.IsNullOrWhiteSpace(permalink))
                {
                    throw new InvalidOperationException("'permalink' is a required query string parameter");
                }
                return CreateCommentsModel(this.Context, permalink);
            };

            Post["/comment"] = _ =>
            {
                var permalink = (string)this.Context.Request.Form.Permalink;
                var author = (string)this.Context.Request.Form.Author;
                var body = (string)this.Context.Request.Form.Body;

                if (string.IsNullOrWhiteSpace(permalink))
                {
                    throw new InvalidOperationException("'permalink' is a required parameter");
                }

                if (string.IsNullOrWhiteSpace(author))
                {
                    throw new InvalidOperationException("'author' is a required parameter");
                }

                if (string.IsNullOrWhiteSpace(body))
                {
                    throw new InvalidOperationException("'body' is a required parameter");
                }

                body = Util.SanitizeBody(body);

                var comment = new CommentModel()
                {
                    PagePermalink = permalink,
                    DateTimeUTC = DateTime.UtcNow,
                    Author = author,
                    Body = body,
                    IsHidden = false,
                    AuthorIsModerator = false,
                };

                Util.InsertComment(comment);

                return CreateCommentsModel(this.Context, permalink);
            };
        }

        private object CreateCommentsModel(NancyContext context, string permalink)
        {
            List<CommentModel> comments = null;
            using (var s = new SqlConnection(connectionString))
            {
                s.Open();
                comments = s.Query<CommentModel>(
                        "SELECT * FROM dbo.Comment WHERE PagePermalink = @PagePermalink AND IsHidden = 0;",
                        param: new { PagePermalink = permalink, })
                    .ToList();
            }

            Util.OrderComments(comments);

            return new CommentsModel
            {
                Permalink = permalink,
                Count = comments.Count,
                Comments = comments.ToArray(),
            };
        }
    }
}