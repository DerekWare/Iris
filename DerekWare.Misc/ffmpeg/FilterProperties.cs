using System.Collections.Generic;
using DerekWare.Strings;

namespace DerekWare.ffmpeg
{
    public class FilterProperties : Dictionary<string, object>
    {
        public string KeyPrefix = "";
        public string KeyValueSeparator = "=";
        public string ParameterSeparator = ":";

        public IEnumerable<string> Parameters
        {
            get
            {
                foreach(var i in this)
                {
                    if(i.Value is null)
                    {
                        yield return i.Key;
                    }
                    else
                    {
                        yield return $"{KeyPrefix}{i.Key}{KeyValueSeparator}{i.Value}";
                    }
                }
            }
        }

        public override string ToString()
        {
            return Parameters.Join(ParameterSeparator);
        }
    }
}
