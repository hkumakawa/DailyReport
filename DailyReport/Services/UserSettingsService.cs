using System.Configuration;

namespace DailyReport.Services
{
    /// <summary>利用者設定の抽象です。<</summary>
    public interface IUserSettingsService
    {
        /// <summary>名前を取得します。<</summary>
        string UserName { get; }
        /// <summary>名前を保存します。<</summary>
        void SaveUserName(string name);
    }
    /// <summary>ApplicationSettingsBaseを使う設定実装です。<</summary>
    public class UserSettingsService : IUserSettingsService
    {
        /// <summary>設定値。<</summary>
        public string UserName { get { return Settings.Default.UserName; } }
        /// <summary>名前を保存します。<</summary>
        public void SaveUserName(string name) { Settings.Default.UserName = name ?? string.Empty; Settings.Default.Save(); }
        private sealed class Settings : ApplicationSettingsBase
        {
            private static readonly Settings instance = (Settings)Synchronized(new Settings());
            public static Settings Default { get { return instance; } }
            [UserScopedSetting, DefaultSettingValue("")]
            public string UserName { get { return (string)this[nameof(UserName)]; } set { this[nameof(UserName)] = value; } }
        }
    }
}
