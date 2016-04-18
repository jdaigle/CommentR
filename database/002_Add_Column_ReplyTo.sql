IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'ReplyTo' AND object_id = OBJECT_ID('dbo.Comment'))
ALTER TABLE dbo.Comment
    ADD ReplyTo bigint NULL;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Comment_ReplyTo' AND parent_object_id = OBJECT_ID('dbo.Comment'))
ALTER TABLE dbo.Comment
    ADD CONSTRAINT FK_Comment_ReplyTo FOREIGN KEY (ReplyTo) 
        REFERENCES dbo.Comment (CommentID);