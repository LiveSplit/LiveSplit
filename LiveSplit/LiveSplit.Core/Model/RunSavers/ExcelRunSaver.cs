using Codaxy.Xlio;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LiveSplit.Model.RunSavers
{
    public class ExcelRunSaver : IRunSaver
    {
        public void Save(IRun run, Stream stream)
        {
            var workbook = new Workbook();
            var splitTimesSheet = workbook.Sheets.AddSheet("Splits");
            var historySheet = workbook.Sheets.AddSheet("History");

            FillSplitTimesSheet(splitTimesSheet, run);
            FillHistorySheet(historySheet, run);

            workbook.SaveToStream(stream, Codaxy.Xlio.IO.XlsxFileWriterOptions.AutoFit);
        }

        private void FillHistorySheet(Sheet sheet, IRun run)
        {
            var header = sheet.Data.Rows[0];

            header[0].Value = "Run ID";
            header[0].Style.Font.Bold = true;
            header[0].Style.Font.Color = Color.White;
            header[0].Style.Alignment.Horizontal = HorizontalAlignment.Center;
            header[0].Style.Border.Bottom = new BorderEdge { Style = BorderStyle.Medium, Color = Color.White };
            header[0].Style.Fill = CellFill.Solid(new Color(128, 128, 128));

            var columnIndex = 1;
            foreach (var segment in run)
            {
                var cell = header[columnIndex];

                cell.Value = segment.Name;

                cell.Style.Font.Bold = true;
                cell.Style.Font.Color = Color.White;
                cell.Style.Alignment.Horizontal = HorizontalAlignment.Center;
                cell.Style.Border.Bottom = new BorderEdge { Style = BorderStyle.Medium, Color = Color.White };
                cell.Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
                cell.Style.Fill = CellFill.Solid(new Color(128, 128, 128));

                columnIndex++;
            }

            var rowIndex = 1;
            foreach (var runHistoryElement in run.RunHistory)
            {
                var row = sheet.Data.Rows[rowIndex];

                row[0].Value = runHistoryElement.Index;
                row[0].Style.Fill = CellFill.Solid(
                    ((rowIndex & 1) == 1)
                    ? new Color(221, 221, 221)
                    : new Color(238, 238, 238));
                columnIndex = 1;
                foreach (var segment in run)
                {
                    var cell = row[columnIndex];
                    var segmentHistoryElement = segment.SegmentHistory.FirstOrDefault(x => x.Index == runHistoryElement.Index);
                    if (segmentHistoryElement != null)
                    {
                        var time = segmentHistoryElement.Time.RealTime;
                        if (time.HasValue)
                            cell.Value = time.Value.TotalDays;
                    }

                    cell.Style.Alignment.Horizontal = HorizontalAlignment.Right;
                    cell.Style.Format = "[HH]:MM:SS.00";
                    cell.Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
                    cell.Style.Fill = CellFill.Solid(
                        ((rowIndex & 1) == 1)
                        ? new Color(221, 221, 221)
                        : new Color(238, 238, 238));

                    columnIndex++;
                }

                ++rowIndex;
            }
        }

        private void FillSplitTimesSheet(Sheet sheet, IRun run)
        {
            var header = sheet.Data.Rows[0];

            header[0].Value = "Segment";
            header[0].Style.Font.Bold = true;
            header[0].Style.Font.Color = Color.White;
            header[0].Style.Alignment.Horizontal = HorizontalAlignment.Center;
            header[0].Style.Border.Bottom = new BorderEdge { Style = BorderStyle.Medium, Color = Color.White };
            header[0].Style.Fill = CellFill.Solid(new Color(128, 128, 128));

            var columnIndex = 1;
            foreach (var comparisonName in run.Comparisons.Where(x => x != NoneComparisonGenerator.ComparisonName))
            {
                var cell = header[columnIndex];

                cell.Value = comparisonName;

                cell.Style.Font.Bold = true;
                cell.Style.Font.Color = Color.White;
                cell.Style.Alignment.Horizontal = HorizontalAlignment.Center;
                cell.Style.Border.Bottom = new BorderEdge { Style = BorderStyle.Medium, Color = Color.White };
                cell.Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
                cell.Style.Fill = CellFill.Solid(new Color(128, 128, 128));

                columnIndex++;
            }

            var rowIndex = 1;
            foreach (var segment in run)
            {
                var row = sheet.Data.Rows[rowIndex];

                row[0].Value = segment.Name;
                row[0].Style.Fill = CellFill.Solid(
                    ((rowIndex & 1) == 1)
                    ? new Color(221, 221, 221)
                    : new Color(238, 238, 238));
                columnIndex = 1;
                foreach (var comparisonName in run.Comparisons.Where(x => x != NoneComparisonGenerator.ComparisonName))
                {
                    var cell = row[columnIndex];
                    var time = segment.Comparisons[comparisonName].RealTime;
                    if (time.HasValue)
                        cell.Value = time.Value.TotalDays;

                    cell.Style.Alignment.Horizontal = HorizontalAlignment.Right;
                    cell.Style.Format = "[HH]:MM:SS.00";
                    cell.Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
                    cell.Style.Fill = CellFill.Solid(
                        ((rowIndex & 1) == 1)
                        ? new Color(221, 221, 221)
                        : new Color(238, 238, 238));

                    columnIndex++;
                }

                ++rowIndex;
            }
        }
    }
}
