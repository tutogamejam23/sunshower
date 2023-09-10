using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Sunshower
{
    public interface IPoolItem<ItemT, DataT>
        where ItemT : MonoBehaviour
        where DataT : IDataTableItem
    {
        public DataT Data { get; }
        void Initialize(IObjectPool<ItemT> pool, DataT data);
    }

    public interface IActivePoolItemCollection<T> where T : MonoBehaviour
    {
        public IReadOnlyCollection<T> InactiveObjects { get; }
    }
}
