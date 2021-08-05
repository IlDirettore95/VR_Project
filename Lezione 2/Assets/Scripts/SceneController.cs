using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private MemoryCard originalCard;
    [SerializeField] private Sprite[] images;

    private MemoryCard _firstUncovered;
    private MemoryCard _secondUncovered;

    public bool canUncovered
    {
        get { return _secondUncovered == null; }
    }

    public const int gridRows = 2;
    public const int gridCols = 4;
    public const float offsetX = 2f;
    public const float offSetY = 2.5f;

    private int _score = 0;
    private int _moves = 0;

    [SerializeField] private TextMesh scoreLabel;
    [SerializeField] private TextMesh movesLabel;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPos = originalCard.transform.position;
        int[] numbers = { 0, 0, 1, 1, 2, 2, 3, 3};
        numbers = ShuffleArray(numbers);
        for (int i = 0; i < gridCols; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                MemoryCard card;
                if(i == 0 && j == 0)
                {
                    card = originalCard;
                }
                else
                {
                    card = Instantiate(originalCard);
                }
                int index = j * gridCols + i;
                int id = numbers[index];
                card.SetCard(id, images[id]);
                float posX = (offsetX * i) + startPos.x;
                float posY = -(offSetY * j) + startPos.y;
                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];
        for(int i = 0; i < newArray.Length; i++)
        {
            int tmp = newArray[i];
            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    public void cardUncovered (MemoryCard card)
    {
        if(_firstUncovered == null)
        {
            _firstUncovered = card;
        }
        else
        {
            _secondUncovered = card;
            StartCoroutine(CardsCheck());
        }
    }

    private IEnumerator CardsCheck()
    {
        if(_firstUncovered.id == _secondUncovered.id)
        {
            _score++;
            _moves++;
            scoreLabel.text = "Score: " + _score;
            movesLabel.text = "Moves: " + _moves;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            _firstUncovered.Cover();
            _secondUncovered.Cover();
            _moves++;
            movesLabel.text = "Moves: " + _moves;
        }
        _firstUncovered = null;
        _secondUncovered = null;
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }
}


