using DerekWare.Strings;

namespace DerekWare.ffmpeg
{
    public abstract class Filter : FilterProperties
    {
        public string Name;

        protected Filter(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            var s = $"-af {Name}";
            var p = base.ToString();

            if(!p.IsNullOrEmpty())
            {
                s = $"{s}={p}";
            }

            return s;
        }
    }
}
