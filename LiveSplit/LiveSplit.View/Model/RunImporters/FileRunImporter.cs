using LiveSplit.Model.Comparisons;
using LiveSplit.Model.RunFactories;
using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.Model.RunImporters
{
    public class FileRunImporter : IRunImporter
    {
        public IRun Import(Form form = null)
        {
            throw new NotSupportedException();
        }

        public string ImportAsComparison(IRun run, Form form = null)
        {
            var splitDialog = new OpenFileDialog();

            var result = splitDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var filePath = splitDialog.FileName;

                using (var stream = File.OpenRead(filePath))
                {
                    var runFactory = new StandardFormatsRunFactory();
                    var comparisonGeneratorsFactory = new StandardComparisonGeneratorsFactory();

                    runFactory.Stream = stream;
                    runFactory.FilePath = filePath;

                    var imported = runFactory.Create(comparisonGeneratorsFactory);

                    var comparisonName = Path.GetFileNameWithoutExtension(splitDialog.FileName);
                    result = InputBox.Show(form, "Enter Comparison Name", "Name:", ref comparisonName);
                    if (result != DialogResult.Cancel)
                        return run.AddComparisonWithNameInput(imported, comparisonName, form);
                }
            }
            return null;
        }
    }
}
