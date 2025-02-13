using UnityEngine;

namespace CustomBehaviorTree
{
    public abstract class ActionNode<T> : BehaviorNode<T> where T : class
    {
        public ActionNode(T baseReference)
        {
            _baseReference = baseReference;
        }

        public override abstract INode.State Evaluate();
    }
}

