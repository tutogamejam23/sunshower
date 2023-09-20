using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using YamlDotNet.Serialization;

namespace Sunshower
{
    public class Effect : MonoBehaviour
    {
        public IObjectPool<Effect> Pool { get; set; }

        public virtual void Play()
        {
        }
    }
}
