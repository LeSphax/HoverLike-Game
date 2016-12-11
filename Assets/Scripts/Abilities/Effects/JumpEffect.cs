using UnityEngine;

public class JumpEffect : AbilityEffect
{
    public override void ApplyOnTarget(params object[] parameters)
    {
        PlayerController controller = (PlayerController)parameters[0];
        controller.abilitiesManager.View.RPC("Jump", RPCTargets.Server);
    }
}

namespace AbilitiesManagement
{
    public class JumpPersistentEffect : PersistentEffect
    {

        float lowestPoint;
        const float highestPoint = 20f;

        AnimationCurve jumpCurve;

        public JumpPersistentEffect(AbilitiesManager manager, AnimationCurve jumpCurve) : base(manager)
        {
            duration = 1.25f;
            this.jumpCurve = jumpCurve;
            lowestPoint = manager.controller.Player.SpawningPoint.y;
            manager.controller.GetComponent<Rigidbody>().useGravity = false;
            manager.EffectsManager.ShockwaveOnPlayer(false);

        }

        public override void StopEffect()
        {
            manager.controller.GetComponent<Rigidbody>().useGravity = true;
            manager.EffectsManager.ShockwaveOnPlayer(true);
        }

        protected override void Apply(float dt)
        {
            float jumpHeightProportion = jumpCurve.Evaluate(time / duration);
            float currentHeight = lowestPoint * (1 - jumpHeightProportion) + highestPoint * jumpHeightProportion;
            manager.controller.transform.position = new Vector3(
                manager.controller.transform.position.x,
                currentHeight,
                manager.controller.transform.position.z
            );
        }
    }
}