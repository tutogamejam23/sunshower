using UnityEngine;

namespace Sunshower
{
    public class OpenPrefabUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup prefab;

        private CanvasGroup instance;

        private void Awake()
        {
            Debug.Assert(prefab);

            Canvas canvas = null;
            Transform parent = transform.parent;
            while (canvas == null)
            {
                if (!parent.TryGetComponent(out canvas))
                {
                    parent = parent.transform.parent;
                }
            }
            instance = Instantiate(prefab, canvas.transform);
            instance.gameObject.SetActive(false);
        }

        public void Open()
        {
            instance.gameObject.SetActive(true);
        }
    }
}
