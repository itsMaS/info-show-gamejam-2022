using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class Genome
{
    Dictionary<GeneSO, float> Cache = new Dictionary<GeneSO, float>();
    public List<Gene> Genes { get; private set; } = new List<Gene>();

    private GameConfigSO.Genes config => State.config.genes;

    public Genome()
    {
    }

    /// <summary>
    /// New generic genome generation
    /// </summary>
    public Genome(bool debug)
    {
        Cache.Clear();
        foreach (var gene in config.defaultGenome)
        {
            Genes.Add(new Gene(gene));
        }
    }

    /// <summary>
    /// Mutation
    /// </summary>
    /// <param name="old"></param>
    /// <param name="radiation"></param>
    public Genome(Genome old, float radiation)
    {
        Cache.Clear();
        int mutations =  Mathf.CeilToInt(Random.value* config.maxMutations);//Mathf.CeilToInt(config.maxMutations * config.mutationCountOverRadiation.Evaluate(radiation) * Random.value);

        Genes = old.Genes.Select(item => new Gene(item)).ToList();

        for (int i = 0; i < mutations; i++)
        {
            Gene picked = Genes.PickRandom();

            float oldValue = picked.value;
            //picked.value = Random.value;

            float direction = Random.value > 0.5f ? -1 : 1;

            float deviation = direction * config.mutationAmount.PickRandom();
            float clamped = Mathf.Clamp01(oldValue + deviation);

            float difference = Mathf.Abs(clamped - oldValue);
            if(difference < config.mutationAmount.x)
            {
                picked.value -= deviation;
            }
            else
            {
                picked.value += deviation;
            }
        }
    }

    /// <summary>
    /// Cross breeding
    /// </summary>
    /// <param name="mother"></param>
    /// <param name="father"></param>
    public Genome(Genome mother, Genome father)
    {
        Cache.Clear();
        // Implement

        Genes = new List<Gene>();

        for (int i = 0; i < mother.Genes.Count; i++)
        {
            float offspringValue = Mathf.Lerp(mother.Genes[i].value, father.Genes[i].value, Random.value);
            Genes.Add(new Gene(mother.Genes[i].data, offspringValue));
        }
    }


    public bool TryGetGeneValue(GeneSO gene, out float value)
    {
        if(Cache.TryGetValue(gene, out value))
        {
            return true;
        }

        var found = Genes.Find(g => g.data == gene);
        if(found != null)
        {
            value = found.value;
            Cache.Add(gene, value);
            return true;
        }
        else
        {
            value = -1;
            return false;
        }
    }

    public static float Relation(Genome g1, Genome g2)
    {
        float difference = 0;

        for (int i = 0; i < g1.Genes.Count; i++)
        {
            float diff = Mathf.Abs(g1.Genes[i].value - g2.Genes[i].value);
            difference += diff;
        }

        difference /= g1.Genes.Count;
        difference = 1 - difference;

        return difference;
    }
}
