using System;

namespace CommentR.Comments
{
    public class CommentModel
    {
        public long CommentID { get; set; }
        public string PagePermalink { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public string Author { get; set; }
        public string Body { get; set; }
        public bool IsHidden { get; set; }
        public bool AuthorIsModerator { get; set; }
        public int? ReplyTo { get; set; }

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
                return Util.TransformMarkdown(Body);
            }
        }

        public string ModClass
        {
            get
            {
                return AuthorIsModerator ? "comment-mod" : "";
            }
        }

        public string ReplyClass
        {
            get
            {
                return AuthorIsModerator ? "comment-reply" : "";
            }
        }

        public bool ShowReplyButton
        {
            get
            {
                return !AuthorIsModerator;
            }
        }
    }
}