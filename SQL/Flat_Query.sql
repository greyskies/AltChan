select
	v.[Title] as VideoTitle,
	vu.UrlId as VideoId,
	vu.Url as VideoUrl, 
	p.Name as Platform,
	vu.Published,
	vu.Views,
	vu.ThumbnailUrl,
	vu.Description as VideoDescription,
	c.Title as ChannelTitle,
	cu.UrlId as ChannelId,
	cu.Url as ChannelUrl,
	cu.Published as ChannelPublished,
	cu.Preference as ChannelPreference,
	cu.Description as ChannelDescription,
	c.Title as Author
from Video v
	inner join VideoUrl vu
		on v.Id = vu.VideoId
	inner join Channel c
		on c.id = v.channelId
	inner join channelUrl cu
		on cu.id = v.ChannelId
	inner join Platform p
		on p.Id = cu.platformId
