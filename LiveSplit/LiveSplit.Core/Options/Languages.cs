using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace LiveSplit.Options
{
    public class Languages : ICloneable
    {
        private static Languages instance = null;
        public static Languages Instance { get
            {
                if (instance == null)
                {
                    instance = new Languages();
                }
                return instance;
            }
        }

        Languages () {
            GetLanguagesFromSettings();
        }

        public Dictionary<string, string> langString = new Dictionary<string, string>()
        {
            {"en", "English" },
            {"zh-CN", "简体中文" },
        };

        public string replaceStr (string langStr)
        {
            return langString[langStr];
        }
        public string[] GetLanguagesList ()
        {
            var allFileNames = GetAllFileNames("Languages");
            if (allFileNames != null)
            {
                var languagesList = allFileNames.ToArray();
                for (var i = 0; i < languagesList.Length; i++)
                {
                    string filename = languagesList[i];
                    XmlDocument xml = new XmlDocument();
                    xml.Load(AppDomain.CurrentDomain.BaseDirectory + "Languages/" + languagesList[i]);
                    XmlNode languageNode = xml.SelectSingleNode("Language");
                    if (languageNode != null)
                    {
                        string displayName = languageNode.SelectSingleNode("DisplayName").InnerText;
                        string value = "";
                        string langKey = languagesList[i].Replace(".cfg", "");
                        if (!langString.TryGetValue(langKey, out value))
                        {
                            langString.Add(langKey, displayName);
                        }
                        languagesList[i] = displayName;
                    }
                    else
                    {
                        languagesList[i] = languagesList[i].Replace(".cfg", "");
                    }
                }
                return languagesList;
            }
            else
            {
                string[] languagesList = {"English"};
                return languagesList;
            }
        }
        // 获取当前系统的语言
        public string localLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        public string nowLanguage;

        public string GetLanguage()
        {
            // Console.WriteLine("获取文件夹下所有文件名" + GetAllFileNames("languages"));
            return nowLanguage;
        }

        public string GetText(string key, string oldString)
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Languages"))
            {
                localLanguage = "en";
                nowLanguage = "en";
                return oldString;
            }
            else
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "Languages/" + nowLanguage + ".cfg");
                XmlNode languageNode = xmlDocument.SelectSingleNode("Language");
                if (languageNode != null)
                {
                    XmlNode textNode = languageNode.SelectSingleNode(key);
                    if (textNode != null)
                    {
                        return textNode.InnerText;
                    }
                    else
                    {
                        Console.WriteLine("This key does not exist：" + key);
                        return oldString;
                    }
                }
                else
                {
                    return oldString;
                }
            }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllFileNames(string path, string pattern="*.cfg") 
        {
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Languages"))
            {
                List<FileInfo> folder = new DirectoryInfo(path).GetFiles(pattern).ToList();
                if (folder.Count > 0)
                {
                    return folder.Select(x => x.Name).ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private string GetLanguagesFromSettings ()
        {
            string settingLanguage = null;
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"settings.cfg"))
            {
                // Settings configuration file does not exist
                nowLanguage = localLanguage;
                settingLanguage = nowLanguage;
            }
            else
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(AppDomain.CurrentDomain.BaseDirectory + "settings.cfg");
                XmlNode settings = xml.SelectSingleNode("Settings");
                if (settings != null)
                {
                    XmlNode lang = settings.SelectSingleNode("Language");
                    if (lang != null)
                    {
                        nowLanguage = lang.InnerText;
                        settingLanguage = nowLanguage;
                    }
                    else
                    {
                        nowLanguage = localLanguage;
                        settingLanguage = nowLanguage;
                    }
                }
                else
                {
                    nowLanguage = localLanguage;
                    settingLanguage = nowLanguage;
                }
            }
            if (settingLanguage != null)
            {
                return settingLanguage;
            }
            else
            {
                nowLanguage = "en";
                return nowLanguage;
            }
        }
    }
}
