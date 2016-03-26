using CLROBS;
using LiveSplit.Options;
using LiveSplit.View;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace LiveSplit.Plugin
{
    public class LiveSplitImageSource : AbstractImageSource, IDisposable
    {
        private XElement config;
        private TimerForm form;
        private Texture texture;
        bool isAnchorBottom;
        bool isAnchorRight;
        string splitsPath;
        string layoutPath;

        public LiveSplitImageSource(XElement config)
        {
            this.config = config;
            LoadSettings();
        }

        public void LoadSettings()
        {
            isAnchorRight = config.GetInt("anchorright") != 0;
            isAnchorBottom = config.GetInt("anchorbottom") != 0;
            splitsPath = config.GetString("splitspath");
            layoutPath = config.GetString("layoutpath");
        }

        public override void UpdateSettings()
        {
            LoadSettings();
        }

        public override void BeginScene()
        {
            var formThread = new Thread(() =>
            {
                Application.EnableVisualStyles();

                form = new TimerForm(
                    splitsPath: splitsPath,
                    layoutPath: layoutPath,
                    drawToBackBuffer: true,
                    basePath: File.ReadAllText(@"plugins\CLRHostPlugin\livesplit.cfg"));
                Application.Run(form);
            }) { Name = "LiveSplit Form Thread" };

            formThread.SetApartmentState(ApartmentState.STA);
            formThread.Start();
        }

        public override void EndScene()
        {
            try
            {
                if (form.Settings.RecentSplits.Any())
                    config.SetString("splitspath", form.Settings.RecentSplits.Last().Path);
                if (form.Settings.RecentLayouts.Any())
                    config.SetString("layoutpath", form.Settings.RecentLayouts.Last());

                var sem = new Semaphore(0, 1);
                Action formCloseAction = () =>
                    {
                        form.TopMost = false;

                        while (form.Visible)
                            form.Close();
                        sem.Release();
                    };

                if (form.InvokeRequired)
                    form.Invoke(formCloseAction);
                else
                    formCloseAction();

                sem.WaitOne();
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                API.Instance.Log(ex.Message);
                API.Instance.Log(ex.StackTrace);
            }
        }

        public override void Preprocess()
        {
            if (form == null || !form.Visible)
                return;

            lock (form.BackBufferLock)
            {
                var buffer = form.BackBuffer;

                if (texture == null || texture.Width != buffer.Width || texture.Height != buffer.Height)
                {
                    if (texture != null)
                        texture.Dispose();

                    texture = GS.CreateTexture((uint)buffer.Width, (uint)buffer.Height, GSColorFormat.GS_BGRA, null, false, false);
                    Size = new Vector2(buffer.Width, buffer.Height);
                }

                Rectangle rect = new Rectangle(0, 0, buffer.Width, buffer.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    buffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    buffer.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * buffer.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                buffer.UnlockBits(bmpData);

                texture.SetImage(rgbValues, GSImageFormat.GS_IMAGEFORMAT_BGRA, (uint)(buffer.Width * 4));
            }
        }

        public override void Render(float x, float y, float width, float height)
        {
            if (form == null || !form.Visible || texture == null)
                return;

            lock (form.BackBufferLock)
            {
                var buffer = form.BackBuffer;

                float renderWidth, renderHeight;

                if (buffer.Width / (float)buffer.Height > width / height)
                {
                    renderWidth = width;

                    var scaleFactor = width / buffer.Width;
                    renderHeight = scaleFactor * buffer.Height;
                    if (isAnchorBottom)
                        y += height - renderHeight;
                }
                else
                {
                    renderHeight = height;

                    var scaleFactor = height / buffer.Height;
                    renderWidth = scaleFactor * buffer.Width;
                    if (isAnchorRight)
                        x += width - renderWidth;
                }

                GS.DrawSprite(texture, 0xFFFFFFFF, x, y, x + renderWidth, y + renderHeight);
            }
        }

        public void Dispose()
        {
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }
        }

        ~LiveSplitImageSource()
        {
            Dispose();
        }
    }
}
