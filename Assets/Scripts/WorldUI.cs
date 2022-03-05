using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldUI : MonoBehaviour
{
    [SerializeField] GenesDisplay mutationMessage;

    GenesDisplay humanDisplay;
    Dictionary<Human, GenesDisplay> MutationDisplays = new Dictionary<Human, GenesDisplay>();
    
    private void Awake()
    {
        mutationMessage.gameObject.SetActive(false);
    }
    private void Start()
    {
        State.Events.onMutation.AddListener(DisplayMutationInfo);
        State.Events.onStartDraggingHuman.AddListener(ShowHumanInfo);
        State.Events.onStopDraggingHuman.AddListener(HideHumanInfo);

        State.Events.onCrossBreedHover.AddListener(BreedHover);
        State.Events.onCrossBreedUnhover.AddListener(BreedUnhover);
    }

    private void BreedUnhover(Human original, Human target)
    {
        Debug.Log($"original is {target.gameObject.name}");
        humanDisplay.Repopulate(original);
    }

    private void BreedHover(Human original, Human target)
    {
        Debug.Log($"original is {original.gameObject.name} new {target.gameObject.name}");
        humanDisplay.Repopulate(original, target);
    }

    private void HideHumanInfo(Human arg0)
    {
        humanDisplay.Hide();
    }

    private void ShowHumanInfo(Human human)
    {
        if(MutationDisplays.TryGetValue(human, out GenesDisplay current))
        {
            current.Hide();
            MutationDisplays.Remove(human);
        }

        humanDisplay = Instantiate(mutationMessage, transform).GetComponent<GenesDisplay>();
        humanDisplay.gameObject.SetActive(true);

        humanDisplay.Populate(human);
    }

    private void DisplayMutationInfo(Human human, Genome arg1, Genome arg2)
    {
        GenesDisplay message = Instantiate(mutationMessage, transform).GetComponent<GenesDisplay>();
        message.gameObject.SetActive(true);
        message.Populate(human, arg1, arg2);

        if (MutationDisplays.TryGetValue(human, out GenesDisplay current))
        {
            current.Hide();
            MutationDisplays[human] = message;
        }
        else
        {
            MutationDisplays.Add(human, message);
        }
    }
}
