# 垃圾佬
在指定NPC的旁边放置箱子，每天打开能随机获得一件物品，有时还能触发一些随机事件。<br>
玩法灵感来自《星露谷物语》。<br>
<br>

例如：<br>

  在渔夫的旁边放置一个垃圾桶，并命名成“鱼篓”。<br>
  打开时箱子里可能会有 旧鞋、海草、锡罐、宝匣药水等其中的一样。<br>
  打开时有几率 获得10秒石化buff、获得60秒恶臭buff、生成3只蝙蝠。<br>

  <br>

  在护士旁边放置一个冰冻箱，并命名成“医疗垃圾”。<br>
  打开时箱子里可能会有 水瓶、治疗药水、生命水晶等其中的一样。<br>
  打开时有几率 获得5秒中毒buff、获得5秒燃烧buff、生成1只老鼠、掉落2金币。<br>

<br>


## 插件下载
插件下载：[TrashMan-1.1.dll](https://github.com/hufang360/TShockTrashMan/releases/download/v1.1/TrashMan-v1.1.dll)

<br>

## 配置文件
安装插件后，启动服务器时会自动生成配置文件，路径位于 `./tshock/TrashMan/config.json`，此文件和本项目的 [./res/config.json](./res/config.json) 文件相同。<br>

配置说明：
```json
{
    // （这个是备注，目的是说明各个字段，实际配置这样写会出错）
    "chest": [
        {
            "id": 369,  // npc id
            "comment": "渔夫",  // 备注：可不写
            "name": "鱼篓", //箱子的名称
            "type": 6, //箱子的类型，可不写，默认是6（垃圾桶）
            "item": [{
                    "id": 2337, //物品id
                    "percent": 50,  //开出的概率
                    "comment": "旧鞋"   //物品名称（备注），可不写
                },
                {
                    "id": 2332,
                    "percent": 1,
                    "prefix": 50,   // 物品词缀
                    "comment": "剑鱼"
                },
                {
                    "id": 2356,
                    "percent": 1,
                    "stack": 2,     // 物品堆叠数量
                    "comment": "宝匣药水"
                }
            ],
            "event": [{
                    "type": 1,  //随机事件类型，1表示获得buff
                    "id": 156,  // buffid
                    "num": 10,  // buff持续时间
                    "comment": "石化",  //备注，可不写
                    "msg": "{player} 在翻垃圾桶的时候受到诅咒，被石化了！", // 提示消息，{player}会被替换成开箱子的玩家名称
                    "serverMsg": true   // 是否全服广播
                },
                {
                    "type": 2,  //随机事件类型，2表示生成npc（城镇npc、敌怪和boss等）
                    "id": 51,   // npcid
                    "num": 3,   // 数量
                    "comment": "3只蝙蝠",
                    "unlock": ["史王"], // 解锁条件
                    "msg": "{player} 在翻垃圾桶的时候，发现了一群蝙蝠！",
                    "serverMsg": true
                },
                {
                    "type": 3,  // 随机事件类型，3表示掉落物品
                    "id": 73,   // 物品id
                    "num": 2,   // 物品数量
                    "comment": "掉落2金币",
                    "msg": "诶嘿，捡到一个红包！"
                }
            ]
        }
    ]
}
```

<br>

[https://docs.qq.com/sheet/DTmxvcXpFSHZOSHNF?tab=8ojz5h](https://docs.qq.com/sheet/DTmxvcXpFSHZOSHNF?tab=8ojz5h)（数据查询：解锁条件）<br>
[https://docs.qq.com/sheet/DTmxvcXpFSHZOSHNF?tab=xpl03x](https://docs.qq.com/sheet/DTmxvcXpFSHZOSHNF?tab=xpl03x)（数据查询：箱子的类型）<br>
[https://docs.qq.com/sheet/DTmxvcXpFSHZOSHNF?tab=x33tc1](https://docs.qq.com/sheet/DTmxvcXpFSHZOSHNF?tab=x33tc1)（数据查询：物品id）<br>

<br>

本插件未新增加任何指令，修改配置文件后，请输入tshock自带的 "/reload" 指令重新载入配置。
