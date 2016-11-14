using PlayerBallControl;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public abstract class AbilityEffect
{
    public abstract void ApplyEffect(PlayerPhysicsModel model);

    public abstract void UnApplyEffect(PlayerPhysicsModel playerPhysicsModel);

    public virtual InputFlag GetInputFlag() { return InputFlag.NONE; }

    public abstract bool IsSerialisable();

    public virtual byte[] Serialize() { return null; }

    public virtual int Deserialize(byte[] data, int currentIndex) { return 0; }


    private static Queue<AbilityEffect> effects = new Queue<AbilityEffect>();

    public static List<AbilityEffect> Deserialize(InputFlag flags, byte[] data, int currentIndex)
    {
        if (flags.HasFlag(InputFlag.LEFTCLICK))
        {
            effects.Enqueue(new ShootEffect());
        }
        if (flags.HasFlag(InputFlag.RIGHTCLICK))
        {
            effects.Enqueue(new MoveEffect());
        }
        if (flags.HasFlag(InputFlag.FIRST))
        {
            effects.Enqueue(new DashEffect());
        }
        if (flags.HasFlag(InputFlag.SECOND))
        {

        }
        if (flags.HasFlag(InputFlag.THIRD))
        {

        }
        if (flags.HasFlag(InputFlag.FOURTH))
        {

        }
        if (flags.HasFlag(InputFlag.SPACE))
        {

        }
        List<AbilityEffect> result = new List<AbilityEffect>();
        while (effects.Count > 0)
        {
            var effect = effects.Dequeue();
            currentIndex += effect.Deserialize(data, currentIndex);
            result.Add(effect);
        }
        Assert.IsTrue(effects.Count == 0);
        return result;
    }

}

[Flags]
public enum InputFlag : byte
{
    NONE = 0,
    LEFTCLICK = 1,
    RIGHTCLICK = 2,
    FIRST = 4,
    SECOND = 8,
    THIRD = 16,
    FOURTH = 32,
    SPACE = 64
    //LEFTCLICK = 1 << 0,
    //RIGHTCLICK = 1 << 1,
    //FIRST = 1 << 2,
    //SECOND = 1 << 3,
    //THIRD = 1 << 4,
    //FOURTH = 1 << 5,
    //SPACE = 1 << 6
}