using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    private float lastExecuteTime = -1;
    public float cooldownTime;
    
    public Cooldown(float cooldownTime)
    {
        this.cooldownTime = cooldownTime;
    }

    // Returns true if the cooldown is ready to execute
    public bool IsReady() {
        return Time.time - lastExecuteTime >= cooldownTime;
    }

    // Resets the cooldown by bypassing it
    public void Reset() {
        lastExecuteTime = 0;
    }

    // Executes the cooldown
    // Returns true if the cooldown was executed
    // Returns false if the cooldown is not yet ready to execute
    public bool Execute() {
        if (!IsReady()) {
            return false;
        }

        lastExecuteTime = Time.time;

        return true;
    }
}