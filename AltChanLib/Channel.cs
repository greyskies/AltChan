using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltChanLib.DataClasses;
using JsonVideoList = AltChanLib.DataClasses.JsonVideoList;
using JsonVideoDetails = AltChanLib.DataClasses.JsonVideoDetails;
using JsonChannel = AltChanLib.DataClasses.JsonChannel;

namespace AltChanLib
{
    public class Channel
    {
        const string ChannelPrefix = "https://www.youtube.com/feeds/videos.xml?channel_id=";

        public string Title { get; set; }
        public string Platform { get; set; }
        public string UrlId { get; set; }
        public string Url { get; set; }
        public DateTime Published { get; set; }
        public string Description { get; set; }
        public List<Video> Videos { get; set; }

        public Channel()
        {
            Videos = new List<Video>();
        }

        public Channel(feed feed, string platform)
        {
            Platform = platform;
            Url = feed.feedlink[0].href;
            UrlId = feed.feedchannelId.Value;
            Title = feed.feedtitle.Value;
            if (DateTime.TryParse(feed.feedpublished.Value, out DateTime date))
            {
                Published = date;
            }
            Description = feed.feedtitle.Value;
            Videos = feed.entry.Select(x => new Video(x, platform)).ToList();
        }

        public Channel(JsonChannel.RootObject channelRoot, string platform)
        {
            Platform = platform;
            if (channelRoot.items.Count > 0)
            {
                Url = $"{ChannelPrefix}{channelRoot.items[0].id}";
                UrlId = channelRoot.items[0].id;
                Title = channelRoot.items[0].snippet.title;
                Published = channelRoot.items[0].snippet.publishedAt;
                Description = channelRoot.items[0].snippet.description;
                Videos = new List<Video>();
            }
        }

        public void AddVideos(List<JsonVideoDetails.Item> extraVideos)
        {
            if (extraVideos != null)
            {
                foreach (var video in extraVideos) //add extra videos from the API call
                {
                    Videos.Add(new Video(video, Platform));
                }
            }
        }
    }
}
