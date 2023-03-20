namespace RaceTo21.Pages
{
    public class Card
    {
        public int Id { get; set; }
        /// <summary>
        /// 简称
        /// </summary>
        public string CardName { get; set; }   
        /// <summary>
        /// 全称
        /// </summary>
        public string CardLongName { get; set; }
        /// <summary>
        /// 花色
        /// </summary>
        public string Suit { get; set; }
        /// <summary>
        /// 点数
        /// </summary>
        public int Score { get; set; }
    }
}
