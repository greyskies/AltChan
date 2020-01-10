using System;
using System.Xml.Serialization;

namespace AltChanLib.DataClasses
{
    [Serializable]
    public class updated
	{
		
		// ELEMENTS
		[XmlText]
		public string Value { get; set; }
		
		// CONSTRUCTOR
		public updated()
		{}
	}
}
