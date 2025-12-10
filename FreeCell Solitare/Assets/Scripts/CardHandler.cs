using UnityEngine;

public class CardHandler : MonoBehaviour
{
    public Sprite cardFront;
    public bool isFaceUp = false;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = cardFront;
    }

    void Update()
    {
        if (isFaceUp)
        {
            GetComponent<SpriteRenderer>().sprite = cardFront;
        }
    }
}
