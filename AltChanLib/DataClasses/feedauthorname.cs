using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]
    public class feedauthorname
	{
		
		// ELEMENTS
		[XmlText]
		public string Value { get; set; }
		
		// CONSTRUCTOR
		public feedauthorname()
		{}
	}
}
