using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCard : MonoBehaviour
{
    [SerializeField] private GameObject cardBack;
    [SerializeField] private SceneController controller;

    private int _id;

    public int id
    {
        get { return _id; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        if (cardBack.activeSelf && controller.canUncovered)
        {
            cardBack.SetActive(false);
            controller.cardUncovered(this);
        }
    }

    public void Cover()
    {
        cardBack.SetActive(true);
    }

    public void SetCard(int id, Sprite image)
    {
        _id = id;
        GetComponent<SpriteRenderer>().sprite = image;
    }
}
