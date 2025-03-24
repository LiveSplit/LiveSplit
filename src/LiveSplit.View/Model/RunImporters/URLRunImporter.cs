using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

using LiveSplit.Model.Comparisons;
using LiveSplit.Model.RunFactories;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.Web.Share;

namespace LiveSplit.Model.RunImporters;

public class URLRunImporter : IRunImporter
{
    private static IRun LoadRunFromURL(string url, Form form = null)
    {
        try
        {
            var runFactory = new StandardFormatsRunFactory();
            var comparisonGeneratorsFactory = new StandardComparisonGeneratorsFactory();

            var uri = new Uri(url);

            var request = WebRequest.Create(uri);

            using WebResponse response = request.GetResponse();
            using Stream stream = response.GetResponseStream();
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            runFactory.Stream = memoryStream;
            runFactory.FilePath = "";

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

    public string ImportAsComparison(IRun run, Form form = null)
    {
        string url = null;
        string name = null;
        if (DialogResult.OK == InputBox.Show(form, "Import Comparison from URL", "Name:", "URL:", ref name, ref url))
        {
            IRun imported = LoadRunFromURL(url, form);
            return run.AddComparisonWithNameInput(imported, name, form);
        }

        return null;
    }
}
