using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using AntDesign;

namespace RaceTo21.Pages
{
    public partial class Index
    {
        /// <summary>
        /// 游戏轮数
        /// </summary>
        private int round = 1;
        /// <summary>
        /// 每次下注量
        /// </summary>
        private int currentBet = 10;
        /// <summary>
        /// 本场总筹码
        /// </summary>
        private int chipSum = 0;
        /// <summary>
        /// 控制玩家输入名字的显示
        /// </summary>
        private bool _visible = false;
        /// <summary>
        /// 下注按钮是否禁用
        /// </summary>
        private bool isBet;
        /// <summary>
        /// 开始发牌按钮是否显示
        /// </summary>
        private int isDeal = 0;
        /// <summary>
        /// 控制游戏界面的显示
        /// </summary>
        private string _startGameVisible = "";
        /// <summary>
        /// 玩家的数量
        /// </summary>
        private int playerNum = 0;
        /// <summary>
        /// 询问发牌显示
        /// </summary>
        private bool _dealVisible;
        private bool ruleModel;
        private string tips;
        private Player player = new Player();
        List<Player> Players = new List<Player>();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _visible = true;
        }

        private void HandleOk(MouseEventArgs e)
        {
            _visible = false;
            for (int i = 0; i < playerNum; i++)
            {
                var num = i + 1;
                Players.Add(new Player { Id = num, Name = "", Chip = 100 });
            }
        }
        private void StartGame()
        {
            _startGameVisible = "display:none;";

        }
        private void GameOver()
        {
            _visible = false;
        }
        /// <summary>
        /// 询问发牌阶段
        /// </summary>
        private void InquireDeal()
        {
            isBet = true;
            _dealVisible = true;
        }

        private void DealOk()
        {
            _dealVisible = false;
            if (isDeal == 1)
            {
                Console.WriteLine(JsonSerializer.Serialize(Players));
                if (Players.All(x => x.Status != PlayerStatus.active))
                {
                    //有手牌情况下 都不摸牌 比分数
                    var winner = Players.OrderByDescending(x => x.Score).FirstOrDefault();
                    winner.Chip += chipSum;
                    winner.Status = PlayerStatus.win;
                    tips = $"玩家{winner.Name}获胜";
                    if (round == 3)
                    {
                        isDeal = 3;
                    }
                    else
                    {
                        isDeal = 2;
                    }
                    StateHasChanged();
                    return;
                }
                foreach (var item in Players)
                {
                    if (item.Status == PlayerStatus.active && item.IsBet == true)
                    {
                        int r = new Random().Next(item.Cards.Count);
                        item.HandCards.Add(item.Cards[r]);
                        item.Cards.RemoveAt(r);
                        item.Score = item.Score + item.HandCards.Last().Score;
                        //如果只剩最一个人active或stay,把最后这个人状态置为win
                        if (Players.Where(x => x.IsBet == true).Count(x => x.Status == PlayerStatus.active || x.Status == PlayerStatus.stay) == 1)
                        {
                            int winnerIndex = Players.Where(x => x.IsBet == true).ToList().FindIndex(x => x.Status == PlayerStatus.active);
                            Players.Where(x => x.IsBet == true).ToList()[winnerIndex].Status = PlayerStatus.win;
                            tips = $"玩家{Players.Where(x => x.IsBet == true).ToList()[winnerIndex].Name}获胜";
                            //获胜人获取所有筹码
                            Players.Where(x => x.IsBet == true).ToList()[winnerIndex].Chip += chipSum;
                            //继续发牌按钮消失 界面显示提示语
                            if (round == 3)
                            {
                                isDeal = 3;
                            }
                            else
                            {
                                isDeal = 2;
                            }
                            StateHasChanged();
                            //本轮游戏结束开始
                            return;
                        }
                        else
                        {
                            //如果有人刚好21
                            if (item.Score == 21)
                            {
                                item.Status = PlayerStatus.win;
                                tips = $"玩家{item.Name}获胜";
                                item.Chip += chipSum;
                                if (round == 3)
                                {
                                    isDeal = 3;
                                }
                                else
                                {
                                    isDeal = 2;
                                }
                                StateHasChanged();
                                //本轮游戏结束开始
                                return;
                            }
                            else if (item.Score > 21)
                            {
                                item.Status = PlayerStatus.bust;
                            }
                        }
                    }
                }
            }
            else
            {
                //开局如果都不要牌 这轮游戏结束 返还下注人的赌注
                if (Players.Where(x => x.IsBet == true).All(x => x.Status == PlayerStatus.stay))
                {
                    isDeal = 2;
                    Players.Where(x => x.IsBet == true).ForEach(y => y.Chip += currentBet);
                }
                else
                {
                    //判断如果只有一个人要牌 这个人直接获得本轮胜利
                    if (Players.Where(x => x.Status == PlayerStatus.active).Count() == 1)
                    {
                        var thisWinner = Players.Find(x => x.Status == PlayerStatus.active);
                        tips = $"只有玩家{thisWinner.Name}要牌，该玩家本轮获胜";
                        thisWinner.Chip += chipSum;
                        if (round == 3)
                        {
                            isDeal = 3;
                        }
                        else
                        {
                            isDeal = 2;
                        }
                        StateHasChanged();
                        return;
                    }
                    else
                    {
                        StartDeal();
                    }
                }
            }
        }

        private void DealCancel()
        {
            _dealVisible = false;
        }
        /// <summary>
        /// 发牌阶段
        /// </summary>
        private void StartDeal()
        {
            List<string> suits = new List<string> { "Spades", "Hearts", "Clubs", "Diamonds" };
            foreach (var item in Players)
            {

                List<Card> cards = new List<Card>();
                Card card = null;

                for (int cardVal = 1; cardVal <= 13; cardVal++)
                {
                    var index = cardVal;
                    foreach (var cardSuit in suits)
                    {
                        string cardName;
                        string cardLongName;
                        switch (index)
                        {
                            case 1:
                                cardName = "A";
                                cardLongName = "Ace";
                                break;
                            case 11:
                                cardName = "J";
                                cardLongName = "Jack";
                                break;
                            case 12:
                                cardName = "Q";
                                cardLongName = "Queen";
                                break;
                            case 13:
                                cardName = "K";
                                cardLongName = "King";
                                break;
                            default:
                                cardName = index.ToString();
                                cardLongName = cardName;
                                break;
                        }
                        card = new Card();
                        card.CardName = cardName;
                        card.CardLongName = cardLongName;
                        card.Suit = cardSuit;
                        if (card.CardName == "J" || card.CardName == "Q" || card.CardName == "K")
                        {
                            card.Score = 10;
                        }
                        else
                        {
                            card.Score = index;
                        }
                        card.Id = index;
                        cards.Add(card);
                    }

                }
                item.Cards = cards.Distinct().ToList();
                //Console.WriteLine(JsonSerializer.Serialize(item.Cards));

                //从牌堆里随机选出一个给手牌
                int r = new Random().Next(item.Cards.Count);
                item.HandCards = new List<Card>();
                //不下赌注不给手牌
                if (item.Status == PlayerStatus.active && item.IsBet == true)
                {
                    item.HandCards.Add(item.Cards[r]);
                    //Console.WriteLine("-----HandCards");
                    //Console.WriteLine(JsonSerializer.Serialize(item.HandCards));
                    item.Cards.RemoveAt(r);
                    //Console.WriteLine("-----Cards");
                    //Console.WriteLine(JsonSerializer.Serialize(item.Cards));
                    item.Score = item.Score + item.HandCards.Last().Score;
                }

            }
            isDeal = 1;
            StateHasChanged();
        }

        /// <summary>
        /// 继续发牌
        /// </summary>
        private void NextDeal()
        {
            InquireDeal();
            StateHasChanged();
        }

        /// <summary>
        /// 开始下一轮
        /// </summary>
        private void NextRoundClick()
        {
            round++;
            currentBet += 10;
            chipSum = 0;
            tips = "";
            Players.ForEach(x => x.Status = PlayerStatus.active);
            Players.ForEach(x => x.Score = 0);
            Players.ForEach(x => x.Cards = new());
            Players.ForEach(x => x.HandCards = new());
            Players.ForEach(x => x.IsBet = false);
            //下注
            isBet = false;
            //询问 
            isDeal = 0;
        }
        private void GetResult()
        {
            var finalWinner = Players.OrderByDescending(x => x.Chip).FirstOrDefault().Name;
            tips = $"本次游戏最终获胜的玩家是{finalWinner}";
            isDeal = 4;
        }
        private void NewGame()
        {
            round = 0;
            currentBet = 10;
            chipSum = 0;
            tips = "";
            Players = new();
            _visible = true;
            _startGameVisible = "";
            isBet = false;
            isDeal = 0;
        }
    }
}
