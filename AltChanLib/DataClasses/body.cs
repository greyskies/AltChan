using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
	[Serializable]
	public class body
	{
		
		// ELEMENTS
		[XmlElement("outline")]
		public List<bodyoutline> bodyoutline { get; set; }
		
		// CONSTRUCTOR
		public body()
		{}
	}
}
