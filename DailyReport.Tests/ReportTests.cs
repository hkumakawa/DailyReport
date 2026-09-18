using DailyReport.Models;
using DailyReport.Services;
using DailyReport.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace DailyReport.Tests
{
    /// <summary>日報の検証・生成テストです。<</summary>
    [TestClass]
    public class ReportTests
    {
        private sealed class SettingsFake : IUserSettingsService
        {
            public string UserName { get; private set; }
            public void SaveUserName(string name) { UserName = name; }
        }

        private sealed class DialogFake : ISaveFileDialogService
        {
            public string PathToReturn { get; set; }
            public string Show(string defaultFileName) { return PathToReturn; }
        }

        private sealed class FileFake : IFileSaveService
        {
            public string Path { get; private set; }
            public string Content { get; private set; }
            public Exception Error { get; set; }
            public void Save(string path, string content)
            {
                if (Error != null) throw Error;
                Path = path;
                Content = content;
            }
        }

        private static DailyReport.Models.DailyReport Valid()
        {
            var r = new DailyReport.Models.DailyReport { Name = "山田", Date = new DateTime(2026, 9, 18) };
            r.RegularTasks.Add(new TaskItem { Content = "作業" }); return r;
        }
        /// <summary>必須項目不足を検出します。<</summary>
        [TestMethod]
        public void Validate_必須項目空_エラー()
        {
            var e = new ReportValidator().Validate(new DailyReport.Models.DailyReport());
            Assert.IsTrue(e.Count >= 3);
        }
        /// <summary>成果物名のみを許可します。<</summary>
        [TestMethod]
        public void Validate_成果物名のみ_成功()
        {
            var r = Valid(); r.Deliverables.Add(new Deliverable { Name = "成果物" });
            Assert.AreEqual(0, new ReportValidator().Validate(r).Count);
        }
        /// <summary>リンクのみを拒否します。<</summary>
        [TestMethod]
        public void Validate_リンクのみ_エラー()
        {
            var r = Valid(); r.Deliverables.Add(new Deliverable { Link = "https://example.com" });
            Assert.IsTrue(new ReportValidator().Validate(r).Any(x => x.Contains("成果物名")));
        }
        /// <summary>URL形式を検証します。<</summary>
        [TestMethod]
        public void Validate_無効URL_エラー()
        {
            var r = Valid(); r.Deliverables.Add(new Deliverable { Name = "x", Link = "ftp://example.com" });
            Assert.IsTrue(new ReportValidator().Validate(r).Any(x => x.Contains("http")));
        }
        /// <summary>空白だけの入力を必須エラーとして扱います。</summary>
        [TestMethod]
        public void Validate_空白入力_エラー()
        {
            var r = Valid();
            r.Name = "  ";
            r.RegularTasks[0].Content = "\t";
            Assert.IsTrue(new ReportValidator().Validate(r).Count >= 2);
        }
        /// <summary>空行となし表示を処理します。<</summary>
        [TestMethod]
        public void Generate_空行のみ_なし表示()
        {
            var r = Valid(); r.RegularTasks.Clear(); r.SkillUpActivities.Add(new TaskItem());
            var text = new MarkdownGenerator().Generate(r);
            StringAssert.Contains(text, "定例作業なし"); StringAssert.Contains(text, "スキルアップ活動なし"); StringAssert.Contains(text, "成果物なし");
        }
        /// <summary>制御文字と日本語をエスケープします。<</summary>
        [TestMethod]
        public void Generate_制御文字_エスケープ()
        {
            var r = Valid(); r.Name = "A*_[x]"; r.RegularTasks[0].Content = "# 内容";
            StringAssert.Contains(new MarkdownGenerator().Generate(r), "A\\*\\_\\[x\\]");
        }
        /// <summary>ファイル名の不正文字を置換します。<</summary>
        [TestMethod]
        public void GetDefaultFileName_不正文字_置換()
        {
            var r = Valid(); r.Name = "a:b";
            StringAssert.Contains(new MarkdownGenerator().GetDefaultFileName(r), "_20260918.md");
        }
        /// <summary>BOMなしで保存します。<</summary>
        [TestMethod]
        public void Save_正常値_BOMなし()
        {
            var path = Path.GetTempFileName();
            try { new FileSaveService().Save(path, "日本語"); Assert.AreNotEqual(0xEF, File.ReadAllBytes(path)[0]); }
            finally { File.Delete(path); }
        }
        /// <summary>保存キャンセル時にファイル保存を実行しません。</summary>
        [TestMethod]
        public void Save_キャンセル_ファイル保存なし()
        {
            var file = new FileFake();
            var dialog = new DialogFake { PathToReturn = null };
            var vm = new MainViewModel(file, new SettingsFake(), dialog);
            vm.Report.Name = "山田";
            vm.SaveCommand.Execute(null);
            Assert.IsNull(file.Path);
            Assert.AreEqual(string.Empty, vm.ErrorMessage);
        }
        /// <summary>保存成功時に選択先へMarkdownを渡します。</summary>
        [TestMethod]
        public void Save_正常値_ファイル保存成功()
        {
            var file = new FileFake();
            var dialog = new DialogFake { PathToReturn = "report.md" };
            var settings = new SettingsFake();
            var vm = new MainViewModel(file, settings, dialog);
            vm.Name = "山田";

            vm.SaveCommand.Execute(null);

            Assert.AreEqual("report.md", file.Path);
            StringAssert.Contains(file.Content, "（山田）作業日報");
            Assert.AreEqual("山田", settings.UserName);
            Assert.AreEqual(string.Empty, vm.ErrorMessage);
        }
        /// <summary>保存失敗時にエラーを表示し入力を保持します。</summary>
        [TestMethod]
        public void Save_書き込み失敗_エラー表示と入力保持()
        {
            var file = new FileFake { Error = new IOException("拒否") };
            var vm = new MainViewModel(file, new SettingsFake(), new DialogFake { PathToReturn = "report.md" });
            vm.Report.Name = "山田";
            vm.SaveCommand.Execute(null);
            StringAssert.Contains(vm.ErrorMessage, "保存に失敗しました");
            Assert.AreEqual("朝礼／夕礼", vm.Report.RegularTasks[0].Content);
        }
    }
}
