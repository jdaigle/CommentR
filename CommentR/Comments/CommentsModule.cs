using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using HtmlAgilityPack;
using Nancy;
using Nancy.Extensions;
using Dapper;

namespace CommentR.Comments
{
    public class CommentsModule : NancyModule
    {
        private static MarkdownSharp.Markdown markdown;
        private static string connectionString;

        static CommentsModule()
        {
            connectionString = ConfigurationManager.ConnectionStrings["CommentR"].ConnectionString;
            markdown = new MarkdownSharp.Markdown();
        }

        public CommentsModule()
        {
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

                body = SanitizeBody(body);

                var comment = new CommentModel()
                {
                    PagePermalink = permalink,
                    DateTimeUTC = DateTime.UtcNow,
                    Author = author,
                    Body = SanitizeBody(body),
                    IsHidden = false,
                    AuthorIsModerator = false,
                };

                using (var s = new SqlConnection(connectionString))
                {
                    s.Open();
                    var insertSQL = @"
INSERT INTO dbo.Comment
([PagePermalink]
,[DateTimeUTC]
,[Author]
,[Body]
,[IsHidden]
,[AuthorIsModerator])
VALUES
(@PagePermalink
,@DateTimeUTC
,@Author
,@Body
,@IsHidden
,@AuthorIsModerator);
";
                    s.Execute(insertSQL, comment);
                }

                return CreateCommentsModel(this.Context, permalink);
            };
        }

        private string SanitizeBody(string body)
        {
            var html = new HtmlDocument();
            html.LoadHtml(body);
            SanitizeNode(html.DocumentNode);
            return html.DocumentNode.WriteTo();
        }

        private void SanitizeNode(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Element)
            {
                // TODO: whitelist?
                node.Remove();
                return;
            }
            if (node.HasChildNodes)
            {
                for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
                {
                    SanitizeNode(node.ChildNodes[i]);
                }
            }
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

            return new CommentsModel
            {
                Permalink = permalink,
                Count = comments.Count,
                Comments = comments.OrderBy(x => x.DateTimeUTC).ToArray(),
            };
        }
    }
}