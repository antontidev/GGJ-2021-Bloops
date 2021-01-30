using System.Collections;
using System.Collections.Generic;
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

    public override void InstallBindings() {
        var type = EnumResolver<MovementType, IPlayerMover>.GetType(movementType);

        Container.Bind<IPlayerMover>().To(type.UnderlyingSystemType).AsSingle();
    }
}
