using Codaxy.Xlio;
using LiveSplit.Model.Comparisons;
using System;
using System.IO;
using System.Linq;

namespace LiveSplit.Model.RunSavers
{
    public class ExcelRunSaver : IRunSaver
    {
        public void Save(IRun run, Stream stream)
        {
            var workbook = new Workbook();
            var splitsRealTimeSheet = workbook.Sheets.AddSheet("Splits (Real Time)");
            var attemptHistoryRealTimeSheet = workbook.Sheets.AddSheet("Attempt History (Real Time)");
            var segmentHistoryRealTimeSheet = workbook.Sheets.AddSheet("Segment History (Real Time)");
            var splitsGameTimeSheet = workbook.Sheets.AddSheet("Splits (Game Time)");
            var attemptHistoryGameTimeSheet = workbook.Sheets.AddSheet("Attempt History (Game Time)");
            var segmentHistoryGameTimeSheet = workbook.Sheets.AddSheet("Segment History (Game Time)");

            FillSplitTimesSheet(splitsRealTimeSheet, run, TimingMethod.RealTime);
            FillAttemptHistorySheet(attemptHistoryRealTimeSheet, run, TimingMethod.RealTime);
            FillSegmentHistorySheet(segmentHistoryRealTimeSheet, run, TimingMethod.RealTime);
            FillSplitTimesSheet(splitsGameTimeSheet, run, TimingMethod.GameTime);
            FillAttemptHistorySheet(attemptHistoryGameTimeSheet, run, TimingMethod.GameTime);
            FillSegmentHistorySheet(segmentHistoryGameTimeSheet, run, TimingMethod.GameTime);

            workbook.SaveToStream(stream, Codaxy.Xlio.IO.XlsxFileWriterOptions.AutoFit);
        }

        private void FillAttemptHistorySheet(Sheet sheet, IRun run, TimingMethod method)
        {
            var attemptIdColumn = 0;
            var startedColumn = 1;
            var endedColumn = 2;
            var timeColumn = 3;

            var header = sheet.Data.Rows[0];

            header[attemptIdColumn].Value = "Attempt ID";
            header[attemptIdColumn].Style.Font.Bold = true;
            header[attemptIdColumn].Style.Font.Color = Color.White;
            header[attemptIdColumn].Style.Alignment.Horizontal = HorizontalAlignment.Center;
            header[attemptIdColumn].Style.Border.Bottom = new BorderEdge { Style = BorderStyle.Medium, Color = Color.White };
            header[attemptIdColumn].Style.Fill = CellFill.Solid(new Color(128, 128, 128));

            header[startedColumn].Value = "Started";

            header[startedColumn].Style.Font.Bold = true;
            header[startedColumn].Style.Font.Color = Color.White;
            header[startedColumn].Style.Alignment.Horizontal = HorizontalAlignment.Center;
            header[startedColumn].Style.Border.Bottom = new BorderEdge { Style = BorderStyle.Medium, Color = Color.White };
            header[startedColumn].Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
            header[startedColumn].Style.Fill = CellFill.Solid(new Color(128, 128, 128));

            header[endedColumn].Value = "Ended";

            header[endedColumn].Style.Font.Bold = true;
            header[endedColumn].Style.Font.Color = Color.White;
            header[endedColumn].Style.Alignment.Horizontal = HorizontalAlignment.Center;
            header[endedColumn].Style.Border.Bottom = new BorderEdge { Style = BorderStyle.Medium, Color = Color.White };
            header[endedColumn].Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
            header[endedColumn].Style.Fill = CellFill.Solid(new Color(128, 128, 128));

            header[timeColumn].Value = "Time";

            header[timeColumn].Style.Font.Bold = true;
            header[timeColumn].Style.Font.Color = Color.White;
            header[timeColumn].Style.Alignment.Horizontal = HorizontalAlignment.Center;
            header[timeColumn].Style.Border.Bottom = new BorderEdge { Style = BorderStyle.Medium, Color = Color.White };
            header[timeColumn].Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
            header[timeColumn].Style.Fill = CellFill.Solid(new Color(128, 128, 128));

            var rowIndex = 1;
            var bestTime = TimeSpan.MaxValue;
            foreach (var attempt in run.AttemptHistory)
            {
                var row = sheet.Data.Rows[rowIndex];

                row[attemptIdColumn].Value = attempt.Index;
                row[attemptIdColumn].Style.Fill = CellFill.Solid(
                    ((rowIndex & 1) == 1)
                    ? new Color(221, 221, 221)
                    : new Color(238, 238, 238));

                var startedCell = row[startedColumn];
                startedCell.Style.Fill = CellFill.Solid(
                    ((rowIndex & 1) == 1)
                    ? new Color(221, 221, 221)
                    : new Color(238, 238, 238));
                startedCell.Style.Alignment.Horizontal = HorizontalAlignment.Right;
                startedCell.Style.Format = "dd mmm yy hh:mm:ss";
                startedCell.Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
                if (attempt.Started.HasValue)
                    startedCell.Value = attempt.Started.Value.Time;

                var endedCell = row[endedColumn];
                endedCell.Style.Fill = CellFill.Solid(
                    ((rowIndex & 1) == 1)
                    ? new Color(221, 221, 221)
                    : new Color(238, 238, 238));
                endedCell.Style.Alignment.Horizontal = HorizontalAlignment.Right;
                endedCell.Style.Format = "dd mmm yy hh:mm:ss";
                endedCell.Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
                if (attempt.Ended.HasValue)
                    endedCell.Value = attempt.Ended.Value.Time;

                var timeCell = row[timeColumn];

                timeCell.Style.Fill = CellFill.Solid(
                    ((rowIndex & 1) == 1)
                    ? new Color(221, 221, 221)
                    : new Color(238, 238, 238));

                var time = attempt.Time[method];
                if (time.HasValue)
                {
                    timeCell.Value = time.Value.TotalDays;
                    if (time.Value < bestTime)
                    {
                        bestTime = time.Value;
                        timeCell.Style.Fill = CellFill.Solid(
                            ((rowIndex & 1) == 1)
                            ? new Color(201, 231, 201)
                            : new Color(218, 248, 218));
                    }
                }

                timeCell.Style.Alignment.Horizontal = HorizontalAlignment.Right;
                timeCell.Style.Format = "[HH]:MM:SS.00";
                timeCell.Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };


                ++rowIndex;
            }

            sheet.AutoFilter = sheet[0, 0, rowIndex - 1, 3];
        }

        private static void FillSegmentHistorySheet(Sheet sheet, IRun run, TimingMethod method)
        {
            var header = sheet.Data.Rows[0];

            header[0].Value = "Attempt ID";
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
            foreach (var attempt in run.AttemptHistory)
            {
                var row = sheet.Data.Rows[rowIndex];

                row[0].Value = attempt.Index;
                row[0].Style.Fill = CellFill.Solid(
                    ((rowIndex & 1) == 1)
                    ? new Color(221, 221, 221)
                    : new Color(238, 238, 238));
                columnIndex = 1;
                foreach (var segment in run)
                {
                    var cell = row[columnIndex];
                    Time segmentHistoryElement;
                    if (segment.SegmentHistory.TryGetValue(attempt.Index, out segmentHistoryElement))
                    {
                        var time = segmentHistoryElement[method];
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

        private static void FillSplitTimesSheet(Sheet sheet, IRun run, TimingMethod method)
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
                    var time = segment.Comparisons[comparisonName][method];
                    if (time.HasValue)
                        cell.Value = time.Value.TotalDays;

                    cell.Style.Alignment.Horizontal = HorizontalAlignment.Right;
                    cell.Style.Format = "[HH]:MM:SS.00";
                    cell.Style.Border.Left = new BorderEdge { Style = BorderStyle.Thin, Color = Color.White };
                    if (comparisonName == Run.PersonalBestComparisonName && time.HasValue && segment.BestSegmentTime[method] == (time.Value - lastTime))
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
