/**
Author: Brandon J. Matthews

https://github.com/brandonjmatthews
brandonjmatthews@outlook.com

Copyright (c) 2020 Brandon J. Matthews
**/

using HPTK.Models.Avatar;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using HPTK.Input;

public class LeapSkeletonTracker : InputDataProvider
{
    [Header("Leap Motion Specific")]
    public RiggedHand leapHand;

    [Header("Initial Thumb Metacarpal Position")]
    public Vector3 thumbMetaDummyPosition;
    public Vector3 thumbMetaDummyRotation;

    
    public override void InitData()
    {
        base.InitData();
    }

    public override void UpdateData()
    {
        base.UpdateData();

        if (!leapHand || leapHand.GetLeapHand() == null) {
            confidence = 0;
            return;
        }

        if (leapHand.GetLeapHand().TimeVisible > 0)
        {
            /*
            * 0 - wrist
            * 1 - forearm
            * 
            * 2 - thumb0
            * 3 - thumb1
            * 4 - thumb2
            * 5 - thumb3
            * 
            * 6 - index1
            * 7 - index2
            * 8 - index3
            * 
            * 9 - middle1
            * 10 - middle2
            * 11 - middle3
            * 
            * 12 - ring1
            * 13 - ring2
            * 14 - ring3
            * 
            * 15 - pinky0
            * 16 - pinky1
            * 17 - pinky2
            * 18 - pinky3
            * 
            * 19 - thumbTip
            * 20 - indexTip
            * 21 - middleTip
            * 22 - ringTip
            * 23 - pinkyTip
            */
            bones[0].space = Space.World;
            bones[0].position = leapHand.palm.position;
            bones[0].rotation = leapHand.palm.rotation;


            RiggedFinger thumb = leapHand.fingers[0] as RiggedFinger;
            SetBone(2, thumb, 1);
            SetBone(4, thumb, 2);
            SetBone(5, thumb, 3);

            /**
                Leap Motion treats the thumb trapesium as the thumb metacarpal.As such we use a 
                'dummy' metacarpal derived from the initial joint position in the HPTK hand. 
                Then we use the Leap model's metacarpal as the trapesium.
            */
            bones[3].space = Space.Self;
            bones[3].position = thumbMetaDummyPosition;
            bones[3].rotation = Quaternion.Euler(thumbMetaDummyRotation);

            RiggedFinger index = leapHand.fingers[1] as RiggedFinger;
            SetBone(6, index, 1);
            SetBone(7, index, 2);
            SetBone(8, index, 3);

            RiggedFinger middle = leapHand.fingers[2] as RiggedFinger;
            SetBone(9, middle, 1);
            SetBone(10, middle, 2);
            SetBone(11, middle, 3);

            RiggedFinger ring = leapHand.fingers[3] as RiggedFinger;
            SetBone(12, ring, 1);
            SetBone(13, ring, 2);
            SetBone(14, ring, 3);

            RiggedFinger pinky = leapHand.fingers[4] as RiggedFinger;
            SetBone(15, pinky, 0);
            SetBone(16, pinky, 1);
            SetBone(17, pinky, 2);
            SetBone(18, pinky, 3);


            SetFingerTip(19, thumb);
            SetFingerTip(20, index);
            SetFingerTip(21, middle);
            SetFingerTip(22, ring);
            SetFingerTip(23, pinky); 
                
            confidence = leapHand.GetLeapHand().Confidence;
        } else
        {
            confidence = 0;
        }
    }


    void SetBone(int handBoneIndex, RiggedFinger finger, int fingerBoneIndex) {
        if (finger.bones[fingerBoneIndex] == null) return;
        bones[handBoneIndex].space = Space.Self;
        bones[handBoneIndex].position = finger.bones[fingerBoneIndex].localPosition;
        bones[handBoneIndex].rotation = finger.bones[fingerBoneIndex].localRotation;
    }

    void SetFingerTip(int handBoneIndex, RiggedFinger finger) {
        Transform bone = finger.bones[finger.bones.Length - 1].GetChild(0);
        bones[handBoneIndex].space = Space.Self;
        bones[handBoneIndex].position = bone.localPosition;
        bones[handBoneIndex].rotation = bone.localRotation;
    }
}
