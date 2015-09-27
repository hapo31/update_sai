using System;
using System.Linq;
using System.Windows;

using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Reactive.Linq;
using CoreTweet.Streaming;
using CoreTweet.Streaming.Reactive;

namespace update_name
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private UserData userdata;
        private IDisposable connection;

        CoreTweet.Tokens tokens;
        CoreTweet.StatusResponse res_my_tweet = null;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }


        private string button_text;
        //メインウインドウのボタンのテキスト
        public string ButtonText
        {
            get { return button_text; }
            set { OnPropertyChanged("ButtonText"); button_text = value; }
        }

        //User Streamが有効かどうか
        public bool IsStreaming
        {
            get; set;
        }

        public bool StartButtonEnable
        {
            get; private set;
        }

        //認証済みの場合はユーザー名を表示
        public string UserNameText
        {
            get; private set;
        }

        //アプリの状態を表示
        public string StatusText
        {
            get; private set;
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            IsStreaming = false;

            //以前認証したデータを読み込む
            var ud = LoadTokens();

            //データが見つかれば読み込む
            if(ud != null)
            {
                userdata = ud;
                UserNameText = userdata.UserName;
                ButtonText = App.ButtonTextStreamStart;
                StatusText = "認証成功";
                StartButtonEnable = true;
            }
            //見つからなければ認証を促す
            else
            {
                StatusText = "認証してください";
                StartButtonEnable = false;
            }
            OnPropertyChanged("StartButtonEnable");
        }
        

        //タイムライン監視の起動と停止
        private void StartUpdatenameButton(object sender, RoutedEventArgs e)
        {
            if(tokens == null)
            {
                return;
            }
            //ストリームの開始
            if (!IsStreaming)
            {
                var streamRx = tokens.Streaming.StartObservableStream(StreamingType.User).Publish();
                Action<StatusMessage> action = (message) =>
                {
                    //データを受け取った時の処理
                    var stat = (message as StatusMessage).Status;
                    //自分のツイートには反応しないようにする
                    if (res_my_tweet == null || stat.Id != res_my_tweet.Id)
                    {
                        if (UpdateName(stat.Text))
                        {
                            string str = "@" + stat.User.ScreenName + " 我が名は" + stat.Text + "である！";
                            res_my_tweet = tokens.Statuses.Update(status => str, in_reply_to_status_id => stat.Id);
                        }
                    }
                };
                streamRx.OfType<StatusMessage>().Subscribe(action);
                connection = streamRx.Connect();
                IsStreaming = true;
                UpdateStatusText("TL監視中");
            }
            //ストリームの停止
            else
            {
                connection.Dispose();
                IsStreaming = false;
                UpdateStatusText("TL監視停止中");
            }
        }

        private void OAuthMenuItemClick(object sender, RoutedEventArgs e)
        {
            var OAuthWindow = new OAuthWindow();
            OAuthWindow.ShowDialog();
            var udl = new UserDataLoader(App.userprofile_filename);
            if(udl.UserDataExist())
            {
                StartButtonEnable = true;
            }
            userdata = udl.Userdata;
        }

        private UserData LoadTokens()
        {
            var udl = new UserDataLoader(App.userprofile_filename);
            if (udl.Userdata != null)
            {
                tokens = udl.GetCoreTweetTokens(App.consumer_key, App.consumer_secret);
                return udl.Userdata;
            }
            else
                return null;
        }


        private bool UpdateName(string text)
        {

            if (System.Text.RegularExpressions.Regex.IsMatch(text, App.UpdateNameRegex))
            {
                try
                {
                    tokens.Account.UpdateProfile(name => text);
                }
                catch (CoreTweet.TwitterException e)
                {
                    System.Console.WriteLine(e.ToString());
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        void UpdateStatusText(string str)
        {
            StatusText = str;
            OnPropertyChanged("StatusText");
        }

    }


}
