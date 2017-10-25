namespace PlayerManagement
{
    public enum MovementState
    {
        PLAYING,
        FROZEN,
        NO_MOVEMENT,
    }

    public enum StealingState
    {
        IDLE,
        STEALING,
        PROTECTED,
    }

    public class PlayerState
    {
        private Player player;

        internal MovementState movement;
        public MovementState Movement
        {
            get
            {
                return movement;
            }
            set
            {
                movement = value;
                player.flagsChanged |= PlayerFlags.MOVEMENT_STATE;
                player.NotifyMovementStateChanged();
            }
        }
        internal StealingState stealing;
        public StealingState Stealing
        {
            get
            {
                return stealing;
            }
            set
            {
                stealing = value;
                player.flagsChanged |= PlayerFlags.STEALING_STATE;
                player.NotifyStealingStateChanged();
            }
        }

        public PlayerState(Player player, MovementState movementState, StealingState stealingState)
        {
            this.player = player;
            movement = movementState;
            stealing = stealingState;
        }
    }
}