namespace BlackjackSim.Players;

public class PlayerStandard : PlayerBase
{
    public double BetSize { get; set; }

    public PlayerStandard(double betSize) : base($"Standard Bet {betSize}")
    {
        BetSize = betSize;
    }

    public override double PlaceInitialBet() => BetSize;
    public override double MakeInsuranceBet() => 0;
    public override bool ShouldHit(Hand hand) => hand.GetValue() <= 16;
    public override bool ShouldDouble(Hand hand) => false;
    public override bool ShouldSplit(Hand hand) => true;
}
