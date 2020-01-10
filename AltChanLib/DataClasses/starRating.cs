using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]
    public class starRating
	{
		// ATTRIBUTES
		[XmlAttribute("max")]
		public int max  { get; set; }
		
		[XmlAttribute("min")]
		public int min  { get; set; }
		
		[XmlAttribute("average")]
		public decimal average  { get; set; }
		
		[XmlAttribute("count")]
		public int count  { get; set; }
		
		// ELEMENTS
		[XmlText]
		public string Value { get; set; }
		
		// CONSTRUCTOR
		public starRating()
		{}
	}
}
