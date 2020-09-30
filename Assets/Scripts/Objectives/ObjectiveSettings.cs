using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ObjectiveSettings : ScriptableObject
{
    public string title;
    public float reward;
    public float comparisonFloat;
    public bool completed = false;

    public ObjectiveSettings nextObjective;

    public void init()
    {
        completed = false;
        FindObjectOfType<movement>().PlayerMoved += CompletionCheck; // TODO: remove this function from the event when finished. 
        Debug.Log("Added CompletionCheck");
    }

    public void CompletionCheck(Vector3 playerPosition)
    {
        if (playerPosition.magnitude > 90)
        {
            Debug.Log(playerPosition.magnitude);
            completed = true;
        }
    }

}
