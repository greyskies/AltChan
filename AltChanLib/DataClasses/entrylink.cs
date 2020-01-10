using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]
    public class entrylink
	{
		// ATTRIBUTES
		[XmlAttribute("href")]
		public string href { get; set; }
		
		[XmlAttribute("rel")]
		public string rel { get; set; }
		
		// ELEMENTS
		[XmlText]
		public string Value { get; set; }
		
		// CONSTRUCTOR
		public entrylink()
		{}
	}
}
