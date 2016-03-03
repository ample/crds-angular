USE [MinistryPlatform]
 GO
 
 SET ANSI_NULLS ON
 GO
 
 SET QUOTED_IDENTIFIER ON
 GO

 --DROP TABLE [dbo].[cr_Registrations]
 
 IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[cr_Registrations]') AND type in (N'U'))
 BEGIN
 CREATE TABLE [dbo].[cr_Registrations](
 	[Registration_ID] [int] IDENTITY(1,1) NOT NULL,
	[Organization_ID] [int] NOT NULL,
	[Location_ID] [int] NOT NULL,
 	[Participant_ID] [int] NOT NULL,
	[Initiative_ID] [int] NOT NULL,
	[Spouse_Participation] [bit] NOT NULL,
 	[Additional_Information] nvarchar(500) NULL,
 	[Role_ID] [int] NOT NULL, 
    [Domain_ID] [int] NOT NULL,
 	
 	CONSTRAINT [FK_Registrations_Initiative] FOREIGN KEY ([Initiative_ID]) REFERENCES [cr_Initiatives]([Initiative_ID]), 
 	CONSTRAINT [FK_Registrations_Location] FOREIGN KEY (Location_ID) REFERENCES Locations(Location_ID), 
 	CONSTRAINT [FK_Registrations_Organization] FOREIGN KEY (Organization_ID) REFERENCES cr_Organizations(Organization_ID), 
 	CONSTRAINT [FK_Registrations_Participant] FOREIGN KEY ([Participant_ID]) REFERENCES [Participants]([Participant_ID]),
	CONSTRAINT [FK_Registrations_Role] FOREIGN KEY ([Role_ID]) REFERENCES [Group_Roles]([Group_Role_ID]),
	CONSTRAINT [FK_Registrations_Domain] FOREIGN KEY ([Domain_ID]) REFERENCES dp_Domains(Domain_ID),
 	CONSTRAINT [PK_Registrations] PRIMARY KEY CLUSTERED([Registration_ID] ASC)
 )
END