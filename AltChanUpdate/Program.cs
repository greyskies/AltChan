using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AltChanLib;
using AltChanLib.DataClasses;
using JsonVideoDetails = AltChanLib.DataClasses.JsonVideoDetails;
using JsonVideoList = AltChanLib.DataClasses.JsonVideoList;
using JsonVideo = AltChanLib.DataClasses.JsonVideo;
using JsonChannel = AltChanLib.DataClasses.JsonChannel;
using Newtonsoft.Json;

namespace AltChanUpdate
{
    class Program
    {
        const string apiKey = "&key=AIzaSyBfxp-tRmtzVjIKDtR8FLRnll6Jv9sSUzY";

        static void Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "sub" && !string.IsNullOrWhiteSpace(args[1]))
            {
                var subscriptionPath = args[1]; // @"c:\Projects\AltChanUpdate\subscription_manager.xml";
                ImportYouTubeSubscriptionFile(subscriptionPath);
            }
            else
            {
                ProcessStagedVideoUrls();
            }
        }

        private static void ProcessStagedVideoUrls()
        {
            var dataSource = new DataSource();
            var urls = dataSource.GetStagedVideoUrls();
            foreach (var url in urls)
            {
                if (url.ToLower().Contains("youtube"))  //can get details from the yt API
                {
                    var platform = "YouTube";
                    var linkPrefix = "https://www.youtube.com/watch?v=";
                    var channelPrefix = "https://www.youtube.com/feeds/videos.xml?channel_id=";
                    var videoId = url.Replace(linkPrefix, "");
                    var apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=id%2C+snippet&id={videoId}{apiKey}";
                    var videoJson = GetResponse(apiUrl);
                    if (videoJson != null)
                    {
                        var jObject = JsonConvert.DeserializeObject<JsonVideo.RootObject>(videoJson);
                        var channelId = jObject.items[0].snippet.channelId;
                        var channelUrl = $"{channelPrefix}{channelId}";
                        ImportYouTubeChannelData(channelId, platform, dataSource);
                        dataSource.ClearStagedVideoUrl(url);
                    }
                }
            }
            //dataSource.ClearStagedVideoUrls();
        }

        private static void ImportYouTubeSubscriptionFile(string subscriptionPath)
        {
            var dataSource = new DataSource();
            var platform = "YouTube";
            var channelUrlPrefix = "https://www.youtube.com/feeds/videos.xml?channel_id=";
            var subscriptionText = File.ReadAllText(subscriptionPath);
            subscriptionText =
                subscriptionText.Replace("<opml version=\"1.1\">", "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            subscriptionText = subscriptionText.Replace("</opml>", "");

            var subscription = ExtractXmlData<body>(subscriptionText);

            foreach (var channelEntry in subscription.bodyoutline[0].outlineoutline)
            {
                var channelUrl = channelEntry.xmlUrl;
                var channelId = channelUrl.Replace(channelUrlPrefix, "");
                ImportYouTubeChannelData(channelId, platform, dataSource);
            }
        }

        private static void ImportYouTubeChannelData(string channelId, string platform, DataSource dataSource, string stagedUrl = null)
        {
            const int dateThresholdMonthsBack = 1;

            //channel data via the API with quota check:
            //var channelUrlPrefix = "https://www.googleapis.com/youtube/v3/channels?part=id%2C+snippet&id=";
            //var channelUrl = $"{channelUrlPrefix}{channelId}{apiKey}";
            //var channelResponse = GetResponse(channelUrl);
            //var channelData = ExtractJsonData<JsonChannel.RootObject>(channelResponse);

            //rather get the channel details via the XML interface because this has no quota check and provides the 15 latest videos
            var channelUrlPrefix = "https://www.youtube.com/feeds/videos.xml?channel_id=";
            var channelUrl = $"{channelUrlPrefix}{channelId}";
            var channelResponse = GetResponse(channelUrl);
            //fix channel xml to allow deserialization
            channelResponse = channelResponse.Replace("&", "&#x26;");
            channelResponse = channelResponse.Replace("xmlns=\"http://www.w3.org/2005/Atom\"", "");
            channelResponse = channelResponse.Replace("<yt:", "<");
            channelResponse = channelResponse.Replace("</yt:", "</");
            channelResponse = channelResponse.Replace("<media:", "<");
            channelResponse = channelResponse.Replace("</media:", "</");
            var channelData = ExtractXmlData<feed>(channelResponse);
            var channel = new Channel(channelData, platform);

            //record videos already included in the channel query
            var videoIdsAlreadyFetched = channelData.entry.Select(x => x.videoId.Value).ToList();
            var videoIdsAlreadyInDatabase = dataSource.GetChannelVideoUrls(channelId);

            //get a list of video id's for the channel later than the specified date
            var threshold = DateTime.Today.AddMonths(-dateThresholdMonthsBack).ToString("yyyy-MM-ddTHH:mm:ssZ");
            var resultsPerPage = 50;
            var videoListUrlPrefix = $"https://www.googleapis.com/youtube/v3/search?part=id&channelId=";
            var channelVideosUrlParams = $"&maxResults={resultsPerPage}"; // &publishedAfter={threshold}";
            var videoListUrl = $"{videoListUrlPrefix}{channelId}{apiKey}{channelVideosUrlParams}&publishedAfter={threshold}";

            var videoListResponse = GetResponse(videoListUrl);
            if (videoListResponse != null)
            {
                var channelVideosUrlPrefix = "https://www.googleapis.com/youtube/v3/videos?part=id%2C+snippet&id=";
                var videoList = ExtractJsonData<JsonVideoList.RootObject>(videoListResponse);
                var videoIds = videoList.items.Select(x => x.id.videoId).Except(videoIdsAlreadyFetched).Except(videoIdsAlreadyInDatabase).ToList();

                //add the Id of the staged video if it is not on the list
                if (stagedUrl != null && !videoIds.Contains(stagedUrl))
                {
                    videoIds.Add(stagedUrl);
                }
                
                SaveChannelVideos(videoIds, channelVideosUrlPrefix, channel);

                //get any extra pages required
                while (videoList.nextPageToken != null && videoList.items.Count > 0)
                {
                    var extraPageUrl = $"{videoListUrl}&pageToken={videoList.nextPageToken}";
                    videoListResponse = GetResponse(extraPageUrl);
                    if (videoListResponse != null)
                    {
                        videoList = ExtractJsonData<JsonVideoList.RootObject>(videoListResponse);
                        var extraVideoIds = videoList.items.Select(x => x.id.videoId).Except(videoIdsAlreadyFetched).Except(videoIdsAlreadyInDatabase).ToList();
                        SaveChannelVideos(extraVideoIds, channelVideosUrlPrefix, channel);
                    }
                }

                dataSource.SaveChannel(channel);
            }
        }

        private static void SaveChannelVideos(List<string> videoIds, string channelVideosUrlPrefix, Channel channel)
        {
            var videosCsv = string.Join("%2C+", videoIds);
            var channelVideosUrl = $"{channelVideosUrlPrefix}{videosCsv}{apiKey}";
            var channelVideosResponse = GetResponse(channelVideosUrl);
            if (channelVideosResponse != null)
            {
                var channelVideos = ExtractJsonData<JsonVideoDetails.RootObject>(channelVideosResponse);
                channel.AddVideos(channelVideos?.items);
            }
        }

        private static T ExtractJsonData<T>(string channelHeaderResponse)
        {
            var channelRoot = JsonConvert.DeserializeObject<T>(channelHeaderResponse);
            return channelRoot;
        }

        private static T ExtractXmlData<T>(string xml)
        {
            var sr = new StringReader(xml);
            var ser = new XmlSerializer(typeof(T));
            var tmpFeed = (T) ser.Deserialize(sr);
            return tmpFeed;
        }

        private static string GetResponse(string requestUrl)
        {
            try
            {
                var request = WebRequest.Create(requestUrl);
                var response = request.GetResponse();
                var dataStream = response.GetResponseStream();
                if (dataStream != null)
                {
                    var reader = new StreamReader(dataStream);
                    var responseString = reader.ReadToEnd();
                    return responseString;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
    }
}
