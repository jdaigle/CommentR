IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.Comment'))
CREATE TABLE dbo.Comment
(
    CommentID BIGINT NOT NULL IDENTITY (1,1),
    PagePermalink NVARCHAR(200) NOT NULL,
    DateTimeUTC DATETIME2 NOT NULL,
    Author NVARCHAR(200) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    IsHidden BIT NOT NULL,
    AuthorIsModerator BIT NOT NULL,
    CONSTRAINT PK_Comment PRIMARY KEY CLUSTERED (CommentID ASC),
)