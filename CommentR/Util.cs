using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using HtmlAgilityPack;

namespace CommentR
{
    public static class Util
    {
        private static MarkdownSharp.Markdown markdown;
        private static string connectionString;

        static Util()
        {
            connectionString = ConfigurationManager.ConnectionStrings["CommentR"].ConnectionString;
            markdown = new MarkdownSharp.Markdown();
        }

        public static string SanitizeBody(string body)
        {
            var html = new HtmlDocument();
            html.LoadHtml(body);
            SanitizeNode(html.DocumentNode);
            return html.DocumentNode.WriteTo();
        }

        public static string TransformMarkdown(string body)
        {
            return markdown.Transform(body);
        }

        private static void SanitizeNode(HtmlNode node)
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

        public static void InsertComment(object comment)
        {
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
,[AuthorIsModerator]
,[ReplyTo])
VALUES
(@PagePermalink
,@DateTimeUTC
,@Author
,@Body
,@IsHidden
,@AuthorIsModerator
,@ReplyTo);
";
                s.Execute(insertSQL, comment);
            }
        }

        public static void OrderComments(List<Comments.CommentModel> comments)
        {
            var copy = new List<Comments.CommentModel>(comments.OrderByDescending(x => x.PagePermalink).ThenBy(x => x.DateTimeUTC));
            var newList = new List<Comments.CommentModel>(copy.Count);
            while (copy.Count > 0)
            {
                var comment = copy[0];
                copy.RemoveAt(0);
                newList.Add(comment);
                // find replies
                var replies = copy.Where(x => x.ReplyTo == comment.CommentID).ToList();
                foreach (var reply in replies)
                {
                    copy.Remove(reply);
                    newList.Add(reply);
                }
            }
            comments.Clear();
            comments.AddRange(newList);
        }
    }
}