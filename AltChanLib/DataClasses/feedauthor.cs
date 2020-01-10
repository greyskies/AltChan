using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]
    public class feedauthor
	{
		
		// ELEMENTS
		[XmlElement("name")]
		public feedauthorname feedauthorname { get; set; }
		
		[XmlElement("uri")]
		public feedauthoruri feedauthoruri { get; set; }
		
		// CONSTRUCTOR
		public feedauthor()
		{}
	}
}
