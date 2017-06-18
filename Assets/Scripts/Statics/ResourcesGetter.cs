//*************************************************************
//THIS CLASS WAS GENERATED, USE THE CORRESPONDING GENERATOR TO CHANGE IT (CodeGenerator folder)
//*************************************************************
using UnityEngine;
public class ResourcesGetter{
private static AudioClip timeSlowSound;
public static AudioClip TimeSlowSound{
get {
if (timeSlowSound == null ){
timeSlowSound= Resources.Load<AudioClip>("Sounds/TimeSlow");
}
return timeSlowSound;
}}
private static AudioClip teleport2Sound;
public static AudioClip Teleport2Sound{
get {
if (teleport2Sound == null ){
teleport2Sound= Resources.Load<AudioClip>("Sounds/Teleport2");
}
return teleport2Sound;
}}
private static AudioClip boostSound;
public static AudioClip BoostSound{
get {
if (boostSound == null ){
boostSound= Resources.Load<AudioClip>("Sounds/Boost");
}
return boostSound;
}}
private static AudioClip passSound;
public static AudioClip PassSound{
get {
if (passSound == null ){
passSound= Resources.Load<AudioClip>("Sounds/Pass");
}
return passSound;
}}
private static AudioClip softErrorSound;
public static AudioClip SoftErrorSound{
get {
if (softErrorSound == null ){
softErrorSound= Resources.Load<AudioClip>("Sounds/SoftError");
}
return softErrorSound;
}}
private static AudioClip landingSound;
public static AudioClip LandingSound{
get {
if (landingSound == null ){
landingSound= Resources.Load<AudioClip>("Sounds/Landing");
}
return landingSound;
}}
private static AudioClip jumpingSound;
public static AudioClip JumpingSound{
get {
if (jumpingSound == null ){
jumpingSound= Resources.Load<AudioClip>("Sounds/Jumping");
}
return jumpingSound;
}}
private static AudioClip butSound;
public static AudioClip ButSound{
get {
if (butSound == null ){
butSound= Resources.Load<AudioClip>("Sounds/But");
}
return butSound;
}}
private static AudioClip joinRoomSound;
public static AudioClip JoinRoomSound{
get {
if (joinRoomSound == null ){
joinRoomSound= Resources.Load<AudioClip>("Sounds/JoinRoom");
}
return joinRoomSound;
}}
private static GameObject settingsPanelPrefab;
public static GameObject SettingsPanelPrefab{
get {
if (settingsPanelPrefab == null ){
settingsPanelPrefab= Resources.Load<GameObject>("Prefabs/SettingsPanel");
}
return settingsPanelPrefab;
}}
private static GameObject tempAudioSourcePrefab;
public static GameObject TempAudioSourcePrefab{
get {
if (tempAudioSourcePrefab == null ){
tempAudioSourcePrefab= Resources.Load<GameObject>("Prefabs/TempAudioSource");
}
return tempAudioSourcePrefab;
}}
private static GameObject timeSlowTargeterPrefab;
public static GameObject TimeSlowTargeterPrefab{
get {
if (timeSlowTargeterPrefab == null ){
timeSlowTargeterPrefab= Resources.Load<GameObject>("Prefabs/TimeSlowTargeter");
}
return timeSlowTargeterPrefab;
}}
private static GameObject passTargeterPrefab;
public static GameObject PassTargeterPrefab{
get {
if (passTargeterPrefab == null ){
passTargeterPrefab= Resources.Load<GameObject>("Prefabs/PassTargeter");
}
return passTargeterPrefab;
}}
private static GameObject screenFaderPrefab;
public static GameObject ScreenFaderPrefab{
get {
if (screenFaderPrefab == null ){
screenFaderPrefab= Resources.Load<GameObject>("Prefabs/ScreenFader");
}
return screenFaderPrefab;
}}
private static GameObject tooltipPrefab;
public static GameObject TooltipPrefab{
get {
if (tooltipPrefab == null ){
tooltipPrefab= Resources.Load<GameObject>("Prefabs/Tooltip");
}
return tooltipPrefab;
}}
private static GameObject shadowPrefab;
public static GameObject ShadowPrefab{
get {
if (shadowPrefab == null ){
shadowPrefab= Resources.Load<GameObject>("Prefabs/Shadow");
}
return shadowPrefab;
}}
private static GameObject keyToUsePrefab;
public static GameObject KeyToUsePrefab{
get {
if (keyToUsePrefab == null ){
keyToUsePrefab= Resources.Load<GameObject>("Prefabs/KeyToUse");
}
return keyToUsePrefab;
}}
private static GameObject playerInfoPrefab;
public static GameObject PlayerInfoPrefab{
get {
if (playerInfoPrefab == null ){
playerInfoPrefab= Resources.Load<GameObject>("Prefabs/PlayerInfo");
}
return playerInfoPrefab;
}}
private static GameObject disabledPrefab;
public static GameObject DisabledPrefab{
get {
if (disabledPrefab == null ){
disabledPrefab= Resources.Load<GameObject>("Prefabs/Abilities/Disabled");
}
return disabledPrefab;
}}
private static GameObject cooldownPrefab;
public static GameObject CooldownPrefab{
get {
if (cooldownPrefab == null ){
cooldownPrefab= Resources.Load<GameObject>("Prefabs/Abilities/Cooldown");
}
return cooldownPrefab;
}}
private static GameObject abilityPrefab;
public static GameObject AbilityPrefab{
get {
if (abilityPrefab == null ){
abilityPrefab= Resources.Load<GameObject>("Prefabs/Abilities/Ability");
}
return abilityPrefab;
}}
private static GameObject moveUIAnimationPrefab;
public static GameObject MoveUIAnimationPrefab{
get {
if (moveUIAnimationPrefab == null ){
moveUIAnimationPrefab= Resources.Load<GameObject>("Prefabs/MoveUIAnimation");
}
return moveUIAnimationPrefab;
}}
private static Material fieldMaterial;
public static Material FieldMaterial{
get {
if (fieldMaterial == null ){
fieldMaterial= Resources.Load<Material>("Materials/Field");
}
return fieldMaterial;
}}
private static Material[] helmetMaterials;
public static Material[] HelmetMaterials{
get {
if (helmetMaterials == null ){
helmetMaterials= new Material[2];
helmetMaterials[0]= Resources.Load<Material>("Materials/BlueHelmet");
helmetMaterials[1]= Resources.Load<Material>("Materials/RedHelmet");
}
return helmetMaterials;
}}
private static Material[] skateMaterials;
public static Material[] SkateMaterials{
get {
if (skateMaterials == null ){
skateMaterials= new Material[2];
skateMaterials[0]= Resources.Load<Material>("Materials/BlueSkate");
skateMaterials[1]= Resources.Load<Material>("Materials/RedSkate");
}
return skateMaterials;
}}
private static GameObject movePrefab;
public static GameObject MovePrefab{
get {
if (movePrefab == null ){
movePrefab= Resources.Load<GameObject>("Prefabs/Abilities/Move");
}
return movePrefab;
}}
private static GameObject dashPrefab;
public static GameObject DashPrefab{
get {
if (dashPrefab == null ){
dashPrefab= Resources.Load<GameObject>("Prefabs/Abilities/Dash");
}
return dashPrefab;
}}
private static GameObject jumpPrefab;
public static GameObject JumpPrefab{
get {
if (jumpPrefab == null ){
jumpPrefab= Resources.Load<GameObject>("Prefabs/Abilities/Jump");
}
return jumpPrefab;
}}
private static GameObject passPrefab;
public static GameObject PassPrefab{
get {
if (passPrefab == null ){
passPrefab= Resources.Load<GameObject>("Prefabs/Abilities/Pass");
}
return passPrefab;
}}
private static GameObject stealPrefab;
public static GameObject StealPrefab{
get {
if (stealPrefab == null ){
stealPrefab= Resources.Load<GameObject>("Prefabs/Abilities/Steal");
}
return stealPrefab;
}}
private static GameObject blockPrefab;
public static GameObject BlockPrefab{
get {
if (blockPrefab == null ){
blockPrefab= Resources.Load<GameObject>("Prefabs/Abilities/Block");
}
return blockPrefab;
}}
private static GameObject timeSlowPrefab;
public static GameObject TimeSlowPrefab{
get {
if (timeSlowPrefab == null ){
timeSlowPrefab= Resources.Load<GameObject>("Prefabs/Abilities/TimeSlow");
}
return timeSlowPrefab;
}}
private static GameObject teleportPrefab;
public static GameObject TeleportPrefab{
get {
if (teleportPrefab == null ){
teleportPrefab= Resources.Load<GameObject>("Prefabs/Abilities/Teleport");
}
return teleportPrefab;
}}
public static void LoadAll(){
var tempTimeSlowSound = TimeSlowSound;
var tempTeleport2Sound = Teleport2Sound;
var tempBoostSound = BoostSound;
var tempPassSound = PassSound;
var tempSoftErrorSound = SoftErrorSound;
var tempLandingSound = LandingSound;
var tempJumpingSound = JumpingSound;
var tempButSound = ButSound;
var tempJoinRoomSound = JoinRoomSound;
var tempSettingsPanelPrefab = SettingsPanelPrefab;
var tempTempAudioSourcePrefab = TempAudioSourcePrefab;
var tempTimeSlowTargeterPrefab = TimeSlowTargeterPrefab;
var tempPassTargeterPrefab = PassTargeterPrefab;
var tempScreenFaderPrefab = ScreenFaderPrefab;
var tempTooltipPrefab = TooltipPrefab;
var tempShadowPrefab = ShadowPrefab;
var tempKeyToUsePrefab = KeyToUsePrefab;
var tempPlayerInfoPrefab = PlayerInfoPrefab;
var tempDisabledPrefab = DisabledPrefab;
var tempCooldownPrefab = CooldownPrefab;
var tempAbilityPrefab = AbilityPrefab;
var tempMoveUIAnimationPrefab = MoveUIAnimationPrefab;
var tempFieldMaterial = FieldMaterial;
var tempHelmetMaterials = HelmetMaterials;
var tempSkateMaterials = SkateMaterials;
var tempMovePrefab = MovePrefab;
var tempDashPrefab = DashPrefab;
var tempJumpPrefab = JumpPrefab;
var tempPassPrefab = PassPrefab;
var tempStealPrefab = StealPrefab;
var tempBlockPrefab = BlockPrefab;
var tempTimeSlowPrefab = TimeSlowPrefab;
var tempTeleportPrefab = TeleportPrefab;
}
}

