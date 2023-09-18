using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Sunshower
{
    public class SoundManager : MonoBehaviour
    {
        public AudioMixer mixer;
        public AudioSource Bgm;
        public AudioClip[] BgmList;

        public static SoundManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            for (int i = 0; i < BgmList.Length; i++)
            {
                if (arg0.name == BgmList[i].name)
                {
                    PlayBGM(BgmList[i]);
                }

            }
        }

        public void PlaySFX(AudioClip clip)
        {
            GameObject go = new GameObject(nameof(clip));
            AudioSource audiosource = go.AddComponent<AudioSource>();
            audiosource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            audiosource.clip = clip;
            audiosource.Play();

            Destroy(go, clip.length);
        }

        public void PlayBGM(AudioClip clip)
        {
            Bgm.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            Bgm.clip = clip;
            Bgm.loop = true;
            Bgm.volume = 0.1f;
            Bgm.Play();
        }

        public void StopBGM() => Bgm.Stop();

    }
}
