using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AltChanLib;
using AltChanLib.DataClasses;

namespace AltChan.Controllers
{
    public class AltChanApiController : ApiController
    {
        public string Get()
        {
            return "Welcome To AltChan Web API - please specify a videoUrl parameter";
        }
        public List<UrlOption> Get(string videoUrl)
        {
            var dataSource = new DataSource();
            var urlList = new List<UrlOption>();
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
