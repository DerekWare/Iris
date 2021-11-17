namespace DerekWare.HomeAutomation.Common.Scenes
{
    public interface ISceneFactory : IFactory<IScene, IReadOnlySceneProperties>
    {
    }

    public class SceneFactory : Factory<IScene, IReadOnlySceneProperties>, ISceneFactory
    {
        public static readonly SceneFactory Instance = new();

        SceneFactory()
        {
        }
    }
}
