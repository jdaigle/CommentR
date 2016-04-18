namespace CommentR.Comments
{
    public class CommentsModel
    {
        public int Count { get; set; }
        public CommentModel[] Comments { get; set; }
        public string Permalink { get; set; }
    }
}