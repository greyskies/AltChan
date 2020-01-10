USE [AltChan]
GO

/****** Object:  StoredProcedure [dbo].[InsertVideo]    Script Date: 07/01/2020 16:59:18 ******/
DROP PROCEDURE [dbo].[InsertVideo]
GO

/****** Object:  StoredProcedure [dbo].[InsertVideo]    Script Date: 07/01/2020 16:59:18 ******/
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


