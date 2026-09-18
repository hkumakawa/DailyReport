using System.Linq;
using System.Text;
using ReportModel = DailyReport.Models.DailyReport;

namespace DailyReport.Services
{
    /// <summary>日報をMarkdownへ変換します。<</summary>
    public class MarkdownGenerator
    {
        /// <summary>日報を仕様のMarkdown文字列へ変換します。<</summary>
        public string Generate(ReportModel report)
        {
            var b = new StringBuilder();
            var name = Escape(report.Name);
            b.AppendLine("## （" + name + "）作業日報").AppendLine();
            b.AppendLine(report.Date.Value.ToString("yyyy.MM.dd")).AppendLine();
            b.AppendLine("### 作業内容");
            var regular = report.RegularTasks.Where(x => !string.IsNullOrWhiteSpace(x?.Content)).Select(x => Escape(x.Content)).ToList();
            if (regular.Count == 0) b.AppendLine("- 定例作業なし");
            else foreach (var x in regular) b.AppendLine("- " + x);
            b.AppendLine("- スキルアップ活動");
            var skills = report.SkillUpActivities.Where(x => !string.IsNullOrWhiteSpace(x?.Content)).Select(x => Escape(x.Content)).ToList();
            if (skills.Count == 0) b.AppendLine("  - スキルアップ活動なし");
            else foreach (var x in skills) b.AppendLine("  - " + x);
            b.AppendLine().AppendLine("### 成果物");
            var deliverables = report.Deliverables.Where(x => !string.IsNullOrWhiteSpace(x?.Name) || !string.IsNullOrWhiteSpace(x?.Link)).ToList();
            if (deliverables.Count == 0) b.AppendLine("- 成果物なし");
            else foreach (var x in deliverables)
                    b.AppendLine(string.IsNullOrWhiteSpace(x.Link) ? "- " + Escape(x.Name) : "- [" + Escape(x.Name) + "](" + x.Link.Trim() + ")");
            return b.ToString();
        }
        /// <summary>Markdown制御文字をエスケープします。<</summary>
        public string Escape(string value)
        {
            if (value == null) return string.Empty;
            var result = value;
            foreach (var c in new[] { '\\', '*', '_', '#', '[', ']' }) result = result.Replace(c.ToString(), "\\" + c);
            return result;
        }
        /// <summary>既定ファイル名を生成します。<</summary>
        public string GetDefaultFileName(ReportModel report)
        {
            var invalid = System.IO.Path.GetInvalidFileNameChars();
            var safe = new string((report.Name ?? string.Empty).Select(c => invalid.Contains(c) ? '_' : c).ToArray());
            return safe + "_" + report.Date.Value.ToString("yyyyMMdd") + ".md";
        }
    }
}
