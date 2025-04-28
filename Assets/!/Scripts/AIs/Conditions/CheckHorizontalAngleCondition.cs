using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check Horizontal Angle", story: "horizontal angle between [Source] and [Target] is [Operator] [Threshold] use absolute [Absolute]", category: "Conditions", id: "3c2e8409b0b35eb79d568d86faff1ed1")]
public partial class CheckAngleCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Transform> Source;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [Comparison(comparisonType: ComparisonType.All)]
    [SerializeReference] public BlackboardVariable<ConditionOperator> Operator;
    [SerializeReference] public BlackboardVariable<float> Threshold;
    [SerializeReference] public BlackboardVariable<bool> Absolute;

    public override bool IsTrue() {
        if (Source.Value == null || Target.Value == null)
        {
            return false;
        }

        Vector2 sourceVector = new Vector2(Source.Value.position.x, Source.Value.position.z);
        Vector2 targetVector = new Vector2(Target.Value.position.x, Target.Value.position.z);

        Vector2 directionToTarget = (targetVector - sourceVector).normalized;
        Vector3 sourceDirection = Source.Value.rotation * Vector3.forward;
        Vector2 sourceDirectionFlat = new Vector2(sourceDirection.x, sourceDirection.z);

        float angle = Vector2.SignedAngle(sourceDirectionFlat, directionToTarget);

        if(Absolute.Value) {
            angle = Mathf.Abs(angle);
        }

        return ConditionUtils.Evaluate(angle, Operator, Threshold.Value);
    }
}
