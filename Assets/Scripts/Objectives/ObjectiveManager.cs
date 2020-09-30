using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public ObjectiveTile objectivePrefab;
    public List<ObjectiveSettings> startingObjectiveSettingsList;
    public List<ObjectiveTile> currentObjectives;
    public List<ObjectiveTile> completedObjectives = new List<ObjectiveTile>();
    public List<ObjectiveTile> addedObjectives = new List<ObjectiveTile>();

    public float sciencePoints;

    void Start()
    {
        foreach (var objectiveSettings in startingObjectiveSettingsList)
        {
            AddObjective(objectiveSettings);
        }
        CleanUpAddedObjectives();
    }

    void AddObjective(ObjectiveSettings objectiveSettings)
    {
        objectivePrefab.objectiveSettings = objectiveSettings;
        ObjectiveTile obj = ObjectiveTile.Instantiate(objectivePrefab, this.transform);
        addedObjectives.Add(obj);
    }

    void CompleteObjective(ObjectiveTile objectiveTile)
    {
        sciencePoints += objectiveTile.objectiveSettings.reward;
        completedObjectives.Add(objectiveTile);
    }

    void CleanUpAddedObjectives()
    {
        foreach (ObjectiveTile objectiveTile in addedObjectives)
        {
            currentObjectives.Add(objectiveTile);
        }
        addedObjectives.Clear();
    }

    void CleanUpCompletedObjectives()
    {
        foreach (ObjectiveTile objectiveTile in completedObjectives)
        {
            currentObjectives.Remove(objectiveTile);
            Destroy(objectiveTile.gameObject);
        }
        completedObjectives.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ObjectiveTile objectiveTile in currentObjectives)
        {
            if (objectiveTile.objectiveSettings.completed)
            {
                Debug.Log($"Completed: {objectiveTile.objectiveSettings.title}");
                if (objectiveTile.objectiveSettings.nextObjective != null)
                {
                    Debug.Log($"Next Objective Queued: {objectiveTile.objectiveSettings.nextObjective.title}");
                    AddObjective(objectiveTile.objectiveSettings.nextObjective);
                }
                CompleteObjective(objectiveTile);
            }
        }
        CleanUpAddedObjectives();
        CleanUpCompletedObjectives();
    }
}
