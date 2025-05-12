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


        Vector3 directionToTarget = Target.Value.position - Source.Value.position;
        directionToTarget.y = 0;
        directionToTarget.Normalize();

        Vector3 forward = Source.Value.forward;
        forward.y = 0;
        forward.Normalize();

        float angle = Vector3.SignedAngle(forward, directionToTarget, Vector3.up);

        if(Absolute.Value) {
            angle = Mathf.Abs(angle);
        }

        return ConditionUtils.Evaluate(angle, Operator, Threshold.Value);
    }
}
