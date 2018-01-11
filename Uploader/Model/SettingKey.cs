﻿namespace Uploader
{
    public struct SettingKey
    {
        public string key;
        public object lockObj;

        public SettingKey(string key, object lockObj)
        {
            this.key = key;
            this.lockObj = lockObj;
        }
    }
}
