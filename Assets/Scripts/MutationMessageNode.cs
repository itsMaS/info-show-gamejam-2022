using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class MutationMessageNode : MonoBehaviour
{
    [SerializeField] Image arrow;
    [SerializeField] TextMeshProUGUI geneName;
    [SerializeField] Image progressImage;
    [SerializeField] Image oldProgress;

    [SerializeField] Color goodColor;
    [SerializeField] Color badColor;
    [SerializeField] AnimationCurve animation;

    public void Populate(float oldValue, float newValue, GeneSO data, float popupAnimation, float popupHold, float popupFadeInDuration)
    {
        bool good = newValue > oldValue;
        Color col = good ? goodColor : badColor;
        arrow.transform.rotation = Quaternion.Euler(0, 0, good ? 0 : 180);

        arrow.color = col;
        geneName.color = col;
        progressImage.color = col;

        float oldNorm = Mathf.InverseLerp(data.clampRange.x, data.clampRange.y, oldValue);
        float newNorm = Mathf.InverseLerp(data.clampRange.x, data.clampRange.y, newValue);

        geneName.SetText(data.name);

        oldProgress.fillAmount = good ? newNorm : oldNorm;
        progressImage.fillAmount = oldNorm;

        DOVirtual.DelayedCall(popupFadeInDuration, () => progressImage.DOFillAmount(newNorm, popupAnimation).SetEase(animation));
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
