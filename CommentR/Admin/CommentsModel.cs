using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommentR.Admin
{
    public class CommentsModel
    {
        public int Count { get; set; }
        public Comments.CommentModel[] Comments { get; set; }
        public string Permalink { get; set; }
    }
}