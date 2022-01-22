using Model.Enums;

/*
 * Converts Blizzard's category to my category heh.
 */

namespace TriggerParser.Converter
{
    public static class CategoryConverter
    {
        public static Category ConvertBlizzardCategory(string category)
        {
            Category _enum = Category.AI; // default

            if(category == "TC_ARITHMETIC" || category == "TC_AI" || category == "TC_CONVERSION")
                _enum = Category.AI;
            if (category == "TC_NOTHING"  ||  category == "TC_CUSTOM" || category == "TC_SKIPACTIONS" || category == "TC_EVENTRESPONSE" || category == "TC_TEXTTAG" || category == "TC_IMAGE" || category == "TC_LIGHTNING" || category == "TC_TRIGGER" || category == "TC_UBERSPLAT")
                _enum = Category.Nothing;
            if (category == "TC_COMMENT")
                _enum = Category.Comment;
            if (category == "TC_CONDITION" || category == "TC_LOGIC" || category == "TC_FORLOOP" || category == "TC_MATH")
                _enum = Category.Logical;
            if (category == "TC_WAIT")
                _enum = Category.Wait;
            if (category == "TC_SETVARIABLE" || category == "TC_LAST" || category == "TC_GAMECACHE" || category == "TC_HASHTABLE")
                _enum = Category.SetVariable;
            if (category == "TC_ABILITY")
                _enum = Category.Ability;
            if (category == "TC_ANIMATION")
                _enum = Category.Animation;
            if (category == "TC_CAMERA" || category == "TC_CINEMATIC" || category == "TC_SPECIALEFFECT")
                _enum = Category.Camera;
            if (category == "TC_TIMER")
                _enum = Category.Timer;
            if (category == "TC_DESTRUCT")
                _enum = Category.Destructible;
            if (category == "TC_DIALOG")
                _enum = Category.Dialog;
            if (category == "TC_ENVIRONMENT")
                _enum = Category.Environment;
            if (category == "TC_GAME")
                _enum = Category.Game;
            if (category == "TC_HERO")
                _enum = Category.Hero;
            if (category == "TC_ITEM")
                _enum = Category.Item;
            if (category == "TC_LEADERBOARD" || category == "TC_MULTIBOARD" || category == "TC_QUEST")
                _enum = Category.Quest;
            if (category == "TC_MELEE")
                _enum = Category.Melee;
            if (category == "TC_NEUTRALBUILDING")
                _enum = Category.Goldmine;
            if (category == "TC_PLAYER")
                _enum = Category.Player;
            if (category == "TC_PLAYERGROUP")
                _enum = Category.PlayerGroup;
            if (category == "TC_REGION")
                _enum = Category.Region;
            if (category == "TC_UNITSEL")
                _enum = Category.UnitSelection;
            if (category == "TC_SOUND")
                _enum = Category.Sound;
            if (category == "TC_TIME")
                _enum = Category.Timer;
            if (category == "TC_UNIT")
                _enum = Category.Unit;
            if (category == "TC_UNITGROUP")
                _enum = Category.UnitGroup;
            if (category == "TC_VISIBILITY")
                _enum = Category.Visibility;

            return _enum;
        }
    }
}
