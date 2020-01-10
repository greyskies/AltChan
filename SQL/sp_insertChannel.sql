USE [AltChan]
GO

/****** Object:  StoredProcedure [dbo].[InsertChannel]    Script Date: 07/01/2020 16:58:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[InsertChannel] 
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


