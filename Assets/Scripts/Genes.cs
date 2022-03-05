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
        Cache.Clear();
        foreach (var gene in config.defaultGenome)
        {
            Genes.Add(new Gene(gene));
        }
    }
    public Genome(Genome old, float radiation)
    {
        Cache.Clear();
        int mutations = Mathf.CeilToInt(config.maxMutations * config.mutationCountOverRadiation.Evaluate(radiation) * Random.value);
        string info = $"Mutation:\nRadiation received: {radiation}\nMutation count: {mutations},\nMutated genes:";

        Genes = old.Genes.Select(item => new Gene(item)).ToList();

        for (int i = 0; i < mutations; i++)
        {
            Gene picked = Genes.PickRandom();

            float deviation = (config.mutationChanceDistribution.Evaluate(Random.value) + config.chanceBoostOverRadiation.Evaluate(radiation))*picked.data.mutationMultiplier * (Random.value > 0.5f ? -1 : 1);

            float oldValue = picked.value;
            picked.value = Mathf.Clamp(picked.value + deviation,picked.data.clampRange.x, picked.data.clampRange.y);
            //picked.value += deviation;

            info += $"\n{i+1}. {picked.data.name} mutated by {deviation}, previous value was {oldValue}, current value is {picked.value}";
        }
        Debug.Log($"{info}");
    }
    public Genome(Genome mother, Genome father)
    {
        Cache.Clear();
        // Implement
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
