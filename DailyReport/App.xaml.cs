using System.Windows;
using DailyReport.Services;
using DailyReport.ViewModels;

namespace DailyReport
{
    /// <summary>アプリケーションのエントリポイントです。<</summary>
    public partial class App : Application
    {
        /// <summary>メインウィンドウを生成します。<</summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var settings = new UserSettingsService();
            var vm = new MainViewModel(new FileSaveService(), settings, new SaveFileDialogService());
            new MainWindow { DataContext = vm }.Show();
        }
    }
}
