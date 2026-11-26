namespace WebPoker.Models;

public class Combinations
{
    public bool HasRoyalFlush(Player pl, List<Card> tableCards)
    {
        var playerCardsConcatToTableCards =
             pl.PlayerCards.Concat(tableCards)
             .Where(card => card.CardRank >= CardRanks.Ten)
             .OrderByDescending(c => c.CardRank)
             .ToList();
        int RoyalFlushCount = 5;
        return playerCardsConcatToTableCards.Count() == RoyalFlushCount && HasStraightFlush(pl, tableCards);
    }
    public bool HasStraightFlush(Player pl, List<Card> tableCards)
    => HasStraight(pl, tableCards) && HasFlush(pl, tableCards);
    public bool HasFlush(Player pl, List<Card> tableCards)
    {
        int flushReuqiredCardsCount = 5;
        int requiredFlushCardGroupCount = 1;
        var playerCardsConcatToTableCards =
            pl.PlayerCards.Concat(tableCards)
            .GroupBy(card => card.CardSuit)
            .ToList();

        return
        playerCardsConcatToTableCards
        .Count(c => c.Count() >= flushReuqiredCardsCount)
        == requiredFlushCardGroupCount;

    }
    public bool HasFullHouse(Player pl, List<Card> tableCards)// 
    {

        int requiredSetCardsCount = 3;
        int setGroupCount = 1;
        int pairGroupCount = 1;
        int requiredPairCardsCount = 2;
        var group =
            pl.PlayerCards.Concat(tableCards)
            .GroupBy(c => c.CardRank)
            .ToList();

        bool hasSet =
             group.Count(g => g.Count() == requiredSetCardsCount) == setGroupCount &&
            !group.Any(g => g.Count() > requiredSetCardsCount);

        bool hasPair =
             group.Count(g => g.Count() == requiredPairCardsCount) == pairGroupCount;

        return (hasSet && hasPair);
    }
    public bool HasOnePair(Player pl, List<Card> tableCards)
    {
        int cardsPerPair = 2;
        int requiredPairCount = 1;

        var cardsGroupedByRank =
            pl.PlayerCards
            .Concat(tableCards)
            .GroupBy(c => c.CardRank)
            .ToList();

        return cardsGroupedByRank.Count(g => g.Count() == cardsPerPair) == requiredPairCount;
    }
    public bool HasStraight(Player pl, List<Card> tableCards)
    {
        tableCards =
        tableCards
        .Concat(pl.PlayerCards)
        .OrderByDescending(c => c.CardRank)
        .DistinctBy(c => c.CardRank)
        .ToList();

        int tableCardsCount = tableCards.Count;
        int StraightSuccesCounter = 5;
        bool isStraight = false;
        int iterationCounter = 0;

        while (isStraight == false)
        {
            for (int i = 0; i < tableCardsCount - 1; i++)
            {
                if (tableCards[i].CardPower - tableCards[i + 1].CardPower == 1)
                    iterationCounter++;

                if (iterationCounter == StraightSuccesCounter)
                {
                    isStraight = true;
                    break;
                }
                else if (tableCards.Count - i == 1)
                    break;
            }
            if (isStraight == false)
                break;
        }
        return isStraight;
    }
    public bool HasTwoPairs(Player pl, List<Card> tableCards)
    {
        int requiredTwoPairCardsCount = 2;
        int twoPairGroupCount = requiredTwoPairCardsCount;
        int setCardsCount = 3;
        bool pocketPair =
            pl.PlayerCards[0].CardRank == pl.PlayerCards[1].CardRank;

        var groups = pl.PlayerCards
       .Concat(tableCards)
       .GroupBy(c => c.CardRank)
       .ToList();

        bool hasTwoPairs =
        pocketPair &&
        groups.Count(g => g.Count() == requiredTwoPairCardsCount) >= twoPairGroupCount &&
        !groups.Any(g => g.Count() >= setCardsCount);

        return hasTwoPairs;
    }

    public string HighCard(Player pl)
        => pl.PlayerCards.Max().ToString();

    public bool HasSet(Player pl, List<Card> tableCards)
    {
        int requiredSetCardCount = 3;
        int setGroupCount = 1;

        var group =
            pl.PlayerCards
            .Concat(tableCards)
            .GroupBy(c => c.CardRank)
            .ToList();

        bool hasSet =
            group.Count(g => g.Count() == requiredSetCardCount) == setGroupCount &&
            !group.Any(g => g.Count() > requiredSetCardCount);

        return hasSet;

    }
    public bool HasKaRe(Player pl, List<Card> tableCards)
    {
        int requiredKaReCardCount = 4;
        int KaReGroupCount = 1;

        var group = pl.PlayerCards
            .Concat(tableCards)
            .GroupBy(c => c.CardRank)
            .ToList();

        bool hasKare =
             group.Count(g => g.Count() == requiredKaReCardCount) == KaReGroupCount;

        return hasKare;
    }
}