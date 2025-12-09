using UnityEngine;

namespace Combat.Views
{
    public class BaseUnitPresentation : MonoBehaviour
    {
        public PresentationData Data { get; private set; }

        public void SetData(PresentationData data)
        {
            Data = data;
        }
    }
}