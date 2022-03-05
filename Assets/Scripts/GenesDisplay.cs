using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenesDisplay : MonoBehaviour
{
    [SerializeField] Color goodColor;
    [SerializeField] Color badColor;
    [SerializeField] Color neutralColor;

    [SerializeField] float duration;
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] GeneDisplayNode node;

    CanvasGroup cg;

    private List<GeneDisplayNode> Nodes = new List<GeneDisplayNode>();

    private void Awake()
    {
        node.gameObject.SetActive(false);
        cg = GetComponent<CanvasGroup>();
    }


    public void Show()
    {
        cg.alpha = 0;
        cg.DOFade(1, fadeDuration);
    }
    public void Hide()
    {
        cg.DOFade(0, fadeDuration).OnComplete(() => Destroy(gameObject));
    }

    public void Populate(Human human, Genome oldGenome, Genome newGenome)
    {
        Show();
        DOVirtual.DelayedCall(duration, Hide);
        StartCoroutine(Tracking(human.transform));

        cg.alpha = 0;
        cg.DOFade(1, 0.2f);
        for (int i = 0; i < oldGenome.Genes.Count; i++)
        {
            if (oldGenome.Genes[i].value != newGenome.Genes[i].value)
            {
                GeneDisplayNode cmp = GeneDisplayNode.Instantiate(node, transform);
                Nodes.Add(cmp);

                float oldValue = oldGenome.Genes[i].value;
                float newValue = newGenome.Genes[i].value;

                bool positive = newValue > oldValue;

                cmp.Populate(oldGenome.Genes[i].data.name, positive ? goodColor : badColor, positive ? newValue : oldValue, positive ? oldValue : newValue, positive ? GeneDisplayNode.ArrowState.Up : GeneDisplayNode.ArrowState.Down);
            }
        }
    }

    public void Populate(Human human)
    {
        Show();
        StartCoroutine(Tracking(human.transform));
        foreach (var gene in human.genome.Genes)
        {
            GeneDisplayNode cmp = GeneDisplayNode.Instantiate(node, transform);
            Nodes.Add(cmp);

            cmp.Populate(gene.data.name, neutralColor, gene.value, gene.value, GeneDisplayNode.ArrowState.None);
        }
    }
    internal void Repopulate(Human original)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Gene gene = original.genome.Genes[i];
            Nodes[i].Populate(gene.data.name, neutralColor, gene.value, gene.value, GeneDisplayNode.ArrowState.None);
        }
    }
    public void Repopulate(Human human1, Human human2)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            float V1 = human1.genome.Genes[i].value;
            float V2 = human2.genome.Genes[i].value;

            float max = Mathf.Max(V1,V2);
            float min = Mathf.Min(V1, V2);

            Nodes[i].Populate(human1.genome.Genes[i].data.name, neutralColor, max, min);
        }
    }

    IEnumerator Tracking(Transform target)
    {
        while(target)
        {
            transform.position = target.position;
            yield return null;
        }
    }
}
