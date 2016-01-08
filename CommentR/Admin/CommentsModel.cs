using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommentR.Admin
{
    public class CommentsModel
    {
        public int Count { get; set; }
        public CommentModel[] Comments { get; set; }
        public string Permalink { get; set; }
    }

    public class CommentModel
    {
        private static MarkdownSharp.Markdown markdown;

        static CommentModel()
        {
            markdown = new MarkdownSharp.Markdown();
        }

        public long CommentID { get; set; }
        public string PagePermalink { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public string Author { get; set; }
        public string Body { get; set; }
        public bool IsHidden { get; set; }
        public bool AuthorIsModerator { get; set; }

        public string DateTimeUTCISOString
        {
            get
            {
                return DateTimeUTC.ToString("o");
            }
        }

        public string MarkdownProcessedBody
        {
            get
            {
                return markdown.Transform(Body);
            }
        }
    }
}