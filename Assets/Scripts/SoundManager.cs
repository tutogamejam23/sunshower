using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Sunshower
{
    public class SoundManager : MonoBehaviour
    {
        public AudioClipTable SFXTable;
        public SFXPoolItem SFXPoolItem;

        public static SoundManager instance;

        private SFXPool _sfxPool;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Debug.Assert(SFXTable);
                SFXTable.CreateTable();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Debug.Assert(SFXPoolItem);
            _sfxPool = new SFXPool(SFXPoolItem);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public bool PlaySFX(string sfxName)
        {
            if (!SFXTable.AudioClips.TryGetValue(sfxName, out var clip))
            {
                return false;
            }
            var sfx = _sfxPool.Get();
            sfx.Play(clip);
            return true;
        }

        public bool PlaySFXAtPosition(string sfxName, Vector3 position)
        {
            if (!SFXTable.AudioClips.TryGetValue(sfxName, out var clip))
            {
                return false;
            }
            var sfx = _sfxPool.Get();
            sfx.PlayAtPosition(clip, position);
            return true;
        }
    }
}
