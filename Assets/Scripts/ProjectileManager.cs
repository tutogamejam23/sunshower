using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sunshower.SkillCommand;
using UnityEngine.Pool;

namespace Sunshower
{
    /// <summary>
    /// 제가 병신이라 죄송합니다. (장기 기증 가능한)송종국 프로그래머 호소인 올림-
    /// </summary>
    public class ProjectileManager : MonoBehaviour
    {
        private readonly Dictionary<string, IObjectPool<Projectile>> _yeoubulPool;
        private readonly Dictionary<string, IObjectPool<Projectile>> _amuletPool;

        /// <summary>
        /// 여우구슬, 부적에 대한 Pool을 생성한다.
        /// </summary>
        /// <returns></returns>

        private void Start()
        {
            //#TODD : Yeoubul, Amulet을 Pool 생성한다.
        }

        void OnGetFromPool(Projectile projectile) => projectile.gameObject.SetActive(true);

        void OnRealseToPool(Projectile projectile) => projectile.gameObject.SetActive(true);
    }
}
