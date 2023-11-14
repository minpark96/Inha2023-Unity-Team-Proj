using System.Collections.Generic;
using BehaviorTree;

public class GuardBT : Tree
{
    public UnityEngine.Transform[] waypoints;

    public static float speed = 2f;
    public static float fovRange = 6f;
    public static float attackRange = 1f;
    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            //어택하는 간단한 노드
            //new sequence(new list<node>
            //{
            //    new checkenemyinattackrange(transform),
            //    new taskattack(transform),
            //}),
            new Sequence(new List<Node>
            {
                new CheckEnemyInFOVRange(transform),
                new TaskGoToTarget(transform),
            }),
            new TaskPatrol(transform, waypoints),
        });

        return root;
    }
}
