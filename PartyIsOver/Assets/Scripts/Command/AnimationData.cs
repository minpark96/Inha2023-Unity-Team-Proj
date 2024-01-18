using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static AniAngleData;
using static AniFrameData;

public class AnimationData
{
    public AnimationData(BodyHandler bodyHandler)
    {
        _bodyHandler = bodyHandler;

        LoadAnimForceData();
        LoadAnimRotateData();
    }

    BodyHandler _bodyHandler;

    Dictionary<string, AniFrameData[]> frameDataLists = new Dictionary<string, AniFrameData[]>();
    Dictionary<string, AniAngleData[]> angleDataLists = new Dictionary<string, AniAngleData[]>();

    public Dictionary<string, AniFrameData[]> FrameDataLists { get { return frameDataLists; } private set { frameDataLists = value; } }
    public Dictionary<string, AniAngleData[]> AngleDataLists { get { return angleDataLists; } private set { angleDataLists = value; } }




    Rigidbody StringToRigid(string text)
    {
        Rigidbody rb = new Rigidbody();
        Define.BodyPart part;
        if (Enum.TryParse(text, out part))
        {
            switch (part)
            {
                case Define.BodyPart.FootL:
                    rb = _bodyHandler.LeftFoot.PartRigidbody; break;
                case Define.BodyPart.FootR:
                    rb = _bodyHandler.RightFoot.PartRigidbody; break;
                case Define.BodyPart.LegLowerL:
                    rb = _bodyHandler.LeftLeg.PartRigidbody; break;
                case Define.BodyPart.LegLowerR:
                    rb = _bodyHandler.RightLeg.PartRigidbody; break;
                case Define.BodyPart.LegUpperL:
                    rb = _bodyHandler.LeftThigh.PartRigidbody; break;
                case Define.BodyPart.LegUpperR:
                    rb = _bodyHandler.RightThigh.PartRigidbody; break;
                case Define.BodyPart.Hip:
                    rb = _bodyHandler.Hip.PartRigidbody; break;
                case Define.BodyPart.Waist:
                    rb = _bodyHandler.Waist.PartRigidbody; break;
                case Define.BodyPart.Chest:
                    rb = _bodyHandler.Chest.PartRigidbody; break;
                case Define.BodyPart.Head:
                    rb = _bodyHandler.Head.PartRigidbody; break;
                case Define.BodyPart.LeftArm:
                    rb = _bodyHandler.LeftArm.PartRigidbody; break;
                case Define.BodyPart.RightArm:
                    rb = _bodyHandler.RightArm.PartRigidbody; break;
                case Define.BodyPart.LeftForeArm:
                    rb = _bodyHandler.LeftForeArm.PartRigidbody; break;
                case Define.BodyPart.RightForeArm:
                    rb = _bodyHandler.RightForeArm.PartRigidbody; break;
                case Define.BodyPart.LeftHand:
                    rb = _bodyHandler.LeftHand.PartRigidbody; break;
                case Define.BodyPart.RightHand:
                    rb = _bodyHandler.RightHand.PartRigidbody; break;
                default:
                    Debug.Log("애니메이션 파츠 불러오기 에러1" + part.ToString());
                    break;
            }
        }
        else
            Debug.Log("애니메이션 파츠 불러오기 에러2" + text);
        return rb;
    }
    ForceDirection StringToForceDir(string text)
    {
        ForceDirection dir = new ForceDirection();
        Define.AnimDirection eDirection;
        if (Enum.TryParse(text, out eDirection))
        {
            switch (eDirection)
            {
                case Define.AnimDirection.Zero:
                    dir = ForceDirection.Zero; break;
                case Define.AnimDirection.Forward:
                    dir = ForceDirection.Forward; break;
                case Define.AnimDirection.Backward:
                    dir = ForceDirection.Backward; break;
                case Define.AnimDirection.Up:
                    dir = ForceDirection.Up; break;
                case Define.AnimDirection.Down:
                    dir = ForceDirection.Down; break;
                case Define.AnimDirection.Right:
                    dir = ForceDirection.Right; break;
                case Define.AnimDirection.Left:
                    dir = ForceDirection.Left; break;
                default:
                    Debug.Log("포스방향 불러오기 에러1"); break;
            }
        }
        else
            Debug.Log("포스방향 불러오기 에러2" + text);

        return dir;
    }

    AniAngle StringToRotateDir(string text)
    {
        AniAngle dir = new AniAngle();
        Define.AnimDirection eDirection;
        if (Enum.TryParse(text, out eDirection))
        {
            switch (eDirection)
            {
                case Define.AnimDirection.Zero:
                    dir = AniAngle.Zero; break;
                case Define.AnimDirection.Forward:
                    dir = AniAngle.Forward; break;
                case Define.AnimDirection.Backward:
                    dir = AniAngle.Backward; break;
                case Define.AnimDirection.Up:
                    dir = AniAngle.Up; break;
                case Define.AnimDirection.Down:
                    dir = AniAngle.Down; break;
                case Define.AnimDirection.Right:
                    dir = AniAngle.Right; break;
                case Define.AnimDirection.Left:
                    dir = AniAngle.Left; break;
                default:
                    Debug.Log("포스방향 불러오기 에러1"); break;
            }
        }
        else
            Debug.Log("포스방향 불러오기 에러2" + text);

        return dir;
    }

    void LoadAnimForceData()
    {
        Define.AniFrameData[] frameDataNames = (Define.AniFrameData[])Enum.GetValues(typeof(Define.AniFrameData));
        int actionCount = 0;
        List<int> partCount = new List<int>();
        List<Rigidbody> standardRb = new List<Rigidbody>();
        List<Rigidbody> actionRb = new List<Rigidbody>();
        List<ForceDirection> forceDir = new List<ForceDirection>();
        List<int> forceVal = new List<int>();



        for (int i = 0; i < (int)Define.AniFrameData.End; i++)
        {
            string filePath = $"Animations/ForceData/{frameDataNames[i]}";
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            //리스트들 클리어해야함
            partCount.Clear();
            standardRb.Clear();
            actionRb.Clear();
            forceDir.Clear();
            forceVal.Clear();
            int index = 0;

            if (textAsset != null)
            {
                string[] lines = textAsset.text.Split('\n');
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        Define.AnimForceValue frameValue;
                        Enum.TryParse(key, out frameValue);

                        if (frameValue == Define.AnimForceValue.ActionCount)
                            actionCount = int.Parse(value);
                        else if (frameValue == Define.AnimForceValue.PartCount)
                            partCount.Add(int.Parse(value));
                        else if (frameValue == Define.AnimForceValue.StandardPart)
                            standardRb.Add(StringToRigid(value));
                        else if (frameValue == Define.AnimForceValue.ActionPart)
                            actionRb.Add(StringToRigid(value));
                        else if (frameValue == Define.AnimForceValue.ForceDirection)
                            forceDir.Add(StringToForceDir(value));
                        else if (frameValue == Define.AnimForceValue.ForcePowerValues)
                            forceVal.Add(int.Parse(value));
                    }
                }

                AniFrameData[] datas = new AniFrameData[actionCount];

                for (int j = 0; j < actionCount; j++)
                {
                    AniFrameData data = new AniFrameData();
                    data.StandardRigidbodies = new Rigidbody[partCount[j]];
                    data.ActionRigidbodies = new Rigidbody[partCount[j]];
                    data.ForceDirections = new ForceDirection[partCount[j]];
                    data.ForcePowerValues = new float[partCount[j]];

                    for (int k = 0; k < partCount[j]; k++)
                    {
                        data.StandardRigidbodies[k] = standardRb[index];
                        data.ActionRigidbodies[k] = actionRb[index];
                        data.ForceDirections[k] = forceDir[index];
                        data.ForcePowerValues[k] = forceVal[index];
                        index++;
                    }
                    datas[j] = data;
                }
                frameDataLists[frameDataNames[i].ToString()] = datas;
            }
            else
                Debug.LogError("File not found: " + filePath);
        }
    }
    void LoadAnimRotateData()
    {
        Define.AniAngleData[] rotateDataNames = (Define.AniAngleData[])Enum.GetValues(typeof(Define.AniAngleData));
        int actionCount = 0;
        List<int> partCount = new List<int>();
        List<Rigidbody> actionRb = new List<Rigidbody>();
        List<Transform> standardPart = new List<Transform>();
        List<Transform> targetPart = new List<Transform>();
        List<AniAngle> standardDir = new List<AniAngle>();
        List<AniAngle> targetDir = new List<AniAngle>();
        List<float> stability = new List<float>();
        List<float> forceVal = new List<float>();



        for (int i = 0; i < (int)Define.AniFrameData.End; i++)
        {
            string filePath = $"Animations/RotateData/{rotateDataNames[i]}";
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            //리스트들 클리어해야함
            partCount.Clear();

            actionRb.Clear();
            standardPart.Clear();
            targetPart.Clear();
            standardDir.Clear();
            targetDir.Clear();

            stability.Clear();
            forceVal.Clear();

            int index = 0;

            if (textAsset != null)
            {
                string[] lines = textAsset.text.Split('\n');
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        Define.AnimRotateValue frameValue;
                        Enum.TryParse(key, out frameValue);

                        if (frameValue == Define.AnimRotateValue.ActionCount)
                            actionCount = int.Parse(value);
                        else if (frameValue == Define.AnimRotateValue.PartCount)
                            partCount.Add(int.Parse(value));
                        else if (frameValue == Define.AnimRotateValue.ActionRigidbodies)
                            actionRb.Add(StringToRigid(value));
                        else if (frameValue == Define.AnimRotateValue.StandardPart)
                            standardPart.Add(StringToRigid(value).transform);
                        else if (frameValue == Define.AnimRotateValue.TartgetPart)
                            targetPart.Add(StringToRigid(value).transform);
                        else if (frameValue == Define.AnimRotateValue.StandardDirections)
                            standardDir.Add(StringToRotateDir(value));
                        else if (frameValue == Define.AnimRotateValue.TargetDirections)
                            targetDir.Add(StringToRotateDir(value));
                        else if (frameValue == Define.AnimRotateValue.AngleStability)
                            stability.Add(float.Parse(value));
                        else if (frameValue == Define.AnimRotateValue.AnglePowerValues)
                            forceVal.Add(float.Parse(value));


                    }
                }

                AniAngleData[] datas = new AniAngleData[actionCount];

                for (int j = 0; j < actionCount; j++)
                {
                    AniAngleData data = new AniAngleData();
                    data.ActionRigidbodies = new Rigidbody[partCount[j]];
                    data.StandardPart = new Transform[partCount[j]];
                    data.TargetPart = new Transform[partCount[j]];
                    data.StandardDirections = new AniAngle[partCount[j]];
                    data.TargetDirections = new AniAngle[partCount[j]];
                    data.AngleStability = new float[partCount[j]];
                    data.AnglePowerValues = new float[partCount[j]];


                    for (int k = 0; k < partCount[j]; k++)
                    {
                        data.ActionRigidbodies[k] = actionRb[index];
                        data.StandardPart[k] = standardPart[index];
                        data.TargetPart[k] = targetPart[index];
                        data.StandardDirections[k] = standardDir[index];

                        data.TargetDirections[k] = targetDir[index];
                        data.AngleStability[k] = stability[index];
                        data.AnglePowerValues[k] = forceVal[index];
                        index++;
                    }
                    datas[j] = data;
                }
                angleDataLists[rotateDataNames[i].ToString()] = datas;
            }
            else
                Debug.LogError("File not found: " + filePath);
        }
    }

}
