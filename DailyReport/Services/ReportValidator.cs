using DailyReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ReportModel = DailyReport.Models.DailyReport;

namespace DailyReport.Services
{
    /// <summary>日報入力を検証します。<</summary>
    public class ReportValidator
    {
        /// <summary>入力エラーを返します。<</summary>
        public IList<string> Validate(ReportModel report)
        {
            var errors = new List<string>();
            if (report == null) { errors.Add("日報が指定されていません。"); return errors; }
            if (string.IsNullOrWhiteSpace(report.Name)) errors.Add("名前を入力してください。");
            if (!report.Date.HasValue) errors.Add("日付を選択してください。");
            var regular = report.RegularTasks?.Any(x => !string.IsNullOrWhiteSpace(x?.Content)) == true;
            var skill = report.SkillUpActivities?.Any(x => !string.IsNullOrWhiteSpace(x?.Content)) == true;
            if (!regular && !skill) errors.Add("定例作業またはスキルアップ活動を入力してください。");
            foreach (var item in report.Deliverables ?? new System.Collections.ObjectModel.ObservableCollection<Deliverable>())
            {
                if (string.IsNullOrWhiteSpace(item?.Name) && !string.IsNullOrWhiteSpace(item?.Link))
                    errors.Add("成果物名が未入力です。");
                if (!string.IsNullOrWhiteSpace(item?.Link) &&
                    (!Uri.TryCreate(item.Link.Trim(), UriKind.Absolute, out var uri) ||
                     (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
                    errors.Add("リンクはhttpまたはhttpsの絶対URLで入力してください。");
            }
            return errors;
        }
    }
}
