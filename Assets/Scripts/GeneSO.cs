using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicGene", menuName = "Data/Genes/Basic")]
public class GeneSO : ScriptableObject
{
    public Vector2 defaultRange;
    public Vector2 clampRange;
    public float mutationMultiplier = 1;
}
[System.Serializable]
public class Gene
{
    public GeneSO data { get; private set; }
    public float value;

    public Gene(GeneSO data)
    {
        this.data = data;
        value = data.defaultRange.PickRandomFromRange();
    }
    public Gene(Gene clone)
    {
        data = clone.data;
        value = clone.value;
    }
}
