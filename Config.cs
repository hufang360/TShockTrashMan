using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Events;
using TShockAPI;

namespace TrashMan
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public class Config
    {
        // 箱子物品
        public List<ChestData> chest = new List<ChestData>();

        // 内嵌配置文件名称
        public const string RES_NAME = "TrashMan.res.config.json";

        public static Config Load(string path)
        {
            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path), new JsonSerializerSettings()
                {
                    Error = (sender, error) => error.ErrorContext.Handled = true
                });
            }
            else
            {
                // 读取内嵌配置文件
                string text = utils.FromEmbeddedPath(RES_NAME);
                Config c = JsonConvert.DeserializeObject<Config>(text, new JsonSerializerSettings()
                {
                    Error = (sender, error) => error.ErrorContext.Handled = true
                });

                // 将内嵌配置文件拷出
                File.WriteAllText(path, text);

                return c;
            }
        }

    }


    public class ChestData
    {
        // 物品
        public List<GiftData> item = new List<GiftData>();

        // 随机事件
        [JsonProperty("event")]
        public List<EventData> tcevent = new List<EventData>();

        // npc id
        public int id = 0;

        // 箱子名
        public string name = "";

        // 箱子类型
        public int type = 0;

        public List<int> GetPercent(List<GiftData> gifts)
        {
            List<int> randList = new List<int>();
            foreach (GiftData i in gifts)
            {
                randList.Add(i.percent);
            }
            return randList;
        }

        public int SumPercent(List<GiftData> gifts)
        {
            int num = 0;
            foreach (GiftData i in gifts)
            {
                num += i.percent;
            }
            return num;
        }

        public List<GiftData> FilterByUnlock(int plrIndex)
        {
            List<GiftData> items = new List<GiftData>();
            foreach (GiftData item in item)
            {
                bool pass = true;
                foreach (string s in item.unlock)
                {
                    pass = CheckUnlock(plrIndex, s);
                }
                if (pass)
                    items.Add(item);
            }
            // 最多 40 格
            if (items.Count > 40) items.RemoveRange(40, items.Count - 40);
            return items;
        }

        public List<EventData> FilterEventByUnlock(int plrIndex)
        {
            List<EventData> eLists = new List<EventData>();

            foreach (EventData e in tcevent)
            {
                bool pass = true;
                foreach (string s in e.unlock)
                {
                    pass = CheckUnlock(plrIndex, s);
                }
                if (pass)
                    eLists.Add(e);
            }

            return eLists;
        }

        /// <summary>
        /// 检查解锁条件
        /// </summary>
        /// <param name="plrIndex"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool CheckUnlock(int plrIndex, string s)
        {
            bool pass = false;
            switch (s)
            {
                // boss
                case "史莱姆王": case "史王": if (!NPC.downedSlimeKing) pass = false; break;
                case "克苏鲁之眼": case "克眼": if (!NPC.downedBoss1) pass = false; break;
                case "鹿角怪": case "巨鹿": if (!NPC.downedDeerclops) pass = false; break;
                case "世界吞噬者": case "世界吞噬怪": case "克苏鲁之脑": case "世吞": case "克脑": if (!NPC.downedBoss2) pass = false; break;
                case "蜂王": if (!NPC.downedQueenBee) pass = false; break;
                case "骷髅王": if (!NPC.downedBoss3) pass = false; break;
                case "血肉墙": case "肉后": case "困难模式": if (!Main.hardMode) pass = false; break;
                case "毁灭者": if (!NPC.downedMechBoss1) pass = false; break;
                case "双子魔眼": if (!NPC.downedMechBoss2) pass = false; break;
                case "机械骷髅王": if (!NPC.downedMechBoss3) pass = false; break;
                case "世纪之花": case "花后": case "世花": if (!NPC.downedPlantBoss) pass = false; break;
                case "石巨人": case "石后": if (!NPC.downedGolemBoss) pass = false; break;
                case "史莱姆皇后": case "史后": if (!NPC.downedQueenSlime) pass = false; break;
                case "光之女皇": case "光女": if (!NPC.downedEmpressOfLight) pass = false; break;
                case "猪龙鱼公爵": case "猪鲨": if (!NPC.downedFishron) pass = false; break;
                case "拜月教邪教徒": case "教徒": if (!NPC.downedAncientCultist) pass = false; break;
                case "月亮领主": case "月后": if (!NPC.downedMoonlord) pass = false; break;
                case "哀木": if (!NPC.downedHalloweenTree) pass = false; break;
                case "南瓜王": if (!NPC.downedHalloweenKing) pass = false; break;
                case "常绿尖叫怪": if (!NPC.downedChristmasTree) pass = false; break;
                case "冰雪女王": if (!NPC.downedChristmasIceQueen) pass = false; break;
                case "圣诞坦克": if (!NPC.downedChristmasSantank) pass = false; break;
                case "火星飞碟": if (!NPC.downedMartians) pass = false; break;
                case "小丑": if (!NPC.downedClown) pass = false; break;
                case "日耀柱": if (!NPC.downedTowerSolar) pass = false; break;
                case "星旋柱": if (!NPC.downedTowerVortex) pass = false; break;
                case "星云柱": if (!NPC.downedTowerNebula) pass = false; break;
                case "星尘柱": if (!NPC.downedTowerStardust) pass = false; break;
                case "一王后": if (!NPC.downedMechBossAny) pass = false; break;
                case "三王后": if (!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3) pass = false; break;
                case "一柱后": pass = (NPC.downedTowerNebula || NPC.downedTowerSolar || NPC.downedTowerStardust || NPC.downedTowerVortex); break;
                case "四柱后": if (!NPC.downedTowerNebula || !NPC.downedTowerSolar || !NPC.downedTowerStardust || !NPC.downedTowerVortex) pass = false; break;
                case "哥布林入侵": if (!NPC.downedGoblins) pass = false; break;
                case "海盗入侵": if (!NPC.downedPirates) pass = false; break;
                case "霜月": if (!NPC.downedFrost) pass = false; break;

                // 天候
                case "血月": if (!Main.bloodMoon) pass = false; break;
                case "雨天": if (!Main.raining) pass = false; break;
                case "白天": if (!Main.dayTime) pass = false; break;
                case "晚上": if (Main.dayTime) pass = false; break;
                case "大风天": if (!Main.IsItAHappyWindyDay) pass = false; break;

                // 节日和派对
                case "万圣节": if (!Main.halloween) pass = false; break;
                case "圣诞节": if (!Main.xMas) pass = false; break;
                case "派对": if (!BirthdayParty.PartyIsUp) pass = false; break;

                // 秘密世界
                case "2020": if (!Main.drunkWorld) pass = false; break;
                case "2021": if (!Main.tenthAnniversaryWorld) pass = false; break;
                case "ftw": if (!Main.getGoodWorld) pass = false; break;
                case "ntb": if (!Main.notTheBeesWorld) pass = false; break;
                case "dst": if (!Main.dontStarveWorld) pass = false; break;

                // 环境
                case "森林": if (!Main.player[plrIndex].ShoppingZone_Forest) pass = false; break;
                case "丛林": if (!Main.player[plrIndex].ZoneJungle) pass = false; break;
                case "沙漠": if (!Main.player[plrIndex].ZoneDesert) pass = false; break;
                case "雪原": if (!Main.player[plrIndex].ZoneSnow) pass = false; break;
                case "洞穴": if (!Main.player[plrIndex].ZoneUnderworldHeight) pass = false; break;
                case "海洋": if (!Main.player[plrIndex].ZoneBeach) pass = false; break;
                case "神圣": if (!Main.player[plrIndex].ZoneHallow) pass = false; break;
                case "蘑菇": if (!Main.player[plrIndex].ZoneGlowshroom) pass = false; break;

                case "腐化之地": if (!Main.player[plrIndex].ZoneCorrupt) pass = false; break;
                case "猩红之地": if (!Main.player[plrIndex].ZoneCrimson) pass = false; break;
                case "地牢": if (!Main.player[plrIndex].ZoneDungeon) pass = false; break;
                case "墓地": if (!Main.player[plrIndex].ZoneGraveyard) pass = false; break;
                case "蜂巢": if (!Main.player[plrIndex].ZoneHive) pass = false; break;
                case "神庙": if (!Main.player[plrIndex].ZoneLihzhardTemple) pass = false; break;
                case "沙尘暴": if (!Main.player[plrIndex].ZoneSandstorm) pass = false; break;
                case "天空": if (!Main.player[plrIndex].ZoneSkyHeight) pass = false; break;

                default: break;
            }
            return pass;
        }
    }



    public class GiftData
    {
        // 物品id
        public int id = 0;

        // 词缀
        public int prefix = 0;

        // 堆叠
        public int stack = 1;

        // 百分比
        public int percent = 50;

        // 通知
        public string msg = "";

        // 是否为全服通知
        public bool serverMsg = false;

        // 解锁条件
        public List<string> unlock = new List<string>();


        public void Trans(TSPlayer op)
        {
            msg = msg.Replace("{player}", op.Name);
        }
    }


    /// <summary>
    /// 翻箱子事件
    /// </summary>
    public class EventData
    {
        // 类型：1 获得Buff，2 生成NPC（含敌怪和boss）
        public int type = 0;

        public int id = 0;
        public int num = 0;

        // 通知
        public string msg = "";

        // 是否为全服通知
        public bool serverMsg = false;

        // 解锁条件
        public List<string> unlock = new List<string>();


        public void Trans(TSPlayer op)
        {
            msg = msg.Replace("{player}", op.Name);
        }
    }
}