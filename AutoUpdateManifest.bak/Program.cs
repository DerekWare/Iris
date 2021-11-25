using System;
using System.IO;
using System.Reflection;
using DerekWare.Iris;

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
        public string version = DerekWare.Iris.Program.AutoUpdaterVersion.ToString();
        public string url = "http://www.derekware.com/software/iris/DerekWare%20Iris.zip";
        public bool mandatory = false;
    }
    
    class Program
    {
        static readonly System.Xml.Serialization.XmlSerializer Serializer = new(typeof(AutoUpdaterManifest));

        static void Main(string[] args)
        {
            using var writer = new StringWriter();
            Serializer.Serialize(writer, new AutoUpdaterManifest());
            Console.WriteLine(writer.ToString());
        }
    }
}
