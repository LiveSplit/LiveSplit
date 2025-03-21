using System;

namespace UpdateManager;

public class UpdateManagerUpdateable : IUpdateable
{
    private UpdateManagerUpdateable() { }

    private static UpdateManagerUpdateable _Instance { get; set; }

    public static UpdateManagerUpdateable Instance
    {
        get
        {
            _Instance ??= new UpdateManagerUpdateable();

            return _Instance;
        }
    }

    public string UpdateName => "Update Manager";

    public string XMLURL => "http://livesplit.org/update/update.updater.xml";

    public string UpdateURL => "http://livesplit.org/update/";

    public Version Version => Version.Parse("2.0.5");
}
