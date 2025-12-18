namespace Core.Events
{
    public static class GameLoopEvents
    {
        public class BootComplete : EventData
        {
        }

        public class LoadingProgressEvent : EventData
        {
            public float Progress => AssetProgress * AssetWeight + SceneProgress * SceneWeight;
            public string Message { get; set; }
            public float AssetProgress { get; set; }
            public float SceneProgress { get; set; }
            private float AssetWeight { get; }
            private float SceneWeight { get; }

            public LoadingProgressEvent(float assetWeight, float sceneWeight)
            {
                // Normalize weights
                AssetWeight = assetWeight / (assetWeight + sceneWeight);
                SceneWeight = sceneWeight / (assetWeight + sceneWeight);
            }
        }

        public class GameStartEvent : EventData
        {
            public GameStartEvent(int levelId)
            {
                LevelID = levelId;
            }

            public int LevelID { get; }
        }

        public class GameExitEvent : EventData
        {
        }

        public class ReturnToMainMenuEvent : EventData
        {
        }
    }
}