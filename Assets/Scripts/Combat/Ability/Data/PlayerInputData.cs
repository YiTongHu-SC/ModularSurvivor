namespace Combat.Ability.Data
{
    public class PlayerInputData : AbilityData
    {
        public float DeadZone = 0.1f;

        public PlayerInputData()
        {
            AbilityType = AbilityType.PlayerInput;
        }
    }
}