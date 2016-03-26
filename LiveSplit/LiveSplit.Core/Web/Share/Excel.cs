﻿using System;
using System.Collections.Generic;
using System.Drawing;
using LiveSplit.Model;
using LiveSplit.Options;
using System.Windows.Forms;
using LiveSplit.Model.RunSavers;
using System.IO;

namespace LiveSplit.Web.Share
{
    public class Excel : IRunUploadPlatform
    {
        protected static Excel _Instance = new Excel();

        public static Excel Instance { get { return _Instance; } }

        public string PlatformName
        {
            get
            {
                return "Excel";
            }
        }

        public string Description
        {
            get
            {
                return "あなたのスプリットをエクセルファイルとして保存します。"
                    + "今まで試行してきた全履歴が保存されています。";
            }
        }

        public ISettings Settings { get; set; }

        protected Excel() { }

        #region Not supported

        IEnumerable<ASUP.IdPair> IRunUploadPlatform.GetGameList()
        {
            yield break;
        }

        IEnumerable<string> IRunUploadPlatform.GetGameNames()
        {
            yield break;
        }

        string IRunUploadPlatform.GetGameIdByName(string gameName)
        {
            return string.Empty;
        }

        IEnumerable<ASUP.IdPair> IRunUploadPlatform.GetGameCategories(string gameId)
        {
            yield break;
        }

        string IRunUploadPlatform.GetCategoryIdByName(string gameId, string categoryName)
        {
            return string.Empty;
        }

        bool IRunUploadPlatform.VerifyLogin(string username, string password)
        {
            return true;
        }

        #endregion

        public bool SubmitRun(IRun run, string username, string password, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel Sheet (*.xlsx)|*.xlsx";
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var path = dialog.FileName;

                    if (!File.Exists(path))
                        File.Create(path).Close();
                    using (var stream = File.Open(path, FileMode.Create, FileAccess.Write))
                    {
                        var runSaver = new ExcelRunSaver();
                        runSaver.Save(run, stream);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
