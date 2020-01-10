//using System.Drawing.Imaging;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using AltChanLib.DataClasses;

namespace AltChanLib
{
    public class DataSource
    {
        //private const string ConnectionStr = @"Server=localhost\SQLEXPRESS01,1433;Database=Science_Blog_Database;User Id=Alex;Password=alex01;";
        //private const string ConnectionStr = @"Server=mssql7.websitelive.net;Database=greyskies_science_blog_database;User Id=greyskies_alex;Password=m8NYg#xU;";

        private readonly string _connectionString;

        public DataSource()
        {
            _connectionString = ConfigurationManager.AppSettings["connectionStringLive"];
        }

        public void SaveChannel(feed channel, string platform)
        {
            Console.WriteLine($"Channel: {channel.feedtitle.Value}");
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "InsertChannel";
                    var channelCommand = new SqlCommand(sql, connection) {CommandType = CommandType.StoredProcedure};

                    channelCommand.Parameters.Add("Platform", SqlDbType.VarChar).Value = platform;
                    channelCommand.Parameters.Add("Url", SqlDbType.VarChar).Value = channel.feedlink[0].href;
                    channelCommand.Parameters.Add("UrlId", SqlDbType.VarChar).Value = channel.feedchannelId.Value;
                    channelCommand.Parameters.Add("Title", SqlDbType.VarChar).Value = channel.feedtitle.Value;
                    channelCommand.Parameters.Add("Published", SqlDbType.DateTime).Value =
                        DateTime.Parse(channel.feedpublished.Value);

                    var channelId = channelCommand.ExecuteScalar();

                    foreach (var video in channel.entry)
                    {

                        sql = "InsertVideo";
                        var videoCommand = new SqlCommand(sql, connection) {CommandType = CommandType.StoredProcedure};

                        videoCommand.Parameters.Add("ChannelId", SqlDbType.Int).Value = channelId;
                        videoCommand.Parameters.Add("Title", SqlDbType.VarChar).Value = video.entrytitle.Value;
                        videoCommand.Parameters.Add("Platform", SqlDbType.VarChar).Value = platform;
                        videoCommand.Parameters.Add("Url", SqlDbType.VarChar).Value = video.entrylink[0].href;
                        videoCommand.Parameters.Add("UrlId", SqlDbType.VarChar).Value = video.videoId.Value;
                        videoCommand.Parameters.Add("Published", SqlDbType.DateTime).Value = DateTime.Parse(video.entrypublished.Value);
                        videoCommand.Parameters.Add("Updated", SqlDbType.DateTime).Value = DateTime.Parse(video.updated.Value);
                        videoCommand.Parameters.Add("Views", SqlDbType.VarChar).Value = video.@group.community.statistics.views;
                        videoCommand.Parameters.Add("ThumbnailUrl", SqlDbType.VarChar).Value = video.@group.thumbnail.url;

                        videoCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public List<string> GetPreferredUrls(string url)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var retVal = new List<string>();
                try
                {
                    connection.Open();
                    string sql = "GetPreferredUrls";
                    var channelCommand = new SqlCommand(sql, connection) { CommandType = CommandType.StoredProcedure };

                    channelCommand.Parameters.Add("Url", SqlDbType.VarChar).Value = @url;

                    var reader = channelCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        retVal.Add(reader.GetString(0));
                    }
                }
                catch (Exception e)
                {
                    //log error
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
                return retVal;
            }
        }
    }
}

