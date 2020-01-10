using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]
    public class entry
	{
		
		// ELEMENTS
		[XmlElement("id")]
		public entryid entryid { get; set; }
		
		[XmlElement("videoId")]
		public videoId videoId { get; set; }
		
		[XmlElement("channelId")]
		public entrychannelId entrychannelId { get; set; }
		
		[XmlElement("title")]
		public entrytitle entrytitle { get; set; }
		
		[XmlElement("link")]
		public List<entrylink> entrylink { get; set; }
		
		[XmlElement("author")]
		public entryauthor entryauthor { get; set; }
		
		[XmlElement("published")]
		public entrypublished entrypublished { get; set; }
		
		[XmlElement("updated")]
		public updated updated { get; set; }
		
		[XmlElement("group")]
		public group group { get; set; }
		
		// CONSTRUCTOR
		public entry()
		{}
	}
}
