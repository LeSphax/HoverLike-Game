
namespace AbilitiesManagement
{
    public abstract class PersistentEffect
    {
        AbilitiesManager manager;

        public PersistentEffect(AbilitiesManager manager)
        {
            this.manager = manager;
            manager.persistentEffects.Add(this);
        }

        protected void DestroyEffect()
        {
            manager.persistentEffects.Remove(this);
        }


        internal abstract void ApplyEffect(float dt);
    }
}
