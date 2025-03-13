using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Send Void Message", story: "Sends void message [Message] to [Target]", category: "Action/GameObject", id: "ee18c17a01a417e689a4e99c1a14ef35")]
public partial class SendVoidMessageAction : Action
{
    [SerializeReference] public BlackboardVariable<string> Message;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart() {
        if(Target.Value == null) {
            return Status.Failure;
        }

        Target.Value.SendMessage(Message.Value);

        return Status.Success;
    }
}

