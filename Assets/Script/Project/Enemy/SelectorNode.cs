using UnityEngine;
using System.Collections.Generic;

namespace CustomBehaviorTree
{
    public class SelectorNode<T> : BehaviorNode<T> where T : class
    {
        public SelectorNode(List<INode> childNodeList)
        {
            _childNodeList = childNodeList;
        }

        public TNode FindNode<TNode>() where TNode : class, INode
        {
            foreach (var node in _childNodeList)
            {
                if (node is TNode findNode)
                {
                    return findNode;
                }

                if (node is SelectorNode<T> selectorNode)
                {
                    var findChildNode = selectorNode.FindNode<TNode>();

                    if (findChildNode != null)
                    {
                        return findChildNode;
                    }
                }
                else if (node is SequenceNode<T> sequenceNode)
                {
                    var findChildNode = sequenceNode.FindNode<TNode>();

                    if(findChildNode != null)
                    {
                        return findChildNode;
                    }
                }
            }

            return null;
        }

        public override void StopBehaviorTree(bool isStop)
        {
            base.StopBehaviorTree(isStop);
        }

        public override INode.State Evaluate()
        {
            bool isList = _childNodeList == null ||
                _childNodeList.Count == 0;

            if (isList)
            {
                return INode.State.Fail;
            }

            foreach(var childNode in _childNodeList)
            {
                INode.State state = childNode.Evaluate();

                if (_isStop)
                {
                    break;
                }

                switch(state)
                {
                    case INode.State.Success:
                        return INode.State.Success;
                    case INode.State.Runing:
                        return INode.State.Runing;
                    case INode.State.Fail:
                        continue;
                }
            }

            return INode.State.Fail;
        }
    }
}

