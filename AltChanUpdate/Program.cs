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
using Newtonsoft.Json;

namespace AltChanUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "sub" && !string.IsNullOrWhiteSpace(args[1]))
            {
                var subscriptionPath = args[1]; // @"c:\Projects\AltChanUpdate\subscription_manager.xml";
                ImportSubscription(subscriptionPath);
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
                    var linkPrefix = "https://www.youtube.com/watch?v=";
                    var channelPrefix = "https://www.youtube.com/feeds/videos.xml?channel_id=";
                    var videoId = url.Replace(linkPrefix, "");
                    var apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=id%2C+snippet&id=" + videoId + "&key=AIzaSyBfxp-tRmtzVjIKDtR8FLRnll6Jv9sSUzY";
                    var videoJson = GetResponse(apiUrl);
                    if (videoJson != null)
                    {
                        var jObject = JsonConvert.DeserializeObject<RootObject>(videoJson);
                        var channelId = jObject.items[0].snippet.channelId;
                        var channelUrl = $"{channelPrefix}{channelId}";
                        ImportChannelData(channelUrl, dataSource);
                    }
                }
            }
            dataSource.ClearStagedVideoUrls();
        }

        private static void ImportSubscription(string subscriptionPath)
        {
            var dataSource = new DataSource();

            var subscriptionText = File.ReadAllText(subscriptionPath);
            subscriptionText =
                subscriptionText.Replace("<opml version=\"1.1\">", "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            subscriptionText = subscriptionText.Replace("</opml>", "");

            var subscription = ExtractData<body>(subscriptionText);

            foreach (var channelEntry in subscription.bodyoutline[0].outlineoutline)
            {
                var channelUrl = channelEntry.xmlUrl;
                ImportChannelData(channelUrl, dataSource);
            }
        }

        private static void ImportChannelData(string channelUrl, DataSource dataSource)
        {
            var channelXml = GetResponse(channelUrl);
            if (channelXml != null)
            {
                //fix channel xml to allow deserialization
                channelXml = channelXml.Replace("&", "&#x26;");
                channelXml = channelXml.Replace("xmlns=\"http://www.w3.org/2005/Atom\"", "");
                channelXml = channelXml.Replace("<yt:", "<");
                channelXml = channelXml.Replace("</yt:", "</");
                channelXml = channelXml.Replace("<media:", "<");
                channelXml = channelXml.Replace("</media:", "</");

                var channel = ExtractData<feed>(channelXml);
                dataSource.SaveChannel(channel, "YouTube");

                //look for alternative platforms in the video descriptions
                foreach (var video in channel.entry)
                {
                    var description = video.@group.description.Value;
                    if (description != null)
                    {
                        var linkPrefix = "https://www.bitchute.com/video/";
                        var pos = description.IndexOf(linkPrefix);
                        if (pos >= 0)
                        {
                            var url = description.Substring(pos, 45).Trim().TrimEnd('/');
                            channel.feedlink[0].href = ""; //clear channel Url
                            channel.feedchannelId.Value = ""; //clear channel UrlId

                            channel.entry = new List<entry>(new[] {video});
                            video.entrylink[0].href = url;
                            video.videoId.Value = url.Replace("linkPrefix", "");
                            video.@group.community.statistics.views = 0;
                            dataSource.SaveChannel(channel, "BitChute");
                        }
                    }
                }
            }
        }

        private static T ExtractData<T>(string xml)
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
