using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltChanLib.DataClasses;
using JsonVideoDetails = AltChanLib.DataClasses.JsonVideoDetails;

namespace AltChanLib
{
    public class Video
    {
        const string VideoPrefix = "https://www.youtube.com/watch?v=";

        public string Title { get; set; }
        public string UrlId { get; set; }
        public string Url { get; set; }
        public string Platform { get; set; }
        public DateTime Published { get; set; }
        public int Views { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }

        public Video()
        {
            
        }

        public Video(entry entry, string platform)
        {
            Title = entry.entrytitle.Value;
            Platform = platform;
            Url = entry.entrylink[0].href;
            UrlId = entry.videoId.Value;
            if (DateTime.TryParse(entry.entrypublished.Value, out DateTime date))
            {
                Published = date;
            }
            Published = date;
            //Updated = DateTime.Parse(entry.updated.Value);
            Views = entry.@group.community.statistics.views;
            ThumbnailUrl = entry.@group.thumbnail.url;
            Description = entry.@group.description.Value;
        }

        public Video(JsonVideoDetails.Item video, string platform)
        {
            Title = video.snippet.title;
            Platform = platform;
            Url = $"{VideoPrefix}{video.id}";
            UrlId = video.id;
            Published = video.snippet.publishedAt;
            ThumbnailUrl = video.snippet.thumbnails.@default.url;
            Description = video.snippet.description;
        }
    }
}
