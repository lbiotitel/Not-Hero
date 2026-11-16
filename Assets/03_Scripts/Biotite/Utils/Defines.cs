using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Defines 
{

    public static EScene[] MapOrder = { EScene.F0, EScene.F1, EScene.Last };

    public static class SoundKey
    {

        //bgm
        public const string BATTLE_BGM = "BattleNode BGM";
        public const string SHOP_BGM = "ShopNode BGM";
        public const string BOSS_BGM = "BossBattle BGM";
        public const string EVENT_BGM = "EventNode BGM";

        //sfx
        public const string TURN_SFX = "Turn SFX";
        public const string SHOP_SELL_SFX = "ShopSell SFX";
        public const string OPEN_TREASURE_SFX = "OpenTreasure SFX";
        public const string GET_EQUIPMENT_SFX = "GetEquipment SFX";
        public const string CARD_MOVE_SFX = "Card SFX";
        public const string GOLD_SFX = "GetGold SFX";
        public const string MAP_OPEN_SFX = "MapOpen SFX";
        public const string TURN_BUTTON_SFX = "TurnButton SFX";
    }


    public static class ResourcePathKey
    {
        public const string CardDataPath = "CardDatas/";
        public const string ItemPath = "ItemDatas/";
        public const string StagePath = "StageDatas/";
        public const string UnitDataPath = "UnitDatas/";
        public const string SoundDataPath = "Sounds/";
    }


    public static class TimeKey
    {
        public const float CARD_DAMAGE_EFFECT_DELAY = 0.1f;
    }

    

}
