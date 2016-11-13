using System;
using PlayerManagement;
using UnityEngine;

namespace PlayerBallControl
{
    public class MoveEffectBuilder : AbilityEffectBuilder
    {

        public override AbilityEffect GetEffect(params object[] parameters)
        {
            return new MoveEffect((Vector3)parameters[0]);
        }

    }

    public class MoveEffect : AbilityEffect
    {
        Vector3 targetPosition;

        public MoveEffect() { }

        public MoveEffect(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        public override void ApplyEffect(PlayerPhysicsModel model)
        {
            model.targetPosition = targetPosition;
        }

        public override int Deserialize(byte[] data, int currentIndex)
        {
            targetPosition = BitConverterExtensions.ToVector3(data, currentIndex);
            return 12;
        }

        public override InputFlag GetInputFlag()
        {
            return InputFlag.RIGHTCLICK;
        }

        public override byte[] Serialize()
        {
            return BitConverterExtensions.GetBytes(targetPosition);
        }

        public override void UnApplyEffect(PlayerPhysicsModel model)
        {
            model.targetPosition = null;
        }
    }
}