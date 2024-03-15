using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class LiftAction : PlayerAction
{
    public LiftAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }

    BodyHandler _bodyHandler;
    PlayerActionContext _context;
    AnimationPlayer _animPlayer;

    Define.ObjectType _type;
    InteractableObject _object;
    Vector3 _inputMoveDir = new Vector3();

    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _bodyHandler = bodyHandler;
        _context = data;
        _animPlayer = animPlayer;
        _inputMoveDir.x = data.InputDirX;
        _inputMoveDir.y = data.InputDirY;
        _inputMoveDir.z = data.InputDirZ;

        _type = _context.RightGrabObject.Type;
        _object = _context.RightGrabObject;


        LiftObject();

        return true;
    }

    void LiftObject()
    {
        if (_type == ObjectType.Player)
        {
            _animPlayer.AlignToVector(_bodyHandler.LeftArm.PartRigidbody, _bodyHandler.LeftArm.PartTransform.forward,
                -_bodyHandler.Waist.PartTransform.forward + _bodyHandler.Chest.PartTransform.right / 2f +
                -_inputMoveDir / 8f, 0.01f, 8f);
            _animPlayer.AlignToVector(_bodyHandler.LeftForeArm.PartRigidbody, _bodyHandler.LeftForeArm.PartTransform.forward,
                -_bodyHandler.Waist.PartTransform.forward, 0.01f, 8f);

            _animPlayer.AlignToVector(_bodyHandler.RightArm.PartRigidbody, _bodyHandler.RightArm.PartTransform.forward,
                -_bodyHandler.Waist.PartTransform.forward + -_bodyHandler.Chest.PartTransform.right / 2f +
                -_inputMoveDir / 8f, 0.01f, 8f);
            _animPlayer.AlignToVector(_bodyHandler.RightForeArm.PartRigidbody, _bodyHandler.RightForeArm.PartTransform.forward, -_bodyHandler.Waist.PartTransform.forward, 0.01f, 8f);

            _object.transform.root.GetComponent<BodyHandler>().Hip.GetComponent<InteractableObject>().PullingForceTrigger(Vector3.up, 5.5f);

            Vector3 vec = _bodyHandler.Hip.PartRigidbody.velocity;
            _bodyHandler.Hip.PartRigidbody.velocity = new Vector3(vec.x * 1.3f, 0f, vec.z * 1.3f);
        }
        else if (_type == ObjectType.Object)
        {
            _object.PullingForceTrigger(Vector3.up, 0.5f);
        }
        else
            Debug.LogError("잘못된 들기 타입");
    }
}
