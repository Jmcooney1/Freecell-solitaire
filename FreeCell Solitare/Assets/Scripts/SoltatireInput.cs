
using UnityEngine;
using UnityEngine.InputSystem;

public class SoltatireInput : MonoBehaviour
{
    private Soltatire solitatire;
    private GameObject selectedCard;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        solitatire = FindAnyObjectByType<Soltatire>();
    }

    void OnBurst(InputValue value)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);
        if (hit != null)
        {
            if (hit.gameObject.CompareTag("Card"))
            {
                Debug.Log("clicked: " + hit.name);
                if (selectedCard != null)
                {
                    // check if valid move
                    if (solitatire.IsValidMove(selectedCard, hit.gameObject))
                    {
                        // make the move
                        solitatire.PlaceCard(selectedCard, hit.gameObject);
                        // deselect card
                        selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                        selectedCard = null;
                        return;
                    }
            }
            else if (hit.gameObject.GetComponent<CardHandler>().isFaceUp)
                    {
                        Debug.Log("Card selected: " + hit.name);
                        selectedCard = hit.gameObject;
                        selectedCard.GetComponent<SpriteRenderer>().color = Color.gray;
                    }
            else if(solitatire.IsBlocked(hit.gameObject))
            {
                Debug.Log("card is blocked: " + hit.name);
                return;
            }
                selectedCard = hit.gameObject;
                selectedCard.GetComponent<SpriteRenderer>().color = Color.gray;
            }
            if (hit.gameObject.CompareTag("Foundation"))
            {
                // check if valid move
                Debug.Log("Foundation clicked: " + hit.name);
                if (solitatire.IsValidMove(selectedCard, hit.gameObject))
                {
                    solitatire.PlaceCard(selectedCard, hit.gameObject);
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
            }
            if (hit.gameObject.CompareTag("Tab"))
            {
                Debug.Log("tab clicked: " + hit.name);
                // check if valid move
                if (solitatire.IsValidMove(selectedCard, hit.gameObject))
                {
                    solitatire.PlaceCard(selectedCard, hit.gameObject);
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
            }
            if(hit.gameObject.CompareTag("FreeCell")){
                if(selectedCard != null){
                    if (solitatire.IsValidMove(selectedCard, hit.gameObject))
                {
                    solitatire.PlaceCard(selectedCard, hit.gameObject);
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
                }
            }
        }
    }
}
