using System;

// Dynamic stat is a stat that can be changed by external means
// It's expected to be unstable with multiplicative and percentage modifiers
// I might disable multiplicative and percentage modifiers for dynamic stats
// in the future
// - @londek
[Serializable]
public class DynamicStat : Stat {
    public DynamicStat(StatType statType, float initValue) : base(statType, initValue) {
    }

    public void Add(float value) {
        raw += value;
        adjusted += value;
    }

    public void Subtract(float value) {
        raw -= value;
        adjusted -= value;
    }

    public void Multiply(float value) {
        raw *= value;
        adjusted *= value;
    }

    public void Divide(float value) {
        raw /= value;
        adjusted /= value;
    }

    // Beware! Potential magic behavior.
    // Difference between raw and value is added to adjusted value
    // Just saying, this is not "force" set
    public void Set(float value) {
        var difference = value - raw;
        raw = value;
        adjusted += difference;
    }
}