using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MissionDisplay : MonoBehaviour
{
    [SerializeField] Color requirementColor;
    [SerializeField] Color goodColor;
    [SerializeField] Color badColor;
    [SerializeField] Color backgroundColor;

    [SerializeField] GeneDisplayNode display;
    [SerializeField] TextMeshProUGUI levelDisplay;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] TextMeshProUGUI tutorialText;

    List<GeneDisplayNode> Nodes = new List<GeneDisplayNode>();

    LevelRequirementSO loaded;

    private void Awake()
    {
        display.gameObject.SetActive(false);
        State.Events.OnLoadLevel.AddListener(LoadLevel);
    }
    private void Start()
    {
        State.Events.OnHoverHumanOverReactor.AddListener(HumanHoveredReactor);
        State.Events.OnUnhoverHumanOverReactor.AddListener(HumanUnhoveredReactor);
        State.Events.OnLevelComplete.AddListener(LevelComplete);
    }

    private void LevelComplete()
    {
        levelDisplay.transform.DOScale(1.1f, 0.4f).SetLoops(2, LoopType.Yoyo);
    }

    private void SetUnhoveredState(LevelRequirementSO.GeneRequirement req, GeneDisplayNode node)
    {
        node.Populate(req.data.name, backgroundColor, req.range.y, req.range.x, GeneDisplayNode.ArrowState.None);
        node.ChangeColors(backgroundColor, requirementColor);
        amountText.color = requirementColor;
        amountText.SetText($"{State.Instance.levelAmount}/{loaded.amount}");
    }

    private void HumanUnhoveredReactor(Human arg0)
    {
        for (int i = 0; i < loaded.Requirements.Count; i++)
        {
            LevelRequirementSO.GeneRequirement req = loaded.Requirements[i];
            SetUnhoveredState(req, Nodes[i]);
        }
    }

    private void HumanHoveredReactor(Human arg0)
    {

        bool allComplete = true;
        for (int i = 0; i < loaded.Requirements.Count; i++)
        {
            LevelRequirementSO.GeneRequirement requirement = loaded.Requirements[i];

            arg0.genome.TryGetGeneValue(requirement.data, out float value);
            bool complete = value >= requirement.range.x && value <= requirement.range.y;
            if (!complete) allComplete = false;

            Nodes[i].Populate(requirement.data.name, complete ? goodColor : badColor, 1, 1, GeneDisplayNode.ArrowState.None);
            Nodes[i].ChangeColors(complete ? goodColor : badColor, complete ? goodColor : badColor);

        }

        amountText.color = allComplete ? goodColor : badColor;
        if(allComplete)
        {
            amountText.SetText($"{State.Instance.levelAmount + 1}/{loaded.amount}");
        }
    }

    private void LoadLevel(LevelRequirementSO arg0)
    {
        levelDisplay.SetText($"LEVEL {State.Instance.CurrentLevelIndex + 1}");
        tutorialText.SetText(arg0.tutorialText);

        loaded = arg0;
        Nodes.ForEach(node => Destroy(node.gameObject));
        Nodes.Clear();

        Debug.Log($"Level loaded");
        foreach (var req in arg0.Requirements)
        {
            var cmp = GeneDisplayNode.Instantiate(display, transform);
            SetUnhoveredState(req, cmp);
            Nodes.Add(cmp);
        }
    }
}
