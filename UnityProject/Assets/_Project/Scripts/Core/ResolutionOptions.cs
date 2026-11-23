using System.Collections.Generic;

namespace Horror.Core
{
    public readonly struct ScreenSize
    {
        public ScreenSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }

        public override string ToString() => $"{Width} x {Height}";
    }

    /// <summary>Builds the resolution dropdown: one entry per size (refresh rates collapsed).</summary>
    public static class ResolutionOptions
    {
        public static List<ScreenSize> Distinct(IEnumerable<ScreenSize> sizes)
        {
            var result = new List<ScreenSize>();
            var seen = new HashSet<long>();
            foreach (var size in sizes)
            {
                if (seen.Add(((long)size.Width << 32) | (uint)size.Height)) result.Add(size);
            }
            return result;
        }

        /// <summary>Index of the given size, or of the largest entry when it is not listed (-1 if the list is empty).</summary>
        public static int IndexOf(IReadOnlyList<ScreenSize> sizes, int width, int height)
        {
            for (int i = 0; i < sizes.Count; i++)
            {
                if (sizes[i].Width == width && sizes[i].Height == height) return i;
            }
            return sizes.Count - 1;
        }

        public static List<string> Labels(IEnumerable<ScreenSize> sizes)
        {
            var labels = new List<string>();
            foreach (var size in sizes) labels.Add(size.ToString());
            return labels;
        }
    }
}
