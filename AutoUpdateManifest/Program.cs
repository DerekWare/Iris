using System;

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

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            Console.WriteLine(@"<item>");
            Console.WriteLine($"    <version>{DerekWare.Iris.Program.Version}</version>");
            Console.WriteLine(@"    <url>http://www.derekware.com/software/iris/DerekWare%20Iris.zip</url>");
            Console.WriteLine(@"    <mandatory>false</mandatory>");
            Console.WriteLine(@"</item>");
        }
    }
}
