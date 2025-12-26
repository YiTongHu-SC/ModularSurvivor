
using cfg.game;
using LubanGenerated.TableTool;

namespace Combat.Actors
{
    public static class ActorAttributeUtils
    {
        public static Tbconfig GameConfig => TableTool.Tables.Tbconfig;
        public const string StrengthHpKey = "strength_hp";
        public const string StrengthHpRegenKey = "strength_hpr";
        public const string AgilityValueKey = "agility";
        public const string IntelligenceValueKey = "intelligence";

        public static float GetCurrentValue(float baseValue, float bonusValue, int level = 1)
        {
            return baseValue + bonusValue * (level - 1);
        }

        public static float GetMaxHp(float baseValue, float bonusValue, int level = 1)
        {
            return GetCurrentValue(baseValue, bonusValue, level) * GameConfig.Get(StrengthHpKey).Value;
        }

        public static float GetHpRegeneration(float baseValue, float bonusValue, int level = 1)
        {
            return GetCurrentValue(baseValue, bonusValue, level) * GameConfig.Get(StrengthHpRegenKey).Value;
        }

        public static float GetAgilityValue(float baseValue, float bonusValue, int level = 1)
        {
            return GetCurrentValue(baseValue, bonusValue, level) * GameConfig.Get(AgilityValueKey).Value;
        }

        public static float GetIntelligenceValue(float baseValue, float bonusValue, int level = 1)
        {
            return GetCurrentValue(baseValue, bonusValue, level) * GameConfig.Get(IntelligenceValueKey).Value;
        }
    }

}
