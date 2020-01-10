declare @title varchar(255)
set @title = (select title from video v inner join videoUrl u on v.id = u.VideoId where u.Url = 'https://www.youtube.com/watch?v=VFyDD4KSLVY')

select vu.Url
from videoUrl vu
	inner join video v
		on v.Id = vu.videoId
	inner join Channel c
		on c.Id = v.ChannelId
	inner join ChannelUrl cu
		on cu.ChannelId = c.Id
where 
v.title = @title
order by cu.Preference desc

--select * 
--from videoUrl vu 
--	inner join video v on v.id = vu.videoId
--where [platformId] = 2

--select * from [dbo].[Platform]
--select * from [dbo].[ChannelUrl]
--select * from [dbo].[Channel] order by title
--select * from [dbo].[Video] order by title
--select * from [dbo].[VideoUrl]



