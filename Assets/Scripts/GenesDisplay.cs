using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenesDisplay : MonoBehaviour
{
    [SerializeField] Color goodColor;
    [SerializeField] Color badColor;
    [SerializeField] Color neutralColor;

    [SerializeField] float duration;
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] GeneDisplayNode node;

    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] TextMeshProUGUI paragraphText;

    CanvasGroup cg;

    private List<GeneDisplayNode> Nodes = new List<GeneDisplayNode>();

    float matingCooldown;

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

                cmp.AnimateSliders(positive, duration, Ease.InOutCirc);
            }
        }

        SetHeaders("Mutation");
    }

    public void SetHeaders(string header = "", string paragraph = "")
    {
        paragraphText.gameObject.SetActive(false);
        headerText.gameObject.SetActive(false);

        if (header.Length > 0)
        {
            headerText.SetText(header);
            headerText.gameObject.SetActive(true);
        }
        if (paragraph.Length > 0)
        {
            paragraphText.SetText(paragraph);
            paragraphText.gameObject.SetActive(true);
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
        SetHeaders("Genes");
    }
    internal void Repopulate(Human original)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Gene gene = original.genome.Genes[i];
            Nodes[i].Populate(gene.data.name, neutralColor, gene.value, gene.value, GeneDisplayNode.ArrowState.None);
        }
        SetHeaders("Genes");
    }
    public void Repopulate(Human human1, Human human2)
    {
        float relation;
        string text;
        float relationThreshold = State.config.human.relationDifferenceRequiredForMating;
        if(!human1.oldEnoughToMate)
        {
            text = $"This human is not old enough to mate. ({Mathf.CeilToInt((State.config.human.ageRequiredToMate - human1.Age) / human1.ageingSpeed)}s)";
        }
        else if(!human2.oldEnoughToMate)
        {
            text = $"The other human is not old enough to mate. ({Mathf.CeilToInt((State.config.human.ageRequiredToMate-human2.Age)/human2.ageingSpeed)}s)";
        }
        else if(human1.matingCooldown > 0)
        {
            text = $"This human is non ready to mate yet. ({Mathf.CeilToInt(human1.matingCooldown)}s)";
        }
        else if(human2.matingCooldown > 0)
        {
            text = $"The other human is not ready to mate yet. ({Mathf.CeilToInt(human1.matingCooldown)}s)";
        }
        else if((relation = Genome.Relation(human1.genome, human2.genome)) > relationThreshold)
        {
            text = $"These humans are too related to reproduce. Their genes are a {100*(relation):0}% match. It has to be {relationThreshold*100:0}% or lower";
        }
        else
        {
            int amount = human1.OffspringAmount(human2);
            if(amount <= 0)
            {
                text = $"These parents are not fertile enough to produce a child";
            }
            else
            {
                string correct = amount == 1 ? "child" : "children";
                text = $"Release the mouse button produce {amount} {correct}.";
            }
        }

        SetHeaders("Mating", text);
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
        Hide();
    }
}
