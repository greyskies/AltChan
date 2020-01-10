using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
	[Serializable]
	public class feed
	{
		
		// ELEMENTS
		[XmlElement("link")]
		public List<feedlink> feedlink { get; set; }
		
		[XmlElement("id")]
		public feedid feedid { get; set; }
		
		[XmlElement("channelId")]
		public feedchannelId feedchannelId { get; set; }
		
		[XmlElement("title")]
		public feedtitle feedtitle { get; set; }
		
		[XmlElement("author")]
		public feedauthor feedauthor { get; set; }
		
		[XmlElement("published")]
		public feedpublished feedpublished { get; set; }
		
		[XmlElement("entry")]
		public List<entry> entry { get; set; }
		
		// CONSTRUCTOR
		public feed()
		{}
	}
}
