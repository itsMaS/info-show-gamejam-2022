using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldUI : MonoBehaviour
{
    [SerializeField] MutationMessage mutationMessage;
    private void Awake()
    {
        mutationMessage.gameObject.SetActive(false);
    }
    private void Start()
    {
        State.Events.onMutation.AddListener(DisplayMutationInfo);
    }

    private void DisplayMutationInfo(Human human, Genome arg1, Genome arg2)
    {
        MutationMessage message = Instantiate(mutationMessage, transform).GetComponent<MutationMessage>();
        message.gameObject.SetActive(true);
        message.Populate(arg1, arg2);
        StartCoroutine(TrackHuman(human, message.transform));
    }

    IEnumerator TrackHuman(Human human, Transform message)
    {
        while(message)
        {
            message.transform.position = human.transform.position;
            yield return null;
        }
    }
}
