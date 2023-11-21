using System.Collections;
using System.Collections.Generic;

namespace BehaviorTree
{
    //3���� ���� ���� ���� ����
    public enum NodeState
    {
        Running,
        Success,
        Failure
    }

    public class Node
    {
        //���� ����� ���¸� ��Ÿ��
        protected NodeState state;

        public Node parent;
        //��������� �����ϱ� ���ؼ� ����Ʈ ���
        protected List<Node> children = new List<Node>();
        //��忡�� ����ϴ� �����͸� ����
        private Dictionary<string, object>_dataContext = new Dictionary<string, object>();

        //������
        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach(Node child in children)
                _Attach(child);
        }

        //�ڽ� ��带 ���� ��忡 �߰�
        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        //����� �򰡸� �����ϰ� ���¸� ��ȯ��
        public virtual NodeState Evaluate() => NodeState.Failure;

        //������ �߰��� ����
        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        //������� ���� �޼���� ���� ��忡�� �����͸�ã�� ã�� ���ϸ� �θ���� �ö󰡸鼭 �˻�
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

        //�޼���� Ư��Ű�� �ش��ϴ� �����͸� ����, �����忡�� ã�� ���ϸ� �θ� ���� �ö󰡸鼭 �˻�
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