using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace update_name
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public const string consumer_key = "";
        public const string consumer_secret = "";
        public const string userprofile_filename = "userprofile.xml";

        public const string ButtonTextStreamStart = "TL監視の開始";
        public const string ButtonTextStreamStop = "TL監視の停止";

        public const string UpdateNameRegex = "^([^(?!RT)|(?!@)]).+菜$";


    }
}
