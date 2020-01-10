USE [greyskies_AltChans]
GO

/****** Object:  StoredProcedure [dbo].[GetPreferredUrls]    Script Date: 10/01/2020 15:22:47 ******/
DROP PROCEDURE [dbo].[GetPreferredUrls]
GO

/****** Object:  StoredProcedure [dbo].[GetPreferredUrls]    Script Date: 10/01/2020 15:22:47 ******/
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

END
GO


