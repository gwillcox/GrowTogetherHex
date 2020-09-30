using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveTile : MonoBehaviour
{
    public Text titleText;
    public Text rewardText;
    public ObjectiveSettings objectiveSettings;

    void Awake()
    {
        objectiveSettings.init();
        titleText.text = objectiveSettings.title;
        rewardText.text = $"Reward: Ɣ{objectiveSettings.reward}";
    }
}
