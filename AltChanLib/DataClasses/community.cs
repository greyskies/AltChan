using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]
    public class community
	{
		
		// ELEMENTS
		[XmlElement("starRating")]
		public starRating starRating { get; set; }
		
		[XmlElement("statistics")]
		public statistics statistics { get; set; }
		
		// CONSTRUCTOR
		public community()
		{}
	}
}
