using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;


namespace TrashMan
{
    [ApiVersion(2, 1)]
    public class TrashMan : TerrariaPlugin
    {
        # region 插件信息
        public override string Name => "TrashMan";
        public override string Description => "垃圾佬";
        public override string Author => "hufang360";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        #endregion

        public static Config _config;
        public static bool isLoading = false;
        public static string saveDir = Path.Combine(TShock.SavePath, "TrashMan");
        public static readonly string configFile = Path.Combine(saveDir, "config.json");

        private int t1 = 0;
        private readonly bool[] warned = new bool[256];
        private readonly List<int> records = new List<int>();


        public TrashMan(Main game) : base(game)
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);

            //GeneralHooks.ReloadEvent += OnReload;

            if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);
            Reload();
        }

        /// <summary>
        /// 重载配置文件
        /// </summary>
        private void OnReload(ReloadEventArgs args)
        {
            Reload(true);
            args.Player.SendSuccessMessage("[垃圾桶]重载配置完成");
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        private void OnGetData(GetDataEventArgs args)
        {
            switch (args.MsgID)
            {
                // 打开箱子
                case PacketTypes.ChestGetContents:
                    using (MemoryStream ms = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length))
                    using (BinaryReader br = new BinaryReader(ms, Encoding.UTF8, true))
                    {
                        int tileX = br.ReadInt16();
                        int tileY = br.ReadInt16();
                        OnChestOpen(TShock.Players[args.Msg.whoAmI], tileX, tileY);
                    }
                    break;

                default:
                    //utils.Log(args.MsgID.ToString());
                    break;
            }
        }

        /// <summary>
        /// 打开箱子
        /// </summary>
        private void OnChestOpen(TSPlayer op, int tileX, int tileY)
        {
            // 找出对应坐标的箱子
            int index = Chest.FindChest(tileX, tileY);
            if (index == -1)
            {
                utils.Log("找不到这个箱子");
                return;
            }

            // 仅指定箱子名
            Chest ch = Main.chest[index];
            if (string.IsNullOrEmpty(ch.name)) return;

            // 仅有配置数据的
            Reload();
            int cIndex = -1;
            for (int i = 0; i < _config.chest.Count; i++)
            {
                if (_config.chest[i].name == ch.name)
                {
                    cIndex = i;
                    break;
                }
            }
            if (cIndex == -1) return;


            // 仅限 指定类型的箱子
            int chestType = _config.chest[cIndex].type > 0 ? _config.chest[cIndex].type : 6;
            if (chestType >= 46701)
            {
                if (Main.tile[tileX, tileY].frameX != (chestType - 46701) * 36 && Main.tile[tileX, tileY].frameX != 0) return;
            }
            else
            {
                if (Main.tile[tileX, tileY].frameX != (chestType) * 36 && Main.tile[tileX, tileY].frameX != 0) return;
            }

            ChestData chData = _config.chest[cIndex];

            // 附近是否有指定的npc
            bool hasNPC = false;
            int npcID = chData.id;
            Rectangle area = new Rectangle(tileX - 61, tileY - 34 + 3, 122, 68);
            foreach (NPC n in Main.npc)
            {
                if (n.active && n.type == npcID && area.Contains((int)Math.Round(n.position.X / 16), (int)Math.Round(n.position.Y / 16)))
                {
                    hasNPC = true;
                    break;
                }
            }
            if (!hasNPC)
            {
                op.SendInfoMessage($"[i:348]{chData.name} 需放在 {TShock.Utils.GetNPCById(npcID).TypeName} 身边");
                return;
            }

            // 每个玩家仅提示一次
            if (!warned[op.Index])
            {
                op.SendInfoMessage("垃圾桶[i:348]内不要放东西，会丢，讲真！");
                warned[op.Index] = true;
            }

            // 填充物品
            if (!records.Contains(npcID))
            {
                // 生成物品
                List<GiftData> gifts = chData.FilterByUnlock(op.Index);
                List<int> randList = chData.GetPercent(gifts);
                int rnum = WorldGen.genRand.Next(chData.SumPercent(gifts));
                int num2 = 0;

                int startIndex = 0;
                for (int i = 0; i < randList.Count; i++)
                {
                    num2 += randList[i];
                    if (rnum < num2)
                    {
                        startIndex = 1;
                        ch.item[0].netID = gifts[i].id;
                        ch.item[0].prefix = (byte)gifts[i].prefix;
                        ch.item[0].stack = gifts[i].stack;

                        gifts[i].Trans(op);
                        if (!string.IsNullOrEmpty(gifts[i].msg))
                        {
                            if (gifts[i].serverMsg)
                                TSPlayer.All.SendInfoMessage(gifts[i].msg);
                            else
                                op.SendInfoMessage(gifts[i].msg);
                        }
                        break;
                    }
                }
                for (int i = startIndex; i < 40; i++)
                {
                    ch.item[i].netDefaults(0);
                }
                NetMessage.SendData(31, -1, -1, null, ch.x, ch.y);
                records.Add(npcID);

                // 触发事件
                List<EventData> tces = chData.FilterEventByUnlock(op.Index);
                if (tces.Count > 0 && WorldGen.genRand.Next(10) > 8)
                {
                    EventData tce = chData.tcevent[Main.rand.Next(tces.Count)];
                    tce.Trans(op);

                    switch (tce.type)
                    {
                        // buff
                        case 1: utils.SetPlayerBuff(op, tce.id, tce.num); break;

                        // 生成NPC
                        case 2: utils.SpawnNPC(op, tce.id, tce.num); break;

                        // 掉落物品
                        case 3: op.GiveItem(tce.id, tce.num); break;
                    }

                    if (!string.IsNullOrEmpty(tce.msg))
                    {
                        if (tce.serverMsg)
                            TSPlayer.All.SendInfoMessage(tce.msg);
                        else
                            op.SendInfoMessage(tce.msg);
                    }
                }
            }
        }


        /// <summary>
        /// 时间流逝
        /// </summary>
        private void OnGameUpdate(EventArgs args)
        {
            if (Main.dayTime && Main.time < 10)
            {
                // 5秒内不重复更新
                if (utils.GetUnixTimestamp - t1 < 5) return;

                utils.Log("新的一天开始了，垃圾佬狂喜~");
                records.Clear();
                t1 = utils.GetUnixTimestamp;
            }
        }

        /// <summary>
        /// 重载配置文件
        /// </summary>
        /// <param name="force">是否强制加载</param>
        private void Reload(bool force = false)
        {
            if (force || isLoading == false)
            {
                _config = Config.Load(configFile);
                isLoading = true;
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);

                //GeneralHooks.ReloadEvent -= OnReload;
            }
            base.Dispose(disposing);
        }

    }

}
