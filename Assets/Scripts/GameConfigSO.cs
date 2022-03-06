using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Data/GameConfig")]
public class GameConfigSO : ScriptableObject
{
    [System.Serializable]
    public class Reactor
    {
        [Header("Functioning")]
        public float baseFuelRequirement;
        public float baseFuelPerClick;
        public float baseReactorRange;
        [Header("Radioactivity")]
        public AnimationCurve radioctivityFaloff;
        public float baseRadioactivity;
    }

    [System.Serializable]
    public class Human
    {
        public float lifeTimeLossPerRadiation = 0.1f;
        public float accelerationSpeed;
        public Vector2 waitTime;
        public Vector2 targetDistance;
        public Vector2 walkSpeedOverGene;
        public float baseLifespan = 120;
        public AnimationCurve offspringOverFertility;
        public AnimationCurve sizeOverAge;
        public float ageRequiredToMate = 0.5f;
        public float matingCooldown = 30;
        public float relationDifferenceRequiredForMating = 0.3f;
        public AnimationCurve sizeOverStrength;
    }

    [System.Serializable]
    public class Gameplay
    {
        public Vector2 startingPopulationAge;
        public int startingPopulationCount;
        public float rangeFromReactor;
    }

    [System.Serializable]
    public class Genes
    {
        public int maxMutations;
        public List<GeneSO> defaultGenome; 
    }

    public Reactor reactor;
    public Human human;
    public Genes genes;
    public Gameplay gameplay;
}
