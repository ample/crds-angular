Use [MinistryPlatform]
GO

UPDATE Events SET Parent_Event_ID = null WHERE Event_Title = '(t) 1Time Mason ChildCare';

DELETE FROM Event_Groups WHERE Event_ID IN (SELECT Group_ID FROM Groups WHERE Group_Name = '(t) 1Time Mason Group with ChildCare');

DELETE FROM Events WHERE Event_Title = '(t) 1Time Mason with ChildCare';

DELETE FROM Events WHERE Event_Title = '(t) 1Time Mason ChildCare';

DELETE FROM Groups WHERE Group_Name = '(t) 1Time Mason Group with ChildCare';