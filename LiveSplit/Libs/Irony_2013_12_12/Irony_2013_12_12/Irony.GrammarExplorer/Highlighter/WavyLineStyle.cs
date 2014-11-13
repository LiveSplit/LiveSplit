// Written by Alexey Yakovlev <yallie@yandex.ru>
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FastColoredTextBoxNS;

namespace Irony.GrammarExplorer.Highlighter {
  /// <summary>
  /// This style draws a wavy line below a given text range.
  /// </summary>
  public class WavyLineStyle : Style {

    public WavyLineStyle(int alpha, Color color) {
      Pen = new Pen(Color.FromArgb(alpha, color));
    }

    private Pen Pen { get; set; }

    public override void Draw(Graphics gr, Point pos, Range range) {
      var size = GetSizeOfRange(range);
      var start = new Point(pos.X, pos.Y + size.Height - 1);
      var end = new Point(pos.X + size.Width, pos.Y + size.Height - 1);
      DrawWavyLine(gr, start, end);
    }

    private void DrawWavyLine(Graphics graphics, Point start, Point end) {
      if (end.X - start.X < 2) {
        graphics.DrawLine(Pen, start, end);
        return;
      }

      var offset = -1;
      var points = new List<Point>();
      for (int i = start.X; i <= end.X; i += 2) {
        points.Add(new Point(i, start.Y + offset));
        offset = -offset;
      }

      graphics.DrawLines(Pen, points.ToArray());
    }
  }
}
