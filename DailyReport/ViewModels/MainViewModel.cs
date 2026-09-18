using DailyReport.Models;
using DailyReport.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ReportModel = DailyReport.Models.DailyReport;

namespace DailyReport.ViewModels
{
    /// <summary>メイン画面の状態と操作を提供します。<</summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IFileSaveService fileService;
        private readonly IUserSettingsService settings;
        private readonly ISaveFileDialogService dialog;
        private string errorMessage;
        /// <summary>日報データ。<</summary>
        public ReportModel Report { get; }
        /// <summary>設定と日報に反映する利用者名。</summary>
        public string Name
        {
            get { return Report.Name; }
            set
            {
                if (Report.Name == value) return;
                Report.Name = value;
                settings.SaveUserName(value);
                OnPropertyChanged();
            }
        }
        /// <summary>エラーメッセージ。<</summary>
        public string ErrorMessage { get { return errorMessage; } private set { errorMessage = value; OnPropertyChanged(); } }
        /// <summary>定例作業追加。<</summary>
        public RelayCommand AddRegularTaskCommand { get; }
        /// <summary>定例作業削除。<</summary>
        public RelayCommand RemoveRegularTaskCommand { get; }
        /// <summary>スキルアップ追加。<</summary>
        public RelayCommand AddSkillCommand { get; }
        /// <summary>スキルアップ削除。<</summary>
        public RelayCommand RemoveSkillCommand { get; }
        /// <summary>成果物追加。<</summary>
        public RelayCommand AddDeliverableCommand { get; }
        /// <summary>成果物削除。<</summary>
        public RelayCommand RemoveDeliverableCommand { get; }
        /// <summary>保存。<</summary>
        public RelayCommand SaveCommand { get; }
        /// <summary>選択中の定例作業。</summary>
        public TaskItem SelectedRegularTask { get; set; }
        /// <summary>選択中のスキルアップ活動。</summary>
        public TaskItem SelectedSkill { get; set; }
        /// <summary>選択中の成果物。</summary>
        public Deliverable SelectedDeliverable { get; set; }
        /// <summary>画面モデルを初期化します。<</summary>
        public MainViewModel(IFileSaveService fileService, IUserSettingsService settings, ISaveFileDialogService dialog)
        {
            this.fileService = fileService; this.settings = settings; this.dialog = dialog;
            Report = new ReportModel { Name = settings.UserName, Date = DateTime.Today };
            Report.RegularTasks.Add(new TaskItem { Content = "朝礼／夕礼" });
            Report.SkillUpActivities.Add(new TaskItem()); Report.Deliverables.Add(new Deliverable());
            AddRegularTaskCommand = new RelayCommand(_ => Report.RegularTasks.Add(new TaskItem()));
            RemoveRegularTaskCommand = new RelayCommand(x => Remove(Report.RegularTasks, x));
            AddSkillCommand = new RelayCommand(_ => Report.SkillUpActivities.Add(new TaskItem()));
            RemoveSkillCommand = new RelayCommand(x => Remove(Report.SkillUpActivities, x));
            AddDeliverableCommand = new RelayCommand(_ => Report.Deliverables.Add(new Deliverable()));
            RemoveDeliverableCommand = new RelayCommand(x => Remove(Report.Deliverables, x));
            SaveCommand = new RelayCommand(_ => Save());
        }
        /// <summary>名前を設定へ反映します。<</summary>
        /// <summary>現在の名前を設定へ保存します。</summary>
        public void SaveName() { settings.SaveUserName(Report.Name); }
        private static void Remove<T>(ObservableCollection<T> list, object item) { if (item is T value && list.Contains(value)) list.Remove(value); }
        private void Save()
        {
            ErrorMessage = string.Empty;
            var errors = new ReportValidator().Validate(Report);
            if (errors.Any()) { ErrorMessage = string.Join(Environment.NewLine, errors); return; }
            try
            {
                SaveName();
                var path = dialog.Show(new MarkdownGenerator().GetDefaultFileName(Report));
                if (string.IsNullOrEmpty(path)) return;
                fileService.Save(path, new MarkdownGenerator().Generate(Report));
            }
            catch (Exception ex) { ErrorMessage = "保存に失敗しました: " + ex.Message; }
        }
        /// <summary>プロパティ変更通知。<</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}
