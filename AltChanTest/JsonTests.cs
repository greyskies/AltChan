using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltChanLib.DataClasses;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AltChanTest
{
    [TestFixture]
    public class JsonTests
    {
        [Test]
        public void JsonDeserialisationTest()
        {
            var path = @"c:\Projects\AltChan\AltChanTest\video.json";
            var jsonString = File.ReadAllText(path);
            var jObject = JsonConvert.DeserializeObject<RootObject>(jsonString);
            Assert.IsNotNull(jObject);
        }
    }
}
