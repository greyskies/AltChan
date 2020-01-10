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

namespace AltChanUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            var datasource = new DataSource();

            var subscriptionPath = @"c:\Projects\AltChanUpdate\subscription_manager.xml";
            var subscriptionText = File.ReadAllText(subscriptionPath);
            subscriptionText =
                subscriptionText.Replace("<opml version=\"1.1\">", "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            subscriptionText = subscriptionText.Replace("</opml>", "");

            var subscription = ExtractData<body>(subscriptionText);

            foreach (var channelEntry in subscription.bodyoutline[0].outlineoutline)
            {
                var channelUrl = channelEntry.xmlUrl;
                var channelXml = GetXml(channelUrl);
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
                    datasource.SaveChannel(channel, "youtube");

                    //look for alternative platforms in the video descriptions
                    foreach (var video in channel.entry)
                    {
                        var description = video.group.description.Value;
                        if (description != null)
                        {
                            var linkPrefix = "https://www.bitchute.com/video/";
                            var pos = description.IndexOf(linkPrefix);
                            if (pos >= 0)
                            {
                                var url = description.Substring(pos, 45).Trim().TrimEnd('/');
                                channel.feedlink[0].href = "";  //clear channel Url
                                channel.feedchannelId.Value = ""; //clear channel UrlId

                                channel.entry = new List<entry>(new []{video});
                                video.entrylink[0].href = url;
                                video.videoId.Value = url.Replace("linkPrefix", "");
                                video.@group.community.statistics.views = 0;
                                datasource.SaveChannel(channel, "bitchute");
                            }
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

        private static string GetXml(string channelUrl)
        {
            try
            {
                var request = WebRequest.Create(channelUrl);
                var response = request.GetResponse();
                var dataStream = response.GetResponseStream();
                if (dataStream != null)
                {
                    var reader = new StreamReader(dataStream);
                    var xml = reader.ReadToEnd();
                    return xml;
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
