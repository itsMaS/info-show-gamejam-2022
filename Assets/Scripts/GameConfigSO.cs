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
        public float fuelPerPerson;
    }

    [System.Serializable]
    public class Human
    {
        public float lifeTimeLossPerRadiation = 0.1f;
        public float accelerationSpeed;
        public Vector2 waitTime;
        public Vector2 targetDistance;
        public Vector2 walkSpeedOverGene;
        public AnimationCurve offspringOverFertility;
        public AnimationCurve sizeOverAge;
        public float ageRequiredToMate = 0.5f;
        public float matingCooldown = 30;
        public float relationDifferenceRequiredForMating = 0.3f;
        public AnimationCurve sizeOverStrength;
        public Vector2 timespanOverLongevity = new Vector2(40, 200);
    }

    [System.Serializable]
    public class Gameplay
    {
        public Vector2 startingPopulationAge;
        public float rangeFromReactor;
        public int maxHumans = 10;
    }

    [System.Serializable]
    public class Genes
    {
        public Vector2 mutationAmount = new Vector2(0.2f, 0.5f);
        public int maxMutations;
        public List<GeneSO> defaultGenome; 
    }

    public Reactor reactor;
    public Human human;
    public Genes genes;
    public Gameplay gameplay;
}
