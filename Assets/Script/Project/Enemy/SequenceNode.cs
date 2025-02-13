using UnityEngine;
using System.Collections.Generic;

namespace CustomBehaviorTree
{
    public class SequenceNode<T> : BehaviorNode<T> where T : class
    {
        public SequenceNode(List<INode> childNodeList)
        {
            _childNodeList = childNodeList;
        }

        public override void StopBehaviorTree(bool isStop)
        {
            base.StopBehaviorTree(isStop);
        }

        public List<BehaviorNode<T>> FindBehaviorNode()
        {
            var findList = new List<BehaviorNode<T>>();

            foreach (var node in _childNodeList)
            {
                if (node is BehaviorNode<T> behaviorNode)
                {
                    findList.Add(behaviorNode);

                    if(behaviorNode is  SequenceNode<T> sequenceNode)
                    {
                        findList.AddRange(sequenceNode.FindBehaviorNode());
                    }
                    else if(behaviorNode is SelectorNode<T> selectorNode)
                    {
                        findList.AddRange(selectorNode.FindBehaviorNode());
                    }
                }
            }

            return findList;
        }

        public TNode FindNode<TNode>() where TNode : class, INode
        {
            foreach(var node in  _childNodeList)
            {
                if(node is TNode findNode)
                {
                    return findNode;
                }

                if(node is SequenceNode<T> sequenceNode)
                {
                    var findChildNode = sequenceNode.FindNode<TNode>();

                    if(findChildNode != null)
                    {
                        return findChildNode;
                    }
                }
                else if(node is SelectorNode<T> selectorNode)
                {
                    var findChildNode = selectorNode.FindNode<TNode>();

                    if(findChildNode != null)
                    {
                        return findChildNode;
                    }
                }
            }

            return null;
        }

        public override INode.State Evaluate()
        {
            bool isList = _childNodeList == null ||
                _childNodeList.Count == 0;

            if(isList)
            {
                return INode.State.Fail;
            }

            foreach(var childNode in  _childNodeList)
            {
                INode.State state = childNode.Evaluate();

                if (_isStop)
                {
                    break;
                }

                switch (state)
                {
                    case INode.State.Success:
                        continue;
                    case INode.State.Runing:
                        return INode.State.Runing;
                    case INode.State.Fail:
                        return INode.State.Fail;
                }
            }

            return INode.State.Success;
        }
    }
}

