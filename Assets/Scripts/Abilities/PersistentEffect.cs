
using System;

namespace AbilitiesManagement
{
    public abstract class PersistentEffect
    {

        protected float time;

        protected float duration = -1;
        protected AbilitiesManager manager;

        public PersistentEffect(AbilitiesManager manager)
        {
            this.manager = manager;
            manager.persistentEffects.Add(this);
        }

        protected void DestroyEffect()
        {
            manager.persistentEffects.Remove(this);
        }


        internal virtual void ApplyEffect(float dt)
        {
            time += dt;
            if (time >= duration)
            {
                StopEffect();
                DestroyEffect();
            }
            else
            {
                Apply(dt);
            }
        }

        protected abstract void Apply(float dt);

        protected abstract void StopEffect();
    }
}
