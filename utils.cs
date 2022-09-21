using System;
using System.IO;
using System.Reflection;
using Terraria;
using TShockAPI;

namespace TrashMan
{
    public class utils
    {
        /// <summary>
        /// 获取内嵌文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FromEmbeddedPath(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }


        /// <summary>
        /// 设置
        /// </summary>
        public static void SetPlayerBuff(TSPlayer op, int id, int time)
        {
            // Max possible buff duration as of Terraria 1.4.2.3 is 35791393 seconds (415 days).
            var timeLimit = (int.MaxValue / 60) - 1;
            if (time < 0 || time > timeLimit)
                time = timeLimit;

            op.SetBuff(id, time * 60);
        }

        public static string GetTimeDesc(int seconds)
        {
            if (seconds == -1) return "不限时";
            else if (seconds == 1) return "";
            else if (seconds < 60) return $"{seconds}秒";
            else if (seconds < 60 * 60) return $"{(int)Math.Floor((float)seconds / 60)}分钟";
            else if (seconds < 60 * 60 * 60) return $"{(int)Math.Floor((float)seconds / (60 * 60))}小时";
            else if (seconds < 60 * 60 * 60 * 24) return $"{(int)Math.Floor((float)seconds / (60 * 60 * 24))}天";
            else return "";
        }

        /// <summary>
        /// 生成NPC
        /// </summary>
        public static void SpawnNPC(TSPlayer op, int npcID, int count = 1)
        {
            NPC npc = new NPC();
            npc.SetDefaults(npcID);
            TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, count, op.TileX, op.TileY, 5, 2);
        }


        /// <summary>
        /// 获取当前的时间戳
        /// </summary>
        public static int GetUnixTimestamp
        {
            get { return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds; }
        }

        /// <summary>
        /// 日志
        /// </summary>
        public static void Log(string msg)
        {
            TShock.Log.ConsoleInfo($"[垃圾佬]：{msg}");
        }

    }
}