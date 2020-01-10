using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]

    public class entryauthor
	{
		
		// ELEMENTS
		[XmlElement("name")]
		public entryauthorname entryauthorname { get; set; }
		
		[XmlElement("uri")]
		public entryauthoruri entryauthoruri { get; set; }
		
		// CONSTRUCTOR
		public entryauthor()
		{}
	}
}
