using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NUnit.Framework;

namespace AltChanTest
{
    [TestFixture]
    public class HtmlAutomationTests
    {
        [Test]
        public void Test1()
        {
            var url = "https://www.bitchute.com/channel/u3QMwGD7bSW6/";
            var web = new HtmlWeb();
            var doc = web.Load(url);
        }
    }
}
