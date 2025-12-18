using UI.Framework;

namespace UI.Loading
{
    public struct LoadingModelData
    {
        public float Progress;
        public string Message;
    }

    public class LoadingModel : BaseModel<LoadingModelData>
    {
        public override LoadingModelData Value { get; protected set; }
    }
}