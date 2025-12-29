using UnityEngine;

namespace UI.Utils
{
    public static class UiTool
    {
        public static bool TryBind<T>(Transform source, string matchName, out T result) where T : Component
        {
            result = null;
            foreach (var target in source.GetComponentsInChildren<T>())
            {
                if (target.name == matchName)
                {
                    result = target;
                    break;
                }
            }

            return result != null;
        }
    }
}