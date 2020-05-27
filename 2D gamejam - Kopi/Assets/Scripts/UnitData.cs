using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public float[] position;
    public bool wasChallenged;
    public List<EnterBattle> pastEnemyChallenged;

    public UnitData(PlayerMovement player)
    {
        position = new float[2];
        position[0] = player.position.x;
        position[1] = player.position.y;
    }

    public UnitData(PlayerMovement player, EnterBattle enemy)
    {
        position = new float[2];
        position[0] = player.position.x;
        position[1] = player.position.y;

        wasChallenged = enemy.hasBeenChallenged;
    }
}
