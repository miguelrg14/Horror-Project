using System;
using HorrorProject.Core;
using NUnit.Framework;

namespace HorrorProject.Tests
{
    public class SoundBankTests
    {
        private sealed class Clip
        {
            public Clip(string name) => Name = name;
            public string Name { get; }
        }

        [Test]
        public void TryGet_ReturnsTheClipRegisteredWithThatName()
        {
            var bank = new SoundBank<Clip>();
            var key = new Clip("pickKey");
            bank.Add("pickKey", key);
            bank.Add("locked", new Clip("locked"));

            Assert.IsTrue(bank.TryGet("pickKey", out Clip found));
            Assert.AreSame(key, found);
            Assert.AreEqual(2, bank.Count);
        }

        [Test]
        public void TryGet_IsCaseSensitive_LikeTheOriginalSwitch()
        {
            var bank = new SoundBank<Clip>();
            bank.Add("OpenDrawer", new Clip("drawer"));

            Assert.IsFalse(bank.TryGet("openDrawer", out _));
        }

        [Test]
        public void UnknownOrEmptyNames_AreNotFound()
        {
            var bank = new SoundBank<Clip>();

            Assert.IsFalse(bank.TryGet("missing", out Clip clip));
            Assert.IsNull(clip);
            Assert.IsFalse(bank.TryGet(null, out _));
            Assert.IsFalse(bank.IsKnown(""));
        }

        [Test]
        public void NameWithoutClip_IsKnownButSilent()
        {
            var bank = new SoundBank<Clip>();
            bank.Add("backgroundAmbient", null);

            Assert.IsTrue(bank.IsKnown("backgroundAmbient"));
            Assert.IsFalse(bank.TryGet("backgroundAmbient", out _));
            Assert.AreEqual(0, bank.Count);
        }

        [Test]
        public void RegisteringANameTwice_Throws()
        {
            var bank = new SoundBank<Clip>();
            bank.Add("chains", new Clip("a"));

            Assert.Throws<ArgumentException>(() => bank.Add("chains", new Clip("b")));
            Assert.Throws<ArgumentException>(() => bank.Add("", new Clip("c")));
        }
    }
}
