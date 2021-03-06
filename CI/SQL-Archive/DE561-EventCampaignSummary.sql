USE [MinistryPlatform]
GO
/****** Object:  StoredProcedure [dbo].[report_CRDS_Event_Selected_Campaign_Summary]    Script Date: 10/27/2015 3:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[report_CRDS_Event_Selected_Campaign_Summary]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[report_CRDS_Event_Selected_Campaign_Summary] AS' 
END
GO

ALTER PROCEDURE [dbo].[report_CRDS_Event_Selected_Campaign_Summary]

	@DomainID varchar(40)
	,@UserID varchar(40)
	,@PageID Int
	,@SelectionID Int
	,@EventID INT
	,@PledgeStatusId Int

AS
BEGIN

DECLARE @C Table (Pledge_Campaign_ID INT)
INSERT INTO @C (Pledge_Campaign_ID)
SELECT Pledge_Campaign_ID
FROM Pledge_Campaigns 
WHERE Event_ID = @EventID

DECLARE @EventDonations Money, @EventDonationsNoPledge Money, @PledgeDonations Money
SELECT  @EventDonations = ISNULL(SUM(Amount),0), @EventDonationsNoPledge = SUM(CASE WHEN Pledge_ID IS NULL THEN Amount ELSE 0 END) FROM Donation_Distributions DD WHERE DD.Target_Event = @EventID
SELECT @PledgeDonations = SUM(DD.Amount) FROM Donation_Distributions DD INNER JOIN Pledges PL ON PL.Pledge_ID = DD.Pledge_ID WHERE PL.Pledge_Campaign_ID IN (SELECT Pledge_Campaign_ID FROM @C)
--SELECT @EventDonations, @EventDonationsNoPledge, @PledgeDonations

SELECT E.Event_Title
, E.Event_Start_Date
, E.Event_End_Date
, E.[Description] AS Event_Description
, @EventDonations AS Event_Donations
, @EventDonationsNoPledge AS Event_Donations_No_Pledge
, SUM(CASE WHEN PL.Pledge_Status_ID = @PledgeStatusId THEN Total_Pledge END) AS Sum_of_Pledges
, Count(PL.Pledge_ID) AS Count_of_Pledges
, @PledgeDonations AS Donation_to_Pledge
, PC.Campaign_Name
, PC.Start_Date
, PC.End_Date
, PC.Campaign_Goal
FROM Events E
 INNER JOIN dp_Domains Dom ON Dom.Domain_ID = E.Domain_ID
 LEFT OUTER JOIN Pledge_Campaigns PC ON PC.Event_ID = E.Event_ID
 LEFT OUTER JOIN Pledges PL ON PL.Pledge_Campaign_ID = PC.Pledge_Campaign_ID

WHERE E.Event_ID = @EventID

GROUP BY Event_Title
, E.[Description]
, Event_Start_Date
, Event_End_Date
, PC.Campaign_Name
, PC.Start_Date
, PC.End_Date
, PC.Campaign_Goal
END