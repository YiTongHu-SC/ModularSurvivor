using System.Collections.Generic;

namespace Core.Units
{
    [System.Serializable]
    public class ViewBaseData
    {
        public ViewEventType ViewEventType = ViewEventType.Add;
        public int SourceId;
        public List<int> TargetIds;
        public float Delay;
        public float Duration;
        public object[] ExtraParams;
    }

    [System.Serializable]
    public enum ViewEventType
    {
        Add,
        Remove,
        Update
    }
}