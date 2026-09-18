using System;
using System.Collections.ObjectModel;

namespace DailyReport.Models
{
    /// <summary>一日分の日報データです。<</summary>
    public class DailyReport
    {
        /// <summary>利用者名。<</summary>
        public string Name { get; set; }
        /// <summary>作業日。<</summary>
        public DateTime? Date { get; set; }
        /// <summary>定例作業。<</summary>
        public ObservableCollection<TaskItem> RegularTasks { get; set; } = new ObservableCollection<TaskItem>();
        /// <summary>スキルアップ活動。<</summary>
        public ObservableCollection<TaskItem> SkillUpActivities { get; set; } = new ObservableCollection<TaskItem>();
        /// <summary>成果物。<</summary>
        public ObservableCollection<Deliverable> Deliverables { get; set; } = new ObservableCollection<Deliverable>();
    }
}
