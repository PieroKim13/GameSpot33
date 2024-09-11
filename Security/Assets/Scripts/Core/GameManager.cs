using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    Player player;
    public Player Player => player;

    protected override void OnPreInitialize()
    {
        base.OnPreInitialize();
    }

    protected override void OnIntialize()
    {
        base.OnIntialize();
        player = FindObjectOfType<Player>();
    }
}
