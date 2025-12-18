using System.Collections;
using UI.Loading;
using UnityEngine;

namespace Samples.Scripts
{
    public class TestLoading : MonoBehaviour
    {
        private ProgressBarUI _progressBar;

        private void Awake()
        {
            _progressBar = GetComponent<ProgressBarUI>();
        }

        private void Start()
        {
            // Simulate loading progress
            StartCoroutine(SimulateLoading());
        }

        private IEnumerator SimulateLoading()
        {
            float progress = 0f;
            while (true)
            {
                if (progress > 1f)
                {
                    progress = 0f;
                }

                progress += Time.deltaTime;
                _progressBar.SetProgress(progress);
                yield return null;
            }
        }
    }
}