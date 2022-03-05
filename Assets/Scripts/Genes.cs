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

    /// <summary>
    /// New generic genome generation
    /// </summary>
    public Genome()
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
            picked.value = Random.value;
            //picked.value += deviation;
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
}
