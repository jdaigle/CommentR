using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Nancy;

namespace CommentR.Admin
{
    public class AdminModule : NancyModule
    {
        private static string connectionString;

        static AdminModule()
        {
            connectionString = ConfigurationManager.ConnectionStrings["CommentR"].ConnectionString;
        }

        public AdminModule()
        {
            Get["/comments/all"] = _ =>
            {
                return CreateCommentsModel(this.Context);
            };

            Get["/comment/{commentID:int}/reply"] = _ =>
            {
                using (var s = new SqlConnection(connectionString))
                {
                    s.Open();
                    return s
                    .Query<CommentReplyModel>(
                        "SELECT * FROM dbo.Comment WHERE IsHidden = 0 AND CommentID = @CommentID;"
                        , new { CommentID = (int)_.commentID })
                        .SingleOrDefault();
                }
            };

            Post["/comment/{commentID:int}/reply"] = _ =>
            {
                var commentID = (int)_.commentID;
                var permalink = (string)this.Context.Request.Form.Permalink;
                var author = (string)this.Context.Request.Form.Author;
                var body = (string)this.Context.Request.Form.Body;
                var isMod = (bool)this.Context.Request.Form.IsMod;

                body = Util.SanitizeBody(body);

                var comment = new CommentModel()
                {
                    PagePermalink = permalink,
                    DateTimeUTC = DateTime.UtcNow,
                    Author = author,
                    Body = body,
                    IsHidden = false,
                    AuthorIsModerator = isMod,
                    ReplyTo = commentID,
                };

                Util.InsertComment(comment);

                return CreateCommentsModel(this.Context);
            };
        }

        private object CreateCommentsModel(NancyContext context)
        {
            List<CommentModel> comments = null;
            using (var s = new SqlConnection(connectionString))
            {
                s.Open();
                comments = s.Query<CommentModel>("SELECT * FROM dbo.Comment WHERE IsHidden = 0;").ToList();
            }

            return new CommentsModel
            {
                Permalink = "",
                Count = comments.Count,
                Comments = comments.OrderByDescending(x => x.PagePermalink).ThenBy(x => x.DateTimeUTC).ToArray(),
            };
        }
    }
}