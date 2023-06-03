namespace WatStudios.DeepestDungeon.Networking
{
    public enum NetworkGameEventCode : byte
    {
        //Only 200 codes max ranging from 0...199
        GamePlaySceneBuilt,
        MonsterDeath,
        ReadyCheckResponse,
        ReadyCheckInit
    }
}
