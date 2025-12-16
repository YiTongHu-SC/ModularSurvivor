namespace Core.Units
{
    public class ViewBaseData
    {
        public PresentationEventType EventType = PresentationEventType.Add;
        public int PresentationId;
        public int TargetId;
        public int UnitId;
    }

    public enum PresentationEventType
    {
        Add,
        Remove,
        Update
    }
}