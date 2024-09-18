using System;
using System.Collections.Generic;

namespace BlackjackSim;

public class Shoe
{
    public int TotalDecks => decks;
    public int TotalCards => 52 * decks;
    public int CardsTaken => 52 * decks - cards.Count;
    public int CardsRemaining => cards.Count;

    private readonly List<Card> cards;
    private readonly int decks;
    private readonly Random rand;

    public Shoe(int decks)
    {
        this.decks = decks;
        cards = [];
        rand = new();

        for (int i = 0; i < decks; i++) cards.AddRange(Card.GetDeck());

        // SHUFFLE: pick a random index out of the cards that haven't been
        //          shuffled and add it to the bottom of the list.
        int notShuffled = cards.Count;
        while (notShuffled > 0)
        {
            int index = rand.Next(notShuffled);
            Card c = cards[index];
            cards.RemoveAt(index);
            cards.Add(c);
            notShuffled--;
        }
    }

    public Card Get()
    {
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }
}
