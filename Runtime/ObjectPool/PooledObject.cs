using UnityEngine;

namespace Ivyyy
{
    public class PooledObject : MonoBehaviour
    {
        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
