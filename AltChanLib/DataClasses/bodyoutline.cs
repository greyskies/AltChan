using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
	[Serializable]
	public class bodyoutline
	{
		// ATTRIBUTES
		[XmlAttribute("text")]
		public string text { get; set; }
		
		[XmlAttribute("title")]
		public string title { get; set; }
		
		// ELEMENTS
		[XmlElement("outline")]
		public List<outlineoutline> outlineoutline { get; set; }
		
		// CONSTRUCTOR
		public bodyoutline()
		{}
	}
}
