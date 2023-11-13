using System.Collections;
using System.Collections.Generic;

namespace BehaviorTree
{

    public enum NodeState
    {
        Running,
        Success,
        Failure
    }

    public class Node
    {
        protected NodeState state;

        public Node parent;
        //양방향으로 진행하기 위해서 리스트 사용
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object>_dataContext = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }
        public Node(List<Node> children)
        {
            foreach(Node child in children)
                _Attach(child);
        }

        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.Failure;

        //함수를 재귀적으로 만들어서 우리가 찾고 있던 키를 찾거나 데이터를 지우기 위해 트리의 루트에 도달할 때까지 분기 위로 올라감
        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        //만약 찾지 못한다면 요청을 무시합니다.
        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;

            Node node = parent;
            while(node != null)
            {
                value = node.GetData(key);
                if(value != null) return value;
                node =node.parent;
            }
            return null;
        }

        public bool ClearData(string key) 
        { 
            if(_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while(node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }

    }
}
