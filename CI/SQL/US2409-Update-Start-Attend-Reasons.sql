USE [MinistryPlatform]
GO

-- Add the Attribute Type
IF NOT EXISTS (SELECT * FROM Attribute_Types WHERE Attribute_Type='StartAttendReason')
BEGIN
        SET IDENTITY_INSERT [dbo].[Attribute_Types] ON
	 
	INSERT INTO [dbo].[Attribute_Types] ([Attribute_Type], [Domain_ID], [Prevent_Multiple_Selection])
	VALUES (59, 'StartAttendReason', '1', 'True')
	
	SET IDENTITY_INSERT [dbo].[Attribute_Types] ON 
END
GO

DECLARE @Attribute_Type_ID int
SELECT @Attribute_Type_ID = (SELECT TOP 1 Attribute_Type_ID FROM Attribute_Types WHERE Attribute_Type='StartAttendReason')

-- Add the Attributes
IF NOT EXISTS (SELECT * FROM Attributes WHERE Attribute_Name='A friend invited me to the weekend')
BEGIN
INSERT INTO [dbo].[Attributes] ([Attribute_Name], [Attribute_Type_ID], [Domain_ID])
VALUES ('A friend invited me to the weekend', @Attribute_TypeID, '1')
END

IF NOT EXISTS (SELECT * FROM Attributes WHERE Attribute_Name='Nobody invited me – I just came to a weekend')
BEGIN
INSERT INTO [dbo].[Attributes] ([Attribute_Name], [Attribute_Type_ID], [Domain_ID])
VALUES ('Nobody invited me – I just came to a weekend', @Attribute_TypeID, '1')
END

IF NOT EXISTS (SELECT * FROM Attributes WHERE Attribute_Name='I attended Awaited')
BEGIN
INSERT INTO [dbo].[Attributes] ([Attribute_Name], [Attribute_Type_ID], [Domain_ID])
VALUES ('I attended Awaited', @Attribute_TypeID, '1')
END

IF NOT EXISTS (SELECT * FROM Attributes WHERE Attribute_Name='I served in GO Cincinnati')
BEGIN
INSERT INTO [dbo].[Attributes] ([Attribute_Name], [Attribute_Type_ID], [Domain_ID])
VALUES ('I served in GO Cincinnati', @Attribute_TypeID, '1')
END

IF NOT EXISTS (SELECT * FROM Attributes WHERE Attribute_Name='I went on a GO Trip')
BEGIN
INSERT INTO [dbo].[Attributes] ([Attribute_Name], [Attribute_Type_ID], [Domain_ID])
VALUES ('I went on a GO Trip', @Attribute_TypeID, '1')
END

IF NOT EXISTS (SELECT * FROM Attributes WHERE Attribute_Name='I served with some other ministry')
BEGIN
INSERT INTO [dbo].[Attributes] ([Attribute_Name], [Attribute_Type_ID], [Domain_ID])
VALUES ('I served with some other ministry', @Attribute_TypeID, '1')
END

IF NOT EXISTS (SELECT * FROM Attributes WHERE Attribute_Name='My reason isn''t listed')
BEGIN
INSERT INTO [dbo].[Attributes] ([Attribute_Name], [Attribute_Type_ID], [Domain_ID])
VALUES ('My reason isn''t listed', @Attribute_TypeID, '1')
END

IF NOT EXISTS (SELECT * FROM Attributes WHERE Attribute_Name='I don''t remember')
BEGIN
INSERT INTO [dbo].[Attributes] ([Attribute_Name], [Attribute_Type_ID], [Domain_ID])
VALUES ('I don''t remember', @Attribute_TypeID, '1')
END
GO
