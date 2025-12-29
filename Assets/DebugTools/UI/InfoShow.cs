using Lean.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace DebugTools.UI
{

    public class InfoShow : MonoBehaviour, IPoolable
    {
        public Image Background;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI InfoText;
        private const string Sepreter = ": ";
        public void Init(string title, string info)
        {
            TitleText.text = title + Sepreter;
            InfoText.text = info;
        }

        public void OnDespawn()
        {
            TitleText.text = string.Empty;
            InfoText.text = string.Empty;
        }

        public void OnSpawn()
        {
        }
    }
}