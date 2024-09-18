using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackjackSim;

public class Game
{
    public DealerBase Dealer { get; private init; }
    public PlayerBase[] Players { get; private init; }

    public int ShoeSize { get; init; } = 6;

    private Shoe? shoe;

    public Game(DealerBase dealer, params PlayerBase[] players)
    {
        Dealer = dealer;
        Players = players;
    }

    public void PlayRound(bool debug = false)
    {
        if (Players.Length == 0)
        {
            if (debug) Console.WriteLine("!!  No players! Ignoring this round.");
            return;
        }
        if (debug)
        {
            Console.WriteLine("    Starting a game of blackjack.");
            Console.WriteLine($"      Dealer is {Dealer.GetType().Name}");

            Dictionary<Type, int> playerTypes = [];
            foreach (PlayerBase p in Players)
            {
                Type pType = p.GetType();
                if (playerTypes.TryGetValue(pType, out int count)) playerTypes[pType] = count + 1;
                else playerTypes.Add(pType, 1);
            }

            if (playerTypes.Count == 1)
            {
                Console.WriteLine($"      Players are {Players.Length} {Players[0].GetType().Name}");
            }
            else
            {
                Console.WriteLine("      Players:");
                foreach (KeyValuePair<Type, int> type in playerTypes)
                {
                    Console.WriteLine($"      {type.Value} {type.Key.Name}");
                }
            }
        }

        Dealer.OnGameBegin(this);
        foreach (PlayerBase p in Players)
        {
            p.OnGameBegin();
            p.DeltaMoneyThisRound = 0;
        }
        if (Dealer.ShouldResetShoe())
        {
            if (debug) Console.WriteLine("!   Dealer has requested a shoe reset!");
            shoe = resetShoe();
        }
        else shoe ??= resetShoe();

        // STEP 0: Bets are collected.
        Hand dealerHand = new(Dealer);
        Dictionary<PlayerBase, List<Hand>> playerHands = [];
        List<Hand> totalHands = [];
        foreach (PlayerBase p in Players)
        {
            Hand hand = new(p)
            {
                bet = Math.Min(p.PlaceInitialBet(), p.Money)
            };
            totalHands.Add(hand);
            if (playerHands.TryGetValue(p, out List<Hand>? hands)) hands.Add(hand);
            else
            {
                List<Hand> newHands = [hand];
                playerHands.Add(p, newHands);
                p.YourGivenHands(newHands);
            }
        }

        // STEP 1: Deal out the cards.
        bool dealerVisibleAce = false;
        for (int i = 0; i < 2; i++)
        {
            // Dealer card first.
            Card dealerCard = tryGetFromShoe();
            if (i == 0)
            {
                // Show the first dealer card to the other players.
                if (debug) Console.WriteLine($"    Dealer drew {dealerCard}");
                foreach (PlayerBase p in Players)
                {
                    p.OnSeenCard(dealerCard, false);
                    p.InitialVisibleDealerCard(dealerCard);
                }
                if (dealerCard == ValueKind.Ace) dealerVisibleAce = true; // For the insurance step.
            }
            else
            {
                // Don't show any future dealer cards to the players.
                if (debug) Console.WriteLine($"    (Hidden) Dealer drew {dealerCard}");
            }
            dealerHand.cards.Add(dealerCard);

            // Deal one card to each hand.
            foreach (Hand h in totalHands)
            {
                Card playerCard = tryGetFromShoe();
                h.cards.Add(playerCard);

                // Notify players.
                foreach (PlayerBase p in Players)
                {
                    p.OnSeenCard(playerCard, h.player == p);
                }
            }
        }

        // STEP 1b: Check for insurance.
        if (dealerVisibleAce)
        {
            if (debug) Console.WriteLine($"!   Dealer has a visible ace! Insurance will commence.");
            bool isBlackjack = dealerHand.IsBlackjack();

            foreach (PlayerBase p in Players)
            {
                double insuranceBet = Math.Min(p.MakeInsuranceBet(), p.Money);
                if (isBlackjack)
                {
                    p.DeltaMoneyThisRound += insuranceBet * 2;
                    p.InsuranceWon++;
                    p.InsuranceDelta = insuranceBet * 2;
                }
                else
                {
                    p.DeltaMoneyThisRound -= insuranceBet;
                    p.InsuranceLost++;
                    p.InsuranceDelta = -insuranceBet;
                }
            }

            if (isBlackjack) goto _handCompare; // You can't beat a dealer blackjack.
        }

        // STEP 2: Play blackjack.
        foreach (KeyValuePair<PlayerBase, List<Hand>> ph in playerHands)
        {
            PlayerBase player = ph.Key;
            List<Hand> hands = ph.Value;
            for (int i = 0; i < hands.Count; i++)
            {
                Hand h = hands[i];

                bool hasDoubled = false;
            _retry:
                if (h.cards.Count == 2)
                {
                    if ((h.cards[0].GetValue(false) == h.cards[1].GetValue(false) ||
                        h.GetValue() == 16) && player.ShouldSplit(h))
                    {
                        // Split the hand into two. You can split multiple times.
                        Hand other = new(player)
                        {
                            bet = h.bet,
                            cards = [h.cards[1]]
                        };
                        h.cards.RemoveAt(1);
                        hands.Add(other);
                        player.HandsSplit++;

                        goto _retry;
                    }

                    if (player.ShouldDouble(h) && !hasDoubled)
                    {
                        // Double bet, get one card, and call it done.
                        h.bet *= 2;
                        Card newCard = tryGetFromShoe();

                        // Notify players of new card.
                        foreach (PlayerBase p in Players) p.OnSeenCard(newCard, player == p);
                        h.cards.Add(newCard);
                        player.OnDouble(h);
                        hasDoubled = true;
                        player.HandsDoubled++;
                    }
                }

                while (h.GetValue() < 21 && player.ShouldHit(h))
                {
                    // Add new card to hand.
                    Card newCard = tryGetFromShoe();

                    // Notify players of new card.
                    foreach (PlayerBase p in Players) p.OnSeenCard(newCard, player == p);
                    h.cards.Add(newCard);
                    player.OnHit(h);
                }
            }
        }

        // STEP 3a: Dealer reveals hidden card.
        if (debug) Console.WriteLine($"    Dealer reveals hand: {dealerHand.GetValue()}");
        for (int i = 1; i < dealerHand.cards.Count; i++)
        {
            // Notify players.
            foreach (PlayerBase p in Players) p.OnSeenCard(dealerHand.cards[i], false);
        }

        // STEP 3b: Dealer draws until its limit (hit on value).
        while (dealerHand.GetValue() <= Dealer.DrawTo)
        {
            // Add new card to hand.
            Card newCard = tryGetFromShoe();
            dealerHand.cards.Add(newCard);
            if (debug) Console.WriteLine($"    Dealer drawing new card: {newCard} (total {dealerHand.GetValue()})");

            // Notify players of new card.
            foreach (PlayerBase p in Players) p.OnSeenCard(newCard, false);
        }

    // STEP 4: Compare hands.
    _handCompare:
        int dealerValue = dealerHand.GetValue();
        int handIndex = 0;
        foreach (Hand h in totalHands)
        {
            int yourValue = h.GetValue();
            HandStatus status;
            if (dealerHand.IsBlackjack())
            {
                if (h.IsBlackjack()) status = HandStatus.Push;
                else status = HandStatus.Lose;
            }
            else if (yourValue > 21) status = HandStatus.Lose; // You've bust.
            else if (dealerValue > 21) status = HandStatus.Won; // Dealer bust.
            else
            {
                if (h.IsBlackjack()) status = HandStatus.WonBlackjack;
                else if (dealerValue > yourValue) status = HandStatus.Lose;
                else if (dealerValue < yourValue) status = HandStatus.Won;
                else status = HandStatus.Push;
            }

            if (debug) Console.WriteLine($"    Hand {handIndex} has {status} ({yourValue}).");

            h.status = status;
            PlayerBase player = (PlayerBase)h.player;
            switch (status)
            {
                case HandStatus.Won:
                    player.DeltaMoneyThisRound += h.bet * Dealer.WinPayment;
                    player.HandsWon++;
                    Dealer.HouseLossesRegular++;
                    break;

                case HandStatus.WonBlackjack:
                    player.DeltaMoneyThisRound += h.bet * Dealer.BlackjackPayment;
                    player.HandsWon++;
                    player.HandsBlackjacked++;
                    Dealer.HouseLossesBlackjack++;
                    break;

                case HandStatus.Lose:
                    player.DeltaMoneyThisRound -= h.bet;
                    player.HandsLost++;
                    Dealer.HouseWins++;
                    break;

                case HandStatus.Push: // Push means money back.
                    player.HandsPushed++;
                    break;

                default: break;
            }

            handIndex++;
        }

        if (totalHands.Count(x => x.status == HandStatus.Won || x.status == HandStatus.WonBlackjack) >= 2) Console.ReadKey();

        // STEP 5: Apply money deltas.
        foreach (PlayerBase p in Players) p.Money += p.DeltaMoneyThisRound;
        // Now we're done.

        Card tryGetFromShoe()
        {
            if (shoe!.CardsRemaining > 0) return shoe.Get();
            else
            {
                if (debug) Console.WriteLine("!!  Shoe has unexpectedly run out of cards!");
                shoe = resetShoe();
                return shoe.Get();
            }
        }
        Shoe resetShoe()
        {
            if (debug) Console.WriteLine($"    Resetting shoe ({ShoeSize} decks).");
            Shoe shoe = new(ShoeSize);
            foreach (PlayerBase p in Players) p.OnShoeReset(ShoeSize);
            return shoe;
        }
    }
}
