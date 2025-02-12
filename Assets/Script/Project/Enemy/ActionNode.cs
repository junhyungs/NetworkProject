using UnityEngine;

namespace CustomBehaviorTree
{
    public abstract class ActionNode<T> : BehaviorNode
    {
        protected T _baseReference;

        public ActionNode(T baseReference)
        {
            _baseReference = baseReference;
        }

        public override abstract INode.State Evaluate();
    }
}

