using System.Collections.Generic;

namespace BlackjackSim;

public abstract class PlayerBase : IPerson
{
    public double InitialMoney { get; set; } = 10_000;
    public double Money { get; set; }
    public double DeltaMoneyThisRound { get; set; }
    public string Name { get; set; }

    public PlayerBase(string name)
    {
        Money = InitialMoney;
        Name = name;
    }

    public int HandsPlayed => HandsWon + HandsLost + HandsPushed;
    public int HandsWon { get; set; }
    public int HandsLost { get; set; }
    public int HandsPushed { get; set; }
    public int HandsBlackjacked { get; set; }

    public int HandsDoubled { get; set; }
    public int HandsSplit { get; set; }

    public int InsurancePlayed => InsuranceWon + InsuranceLost;
    public int InsuranceWon { get; set; }
    public int InsuranceLost { get; set; }
    public double InsuranceDelta { get; set; }

    public virtual void OnGameBegin() { }
    public virtual void OnShoeReset(int decks) { }

    public abstract double PlaceInitialBet();

    public virtual void YourGivenHands(List<Hand> hands) { }
    public virtual void OnSeenCard(Card card, bool yours) { }
    public virtual void InitialVisibleDealerCard(Card card) { }

    public abstract double MakeInsuranceBet();

    public abstract bool ShouldHit(Hand hand);
    public virtual void OnHit(Hand hand) { }

    public abstract bool ShouldDouble(Hand hand);
    public virtual void OnDouble(Hand hand) { }

    public abstract bool ShouldSplit(Hand hand);
    public virtual void OnSplit(Hand hand) { }

    public virtual void OnWonHand(Hand hand) { }
    public virtual void OnLostHand(Hand hand) { }
    public virtual void OnHandIsBlackjack(Hand hand) { }
    public virtual void OnHandPush(Hand hand) { }
}
