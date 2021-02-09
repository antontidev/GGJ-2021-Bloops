using UnityEngine;
using Zenject;
using SukharevShared;

public class ModuleInstaller : MonoInstaller
{
    enum MovementType {
        KeyboardMover,
        JoystickMover
    }

    [SerializeField]
    private MovementType movementType;

    [SerializeField]
    private PreviousSceneGameSettings settings;

    [SerializeField]
    private TimeManager timeManager;

    [SerializeField]
    private Levels levels;

    public override void InstallBindings() {
        var type = EnumResolver<MovementType, IPlayerMover>.GetType(movementType);

        var levelLoader = new LevelLoader(settings);
#if UNITY_EDITOR
        Container.Bind<IPlayerMover>().To(type.UnderlyingSystemType).AsSingle();
#elif UNITY_WEBGL || UNITY_STANDALONE
        Container.Bind<IPlayerMover>().To<KeyboardMover>().AsSingle();
#elif UNITY_ANDROID || UNITY_IOS
        Container.Bind<IPlayerMover>().To<JoystickMover>().AsSingle();
#endif
        LevelLoader.timeManager = timeManager;

        Container.Bind<PreviousSceneGameSettings>().FromInstance(settings).AsSingle();
        Container.Bind<LevelLoader>().FromInstance(levelLoader).AsSingle();
        Container.Bind<Levels>().FromInstance(levels).AsSingle();
    }
}
