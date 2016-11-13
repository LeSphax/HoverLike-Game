using System;
using PlayerManagement;
using UnityEngine;

namespace PlayerBallControl
{
    public class ShootEffectBuilder : AbilityEffectBuilder
    {

        public override AbilityEffect GetEffect(params object[] parameters)
        {
            return new ShootEffect((float)parameters[0], (Vector3)parameters[1]);
        }
    }

    public class ShootEffect : AbilityEffect
    {
        public float powerValue;
        public Vector3 position;

        public ShootEffect() { }

        public ShootEffect(float powerValue, Vector3 position)
        {
            this.position = position;
            this.powerValue = powerValue;
        }

        public override void ApplyEffect(PlayerPhysicsModel model)
        {
            model.ballController.ClientThrowBall(position, powerValue);
        }

        public override void UnApplyEffect(PlayerPhysicsModel playerPhysicsModel)
        {
            //DoNothing
        }

        public override InputFlag GetInputFlag()
        {
            return InputFlag.LEFTCLICK;
        }

        public override byte[] Serialize()
        {
            return ArrayExtensions.Concatenate(BitConverter.GetBytes(powerValue), BitConverterExtensions.GetBytes(position));
        }

        public override int Deserialize(byte[] data, int currentIndex)
        {
            int offset = 0;
            powerValue = BitConverter.ToSingle(data, currentIndex + offset);
            offset += 4;
            position = BitConverterExtensions.ToVector3(data, currentIndex + offset);
            offset += 12;
            return offset;
        }
    }

}