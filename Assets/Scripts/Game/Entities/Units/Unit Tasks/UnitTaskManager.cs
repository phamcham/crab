using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTaskManager : MonoBehaviour {
    public IUnitTask[] allTasks;
    private void Awake() {
        allTasks = GetComponents<IUnitTask>();
    }

    // No task is running
    public bool IsIdle() {
        if (allTasks == null) return true;
        bool noTaskRunning = true;
        foreach (IUnitTask task in allTasks) {
            if (task.IsTaskRunning){
                noTaskRunning = false;
                break;
            }
        }
        return noTaskRunning;
    }

    public void StopAllTasks() {
        if (allTasks != null) {
            foreach (IUnitTask task in allTasks) {
                task.EndDoTask();
            }
        }
    }
}
