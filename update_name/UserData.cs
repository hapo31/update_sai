using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace update_name
{
	[System.Xml.Serialization.XmlRoot("UserData")]
	public class UserData
	{
		[System.Xml.Serialization.XmlElement("ScreenName")]
		public string UserName { get; set; }
		[System.Xml.Serialization.XmlElement("UserID")]
		public string UserID { get; set; }
		[System.Xml.Serialization.XmlElement("AccessToken")]
		public string AccessToken { get; set; }
		[System.Xml.Serialization.XmlElement("AccessTokenSecret")]
		public string AccessTokenSecret { get; set; }
	}
}
