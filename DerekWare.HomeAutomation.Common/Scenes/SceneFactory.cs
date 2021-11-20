namespace DerekWare.HomeAutomation.Common.Scenes
{
    public interface ISceneFactory : IFactory<IScene>
    {
    }

    public class SceneFactory : Factory<IScene>, ISceneFactory
    {
        public static readonly SceneFactory Instance = new();

        SceneFactory()
        {
        }
    }
}
