USE [MinistryPlatform]
GO	

ALTER TABLE [dbo].[Recurring_Gifts] add Consecutive_Failure_Count INT NOT NULL  DEFAULT ((0))
GO