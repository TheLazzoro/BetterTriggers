using DataAccess.Data;

/*
 * Converts Blizzard's category to my category heh.
 */

namespace TriggerParser.Converter
{
    public static class CategoryConverter
    {
        public static EnumCategory ConvertBlizzardCategory(string category)
        {
            EnumCategory _enum = EnumCategory.AI; // default

            if(category == "TC_ARITHMETIC" || category == "TC_AI" || category == "TC_CONVERSION")
                _enum = EnumCategory.AI;
            if (category == "TC_NOTHING"  ||  category == "TC_CUSTOM" || category == "TC_SKIPACTIONS" || category == "TC_EVENTRESPONSE" || category == "TC_TEXTTAG" || category == "TC_IMAGE" || category == "TC_LIGHTNING" || category == "TC_TRIGGER" || category == "TC_UBERSPLAT")
                _enum = EnumCategory.Nothing;
            if (category == "TC_COMMENT")
                _enum = EnumCategory.Comment;
            if (category == "TC_CONDITION" || category == "TC_LOGIC" || category == "TC_FORLOOP" || category == "TC_MATH")
                _enum = EnumCategory.Logical;
            if (category == "TC_WAIT")
                _enum = EnumCategory.Wait;
            if (category == "TC_SETVARIABLE" || category == "TC_LAST" || category == "TC_GAMECACHE" || category == "TC_HASHTABLE")
                _enum = EnumCategory.SetVariable;
            if (category == "TC_ABILITY")
                _enum = EnumCategory.Ability;
            if (category == "TC_ANIMATION")
                _enum = EnumCategory.Animation;
            if (category == "TC_CAMERA" || category == "TC_CINEMATIC" || category == "TC_SPECIALEFFECT")
                _enum = EnumCategory.Camera;
            if (category == "TC_TIMER")
                _enum = EnumCategory.Timer;
            if (category == "TC_DESTRUCT")
                _enum = EnumCategory.Destructible;
            if (category == "TC_DIALOG")
                _enum = EnumCategory.Dialog;
            if (category == "TC_ENVIRONMENT")
                _enum = EnumCategory.Environment;
            if (category == "TC_GAME")
                _enum = EnumCategory.Game;
            if (category == "TC_HERO")
                _enum = EnumCategory.Hero;
            if (category == "TC_ITEM")
                _enum = EnumCategory.Item;
            if (category == "TC_LEADERBOARD" || category == "TC_MULTIBOARD" || category == "TC_QUEST")
                _enum = EnumCategory.Quest;
            if (category == "TC_MELEE")
                _enum = EnumCategory.Melee;
            if (category == "TC_NEUTRALBUILDING")
                _enum = EnumCategory.Goldmine;
            if (category == "TC_PLAYER")
                _enum = EnumCategory.Player;
            if (category == "TC_PLAYERGROUP")
                _enum = EnumCategory.PlayerGroup;
            if (category == "TC_REGION")
                _enum = EnumCategory.Region;
            if (category == "TC_UNITSEL")
                _enum = EnumCategory.UnitSelection;
            if (category == "TC_SOUND")
                _enum = EnumCategory.Sound;
            if (category == "TC_TIME")
                _enum = EnumCategory.Timer;
            if (category == "TC_UNIT")
                _enum = EnumCategory.Unit;
            if (category == "TC_UNITGROUP")
                _enum = EnumCategory.UnitGroup;
            if (category == "TC_VISIBILITY")
                _enum = EnumCategory.Visibility;

            return _enum;
        }
    }
}
