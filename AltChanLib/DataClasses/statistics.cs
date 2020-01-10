using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]
    public class statistics
	{
		// ATTRIBUTES
		[XmlAttribute("views")]
		public int views  { get; set; }
		
		// ELEMENTS
		[XmlText]
		public string Value { get; set; }
		
		// CONSTRUCTOR
		public statistics()
		{}
	}
}
