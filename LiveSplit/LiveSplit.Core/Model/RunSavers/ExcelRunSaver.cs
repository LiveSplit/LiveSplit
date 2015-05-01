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
            var runHistorySheet = workbook.Sheets.AddSheet("Run History");
            var segmentHistorySheet = workbook.Sheets.AddSheet("Segment History");

            FillSplitTimesSheet(splitTimesSheet, run);
            FillRunHistorySheet(runHistorySheet, run);
            FillSegmentHistorySheet(segmentHistorySheet, run);

            workbook.SaveToStream(stream, Codaxy.Xlio.IO.XlsxFileWriterOptions.AutoFit);
        }

        private void FillRunHistorySheet(Sheet sheet, IRun run)
        {
            var header = sheet.Data.Rows[0];

            header[0].Value = "Run ID";
            header[0].Style.Font.Bold = true;
            header[0].Style.Font.Color = Color.White;
            header[0].Style.Alignment.Horizontal = HorizontalAlignment.Center;
            header[0].Style.Border.Bottom = new BorderEdge { Style = BorderStyle.Medium, Color = Color.White };
            header[0].Style.Fill = CellFill.Solid(new Color(128, 128, 128));

            header[1].Value = "Time";

            header[1].Style.Font.Bold = true;
            header[1].Style.Font.Color = Color.White;
            header[1].Style.Alignment.Horizontal = HorizontalAlignment.Center;
            header[1].Style.Border.Bottom = new BorderEdge { Style = BorderStyle.Medium, Color = Color.White };
            header[1].Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
            header[1].Style.Fill = CellFill.Solid(new Color(128, 128, 128));

            var rowIndex = 1;
            var bestTime = TimeSpan.MaxValue;
            foreach (var runHistoryElement in run.AttemptHistory)
            {
                var row = sheet.Data.Rows[rowIndex];

                row[0].Value = runHistoryElement.Index;
                row[0].Style.Fill = CellFill.Solid(
                    ((rowIndex & 1) == 1)
                    ? new Color(221, 221, 221)
                    : new Color(238, 238, 238));

                var cell = row[1];

                cell.Style.Fill = CellFill.Solid(
                    ((rowIndex & 1) == 1)
                    ? new Color(221, 221, 221)
                    : new Color(238, 238, 238));

                var time = runHistoryElement.Time.RealTime;
                if (time.HasValue)
                {
                    cell.Value = time.Value.TotalDays;
                    if (time.Value < bestTime)
                    {
                        bestTime = time.Value;
                        cell.Style.Fill = CellFill.Solid(
                            ((rowIndex & 1) == 1)
                            ? new Color(201, 231, 201)
                            : new Color(218, 248, 218));
                    }
                }

                cell.Style.Alignment.Horizontal = HorizontalAlignment.Right;
                cell.Style.Format = "[HH]:MM:SS.00";
                cell.Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };


                ++rowIndex;
            }

            sheet.AutoFilter = sheet[0, 0, rowIndex - 1, 1];
        }

        private void FillSegmentHistorySheet(Sheet sheet, IRun run)
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
            foreach (var runHistoryElement in run.AttemptHistory)
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


            sheet.AutoFilter = sheet[0, 0, rowIndex - 1, columnIndex - 1];
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

            var lastTime = TimeSpan.Zero;

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
                    if (comparisonName == Run.PersonalBestComparisonName && time.HasValue && segment.BestSegmentTime.RealTime == (time.Value - lastTime))
                    {
                        cell.Style.Fill = CellFill.Solid(
                            ((rowIndex & 1) == 1)
                            ? new Color(241, 231, 181)
                            : new Color(255, 245, 198));
                    }
                    else
                    {
                        cell.Style.Fill = CellFill.Solid(
                            ((rowIndex & 1) == 1)
                            ? new Color(221, 221, 221)
                            : new Color(238, 238, 238));
                    }

                    if (comparisonName == Run.PersonalBestComparisonName && time.HasValue)
                        lastTime = time.Value;

                    columnIndex++;
                }

                ++rowIndex;
            }

            sheet.AutoFilter = sheet[0, 0, rowIndex - 1, columnIndex - 1];
        }
    }
}
