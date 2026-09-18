using System;
using System.IO;
using System.Text;

namespace DailyReport.Services
{
    /// <summary>ファイル出力を担当します。<</summary>
    public class FileSaveService : IFileSaveService
    {
        /// <summary>UTF-8 BOMなしで保存します。<</summary>
        public void Save(string path, string content)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("保存先が指定されていません。", nameof(path));
            File.WriteAllText(path, content ?? string.Empty, new UTF8Encoding(false));
        }
    }
    /// <summary>ファイル保存サービスの抽象です。<</summary>
    public interface IFileSaveService
    {
        /// <summary>内容を保存します。<</summary>
        void Save(string path, string content);
    }
    /// <summary>保存ダイアログの抽象です。<</summary>
    public interface ISaveFileDialogService
    {
        /// <summary>保存先を選択します。<</summary>
        string Show(string defaultFileName);
    }
}
