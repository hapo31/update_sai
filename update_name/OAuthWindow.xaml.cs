using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using CoreTweet.Core;

namespace update_name
{
	/// <summary>
	/// OAuth.xaml の相互作用ロジック
	/// </summary>
	public partial class OAuthWindow : Window, INotifyPropertyChanged
	{
		private string button_text;
		public string ButtonText
		{
			get
			{
				return button_text;
			}
			set
			{
				button_text = value;
				OnPropertyChanged("ButtonText");
			}
		}
		private string PINText { get; set; }

		private bool button_enable = true;
		public bool ButtonEnable
		{
			get { return button_enable; }
			set 
			{
				button_enable = value;
				OnPropertyChanged("ButtonEnable");
			}
		}

		private CoreTweet.OAuth.OAuthSession session;
		private CoreTweet.Tokens tokens;

        private const string filename = App.userprofile_filename;

		public OAuthWindow()
		{
			InitializeComponent();
			this.DataContext = this;

			ButtonEnable = true;
         
			session = CoreTweet.OAuth.Authorize(App.consumer_key, App.consumer_secret);
			
			var url = session.AuthorizeUri;
			System.Diagnostics.Process.Start(url.ToString());

			ButtonText = "送信";
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			if (PINText != null && PINText.Length != 0)
			{
				ButtonText = "処理中";
				ButtonEnable = false;
				try
				{
					tokens = await CoreTweet.OAuth.GetTokensAsync(session, PINText);
                    var userdata = new UserData();

                    userdata.UserID = tokens.UserId.ToString();
                    userdata.UserName = tokens.ScreenName;
                    userdata.AccessToken = tokens.AccessToken;
                    userdata.AccessTokenSecret = tokens.AccessTokenSecret;

                    var rdl = new UserDataLoader(filename);
                    rdl.Userdata = userdata;
                    rdl.UserDataSave();
                }
                catch (CoreTweet.TwitterException exception)
				{
					string caption = "エラー";
					string message = "次のエラーが発生しました\n" + exception.Message.ToString();
					var button = MessageBoxButton.OK;
					MessageBox.Show(message, caption, button);
				}
				finally
				{
					Close();
				}
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			PINText = (sender as TextBox).Text;
		}


		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string name)
		{
			if(PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
