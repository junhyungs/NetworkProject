using UnityEngine;
using System.Collections.Generic;

namespace CustomBehaviorTree
{
    public class SequenceNode : INode
    {
        private List<INode> _childNodeList;

        public SequenceNode(List<INode> childNodeList)
        {
            _childNodeList = childNodeList;
        }

        public INode.State Evaluate()
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

