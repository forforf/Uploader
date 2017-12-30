using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploader
{
    class Settings : ISettings
    {
        private readonly SettingKey WATCH_PATH_KEY = new SettingKey("WatchPath", new object());
        private readonly SettingKey S3_PATH_KEY = new SettingKey("S3Path", new object());
        Properties.Settings settings;

        public Settings()
        {
            this.settings = Properties.Settings.Default;
        }

        public string WatchPath
        {
            get { return GetSetting(WATCH_PATH_KEY); }
            set { SaveSetting(WATCH_PATH_KEY, value);  }
        }

        public string S3Path
        {
            get { return GetSetting(S3_PATH_KEY); }
            set { SaveSetting(S3_PATH_KEY, value); }
        }

        private string GetSetting(SettingKey skey)
        {
            return (string)this.settings[skey.key];
        }

        private void SaveSetting(SettingKey skey, string value)
        {
            lock (skey.lockObj)
            {
                this.settings[skey.key] = value;
                this.settings.Save();
            }
        }
    }
}
