namespace Core.Units
{
    [System.Serializable]
    public class ViewBaseData
    {
        public PresentationEventType EventType = PresentationEventType.Add;
        public int PresentationId;
        public int TargetId;
        public int UnitId;
        public object[] ExtraParams;
    }

    [System.Serializable]
    public enum PresentationEventType
    {
        Add,
        Remove,
        Update
    }
}