using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]
    public class content
	{
		// ATTRIBUTES
		[XmlAttribute("height")]
		public int height  { get; set; }
		
		[XmlAttribute("width")]
		public int width  { get; set; }
		
		[XmlAttribute("type")]
		public string type { get; set; }
		
		[XmlAttribute("url")]
		public string url { get; set; }
		
		// ELEMENTS
		[XmlText]
		public string Value { get; set; }
		
		// CONSTRUCTOR
		public content()
		{}
	}
}
