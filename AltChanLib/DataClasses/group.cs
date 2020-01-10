using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]
    public class group
	{
		
		// ELEMENTS
		[XmlElement("title")]
		public grouptitle grouptitle { get; set; }
		
		[XmlElement("content")]
		public content content { get; set; }
		
		[XmlElement("thumbnail")]
		public thumbnail thumbnail { get; set; }
		
		[XmlElement("description")]
		public description description { get; set; }
		
		[XmlElement("community")]
		public community community { get; set; }
		
		// CONSTRUCTOR
		public group()
		{}
	}
}
