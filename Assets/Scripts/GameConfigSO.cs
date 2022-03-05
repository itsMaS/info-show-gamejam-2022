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
        public float accelerationSpeed;
        public Vector2 waitTime;
        public Vector2 targetDistance;
    }

    [System.Serializable]
    public class Genes
    {
        public AnimationCurve mutationChanceDistribution;
        public AnimationCurve chanceBoostOverRadiation;
        public AnimationCurve mutationCountOverRadiation;
        public int maxMutations;

        public List<GeneSO> defaultGenome; 
    }

    public Reactor reactor;
    public Human human;
    public Genes genes;
}