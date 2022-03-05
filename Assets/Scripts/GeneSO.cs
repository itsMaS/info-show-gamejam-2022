using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicGene", menuName = "Data/Genes/Basic")]
public class GeneSO : ScriptableObject
{
    public Vector2 defaultRange;
}
[System.Serializable]
public class Gene
{
    public GeneSO data { get; private set; }
    public float value;

    public Gene(GeneSO data)
    {
        this.data = data;
        value = data.defaultRange.PickRandom();
    }
    public Gene(Gene clone)
    {
        data = clone.data;
        value = clone.value;
    }
    public Gene(GeneSO data, float value)
    {
        this.data = data;
        this.value = value;
    }
}
