USE [ASKON-TestTaskDb]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE SelectDetailsForReport @RootDetailId int, @RootHierarchyId hierarchyid
AS
SELECT [d].[Name] AS 'DetailName', [dr].[Count] FROM [ASKON-TestTaskDb].[dbo].[Details] AS d
JOIN [dbo].[DetailRelations] AS dr ON d.DetailId = dr.DetailId
WHERE d.DetailId = @RootDetailId AND dr.HierarchyLevel = @RootHierarchyId
