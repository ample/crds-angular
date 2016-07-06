USE [MinistryPlatform]
GO

/****** Object:  Table [dbo].[cr_Categories]    Script Date: 7/6/2016 08:48:47 AM ******/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF NOT EXISTS (select * from sys.objects where Object_ID = Object_ID(N'dbo.cr_Category_Details') and Type = N'U')
BEGIN
CREATE TABLE dbo.cr_Category_Details
	(
	Category_Detail_ID int NOT NULL IDENTITY (1, 1),
	Category_ID int NOT NULL,
	Category_Detail nvarchar(100) NOT NULL,
	Description nvarchar(100) NULL,
	Domain_ID int NOT NULL
	)  ON [PRIMARY]

ALTER TABLE dbo.cr_Category_Details ADD CONSTRAINT
	DF_cr_Category_Details_Domain_ID DEFAULT (1) FOR Domain_ID

ALTER TABLE dbo.cr_Category_Details ADD CONSTRAINT
	PK_cr_Category_Details PRIMARY KEY CLUSTERED 
	(
	Category_Detail_ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


ALTER TABLE dbo.cr_Category_Details ADD CONSTRAINT
	FK_cr_Category_Detail_cr_Categories FOREIGN KEY
	(
	Category_ID
	) REFERENCES dbo.cr_Categories
	(
	Category_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 
ALTER TABLE [dbo].[cr_Category_Details]  WITH CHECK ADD  CONSTRAINT [FK_cr_Category_Details_Dp_Domains] FOREIGN KEY([Domain_ID])
REFERENCES [dbo].[dp_Domains] ([Domain_ID])


ALTER TABLE [dbo].[cr_Category_Details] CHECK CONSTRAINT [FK_cr_Category_Details_Dp_Domains]
	

ALTER TABLE dbo.cr_Category_Details SET (LOCK_ESCALATION = TABLE)
END
