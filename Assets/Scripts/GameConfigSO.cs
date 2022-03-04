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

    [System.Serializable]
    public class Human
    {
        public float maxSpeed;
        public float accelerationSpeed;
        public Vector2 waitTime;
        public Vector2 targetDistance;
    }

    public Reactor reactor;
    public Human human;
}
