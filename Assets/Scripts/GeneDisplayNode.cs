using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneDisplayNode : MonoBehaviour
{
    public enum ArrowState { None, Up, Down }

    [SerializeField] Image arrow;
    [SerializeField] TextMeshProUGUI geneName;
    [SerializeField] Image progressImage;
    [SerializeField] Image oldProgress;
    [SerializeField] Image backgroundImage;


    public void Populate(string name, Color color, float v1, float v2, ArrowState state = ArrowState.None)
    {
        geneName.SetText(name);

        geneName.color = color;
        arrow.color = color;
        progressImage.color = color;

        oldProgress.fillAmount = v1;
        progressImage.fillAmount = v2;

        arrow.gameObject.SetActive(true);
        switch (state)
        {
            case ArrowState.None:
                arrow.gameObject.SetActive(false);
                break;
            case ArrowState.Up:
                arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case ArrowState.Down:
                arrow.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            default:
                break;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
    public void ChangeColors(Color background, Color text)
    {
        backgroundImage.color = background;
        geneName.color = text;
    }

    public void AnimateSliders(bool colored, float duration, Ease ease)
    {
        Image from = colored ? progressImage : oldProgress;
        Image to = colored ? oldProgress : progressImage;

        from.DOFillAmount(to.fillAmount, duration).SetEase(ease);
    }
    public static GeneDisplayNode Instantiate(GeneDisplayNode example, Transform parent)
    {
        var cmp = Instantiate(example.gameObject, parent).GetComponent<GeneDisplayNode>();
        cmp.gameObject.SetActive(true);
        cmp.transform.SetSiblingIndex(cmp.transform.childCount-1);

        return cmp;
    }
}
