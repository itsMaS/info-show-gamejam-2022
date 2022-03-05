using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationMessage : MonoBehaviour
{
    [SerializeField] float popupFadeInDuration;
    [SerializeField] float popupHoldDuration;
    [SerializeField] float popupAnimationDuration;
    [SerializeField] float popupFadeOutDuration;

    [SerializeField] MutationMessageNode node;

    CanvasGroup cg;

    private void Awake()
    {
        node.gameObject.SetActive(false);
        cg = GetComponent<CanvasGroup>();
    }
    internal void Populate(Genome oldGenome, Genome newGenome)
    {
        cg.alpha = 0;
        cg.DOFade(1, popupFadeInDuration);
        for (int i = 0; i < oldGenome.Genes.Count; i++)
        {
            if (oldGenome.Genes[i].value != newGenome.Genes[i].value)
            {
                MutationMessageNode go = Instantiate(node.gameObject, transform).GetComponent<MutationMessageNode>();
                go.gameObject.SetActive(true);

                float oldValue = oldGenome.Genes[i].value;
                float newValue = newGenome.Genes[i].value;

                go.Populate(oldValue, newValue, oldGenome.Genes[i].data, popupAnimationDuration, popupHoldDuration, popupFadeInDuration);
                DOVirtual.DelayedCall(popupAnimationDuration+popupFadeInDuration+popupHoldDuration, () => cg.DOFade(0, popupFadeOutDuration).OnComplete(() => Destroy(gameObject)));
            }
        }
    }
}
