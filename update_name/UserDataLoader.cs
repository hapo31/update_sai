using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace update_name
{
	class UserDataLoader
	{
		private string filename;
		public string FileName { get { return filename; } }

		public UserData Userdata { get; set; }

		//XMLファイルに保存したデータを読む
		public UserDataLoader(string filename)
		{
			this.filename = filename;
			if(UserDataExist())
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(UserData));
				var reader = new System.IO.StreamReader(filename, new System.Text.UTF8Encoding(false));
				try
				{
					Userdata = (UserData)serializer.Deserialize(reader);
				}
				finally
				{
					reader.Close();
				}
			}
		}

		//ユーザーデータを保存する
		public bool UserDataSave(bool overwrite = true)
		{
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(UserData));
			var writer = new System.IO.StreamWriter(filename, false, new System.Text.UTF8Encoding(false));
			
			//上書きを許可かつ、ユーザーデータが存在しない場合は書き込み
			if (overwrite || !UserDataExist())
			{
				try
				{
					serializer.Serialize(writer, Userdata);
				}
				finally
				{
					writer.Close();
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool UserDataExist()
		{
			return System.IO.File.Exists(filename);
		}

		//APIキーからCoreTweet.Tokensを得る
		public CoreTweet.Tokens GetCoreTweetTokens(string consumer_key, string consumer_secret)
		{
			if (Userdata != null)
				return CoreTweet.Tokens.Create(consumer_key, consumer_secret, Userdata.AccessToken, Userdata.AccessTokenSecret);
			else
				return null;
		}
	}
}
