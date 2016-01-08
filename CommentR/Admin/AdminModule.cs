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
        private static MarkdownSharp.Markdown markdown;
        private static string connectionString;

        static AdminModule()
        {
            connectionString = ConfigurationManager.ConnectionStrings["CommentR"].ConnectionString;
            markdown = new MarkdownSharp.Markdown();
        }

        public AdminModule()
        {
            Get["/comments/all"] = _ =>
            {
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