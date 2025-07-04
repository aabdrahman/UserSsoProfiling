--CREATE DATABASE

CREATE DATABASE [SsoProfiling]
GO

ALTER DATABASE [SsoProfiling] SET COMPATIBILITY_LEVEL = 150
GO
ALTER DATABASE [SsoProfiling] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SsoProfiling] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SsoProfiling] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SsoProfiling] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SsoProfiling] SET ARITHABORT OFF 
GO
ALTER DATABASE [SsoProfiling] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SsoProfiling] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SsoProfiling] SET AUTO_CREATE_STATISTICS OFF
GO
ALTER DATABASE [SsoProfiling] SET AUTO_UPDATE_STATISTICS OFF 
GO
ALTER DATABASE [SsoProfiling] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SsoProfiling] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SsoProfiling] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SsoProfiling] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SsoProfiling] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SsoProfiling] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SsoProfiling] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SsoProfiling] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SsoProfiling] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SsoProfiling] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SsoProfiling] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SsoProfiling] SET  READ_WRITE 
GO
ALTER DATABASE [SsoProfiling] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SsoProfiling] SET  MULTI_USER 
GO
ALTER DATABASE [SsoProfiling] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SsoProfiling] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SsoProfiling] SET DELAYED_DURABILITY = DISABLED 
GO
USE [SsoProfiling]
GO
IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') ALTER DATABASE [SsoProfiling] MODIFY FILEGROUP [PRIMARY] DEFAULT
GO

--CREATE [dbo].[Module]
USE [SsoProfiling]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Module](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Resource] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[NormalizedName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [Index_Module_Fetch] ON [dbo].[Module]
(
	[Name] ASC,
	[Resource] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--CREATE [dbo].[Resources]
USE [SsoProfiling]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Resources](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[NormalizedName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Resources] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [Fetch_Resource_Index] ON [dbo].[Resources]
(
	[NormalizedName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--CREATE [dbo].[SSOADM.SSO_MODULE_ACCESS_TBL]
USE [SsoProfiling]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SSOADM.SSO_MODULE_ACCESS_TBL](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[USER_ID] [nvarchar](50) NOT NULL,
	[ENTITY_ID] [nvarchar](5) NOT NULL,
	[RES_ID] [nvarchar](50) NOT NULL,
	[MODULE_ID] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SSOADM.SSO_MODULE_ACCESS_TBL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [Index_SSOADM.USERID] ON [dbo].[SSOADM.SSO_MODULE_ACCESS_TBL]
(
	[USER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--CREATE [dbo].[SSOADM.SSO_RESOURCE_ACCESS_TBL]
USE [SsoProfiling]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SSOADM.SSO_RESOURCE_ACCESS_TBL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[USER_ID] [nvarchar](50) NOT NULL,
	[ENTITY_ID] [nvarchar](5) NOT NULL,
	[RES_ID] [nvarchar](50) NOT NULL,
	[IS_DEFAULT] [nvarchar](1) NOT NULL,
 CONSTRAINT [PK_SSOADM.SSO_RESOURCE_ACCESS_TBL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [Index_SSOADM.SSO_RESOURCE_ACCESS_TBL_USER_ID] ON [dbo].[SSOADM.SSO_RESOURCE_ACCESS_TBL]
(
	[USER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--CREATE [dbo].[SSOADM.USER_CREDS_TBL]
USE [SsoProfiling]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SSOADM.USER_CREDS_TBL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[USER_ID] [nvarchar](50) NOT NULL,
	[USER_PW] [nvarchar](max) NOT NULL,
	[NUM_PWD_HISTORY] [bigint] NOT NULL,
	[PWD_HISTORY] [text] NOT NULL,
	[PWD_LAST_MOD_TIME] [bigint] NOT NULL,
	[NUM_PWD_ATTEMPTS] [int] NOT NULL,
	[NEW_USER_FLG] [nvarchar](1) NOT NULL,
	[LOGIN_TIME_LOW] [datetime2](7) NOT NULL,
	[LOGIN_TIME_HIGH] [datetime2](7) NOT NULL,
	[DISABLED_FROM_DATE] [datetime2](7) NOT NULL,
	[DISABLED_UPTO_DATE] [datetime2](7) NOT NULL,
	[PW_EXPY_DATE] [datetime2](7) NOT NULL,
	[ACCT_EXPY_DATE] [datetime2](7) NOT NULL,
	[ACCT_INACTIVE_DAYS] [int] NOT NULL,
	[LAST_ACCESS_TIME] [bigint] NOT NULL,
	[TS_CNT] [int] NOT NULL,
 CONSTRAINT [PK_SSOADM.USER_CREDS_TBL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [Index_SSOADM.USER_CREDS_TBL_UserID] ON [dbo].[SSOADM.USER_CREDS_TBL]
(
	[USER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--CREATE [dbo].[SSOADM.USER_PROFILE_TBL]
USE [SsoProfiling]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SSOADM.USER_PROFILE_TBL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[USER_ID] [nvarchar](50) NOT NULL,
	[USER_NAME] [nvarchar](50) NOT NULL,
	[DEL_FLG] [nvarchar](5) NOT NULL,
	[HOME_ENTITY] [nvarchar](10) NOT NULL,
	[DEFAULT_TZ] [nvarchar](10) NOT NULL,
	[DEFAULT_CALENDAR] [nvarchar](10) NOT NULL,
	[USER_MAX_INACTIVE_TIME] [int] NOT NULL,
	[REQ_TWO_FACTOR_AUTH] [nvarchar](5) NOT NULL,
	[IS_GLOBAL_ADMIN] [nvarchar](5) NOT NULL,
	[TS_CNT] [int] NOT NULL,
 CONSTRAINT [PK_SSOADM.USER_PROFILE_TBL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [Index_SSOADM.USER_PROFILE_TBL_UserId_1] ON [dbo].[SSOADM.USER_PROFILE_TBL]
(
	[USER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [Index_SSOADM.USER_PROFILE_TBL_UserName_1] ON [dbo].[SSOADM.USER_PROFILE_TBL]
(
	[USER_NAME] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

