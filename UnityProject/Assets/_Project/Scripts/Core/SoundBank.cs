using System;
using System.Collections.Generic;

namespace Horror.Core
{
    /// <summary>
    /// Maps the sound names used by gameplay code ("pickKey", "menu"...) to clips.
    /// Generic so the lookup rules can be tested without Unity's AudioClip.
    /// </summary>
    public sealed class SoundBank<TClip> where TClip : class
    {
        private readonly Dictionary<string, TClip> clips = new Dictionary<string, TClip>(StringComparer.Ordinal);
        private readonly HashSet<string> silent = new HashSet<string>(StringComparer.Ordinal);

        public int Count => clips.Count;

        public IEnumerable<string> Names => clips.Keys;

        /// <summary>
        /// Registers a clip. A name registered with no clip (an empty inspector slot, or a sound that is
        /// played elsewhere) is known but silent: <see cref="TryGet"/> returns false for it.
        /// </summary>
        public void Add(string name, TClip clip)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("A sound needs a name.", nameof(name));
            if (IsKnown(name)) throw new ArgumentException($"Sound '{name}' is registered twice.", nameof(name));
            if (clip != null) clips.Add(name, clip);
            else silent.Add(name);
        }

        /// <summary>True if the name was registered, with or without a clip.</summary>
        public bool IsKnown(string name) => !string.IsNullOrEmpty(name) && (clips.ContainsKey(name) || silent.Contains(name));

        public bool TryGet(string name, out TClip clip)
        {
            clip = null;
            return !string.IsNullOrEmpty(name) && clips.TryGetValue(name, out clip);
        }
    }
}
