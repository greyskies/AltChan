using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AltChanLib;

namespace AltChan.Controllers
{
    public class AltChanApiController : ApiController
    {
        public string Get()
        {
            return "Welcome To AltChan Web API - please specify a videoUrl parameter";
        }
        public List<string> Get(string videoUrl)
        {
            var dataSource = new DataSource();
            var urlList = new List<string>();
            try
            {
                urlList = dataSource.GetPreferredUrls(videoUrl);
            }
            catch (Exception e)
            {
                //log error
            }

            return urlList;
        }

    }
}
