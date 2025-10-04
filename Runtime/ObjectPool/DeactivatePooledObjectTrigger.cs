using UnityEngine;

namespace Ivyyy
{
    public class DeactivatePooledObjectTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            PooledObject pooledObject = other.gameObject.GetComponent<PooledObject>();
            pooledObject?.gameObject.SetActive(false);
        }
    }
}
