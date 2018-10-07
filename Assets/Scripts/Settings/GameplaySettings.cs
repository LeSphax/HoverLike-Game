//*************************************************************
//THIS CLASS WAS GENERATED, USE THE CORRESPONDING GENERATOR TO CHANGE IT (CodeGenerator folder)
//*************************************************************
using System;
using UnityEngine;
using UnityEngine.UI;
using TimeSlow;
public class GameplaySettings : SlideBall.NetworkMonoBehaviour {
public GameObject panel;
public Toggle continuouMovementToggle;
public InputField attackerAccelerationInputField;
public InputField attackerMaxSpeedInputField;
public InputField goalieSpeedInputField;
public InputField shootPowerLevelInputField;
public InputField brakeProportionInputField;
public InputField timeSlowProportionInputField;
public InputField moveCooldownInputField;
public InputField dashCooldownInputField;
public InputField jumpCooldownInputField;
public InputField passCooldownInputField;
public InputField stealCooldownInputField;
public InputField blockCooldownInputField;
public InputField timeSlowCooldownInputField;
public InputField teleportCooldownInputField;
private void Start()
{
Reset();
}
private float ParseFloat(string value, float defaultValue)
{
   try { return float.Parse(value); }
    catch (Exception)
   {
       Debug.LogWarning("Couldn't parse " + value);
       return defaultValue;
    }
}
public bool ContinuouMovement
{
get
{
return MoveInput.ContinuousMovement;
}
set
{
MoveInput.ContinuousMovement = value;
}
}
public float AttackerAcceleration
{
get
{
return AttackerMovementStrategy.Acceleration;
}
set
{
AttackerMovementStrategy.Acceleration = value;
}
}
public float AttackerMaxSpeed
{
get
{
return AttackerMovementStrategy.MaxVelocity;
}
set
{
AttackerMovementStrategy.MaxVelocity = value;
}
}
public float GoalieSpeed
{
get
{
return GoalieMovementStrategy.Speed;
}
set
{
GoalieMovementStrategy.Speed = value;
}
}
public float ShootPowerLevel
{
get
{
return BallMovementView.ShootPowerLevel;
}
set
{
BallMovementView.ShootPowerLevel = value;
}
}
public float BrakeProportion
{
get
{
return AttackerMovementStrategy.BrakeProportion;
}
set
{
AttackerMovementStrategy.BrakeProportion = value;
}
}
public float TimeSlowProportion
{
get
{
return TimeSlowApplier.PlayerSlowProportion;
}
set
{
TimeSlowApplier.PlayerSlowProportion = value;
}
}
public float MoveCooldown
{
get
{
return ResourcesGetter.MovePrefab.GetComponent<Ability>().CooldownDuration;
}
set
{
ResourcesGetter.MovePrefab.GetComponent<Ability>().CooldownDuration = value;
}
}
public float DashCooldown
{
get
{
return ResourcesGetter.DashPrefab.GetComponent<Ability>().CooldownDuration;
}
set
{
ResourcesGetter.DashPrefab.GetComponent<Ability>().CooldownDuration = value;
}
}
public float JumpCooldown
{
get
{
return ResourcesGetter.JumpPrefab.GetComponent<Ability>().CooldownDuration;
}
set
{
ResourcesGetter.JumpPrefab.GetComponent<Ability>().CooldownDuration = value;
}
}
public float PassCooldown
{
get
{
return ResourcesGetter.PassPrefab.GetComponent<Ability>().CooldownDuration;
}
set
{
ResourcesGetter.PassPrefab.GetComponent<Ability>().CooldownDuration = value;
}
}
public float StealCooldown
{
get
{
return ResourcesGetter.StealPrefab.GetComponent<Ability>().CooldownDuration;
}
set
{
ResourcesGetter.StealPrefab.GetComponent<Ability>().CooldownDuration = value;
}
}
public float BlockCooldown
{
get
{
return ResourcesGetter.BlockPrefab.GetComponent<Ability>().CooldownDuration;
}
set
{
ResourcesGetter.BlockPrefab.GetComponent<Ability>().CooldownDuration = value;
}
}
public float TimeSlowCooldown
{
get
{
return ResourcesGetter.TimeSlowPrefab.GetComponent<Ability>().CooldownDuration;
}
set
{
ResourcesGetter.TimeSlowPrefab.GetComponent<Ability>().CooldownDuration = value;
}
}
public float TeleportCooldown
{
get
{
return ResourcesGetter.TeleportPrefab.GetComponent<Ability>().CooldownDuration;
}
set
{
ResourcesGetter.TeleportPrefab.GetComponent<Ability>().CooldownDuration = value;
}
}
[MyRPC]
private void SettingsAsked()
{
    Save();
 }
public void Save()
{
   View.RPC("SetSettings", RPCTargets.All, continuouMovementToggle.isOn, ParseFloat(attackerAccelerationInputField.text, AttackerAcceleration), ParseFloat(attackerMaxSpeedInputField.text, AttackerMaxSpeed), ParseFloat(goalieSpeedInputField.text, GoalieSpeed), ParseFloat(shootPowerLevelInputField.text, ShootPowerLevel), ParseFloat(brakeProportionInputField.text, BrakeProportion), ParseFloat(timeSlowProportionInputField.text, TimeSlowProportion), ParseFloat(moveCooldownInputField.text, MoveCooldown), ParseFloat(dashCooldownInputField.text, DashCooldown), ParseFloat(jumpCooldownInputField.text, JumpCooldown), ParseFloat(passCooldownInputField.text, PassCooldown), ParseFloat(stealCooldownInputField.text, StealCooldown), ParseFloat(blockCooldownInputField.text, BlockCooldown), ParseFloat(timeSlowCooldownInputField.text, TimeSlowCooldown), ParseFloat(teleportCooldownInputField.text, TeleportCooldown));
}
[MyRPC]
public void SetSettings(bool continuouMovement, float attackerAcceleration, float attackerMaxSpeed, float goalieSpeed, float shootPowerLevel, float brakeProportion, float timeSlowProportion, float moveCooldown, float dashCooldown, float jumpCooldown, float passCooldown, float stealCooldown, float blockCooldown, float timeSlowCooldown, float teleportCooldown)
{
ContinuouMovement = continuouMovement;
continuouMovementToggle.isOn = ContinuouMovement;
AttackerAcceleration = attackerAcceleration;
attackerAccelerationInputField.text = AttackerAcceleration+"";
AttackerMaxSpeed = attackerMaxSpeed;
attackerMaxSpeedInputField.text = AttackerMaxSpeed+"";
GoalieSpeed = goalieSpeed;
goalieSpeedInputField.text = GoalieSpeed+"";
ShootPowerLevel = shootPowerLevel;
shootPowerLevelInputField.text = ShootPowerLevel+"";
BrakeProportion = brakeProportion;
brakeProportionInputField.text = BrakeProportion+"";
TimeSlowProportion = timeSlowProportion;
timeSlowProportionInputField.text = TimeSlowProportion+"";
MoveCooldown = moveCooldown;
moveCooldownInputField.text = MoveCooldown+"";
DashCooldown = dashCooldown;
dashCooldownInputField.text = DashCooldown+"";
JumpCooldown = jumpCooldown;
jumpCooldownInputField.text = JumpCooldown+"";
PassCooldown = passCooldown;
passCooldownInputField.text = PassCooldown+"";
StealCooldown = stealCooldown;
stealCooldownInputField.text = StealCooldown+"";
BlockCooldown = blockCooldown;
blockCooldownInputField.text = BlockCooldown+"";
TimeSlowCooldown = timeSlowCooldown;
timeSlowCooldownInputField.text = TimeSlowCooldown+"";
TeleportCooldown = teleportCooldown;
teleportCooldownInputField.text = TeleportCooldown+"";
}
public void Show(bool show)
{
    if (show){ Reset();}
    panel.SetActive(show);
}
private void Reset()
{
if (!NetworkingState.IsServer)
  View.RPC("SettingsAsked", RPCTargets.Server);
continuouMovementToggle.isOn = ContinuouMovement;
attackerAccelerationInputField.text = AttackerAcceleration+"";
attackerMaxSpeedInputField.text = AttackerMaxSpeed+"";
goalieSpeedInputField.text = GoalieSpeed+"";
shootPowerLevelInputField.text = ShootPowerLevel+"";
brakeProportionInputField.text = BrakeProportion+"";
timeSlowProportionInputField.text = TimeSlowProportion+"";
moveCooldownInputField.text = MoveCooldown+"";
dashCooldownInputField.text = DashCooldown+"";
jumpCooldownInputField.text = JumpCooldown+"";
passCooldownInputField.text = PassCooldown+"";
stealCooldownInputField.text = StealCooldown+"";
blockCooldownInputField.text = BlockCooldown+"";
timeSlowCooldownInputField.text = TimeSlowCooldown+"";
teleportCooldownInputField.text = TeleportCooldown+"";
}
}

