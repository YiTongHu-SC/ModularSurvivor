namespace GameLoop.Game
{
    public enum LoadSceneType
    {
        MainMenu = 0,
        Game = 1,
        Exit = 2,
    }

    public struct LoadSceneStruct
    {
        public int LevelID;
        public string LevelName;
        public string SceneName;
        public string Description;
    }
}