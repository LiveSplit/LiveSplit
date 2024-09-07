using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using WinFormsColor;

namespace Fetze.WinFormsColor;

public partial class ColorPickerDialog : Form
{
    public enum PrimaryAttrib
    {
        Hue,
        Saturation,
        Brightness,
        Red,
        Green,
        Blue
    }

    private struct InternalColor
    {
        public float h;
        public float s;
        public float v;
        public float a;

        public InternalColor(float h, float s, float v, float a)
        {
            this.h = h;
            this.s = s;
            this.v = v;
            this.a = a;
        }
        public InternalColor(Color c)
        {
            h = c.GetHSVHue();
            s = c.GetHSVSaturation();
            v = c.GetHSVBrightness();
            a = c.A / 255.0f;
        }
        public InternalColor(InternalColor c)
        {
            h = c.h;
            s = c.s;
            v = c.v;
            a = c.a;
        }

        public readonly Color ToColor()
        {
            return Color.FromArgb((int)Math.Round(a * 255.0f), ExtMethodsSystemDrawingColor.ColorFromHSV(h, s, v));
        }
    }

    private bool alphaEnabled = true;
    private InternalColor oldColor = new(Color.Red);
    private InternalColor _selColor = new(Color.Red);
    private InternalColor selColor
    {
        get => _selColor;
        set
        {
            SelectedColorChanged?.Invoke(this, new EventArgs());

            _selColor = value;
        }
    }
    private PrimaryAttrib primAttrib = PrimaryAttrib.Hue;
    private bool suspendTextEvents = false;

    public event EventHandler SelectedColorChanged;

    public bool AlphaEnabled
    {
        get => alphaEnabled;
        set
        {
            alphaEnabled = value;
            alphaSlider.Enabled = alphaEnabled;
            numAlpha.Enabled = alphaEnabled;
        }
    }
    public Color OldColor
    {
        get => oldColor.ToColor();
        set
        {
            oldColor = new InternalColor(value);
            UpdateColorShowBox();
        }
    }
    public Color SelectedColor
    {
        get => selColor.ToColor();
        set
        {
            selColor = new InternalColor(value);
            UpdateColorControls();
        }
    }
    public PrimaryAttrib PrimaryAttribute
    {
        get => primAttrib;
        set
        {
            primAttrib = value;
            UpdateColorControls();
        }
    }

    public ColorPickerDialog()
    {
        InitializeComponent();
        btnPickColor.BackgroundImage = Resources.Colorpicker;
        btnPickColor.BackgroundImageLayout = ImageLayout.Zoom;
    }

    private void UpdateColorControls()
    {
        UpdatePrimaryAttributeRadioBox();
        UpdateText();
        UpdateColorShowBox();
        UpdateColorPanelGradient();
        UpdateColorSliderGradient();
        UpdateAlphaSliderGradient();
        UpdateColorPanelValue();
        UpdateColorSliderValue();
        UpdateAlphaSliderValue();
    }
    private void UpdatePrimaryAttributeRadioBox()
    {
        switch (primAttrib)
        {
            default:
            case PrimaryAttrib.Hue:
                radioHue.Checked = true;
                break;
            case PrimaryAttrib.Saturation:
                radioSaturation.Checked = true;
                break;
            case PrimaryAttrib.Brightness:
                radioValue.Checked = true;
                break;
            case PrimaryAttrib.Red:
                radioRed.Checked = true;
                break;
            case PrimaryAttrib.Green:
                radioGreen.Checked = true;
                break;
            case PrimaryAttrib.Blue:
                radioBlue.Checked = true;
                break;
        }
    }
    private void UpdateText()
    {
        var tmp = selColor.ToColor();
        suspendTextEvents = true;
        int color = tmp.ToArgb();
        if (!AlphaEnabled)
        {
            color &= 0xffffff;
        }

        textBoxHex.Text = string.Format("{0:X}", color);

        numRed.Value = tmp.R;
        numGreen.Value = tmp.G;
        numBlue.Value = tmp.B;
        numAlpha.Value = tmp.A;

        numHue.Value = (decimal)(selColor.h * 360.0f);
        numSaturation.Value = (decimal)(selColor.s * 100.0f);
        numValue.Value = (decimal)(selColor.v * 100.0f);

        suspendTextEvents = false;
    }
    private void UpdateColorShowBox()
    {
        colorShowBox.UpperColor = alphaEnabled ? oldColor.ToColor() : Color.FromArgb(255, oldColor.ToColor());
        colorShowBox.LowerColor = alphaEnabled ? selColor.ToColor() : Color.FromArgb(255, selColor.ToColor());
    }
    private void UpdateColorPanelGradient()
    {
        Color tmp;
        switch (primAttrib)
        {
            default:
            case PrimaryAttrib.Hue:
                colorPanel.SetupXYGradient(
                    Color.White,
                    ExtMethodsSystemDrawingColor.ColorFromHSV(selColor.h, 1.0f, 1.0f),
                    Color.Black,
                    Color.Transparent);
                break;
            case PrimaryAttrib.Saturation:
                colorPanel.SetupHueBrightnessGradient(selColor.s);
                break;
            case PrimaryAttrib.Brightness:
                colorPanel.SetupHueSaturationGradient(selColor.v);
                break;
            case PrimaryAttrib.Red:
                tmp = selColor.ToColor();
                colorPanel.SetupGradient(
                    Color.FromArgb(255, tmp.R, 255, 0),
                    Color.FromArgb(255, tmp.R, 255, 255),
                    Color.FromArgb(255, tmp.R, 0, 0),
                    Color.FromArgb(255, tmp.R, 0, 255),
                    32);
                break;
            case PrimaryAttrib.Green:
                tmp = selColor.ToColor();
                colorPanel.SetupGradient(
                    Color.FromArgb(255, 255, tmp.G, 0),
                    Color.FromArgb(255, 255, tmp.G, 255),
                    Color.FromArgb(255, 0, tmp.G, 0),
                    Color.FromArgb(255, 0, tmp.G, 255),
                    32);
                break;
            case PrimaryAttrib.Blue:
                tmp = selColor.ToColor();
                colorPanel.SetupGradient(
                    Color.FromArgb(255, 255, 0, tmp.B),
                    Color.FromArgb(255, 255, 255, tmp.B),
                    Color.FromArgb(255, 0, 0, tmp.B),
                    Color.FromArgb(255, 0, 255, tmp.B),
                    32);
                break;
        }
    }
    private void UpdateColorPanelValue()
    {
        Color tmp;
        switch (primAttrib)
        {
            default:
            case PrimaryAttrib.Hue:
                colorPanel.ValuePercentual = new PointF(
                    selColor.s,
                    selColor.v);
                break;
            case PrimaryAttrib.Saturation:
                colorPanel.ValuePercentual = new PointF(
                    selColor.h,
                    selColor.v);
                break;
            case PrimaryAttrib.Brightness:
                colorPanel.ValuePercentual = new PointF(
                    selColor.h,
                    selColor.s);
                break;
            case PrimaryAttrib.Red:
                tmp = selColor.ToColor();
                colorPanel.ValuePercentual = new PointF(
                    tmp.B / 255.0f,
                    tmp.G / 255.0f);
                break;
            case PrimaryAttrib.Green:
                tmp = selColor.ToColor();
                colorPanel.ValuePercentual = new PointF(
                    tmp.B / 255.0f,
                    tmp.R / 255.0f);
                break;
            case PrimaryAttrib.Blue:
                tmp = selColor.ToColor();
                colorPanel.ValuePercentual = new PointF(
                    tmp.G / 255.0f,
                    tmp.R / 255.0f);
                break;
        }
    }
    private void UpdateColorSliderGradient()
    {
        Color tmp;
        switch (primAttrib)
        {
            default:
            case PrimaryAttrib.Hue:
                colorSlider.SetupHueGradient(/*this.selColor.GetHSVSaturation(), this.selColor.GetHSVBrightness()*/);
                break;
            case PrimaryAttrib.Saturation:
                colorSlider.SetupGradient(
                    ExtMethodsSystemDrawingColor.ColorFromHSV(selColor.h, 0.0f, selColor.v),
                    ExtMethodsSystemDrawingColor.ColorFromHSV(selColor.h, 1.0f, selColor.v));
                break;
            case PrimaryAttrib.Brightness:
                colorSlider.SetupGradient(
                    ExtMethodsSystemDrawingColor.ColorFromHSV(selColor.h, selColor.s, 0.0f),
                    ExtMethodsSystemDrawingColor.ColorFromHSV(selColor.h, selColor.s, 1.0f));
                break;
            case PrimaryAttrib.Red:
                tmp = selColor.ToColor();
                colorSlider.SetupGradient(
                    Color.FromArgb(255, 0, tmp.G, tmp.B),
                    Color.FromArgb(255, 255, tmp.G, tmp.B));
                break;
            case PrimaryAttrib.Green:
                tmp = selColor.ToColor();
                colorSlider.SetupGradient(
                    Color.FromArgb(255, tmp.R, 0, tmp.B),
                    Color.FromArgb(255, tmp.R, 255, tmp.B));
                break;
            case PrimaryAttrib.Blue:
                tmp = selColor.ToColor();
                colorSlider.SetupGradient(
                    Color.FromArgb(255, tmp.R, tmp.G, 0),
                    Color.FromArgb(255, tmp.R, tmp.G, 255));
                break;
        }
    }
    private void UpdateColorSliderValue()
    {
        Color tmp;
        switch (primAttrib)
        {
            default:
            case PrimaryAttrib.Hue:
                colorSlider.ValuePercentual = selColor.h;
                break;
            case PrimaryAttrib.Saturation:
                colorSlider.ValuePercentual = selColor.s;
                break;
            case PrimaryAttrib.Brightness:
                colorSlider.ValuePercentual = selColor.v;
                break;
            case PrimaryAttrib.Red:
                tmp = selColor.ToColor();
                colorSlider.ValuePercentual = tmp.R / 255.0f;
                break;
            case PrimaryAttrib.Green:
                tmp = selColor.ToColor();
                colorSlider.ValuePercentual = tmp.G / 255.0f;
                break;
            case PrimaryAttrib.Blue:
                tmp = selColor.ToColor();
                colorSlider.ValuePercentual = tmp.B / 255.0f;
                break;
        }
    }
    private void UpdateAlphaSliderGradient()
    {
        alphaSlider.SetupGradient(Color.Transparent, Color.FromArgb(255, selColor.ToColor()));
    }
    private void UpdateAlphaSliderValue()
    {
        alphaSlider.ValuePercentual = selColor.a;
    }

    private void UpdateSelectedColorFromSliderValue()
    {
        Color tmp;
        switch (primAttrib)
        {
            default:
            case PrimaryAttrib.Hue:
                selColor = new InternalColor(selColor) { h = colorSlider.ValuePercentual };
                break;
            case PrimaryAttrib.Saturation:
                selColor = new InternalColor(selColor) { s = colorSlider.ValuePercentual };
                break;
            case PrimaryAttrib.Brightness:
                selColor = new InternalColor(selColor) { v = colorSlider.ValuePercentual };
                break;
            case PrimaryAttrib.Red:
                tmp = selColor.ToColor();
                selColor = new InternalColor(Color.FromArgb(
                    tmp.A,
                    (int)Math.Round(colorSlider.ValuePercentual * 255.0f),
                    tmp.G,
                    tmp.B));
                break;
            case PrimaryAttrib.Green:
                tmp = selColor.ToColor();
                selColor = new InternalColor(Color.FromArgb(
                    tmp.A,
                    tmp.R,
                    (int)Math.Round(colorSlider.ValuePercentual * 255.0f),
                    tmp.B));
                break;
            case PrimaryAttrib.Blue:
                tmp = selColor.ToColor();
                selColor = new InternalColor(Color.FromArgb(
                    tmp.A,
                    tmp.R,
                    tmp.G,
                    (int)Math.Round(colorSlider.ValuePercentual * 255.0f)));
                break;
        }
    }
    private void UpdateSelectedColorFromPanelValue()
    {
        Color tmp;
        switch (primAttrib)
        {
            default:
            case PrimaryAttrib.Hue:
                selColor = new InternalColor(selColor) { s = colorPanel.ValuePercentual.X };
                selColor = new InternalColor(selColor) { v = colorPanel.ValuePercentual.Y };
                break;
            case PrimaryAttrib.Saturation:
                selColor = new InternalColor(selColor) { h = colorPanel.ValuePercentual.X };
                selColor = new InternalColor(selColor) { v = colorPanel.ValuePercentual.Y };
                break;
            case PrimaryAttrib.Brightness:
                selColor = new InternalColor(selColor) { h = colorPanel.ValuePercentual.X };
                selColor = new InternalColor(selColor) { s = colorPanel.ValuePercentual.Y };
                break;
            case PrimaryAttrib.Red:
                tmp = selColor.ToColor();
                selColor = new InternalColor(Color.FromArgb(
                    tmp.A,
                    tmp.R,
                    (int)Math.Round(colorPanel.ValuePercentual.Y * 255.0f),
                    (int)Math.Round(colorPanel.ValuePercentual.X * 255.0f)));
                break;
            case PrimaryAttrib.Green:
                tmp = selColor.ToColor();
                selColor = new InternalColor(Color.FromArgb(
                    tmp.A,
                    (int)Math.Round(colorPanel.ValuePercentual.Y * 255.0f),
                    tmp.G,
                    (int)Math.Round(colorPanel.ValuePercentual.X * 255.0f)));
                break;
            case PrimaryAttrib.Blue:
                tmp = selColor.ToColor();
                selColor = new InternalColor(Color.FromArgb(
                    tmp.A,
                    (int)Math.Round(colorPanel.ValuePercentual.Y * 255.0f),
                    (int)Math.Round(colorPanel.ValuePercentual.X * 255.0f),
                    tmp.B));
                break;
        }
    }
    private void UpdateSelectedColorFromAlphaValue()
    {
        selColor = new InternalColor(selColor) { a = alphaSlider.ValuePercentual };
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        selColor = oldColor;
        UpdateColorControls();
    }

    private void radioHue_CheckedChanged(object sender, EventArgs e)
    {
        if (radioHue.Checked)
        {
            PrimaryAttribute = PrimaryAttrib.Hue;
        }
    }
    private void radioSaturation_CheckedChanged(object sender, EventArgs e)
    {
        if (radioSaturation.Checked)
        {
            PrimaryAttribute = PrimaryAttrib.Saturation;
        }
    }
    private void radioValue_CheckedChanged(object sender, EventArgs e)
    {
        if (radioValue.Checked)
        {
            PrimaryAttribute = PrimaryAttrib.Brightness;
        }
    }
    private void radioRed_CheckedChanged(object sender, EventArgs e)
    {
        if (radioRed.Checked)
        {
            PrimaryAttribute = PrimaryAttrib.Red;
        }
    }
    private void radioGreen_CheckedChanged(object sender, EventArgs e)
    {
        if (radioGreen.Checked)
        {
            PrimaryAttribute = PrimaryAttrib.Green;
        }
    }
    private void radioBlue_CheckedChanged(object sender, EventArgs e)
    {
        if (radioBlue.Checked)
        {
            PrimaryAttribute = PrimaryAttrib.Blue;
        }
    }

    private void colorPanel_PercentualValueChanged(object sender, EventArgs e)
    {
        if (ContainsFocus)
        {
            UpdateSelectedColorFromPanelValue();
        }

        UpdateColorSliderGradient();
        UpdateAlphaSliderGradient();
        UpdateColorShowBox();
        UpdateText();
    }
    private void colorSlider_PercentualValueChanged(object sender, EventArgs e)
    {
        if (ContainsFocus)
        {
            UpdateSelectedColorFromSliderValue();
        }

        UpdateColorPanelGradient();
        UpdateAlphaSliderGradient();
        UpdateColorShowBox();
        UpdateText();
    }
    private void alphaSlider_PercentualValueChanged(object sender, EventArgs e)
    {
        if (ContainsFocus)
        {
            UpdateSelectedColorFromAlphaValue();
        }

        UpdateColorSliderGradient();
        UpdateColorPanelGradient();
        UpdateColorShowBox();
        UpdateText();
    }

    private void numHue_ValueChanged(object sender, EventArgs e)
    {
        if (suspendTextEvents)
        {
            return;
        }

        selColor = new InternalColor(selColor) { h = (float)numHue.Value / 360.0f };
        UpdateColorControls();
    }
    private void numSaturation_ValueChanged(object sender, EventArgs e)
    {
        if (suspendTextEvents)
        {
            return;
        }

        selColor = new InternalColor(selColor) { s = (float)numSaturation.Value / 100.0f };
        UpdateColorControls();
    }
    private void numValue_ValueChanged(object sender, EventArgs e)
    {
        if (suspendTextEvents)
        {
            return;
        }

        selColor = new InternalColor(selColor) { v = (float)numValue.Value / 100.0f };
        UpdateColorControls();
    }
    private void numRed_ValueChanged(object sender, EventArgs e)
    {
        if (suspendTextEvents)
        {
            return;
        }

        var tmp = selColor.ToColor();
        selColor = new InternalColor(Color.FromArgb(
            tmp.A,
            (byte)numRed.Value,
            tmp.G,
            tmp.B));
        UpdateColorControls();
    }
    private void numGreen_ValueChanged(object sender, EventArgs e)
    {
        if (suspendTextEvents)
        {
            return;
        }

        var tmp = selColor.ToColor();
        selColor = new InternalColor(Color.FromArgb(
            tmp.A,
            tmp.R,
            (byte)numGreen.Value,
            tmp.B));
        UpdateColorControls();
    }
    private void numBlue_ValueChanged(object sender, EventArgs e)
    {
        if (suspendTextEvents)
        {
            return;
        }

        var tmp = selColor.ToColor();
        selColor = new InternalColor(Color.FromArgb(
            tmp.A,
            tmp.R,
            tmp.G,
            (byte)numBlue.Value));
        UpdateColorControls();
    }
    private void numAlpha_ValueChanged(object sender, EventArgs e)
    {
        if (suspendTextEvents)
        {
            return;
        }

        var tmp = selColor.ToColor();
        selColor = new InternalColor(Color.FromArgb(
            (byte)numAlpha.Value,
            tmp.R,
            tmp.G,
            tmp.B));
        UpdateColorControls();
    }
    private void textBoxHex_TextChanged(object sender, EventArgs e)
    {
        if (suspendTextEvents)
        {
            return;
        }

        if (int.TryParse(textBoxHex.Text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentUICulture, out int argb))
        {
            var tmp = Color.FromArgb(argb);
            if (!AlphaEnabled)
            {
                tmp = Color.FromArgb(255, tmp);
            }

            selColor = new InternalColor(tmp);
            UpdateColorControls();
        }
    }

    private void colorShowBox_UpperClick(object sender, EventArgs e)
    {
        selColor = oldColor;
        UpdateColorControls();
    }

    private void buttonOk_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Hide();
    }
    private void ColorPickerDialog_FormClosed(object sender, FormClosedEventArgs e)
    {
        if (DialogResult == DialogResult.Cancel)
        {
            SelectedColor = OldColor;
        }
        else
        {
            OldColor = SelectedColor;
        }
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

    private Color GetPixelColor(int x, int y)
    {
        IntPtr hdc = GetDC(IntPtr.Zero);
        uint pixel = GetPixel(hdc, x, y);
        ReleaseDC(IntPtr.Zero, hdc);
        var color = Color.FromArgb((int)(pixel & 0x000000FF),
                     (int)(pixel & 0x0000FF00) >> 8,
                     (int)(pixel & 0x00FF0000) >> 16);
        return color;
    }

    private void btnPickColor_Click(object sender, EventArgs e)
    {

    }

    private void btnPickColor_MouseDown(object sender, MouseEventArgs e)
    {
        MouseHook.Start();
        bool pick = true;
        Cursor = new Cursor(Resources.ColorpickerCursor.GetHicon());
        MouseHook.MouseAction += (s, x) =>
        {
            MouseHook.stop();
            pick = false;
            Cursor = Cursors.Default;
        };
        Action<Color> setColor = x => SelectedColor = x;
        new System.Threading.Thread(() =>
            {
                while (pick)
                {
                    Color color = GetPixelColor(Cursor.Position.X, Cursor.Position.Y);
                    if (InvokeRequired)
                    {
                        Invoke(setColor, color);
                    }
                    else
                    {
                        setColor(color);
                    }

                    System.Threading.Thread.Sleep(10);
                }
            }).Start();
    }
}
