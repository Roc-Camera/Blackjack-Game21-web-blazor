using System.Collections.Generic;

namespace RaceTo21.Pages
{
    public class Player
    {
        public int Id { get; set; }
        /// <summary>
        /// 玩家姓名
        /// </summary>
        public string Name {set;get;}

        /// <summary>
        /// 筹码
        /// </summary>
        public int Chip {set;get;}

        /// <summary>
        /// 状态active0,stay1,bust2,win3,leave4
        /// </summary>
        public PlayerStatus Status{set;get;} = PlayerStatus.bust;

        /// <summary>
        /// 累计点数
        /// </summary>
        public int Score {set;get;}
        /// <summary>
        /// 每个玩家的牌堆
        /// </summary>
        public List<Card> Cards {set;get;}
        /// <summary>
        /// 展示的手牌
        /// </summary>
        public List<Card> HandCards {set;get;}

        /// <summary>
        /// 是否下注
        /// </summary>
        public bool IsBet{set;get;}
    }
}
