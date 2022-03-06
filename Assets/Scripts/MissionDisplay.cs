using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDisplay : MonoBehaviour
{
    [SerializeField] Color requirementColor;
    [SerializeField] Color goodColor;
    [SerializeField] Color badColor;

    [SerializeField] GeneDisplayNode display;

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
    }

    private void HumanUnhoveredReactor(Human arg0)
    {
        for (int i = 0; i < loaded.Requirements.Count; i++)
        {
            LevelRequirementSO.GeneRequirement req = loaded.Requirements[i];
            Nodes[i].Populate(req.data.name, requirementColor, req.range.y, req.range.x, GeneDisplayNode.ArrowState.None);
        }
    }

    private void HumanHoveredReactor(Human arg0)
    {
        for (int i = 0; i < loaded.Requirements.Count; i++)
        {
            LevelRequirementSO.GeneRequirement requirement = loaded.Requirements[i];

            arg0.genome.TryGetGeneValue(requirement.data, out float value);
            bool complete = value >= requirement.range.x && value <= requirement.range.y;

            Nodes[i].Populate(requirement.data.name, complete ? goodColor : badColor, requirement.range.y, complete ? 1 : requirement.range.x, GeneDisplayNode.ArrowState.None);
        }
    }

    private void LoadLevel(LevelRequirementSO arg0)
    {
        loaded = arg0;
        Nodes.ForEach(node => Destroy(node.gameObject));
        Nodes.Clear();

        Debug.Log($"Level loaded");
        foreach (var req in arg0.Requirements)
        {
            var cmp = GeneDisplayNode.Instantiate(display, transform);
            cmp.Populate(req.data.name, requirementColor, req.range.y, req.range.x, GeneDisplayNode.ArrowState.None);
            Nodes.Add(cmp);
        }
    }
}
