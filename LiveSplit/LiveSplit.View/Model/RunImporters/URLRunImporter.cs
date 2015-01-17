using LiveSplit.Model.Comparisons;
using LiveSplit.Model.RunFactories;
using LiveSplit.Options;
using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.Model.RunImporters
{
    public class URLRunImporter : IRunImporter
    {
        private IRun LoadRunFromURL(string url, Form form = null)
        {
            try
            {
                var runFactory = new StandardFormatsRunFactory();
                var comparisonGeneratorsFactory = new StandardComparisonGeneratorsFactory();

                var uri = new Uri(url);
                if (uri.Host.ToLowerInvariant() == "splits.io"
                    && uri.LocalPath.Length > 0
                    && !uri.LocalPath.Substring(1).Contains('/'))
                {
                    uri = new Uri(string.Format("{0}/download/livesplit", url));
                }
                if (uri.Host.ToLowerInvariant() == "ge.tt"
                    && uri.LocalPath.Length > 0
                    && !uri.LocalPath.Substring(1).Contains('/'))
                {
                    uri = new Uri(string.Format("http://ge.tt/api/1/files{0}/0/blob?download", uri.LocalPath));
                }

                var request = WebRequest.Create(uri);
                using (var stream = request.GetResponse().GetResponseStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        runFactory.Stream = memoryStream;
                        runFactory.FilePath = null;

                        try
                        {
                            return runFactory.Create(comparisonGeneratorsFactory);

                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                            MessageBox.Show(form, "The selected file was not recognized as a splits file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show(form, "The splits file couldn't be downloaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public IRun Import(Form form = null)
        {
            string url = null;
            if (DialogResult.OK == InputBox.Show(form, "Open Splits from URL", "URL:", ref url))
            {
                return LoadRunFromURL(url, form);
            }
            return null;
        }

        public void ImportAsComparison(IRun run, Form form = null)
        {
            string url = null;
            string name = null;
            if (DialogResult.OK == InputBox.Show(form, "Import Comparison from URL", "Name:", "URL:", ref name, ref url))
            {
                var imported = LoadRunFromURL(url, form);
                run.AddComparisonWithNameInput(imported, name, form);
            }
        }
    }
}
