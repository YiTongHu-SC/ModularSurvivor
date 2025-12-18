using UnityEngine;

namespace UI.Loading
{
    public class ProgressBarUI : MonoBehaviour
    {
        public RectTransform ProgressBarFill;

        public void SetProgress(float progress)
        {
            if (ProgressBarFill != null)
            {
                float clampedProgress = Mathf.Clamp01(progress);
                // 使用anchorMax来控制填充进度，从左到右填充
                ProgressBarFill.anchorMax = new Vector2(clampedProgress, 1f);
            }
        }
    }
}