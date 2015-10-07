USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '[dbo].[cr_Documents]'))
BEGIN
	
	CREATE TABLE [dbo].[cr_Documents](
		[Document_ID] [int] IDENTITY(1,1) NOT NULL,
		[Document] [nvarchar](100) NOT NULL,
		[Description] [nvarchar](255) NULL,
	 CONSTRAINT [PK_cr_Document] PRIMARY KEY CLUSTERED
	(
		[Document_ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END