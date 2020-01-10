using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
	[Serializable]
	public class outlineoutline
	{
		// ATTRIBUTES
		[XmlAttribute("text")]
		public string text { get; set; }
		
		[XmlAttribute("title")]
		public string title { get; set; }
		
		[XmlAttribute("type")]
		public string type { get; set; }
		
		[XmlAttribute("xmlUrl")]
		public string xmlUrl { get; set; }
		
		// ELEMENTS
		[XmlText]
		public string Value { get; set; }
		
		// CONSTRUCTOR
		public outlineoutline()
		{}
	}
}
