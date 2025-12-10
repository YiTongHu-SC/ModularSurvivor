namespace Core.Units
{
    public class ViewBaseData
    {
        public int PresentationId;
        public int UnitId;
        public int TargetId;
        public PresentationEventType EventType = PresentationEventType.Add;
    }

    public enum PresentationEventType
    {
        Add,
        Remove,
        Update
    }
}