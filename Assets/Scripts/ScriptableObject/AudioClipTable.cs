using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sunshower
{
    [CreateAssetMenu(fileName = "AudioClipTable", menuName = "ScriptableObjects/AudioClipTable", order = 1)]
    public class AudioClipTable : ScriptableObject
    {
        [SerializeField] private List<AudioClip> audioClips = new();

        public Dictionary<string, AudioClip> AudioClips { get; private set; }

        public void CreateTable()
        {
            AudioClips = new Dictionary<string, AudioClip>();
            foreach (var clip in audioClips)
            {
                if (clip == null)
                {
                    continue;
                }
                AudioClips.TryAdd(clip.name, clip);
            }
        }
    }
}
