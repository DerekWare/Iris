using System;
using System.IO;
using System.Xml.Serialization;

namespace AutoUpdateManifest
{
    /*
        <?xml version="1.0" encoding="UTF-8"?>
        <item>
            <version>1.0.0.9</version>
            <url>http://www.derekware.com/software/iris/DerekWare%20Iris.zip</url>
            <mandatory>false</mandatory>
        </item>
     */
    public class AutoUpdaterManifest
    {
        public bool mandatory = false;
        public string url = "http://www.derekware.com/software/iris/DerekWare%20Iris.zip";
        public string version = DerekWare.Iris.Program.AutoUpdaterVersion.ToString();
    }

    class Program
    {
        static readonly XmlSerializer Serializer = new XmlSerializer(typeof(AutoUpdaterManifest));

        static void Main(string[] args)
        {
            using(var writer = new StringWriter())
            {
                Serializer.Serialize(writer, new AutoUpdaterManifest());
                Console.WriteLine(writer.ToString());
            }
        }
    }
}
