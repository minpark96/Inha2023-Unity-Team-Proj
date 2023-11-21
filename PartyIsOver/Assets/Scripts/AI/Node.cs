using System.Collections;
using System.Collections.Generic;

namespace BehaviorTree
{
    //3가지 생태 실행 성공 실패
    public enum NodeState
    {
        Running,
        Success,
        Failure
    }

    public class Node
    {
        //현재 노드의 상태를 나타냄
        protected NodeState state;

        public Node parent;
        //양방향으로 진행하기 위해서 리스트 사용
        protected List<Node> children = new List<Node>();
        //노드에서 사용하는 데이터를 관리
        private Dictionary<string, object>_dataContext = new Dictionary<string, object>();

        //생성자
        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach(Node child in children)
                _Attach(child);
        }

        //자식 노드를 현재 노드에 추가
        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        //노드의 평가를 수행하고 상태를 반환함
        public virtual NodeState Evaluate() => NodeState.Failure;

        //데이터 추가를 대입
        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        //재귀적인 상태 메서드는 현재 노드에서 데이터를찾고 찾지 못하면 부모노드로 올라가면서 검색
        public object GetData(string key)
        {
            object value = null;
            //Dictionary
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

        //메서드는 특정키에 해당하는 데이터를 삭제, 현재노드에서 찾지 못하면 부모 노드로 올라가면서 검색
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
