using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data to save and load
[System.Serializable]
public class PlayerData
{
    private float _range;

    public PlayerData(GameManager gm)
    {
        _range = gm.Range;
    }

}
