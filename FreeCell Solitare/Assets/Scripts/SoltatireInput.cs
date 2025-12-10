
using UnityEngine;
using UnityEngine.InputSystem;

public class SoltatireInput : MonoBehaviour
{
    private Soltatire solitatire;
    private GameObject selectedCard;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        solitatire = GetComponent<Soltatire>();
    }

    void OnBurst(InputValue value)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
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
            }
            if (hit.gameObject.CompareTag("Foundation"))
            {
                Debug.Log("foundation clicked: " + hit.name);
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
            if (hit.gameObject.CompareTag("Tableau"))
            {
                Debug.Log("tab clicked: " + hit.name);
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
        }
        }
    }
