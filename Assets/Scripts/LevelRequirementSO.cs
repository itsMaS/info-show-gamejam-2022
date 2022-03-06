using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelRequirement", menuName = "Data/LevelRequirement")]
public class LevelRequirementSO : ScriptableObject
{
    [System.Serializable]
    public class GeneRequirement
    {
        public GeneSO data;
        public Vector2 range;
    }

    [TextArea]
    public string tutorialText = "";
    public List<GeneRequirement> Requirements;
    public int amount = 1;
}
