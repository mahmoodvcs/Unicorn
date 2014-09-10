
/****** Object:  StoredProcedure [dbo].[aspnet_Authorization_AddActionForRole]    Script Date: 2014/04/05 10:11:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



create PROCEDURE [dbo].[aspnet_Authorization_AddActionForRole]
	@RoleName         nvarchar(256),
	@ActionText       nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RoleId uniqueidentifier
    SELECT  @RoleId = NULL

    SELECT  @RoleId = RoleId
    FROM    dbo.Roles
    WHERE   RoleName = @RoleName
    
    Insert into dbo.aspnet_UserRoleActions ( UserRoleId,ActionName) values (@RoleId,@ActionText)
END


GO

/****** Object:  StoredProcedure [dbo].[aspnet_Authorization_AddActionForUser]    Script Date: 2014/04/05 10:11:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




create PROCEDURE [dbo].[aspnet_Authorization_AddActionForUser]
	@UserName         nvarchar(256),
	@ActionText       nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.Users
    WHERE   UserName = @UserName
    
    Insert into dbo.aspnet_UserRoleActions ( UserRoleId,ActionName) values (@UserId,@ActionText)
END


GO

/****** Object:  StoredProcedure [dbo].[aspnet_Authorization_ClearRoleActions]    Script Date: 2014/04/05 10:11:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




create PROCEDURE [dbo].[aspnet_Authorization_ClearRoleActions]
	@RoleName         nvarchar(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RoleId uniqueidentifier
    SELECT  @RoleId = NULL

    SELECT  @RoleId = RoleId
    FROM    dbo.Roles
    WHERE   RoleName = @RoleName
    
    delete dbo.aspnet_UserRoleActions where UserRoleId=@RoleId
END


GO

/****** Object:  StoredProcedure [dbo].[aspnet_Authorization_ClearUserActions]    Script Date: 2014/04/05 10:11:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



create PROCEDURE [dbo].[aspnet_Authorization_ClearUserActions]
	@UserName         nvarchar(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.Users
    WHERE   UserName = @UserName
    
    delete dbo.aspnet_UserRoleActions where UserRoleId=@UserId
END


GO

/****** Object:  StoredProcedure [dbo].[aspnet_Authorization_GetRoleActions]    Script Date: 2014/04/05 10:11:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



create PROCEDURE [dbo].[aspnet_Authorization_GetRoleActions]
	@RoleName         nvarchar(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RoleId uniqueidentifier
    SELECT  @RoleId = NULL

    SELECT  @RoleId = RoleId
    FROM    dbo.Roles
    WHERE   RoleName = @RoleName
    
    SELECT ActionName
    FROM   dbo.aspnet_UserRoleActions WHERE UserRoleId=@RoleId
END


GO

/****** Object:  StoredProcedure [dbo].[aspnet_Authorization_GetUserActions]    Script Date: 2014/04/05 10:11:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[aspnet_Authorization_GetUserActions]
	@UserName         nvarchar(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.Users
    WHERE   UserName = @UserName
    
    SELECT ActionName
    FROM   dbo.aspnet_UserRoleActions WHERE UserRoleId=@UserId
END


GO


