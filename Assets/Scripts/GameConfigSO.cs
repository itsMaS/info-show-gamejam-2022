using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Data/GameConfig")]
public class GameConfigSO : ScriptableObject
{
    [System.Serializable]
    public class Reactor
    {
        public float baseFuelRequirement;
        public float baseFuelPerClick;
        public float baseReactorRange;
    }

    public Reactor reactor;
}
