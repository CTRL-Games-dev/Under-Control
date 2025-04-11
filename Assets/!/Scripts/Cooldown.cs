using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    private float _lastExecuteTime = -1;
    public float CooldownTime;
    
    public Cooldown(float cooldownTime)
    {
        CooldownTime = cooldownTime;
    }

    // Returns true if the cooldown is ready to execute
    public bool IsReady() {
        return Time.time - _lastExecuteTime >= CooldownTime;
    }

    // Resets the cooldown by bypassing it
    public void Reset() {
        _lastExecuteTime = 0;
    }

    // Executes the cooldown
    // Returns true if the cooldown was executed
    // Returns false if the cooldown is not yet ready to execute
    public bool Execute() {
        if (!IsReady()) {
            return false;
        }

        _lastExecuteTime = Time.time;

        return true;
    }

    public void ForceExecute() {
        _lastExecuteTime = Time.time;
    }
}