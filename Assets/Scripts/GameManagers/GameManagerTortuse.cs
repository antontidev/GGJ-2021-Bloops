using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTortuse : BaseGameManager
{
    private void Start() {
        base.Start();

        timeManager.MakeEvening();
    }
}
