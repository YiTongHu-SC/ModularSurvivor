using UnityEngine;

namespace GameLoop.Utils
{
    public class CreateObjects : MonoBehaviour
    {
        public GameObject ObjectPrefab;

        public void Perform()
        {
            var obj = Instantiate(ObjectPrefab, transform, false);
            obj.SetActive(true);
        }
    }
}