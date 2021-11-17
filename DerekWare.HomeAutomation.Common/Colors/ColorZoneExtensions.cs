using System;
using System.Collections.Generic;

namespace DerekWare.HomeAutomation.Common.Colors
{
    public static partial class Extensions
    {
        // From a collection of raw color values, produce a list that has no sequential
        // redundant values, reducing the total size of the data to as little as possible.
        // This version of Compress also removes any color values that are already in the
        // source list that don't need to be changed in the destination.
        // TODO this can be combined with the version that removes adjacent colors
        public static IEnumerable<ColorZone> Compress(this IReadOnlyCollection<Color> dst, IReadOnlyCollection<Color> src)
        {
            // Convert the target colors to an array of color zones and remove any redundant colors
            var count = Math.Min(src.Count, dst.Count);
            var zones = new List<ColorZone>();

            var si = src.GetEnumerator();
            var di = dst.GetEnumerator();
            byte index = 0;

            while(si.MoveNext() && di.MoveNext())
            {
                if(si.Current is null || di.Current is null || Equals(si.Current, di.Current))
                {
                    continue;
                }

                zones.Add(new ColorZone { StartIndex = index, EndIndex = index, Color = di.Current });
            }

            // Remove any redundant adjacent colors
            return Compress(zones);
        }

        // Remove any redundant adjacent colors
        public static IEnumerable<ColorZone> Compress(this IEnumerable<Color> colors)
        {
            return Compress(ToColorZones(colors));
        }

        // Remove any redundant adjacent colors
        public static IEnumerable<ColorZone> Compress(this IEnumerable<ColorZone> zones)
        {
            ColorZone first = null;

            foreach(var z in zones)
            {
                if(first is null)
                {
                    first = z;
                    continue;
                }

                if(Equals(z.Color, first.Color))
                {
                    ++first.EndIndex;
                    continue;
                }

                yield return first;

                first = z;
            }

            if(first is not null)
            {
                yield return first;
            }
        }

        // Expand a list of ColorZone back into a full set of Color, including any redundant
        // adjacent values.
        public static IEnumerable<Color> Expand(this IEnumerable<ColorZone> src, int zoneCount)
        {
            // In order to handle potentially out of order zones as well as gaps, store all
            // colors in an array, then convert the array to a sorted enumerable.
            var dst = new Color[256];

            foreach(var c in src)
            {
                for(var i = c.StartIndex; i <= c.EndIndex; ++i)
                {
                    dst[i] = c.Color;
                }
            }

            for(var i = 0; i < zoneCount; ++i)
            {
                yield return dst[i] ?? new Color();
            }
        }

        public static IEnumerable<ColorZone> ToColorZones(this IEnumerable<Color> colors)
        {
            var i = 0;

            foreach(var color in colors)
            {
                yield return new ColorZone { StartIndex = (byte)i, EndIndex = (byte)i, Color = color };
                ++i;
            }
        }
    }
}
