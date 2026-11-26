namespace WebPoker.Models;

public class Card
{
    public int Id { get; set; }
    public int CardPower { get; set; }
    public CardRanks CardRank { get; set; }
    public CardSuits CardSuit { get; set; }
}
 