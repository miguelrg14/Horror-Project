using System.Collections.Generic;
using HorrorProject.Core;
using NUnit.Framework;

namespace HorrorProject.Tests
{
    public class ResolutionOptionsTests
    {
        private static readonly ScreenSize[] Monitor =
        {
            new ScreenSize(1280, 720),
            new ScreenSize(1280, 720), // same size at another refresh rate
            new ScreenSize(1920, 1080),
            new ScreenSize(1920, 1080),
            new ScreenSize(2560, 1440),
        };

        [Test]
        public void Distinct_CollapsesRefreshRates()
        {
            List<ScreenSize> sizes = ResolutionOptions.Distinct(Monitor);

            Assert.AreEqual(3, sizes.Count);
            CollectionAssert.AreEqual(new[] { "1280 x 720", "1920 x 1080", "2560 x 1440" }, ResolutionOptions.Labels(sizes));
        }

        [Test]
        public void IndexOf_FindsTheCurrentSize()
        {
            List<ScreenSize> sizes = ResolutionOptions.Distinct(Monitor);

            Assert.AreEqual(1, ResolutionOptions.IndexOf(sizes, 1920, 1080));
        }

        [Test]
        public void IndexOf_FallsBackToTheLargestSize()
        {
            List<ScreenSize> sizes = ResolutionOptions.Distinct(Monitor);

            Assert.AreEqual(2, ResolutionOptions.IndexOf(sizes, 1366, 768));
            Assert.AreEqual(-1, ResolutionOptions.IndexOf(new List<ScreenSize>(), 800, 600));
        }
    }
}
