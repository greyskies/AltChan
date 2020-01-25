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
        private readonly string _connectionString;

        public DataSource()
        {
            _connectionString = ConfigurationManager.AppSettings["connectionStringLocal"];
        }

        public void SaveChannel(Channel channel)
        {
            Console.WriteLine($"Channel: {channel.Title}");
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "InsertChannel";
                    var channelCommand = new SqlCommand(sql, connection) {CommandType = CommandType.StoredProcedure};

                    channelCommand.Parameters.Add("Platform", SqlDbType.VarChar).Value = channel.Platform;
                    channelCommand.Parameters.Add("Url", SqlDbType.VarChar).Value = channel.Url;
                    channelCommand.Parameters.Add("UrlId", SqlDbType.VarChar).Value = channel.UrlId;
                    channelCommand.Parameters.Add("Title", SqlDbType.VarChar).Value = channel.Title;
                    channelCommand.Parameters.Add("Published", SqlDbType.DateTime).Value = channel.Published;
                    channelCommand.Parameters.Add("Description", SqlDbType.VarChar).Value = channel.Description;

                    object channelId = 0;
                    try
                    {
                        channelId = channelCommand.ExecuteScalar();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to execute InsertChannel: ChannelId={channel.UrlId} {e}");
                    }

                    int index = 0;
                    foreach (var video in channel.Videos)
                    {

                        sql = "InsertVideo";
                        var videoCommand = new SqlCommand(sql, connection) {CommandType = CommandType.StoredProcedure};

                        videoCommand.Parameters.Add("ChannelId", SqlDbType.Int).Value = channelId;
                        videoCommand.Parameters.Add("Title", SqlDbType.VarChar).Value = video.Title;
                        videoCommand.Parameters.Add("Platform", SqlDbType.VarChar).Value = channel.Platform;
                        videoCommand.Parameters.Add("Url", SqlDbType.VarChar).Value = video.Url;
                        videoCommand.Parameters.Add("UrlId", SqlDbType.VarChar).Value = video.UrlId;
                        videoCommand.Parameters.Add("Published", SqlDbType.DateTime).Value = video.Published;
                        videoCommand.Parameters.Add("Views", SqlDbType.VarChar).Value = video.Views;
                        videoCommand.Parameters.Add("ThumbnailUrl", SqlDbType.VarChar).Value = video.ThumbnailUrl;
                        videoCommand.Parameters.Add("Description", SqlDbType.VarChar).Value = channel.Description;

                        try
                        {
                            videoCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Failed to execute InsertVideo for video {index} {video.UrlId} {e}");
                        }
                        index++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to save channel {channel.UrlId} {e}");
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

        public List<UrlOption> GetPreferredUrls(string url)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var retVal = new List<UrlOption>();
                try
                {
                    connection.Open();
                    string sql = "GetPreferredUrls";
                    var channelCommand = new SqlCommand(sql, connection) { CommandType = CommandType.StoredProcedure };

                    channelCommand.Parameters.Add("Url", SqlDbType.VarChar).Value = @url;

                    var reader = channelCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        var videoUrl = reader.GetString(0);
                        var channel = reader.GetString(1);
                        var platform = reader.GetString(2);

                        retVal.Add(new UrlOption {Url=videoUrl, Channel=channel, Platform = platform});
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

        public List<string> GetStagedVideoUrls()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var retVal = new List<string>();
                try
                {
                    connection.Open();
                    string sql = "select Url from VideoUrlStaging";
                    var channelCommand = new SqlCommand(sql, connection);

                    var reader = channelCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        var videoUrl = reader.GetString(0);
                        retVal.Add(videoUrl);
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

        public List<string> GetChannelVideoUrls(string channelId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var retVal = new List<string>();
                try
                {
                    connection.Open();
                    string sql = "select vu.Url from VideoUrl vu "
                                 +"inner join Video v on v.id = vu.VideoId "
                                 +"inner join channelUrl cu on cu.ChannelId = v.ChannelId "
                                 +"where cu.urlId = '"+channelId+"'";
                    var channelCommand = new SqlCommand(sql, connection);

                    var reader = channelCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        var videoUrl = reader.GetString(0);
                        retVal.Add(videoUrl);
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


        public void ClearStagedVideoUrls()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "delete VideoUrlStaging";
                    var channelCommand = new SqlCommand(sql, connection);

                    channelCommand.ExecuteNonQuery();
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
            }
        }

        public void ClearStagedVideoUrl(string url)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = $"delete VideoUrlStaging where Url='{url}'";
                    var channelCommand = new SqlCommand(sql, connection);

                    channelCommand.ExecuteNonQuery();
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
            }
        }

    }
}

