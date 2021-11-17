using System.Linq;
using System.Text;

namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    public static class Extensions
    {
        public static string DeserializeString(this byte[] b)
        {
            return Encoding.Default.GetString(b).TrimEnd('\0');
        }

        public static string FormatByteArray(this byte[] data, string separator = ",")
        {
            if(data is null || (data.Length == 0))
            {
                return "";
            }

            return string.Join(separator, data.Select(b => b.ToString("X2")));
        }
    }
}
