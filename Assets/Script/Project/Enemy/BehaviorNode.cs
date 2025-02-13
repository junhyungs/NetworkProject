using UnityEngine;
using System.Collections.Generic;

namespace CustomBehaviorTree
{
    public abstract class BehaviorNode<T> : INode
    {
        protected List<INode> _childNodeList;
        protected T _baseReference;
        protected bool _isStop;
        public abstract INode.State Evaluate();
        public virtual void StopBehaviorTree(bool isStop)
        {
            _isStop = isStop;
        }
    }
}

