namespace WebPoker.Models;

public class PocketMaker
{
    public List<Card>? Pocket { get; set; }
    public void MakePocket()
    {
        foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits)))
        {
            foreach (CardRanks rank in Enum.GetValues(typeof(CardRanks)))
            {
                Card card = new Card
                {
                    CardSuit = suit,
                    CardRank = rank,
                    CardPower = (int)rank + 2
                };
                Pocket?.Add(card);
            }
        }
    }
}
