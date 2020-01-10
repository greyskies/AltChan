
GO
/****** Object:  Table [dbo].[Channel]    Script Date: 09/01/2020 14:36:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Channel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](255) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChannelUrl]    Script Date: 09/01/2020 14:36:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChannelUrl](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlatformId] [int] NOT NULL,
	[UrlId] [varchar](255) NOT NULL,
	[Url] [varchar](255) NOT NULL,
	[ChannelId] [int] NOT NULL,
	[Published] [datetime] NOT NULL,
	[Preference] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Platform]    Script Date: 09/01/2020 14:36:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Platform](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Video]    Script Date: 09/01/2020 14:36:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Video](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ChannelId] [varchar](255) NOT NULL,
	[Title] [varchar](255) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VideoUrl]    Script Date: 09/01/2020 14:36:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VideoUrl](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VideoId] [int] NOT NULL,
	[UrlId] [varchar](255) NOT NULL,
	[Url] [varchar](255) NOT NULL,
	[PlatformId] [int] NOT NULL,
	[Published] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[Views] [int] NOT NULL,
	[ThumbnailUrl] [varchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[GetPreferredUrls]    Script Date: 09/01/2020 14:36:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPreferredUrls] 
(
	@Url varchar(255)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

declare @title varchar(255)
set @title = (select title from video v inner join videoUrl u on v.id = u.VideoId where u.Url = @Url)

select vu.Url, cu.preference
from videoUrl vu
	inner join video v
		on v.Id = vu.videoId
	inner join Channel c
		on c.Id = v.ChannelId
	inner join ChannelUrl cu
		on cu.ChannelId = c.Id
		and cu.PlatformId = vu.PlatformId
where 
v.title = @title
order by cu.Preference

END
GO
/****** Object:  StoredProcedure [dbo].[InsertChannel]    Script Date: 09/01/2020 14:36:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[InsertChannel] 
(
	@Platform varchar(255),
	@Url varchar(255),
	@UrlId varchar(255),
	@Title varchar(255),
	@published dateTime
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

if not exists(select * from [Platform] where Name = @Platform)
	insert into [Platform] ([Name]) values (@Platform)
declare @platformId int
select @platformId = Id from [Platform] where Name = @Platform

if not exists(select * from [Channel] where Title = @Title)
	insert into [Channel] ([Title]) values (@Title)
declare @channelId int
select @channelId = Id from [Channel] where Title = @Title

if not exists(select * from [ChannelUrl] where platformId = @platformId and channelId = @channelId)
	insert into [ChannelUrl] (PlatformId, UrlId, Url, ChannelId, Published, Preference) values (@platformId, @UrlId, @Url, @channelId, @Published, 1)

select @channelId

END
GO
/****** Object:  StoredProcedure [dbo].[InsertVideo]    Script Date: 09/01/2020 14:36:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[InsertVideo] 
(
	@channelId int,
	@Title varchar(255),
	@Platform varchar(255),
	@Url varchar(255),
	@UrlId varchar(255),
	@published dateTime,
	@updated dateTime,
	@views int,
	@thumbnailUrl varchar(255)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

if not exists(select * from [Platform] where Name = @Platform)
	insert into [Platform] ([Name]) values (@Platform)
declare @platformId int
select @platformId = Id from [Platform] where Name = @Platform

if not exists(select * from [Video] where Title = @Title)
	insert into [Video] ([Title],[channelId]) values (@Title, @channelId)
declare @videoId int
select @videoId = Id from [Video] where Title = @Title

if not exists(select * from [VideoUrl] where platformId = @platformId and videoId = @videoId)
	insert into [VideoUrl] (VideoId, UrlId, Url, PlatformId, Published, Updated, [Views], ThumbnailUrl) 
	values (@VideoId, @UrlId, @Url, @PlatformId, @Published, @Updated, @Views, @thumbnailUrl)

END
GO
