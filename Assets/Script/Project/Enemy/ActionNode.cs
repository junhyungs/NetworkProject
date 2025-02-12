using UnityEngine;

namespace CustomBehaviorTree
{
    public abstract class ActionNode<T> : INode
    {
        protected T _baseReference;

        public ActionNode(T baseReference)
        {
            _baseReference = baseReference;
        }

        public abstract INode.State Evaluate();
    }
}

