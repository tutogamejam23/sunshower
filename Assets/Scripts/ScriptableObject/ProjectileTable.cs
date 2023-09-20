using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    [CreateAssetMenu(fileName = "ProjectileTable", menuName = "ScriptableObjects/ProjectileTable")]
    public class ProjectileTable : ScriptableObject
    {
        [SerializeField] private List<Projectile> _projectiles = new();

        public Dictionary<string, Projectile> Projectiles { get; private set; }

        public void CreateTable()
        {
            Projectiles = new Dictionary<string, Projectile>();
            foreach (var projectile in _projectiles)
            {
                if (projectile == null)
                {
                    continue;
                }
                Projectiles.TryAdd(projectile.name, projectile);
            }
        }
    }
}
