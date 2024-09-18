using System;

namespace BlackjackSim.Players;

public class PlayerRandomized : PlayerBase
{
    public double MinBetValue { get; set; } = 100;
    public double MaxBetValue { get; set; } = 100;

    public double BetInsuranceOdds { get; set; } = 0.5;
    public double HitOdds { get; set; } = 0.5;
    public double DoubleOdds { get; set; } = 0.5;
    public double SplitOdds { get; set; } = 0.5;

    private readonly Random rand = new();
    private double betValue;

    public PlayerRandomized() : base("Randomized Player") { }
    
    public override double PlaceInitialBet()
    {
        betValue = rand.NextDouble() * (MaxBetValue - MinBetValue) + MinBetValue;
        return betValue;
    }
    public override double MakeInsuranceBet() => rand.NextDouble() <= BetInsuranceOdds ? betValue / 2 : 0;

    public override bool ShouldHit(Hand hand) => rand.NextDouble() <= HitOdds;
    public override bool ShouldDouble(Hand hand) => rand.NextDouble() <= DoubleOdds;
    public override bool ShouldSplit(Hand hand) => rand.NextDouble() <= SplitOdds;
}
