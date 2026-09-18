using Microsoft.Win32;

namespace DailyReport.Services
{
    /// <summary>WPFの保存ダイアログを提供します。<</summary>
    public class SaveFileDialogService : ISaveFileDialogService
    {
        /// <summary>保存先を表示し、キャンセル時はnullを返します。<</summary>
        public string Show(string defaultFileName)
        {
            var dialog = new SaveFileDialog { FileName = defaultFileName, Filter = "Markdown (*.md)|*.md|すべてのファイル (*.*)|*.*", AddExtension = true, DefaultExt = ".md", OverwritePrompt = true };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}
