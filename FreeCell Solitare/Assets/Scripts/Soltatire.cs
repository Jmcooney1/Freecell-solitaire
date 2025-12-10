using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using Unity.VisualScripting;

public class Soltatire : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Sprite[] cardFaces;

    bool isFaceUp = false;
    public GameObject cardPrefab;
    public Sprite emptyPosition;
    public string[] suits = { "H", "D", "C", "S"};
    public string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public GameObject[] foundationPositions;
    public GameObject[] tableauPositions;

    public GameObject[] freecellPosition;

    List<string> deck;


    List<string>[] foundations;

    List<string>[] tableaus;

    List<string>[] freecells;

    public List<string> foundation0 = new List<string>();

    public List<string> foundation1 = new List<string>();

    public List<string> foundation2 = new List<string>();

    public List<string> foundation3 = new List<string>();

    public List<string> tableu0 = new List<string>();
    public List<string> tableu1 = new List<string>();
    public List<string> tableu2 = new List<string>();
    public List<string> tableu3 = new List<string>();
    public List<string> tableu4 = new List<string>();

    public List<string> tableu5 = new List<string>();
    public List<string> tableu6 = new List<string>();

    public List<string> tableu7 = new List<string>();

    public List<string> freeCell1 = new List<string>();

    public List<string> freeCell2 = new List<string>();
    public List<string> freeCell3 = new List<string>();
    public List<string> freeCell4 = new List<string>();




    private System.Random random = new System.Random();
    public Vector3 cardOffset = new Vector3(0, -0.3f, -.1f);
    public float zoffset = -0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Starting Solitaire");
        tableaus = new List<string>[] { tableu0, tableu1, tableu2, tableu3, tableu4, tableu5, tableu6, tableu7};
        foundations = new List<string>[] { foundation0, foundation1, foundation2, foundation3 };
        freecells = new List<string>[]{freeCell1, freeCell2, freeCell3, freeCell4};
        PlayGame();
    }

    void PlayGame()
    {
        Debug.Log("Playing Solitaire");
        deck = GenerateDeck();
        foreach (string card in deck)
        {
            Debug.Log(card);
        }
        Deal();
    }

    List<string> GenerateDeck()
    {
        Debug.Log("Generating Deck");
        List<string> newDeck = new List<string>();
        foreach(string suit in suits){
            foreach (string rank in ranks){
                newDeck.Add(suit + rank);
                newDeck = newDeck.OrderBy(x => random.Next()).ToList();
            }
        }

        return newDeck;
    }

    void Deal(){
        Debug.Log("Dealing cards...");
        int tabIndex = 0;

        for (int i = deck.Count - 1; i >= 0; i--)
        {
            if (tabIndex > 7)
            {
                break;
            }
            string card = deck[i];
            deck.RemoveAt(i);
            List<string> currentTab = tableaus[tabIndex];
            tableaus[tabIndex].Add(card);

            if(tabIndex < 4){ if(currentTab.Count >= 7)tabIndex++;}
            else if(currentTab.Count >= 6)tabIndex++;
        }

        foreach (GameObject tabPosition in tableauPositions)
        {
            Debug.Log("Dealing to tableau position " + tabPosition.name);
            int index = Array.IndexOf(tableauPositions, tabPosition);
            Vector3 currentPosition = tabPosition.transform.position + new Vector3(0, 0, -.1f);
            foreach (string card in tableaus[index])
            {
                Debug.Log("Dealing card " + card + " to tableau " + index);
                // create card
                CreateCard(card, currentPosition, tabPosition.transform, isFaceUp);
                currentPosition += cardOffset;
            }
        }
    }

    void CreateCard(string cardName, Vector3 position, Transform parent, bool isFaceUp)
    {
        Debug.Log("Creating card: " + cardName);
        GameObject newCard = Instantiate(cardPrefab, position, Quaternion.identity, parent);
        newCard.name = cardName;
        Debug.Log("Card sprite name: " + newCard.name);
        newCard.GetComponent<CardHandler>().cardFront = cardFaces.First(s => s.name.Remove(s.name.Length - 2) == newCard.name);
        newCard.GetComponent<CardHandler>().isFaceUp = true;
    }
    

    public bool IsValidMove(GameObject fromLocation, GameObject toLocation)
    {
        if (fromLocation == null || toLocation == null || fromLocation == toLocation) return false;
        ResolveTarget(toLocation, out GameObject clickedTag, out int foundationIndex, out int tabIndex);
        // waste -> tab/foundation
        if (fromLocation.transform.parent.CompareTag("FreeCell"))
        {
            if (clickedTag.transform.CompareTag("Tab") && tabIndex >= 0)
            {
                Debug.Log("moving from Freecell to tab: " + CanPlaceOnTableau(fromLocation.name, tabIndex));
                return CanPlaceOnTableau(fromLocation.name, tabIndex);
            }
            if (clickedTag.transform.CompareTag("Foundation") && foundationIndex >= 0)
            {
                Debug.Log("moving from FreeCell to foundation: " + CanPlaceOnFoundation(fromLocation.name, foundationIndex));
                return CanPlaceOnFoundation(fromLocation.name, foundationIndex);
            }
        }

        // tab -> tab/foundation
        if (fromLocation.transform.parent.CompareTag("Tab"))
        {
            if (clickedTag.transform.CompareTag("Tab") && tabIndex >= 0)
            {
                Debug.Log("moving from tab to tab: " + CanPlaceOnTableau(fromLocation.name, tabIndex));
                return CanPlaceOnTableau(fromLocation.name, tabIndex);
            }
            if (clickedTag.transform.CompareTag("Foundation") && foundationIndex >= 0)
            {
                if (IsBlocked(fromLocation))
                {
                    Debug.Log("blocked");
                    return false;
                }
                Debug.Log("moving from tab to foundation: " + CanPlaceOnFoundation(fromLocation.name, foundationIndex));
                return CanPlaceOnFoundation(fromLocation.name, foundationIndex);
            }
        }
        // foundation -> tab
        if (fromLocation.transform.parent.CompareTag("Foundation"))
        {
            if (clickedTag.transform.CompareTag("Tab") && tabIndex >= 0)
            {
                Debug.Log("moving from foundation to tab: " + CanPlaceOnTableau(fromLocation.name, tabIndex));
                return CanPlaceOnTableau(fromLocation.name, tabIndex);
            }
        }
        return false;
    }

    public void PlaceCard(GameObject fromLocation, GameObject toLocation)
    {
        if (fromLocation == null || toLocation == null) return;
        ResolveTarget(toLocation, out GameObject clickedTag, out int foundationIndex, out int tabIndex);
        int originalTabIndex = -1;
        int cardsToMoveCount = 1;
        GameObject originalParent = fromLocation.transform.parent.gameObject;
        // if coming from tab, need to remove card and all cards on top of it from their original tab
        if (fromLocation.transform.parent.CompareTag("Tab"))
        {
            foreach(List<string> tableau in tableaus)
            {
                if (tableau.Contains(fromLocation.name))
                {
                    originalTabIndex = System.Array.IndexOf(tableaus, tableau);
                    cardsToMoveCount = tableau.Count - tableau.IndexOf(fromLocation.name);
                    tableau.Remove(fromLocation.name);
                    break;
                }
            }
        }


        // if coming from foundation, remove card from correct foundation
        if (fromLocation.transform.parent.CompareTag("Foundation"))
        {
            foreach(List<string> foundation in foundations)
            {
                if (foundation.Contains(fromLocation.name))
                {
                    foundation.Remove(fromLocation.name);
                    break;
                }
            }
        }
        

        // if moving to tab, add the card to the correct tab
        if (clickedTag.transform.CompareTag("Tab"))
        {
            // add it to the right tab
            int tableauIndex = System.Array.IndexOf(tableauPositions, clickedTag.transform.gameObject);
            tableaus[tableauIndex].Add(fromLocation.name);
            // move the card position
            if (tableaus[tableauIndex].Count == 1)
            {
                fromLocation.transform.position = toLocation.transform.position + new Vector3(0f, 0f, -.1f);
            }
            else
            {
                fromLocation.transform.position = toLocation.transform.position + cardOffset;
            }
            MoveStackedCards(originalParent, originalTabIndex, tabIndex, cardsToMoveCount, clickedTag, fromLocation);
        }
        // move all other cards on top of the original cardObject (probably put this in a helper function)

        // if moving to foundation, add card to correct foundation
        if (clickedTag.transform.CompareTag("Foundation"))
        {
            int fIndex = System.Array.IndexOf(foundationPositions, clickedTag.transform.gameObject);
            foundations[fIndex].Add(fromLocation.name);
            fromLocation.transform.position = toLocation.transform.position + new Vector3(0f, 0f, -.1f);
        }

        // update parent
        fromLocation.transform.parent = clickedTag.transform;
    }

    public void MoveStackedCards(GameObject originalParent, int originalTabIndex, int destTabIndex, int cardsToMoveCount, GameObject clickedTag, GameObject cardObject)
    {
        if (originalTabIndex == -1 || cardsToMoveCount <= 1) return;
        List<string> originalTab = tableaus[originalTabIndex];
        int origCount = originalTab.Count;
        int origIndex = origCount - cardsToMoveCount + 1;
        for (int i=0; i < cardsToMoveCount -1; i++)
        {
            string moveCard = originalTab[originalTabIndex];
            originalTab.RemoveAt(origIndex);
            tableaus[destTabIndex].Add(moveCard);
            GameObject moveCardObject = null;
            foreach (Transform child in originalParent.transform)
            {
                if (child.gameObject.name == moveCard)
                {
                    moveCardObject = child.gameObject;
                    break;
                }
            }
            if (moveCardObject != null)
            {
                moveCardObject.transform.parent = clickedTag.transform;
                moveCardObject.transform.position = cardObject.transform.position + (cardOffset * (i + 1));
            }
        }
    }

    public bool IsLastInTab(GameObject card)
    {
        foreach (List<string> tab in tableaus)
        {
            if (tab.Count > 0 && tab.Last() == card.name)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsBlocked(GameObject card)
    {
        foreach (Transform child in card.transform.parent)
        {
            if (child.gameObject != card && child.position.z < card.transform.position.z)
                return true;
        }
        return false;
    }
    public bool IsAlternatingColor(string card1, string card2)
    {
        char suit1 = card1[0];
        char suit2 = card2[0];
        bool isRed1 = (suit1 == 'D' || suit1 == 'H');
        bool isRed2 = (suit2 == 'D' || suit2 == 'H');
        return isRed1 != isRed2;
    }

    public bool IsSameSuit(string card1, string card2)
    {
        return card1[0] == card2[0];
    }
    public bool IsOneRankLower(string card1, string card2)
    {
        string rank1 = card1.Substring(1);
        string rank2 = card2.Substring(1);
        int index1 = System.Array.IndexOf(ranks, rank1);
        int index2 = System.Array.IndexOf(ranks, rank2);
        Debug.Log("index1: " + index1);
        Debug.Log("index2: " + index2);
        return index1 + 1 == index2;
    }

    public bool IsOneRankHigher(string card1, string card2)
    {
        string rank1 = card1.Substring(1);
        string rank2 = card2.Substring(1);
        int index1 = System.Array.IndexOf(ranks, rank1);
        int index2 = System.Array.IndexOf(ranks, rank2);
        return index1 == index2 + 1;
    }
    public bool CanPlaceOnFoundation(string card, int foundationIndex)
    {
        List<string> foundation = foundations[foundationIndex];
        if (foundation.Count == 0)
        {
            return card.EndsWith("A");
        }
        string top = foundation.Last();
        return IsOneRankHigher(card, top) && IsSameSuit(card, top);
    }

    public bool CanPlaceOnTableau(string card, int tabIndex)
    {
        List<string> tab = tableaus[tabIndex];
        if (tab.Count == 0)
        {
            Debug.Log("tab count is 0");
            return card.EndsWith("K");
        }
        string top = tab.Last();
        Debug.Log("is one lower: " + IsOneRankLower(card, top));
        Debug.Log("is alternating: " + IsAlternatingColor(card, top));
        return IsOneRankLower(card, top) && IsAlternatingColor(card, top);
    }

    void ResolveTarget(GameObject toLocation, out GameObject clickedTag, out int foundationIndex, out int tabIndex)
    {
        clickedTag = toLocation.transform.CompareTag("Card") ? toLocation.transform.parent.gameObject : toLocation;       
        foundationIndex = -1;
        tabIndex = -1;
        if (clickedTag.transform.CompareTag("Foundation"))
        {
            foundationIndex = System.Array.IndexOf(foundationPositions, clickedTag);
        }
        if (clickedTag.transform.CompareTag("Tab"))
        {
            tabIndex = System.Array.IndexOf(tableauPositions, clickedTag);
        }
    }

}
