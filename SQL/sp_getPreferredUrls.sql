USE [AltChan]
GO

/****** Object:  StoredProcedure [dbo].[GetPreferredUrls]    Script Date: 14/01/2020 16:53:16 ******/
DROP PROCEDURE [dbo].[GetPreferredUrls]
GO

/****** Object:  StoredProcedure [dbo].[GetPreferredUrls]    Script Date: 14/01/2020 16:53:16 ******/
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

if @title is null
begin
	if not exists(select * from VideoUrlStaging where url = @Url)
	begin
		insert into [VideoUrlStaging] (Url) values (@Url)
	end
	select @Url, 'Unknown', 'Unknown'
end
else
begin
	select vu.Url, c.Title as Channel, p.[Name] as Platform
	from videoUrl vu
		inner join video v
			on v.Id = vu.videoId
		inner join Channel c
			on c.Id = v.ChannelId
		inner join ChannelUrl cu
			on cu.ChannelId = c.Id
			and cu.PlatformId = vu.PlatformId
		inner join [Platform] p
			on p.id = vu.platformId
	where 
	v.title = @title
	order by cu.Preference
end

END
GO


