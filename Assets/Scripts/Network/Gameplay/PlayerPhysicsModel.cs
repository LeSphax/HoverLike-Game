using Byn.Net;
using PlayerBallControl;
using PlayerManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void TargetChange(Vector3? position);

[RequireComponent(typeof(CustomRigidbody))]
public class PlayerPhysicsModel : PhysicsModel
{
    private const float TARGET_POSITION_RANGE = 3f;

    private ConnectionId? playerConnectionId = null;
    private Player Player
    {
        get
        {
            Assert.IsTrue(playerConnectionId != null);
            return PlayerView.GetMyPlayer(playerConnectionId.Value);
        }
    }
    [SerializeField]
    public PlayerController controller;
    public PlayerBallController ballController;
    private PlayerMovementStrategy strategy;

    private CustomRigidbody myRigidbody;

    private Dictionary<short, List<AbilityEffect>> unacknowlegedEffects = new Dictionary<short, List<AbilityEffect>>();

    private List<AbilityEffect> appliedEffects = new List<AbilityEffect>();

    internal Vector3? targetPosition
    {
        set
        {
            //Debug.LogError("TargetPosition changed " + strategy.targetPosition + "   " + value);
            strategy.targetPosition = value;
            if (TargetPositionChanged != null)
                TargetPositionChanged.Invoke(strategy.targetPosition);
        }
    }

    public event TargetChange TargetPositionChanged;

    private float SimulationTime
    {
        get
        {
            return MyComponents.TimeManagement.NetworkTimeInSeconds - ClientDelay.Delay;
        }
    }

    protected void Awake()
    {
        myRigidbody = GetComponent<CustomRigidbody>();
        ballController = controller.GetComponent<PlayerBallController>();
    }


    public void Reset(ConnectionId connectionId)
    {
        playerConnectionId = connectionId;
        if (strategy != null)
        {
            Destroy(strategy);
        }
        switch (Player.AvatarSettingsType)
        {
            case AvatarSettings.AvatarSettingsTypes.GOALIE:
                strategy = gameObject.AddComponent<GoalieMovementStrategy>();
                break;
            case AvatarSettings.AvatarSettingsTypes.ATTACKER:
                strategy = gameObject.AddComponent<AttackerMovementStrategy>();
                break;
            default:
                throw new UnhandledSwitchCaseException(Player.AvatarSettingsType);
        }
    }

    public override void Simulate(short frameNumber, float dt, bool isRealSimulation)
    {
        if (controller.Player.IsMyPlayer && isRealSimulation)
        {
            unacknowlegedEffects.Add(frameNumber, MyComponents.AbilitiesManager.UpdateAbilities());
        }

        List<AbilityEffect> effects;
        if (unacknowlegedEffects.TryGetValue(frameNumber, out effects))
        {
            foreach (var effect in effects)
            {
                effect.ApplyEffect(this);
                appliedEffects.Add(effect);
            }
        }

        strategy.UpdateMovement();

        myRigidbody.Simulate(Time.fixedDeltaTime);
    }

    public override byte[] Serialize()
    {
        byte[] data;
        if (strategy.targetPosition != null)
        {
            data = BitConverter.GetBytes(true);
            data = ArrayExtensions.Concatenate(data, BitConverterExtensions.GetBytes(strategy.targetPosition.Value));
        }
        else
        {
            data = BitConverter.GetBytes(false);
        }
        data = ArrayExtensions.Concatenate(data, BitConverterExtensions.GetBytes(myRigidbody.velocity));
        data = ArrayExtensions.Concatenate(data, BitConverterExtensions.GetBytes(transform.position));
        data = ArrayExtensions.Concatenate(data, BitConverter.GetBytes(transform.rotation.eulerAngles.y));
        return data;
    }

    public override int DeserializeAndRewind(byte[] data, int currentIndex)
    {
        UnapplyEffects();
        int offset = 0;
        bool hasTarget = BitConverter.ToBoolean(data, currentIndex + offset);
        offset += 1;
        if (hasTarget)
        {
            targetPosition = BitConverterExtensions.ToVector3(data, currentIndex + offset);
            offset += 12;
        }
        else
        {
            targetPosition = null;
        }
        myRigidbody.velocity = BitConverterExtensions.ToVector3(data, currentIndex + offset);
        offset += 12;
        transform.position = BitConverterExtensions.ToVector3(data, currentIndex + offset);
        offset += 12;
        float yRotation = BitConverter.ToSingle(data, currentIndex + offset);
        transform.rotation = Quaternion.Euler(Vector3.up * yRotation);
        offset += 4;
        return offset;
    }

    public override void CheckForPostSimulationActions()
    {
        ballController.TryCatchBall(transform.position);
        if (strategy.targetPosition != null)
        {
            float distance = Vector3.Distance(strategy.targetPosition.Value, transform.position);
            if (distance <= TARGET_POSITION_RANGE)
            {
                targetPosition = null;
            }
        }
    }

    public override void CheckForPreSimulationActions()
    {
        ballController.TryAttractBall(transform.position);
    }

    private void UnapplyEffects()
    {
        foreach (var effect in appliedEffects)
        {
            effect.UnApplyEffect(this);
        }
        appliedEffects.Clear();
    }

    private Dictionary<InputFlag, AbilityEffect> flagsToEffects = new Dictionary<InputFlag, AbilityEffect>();

    public override byte[] SerializeInputs(short frame)
    {
        flagsToEffects.Clear();
        InputFlag flags = InputFlag.NONE;
        List<AbilityEffect> effects;
        if (unacknowlegedEffects.TryGetValue(frame, out effects))
        {
            foreach (var effect in effects)
            {
                flags = flags | effect.GetInputFlag();
                flagsToEffects.Add(effect.GetInputFlag(), effect);
            }
        }

        byte[] data = new byte[1] { (byte)flags };
        foreach (InputFlag flag in Enum.GetValues(typeof(InputFlag)))
        {
            AbilityEffect effect;
            if (flagsToEffects.TryGetValue(flag, out effect))
                data = ArrayExtensions.Concatenate(data, effect.Serialize());
        }
        return data;
    }

}

