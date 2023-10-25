using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SnakeController : MonoBehaviour
{
    Vector2 _areaLimit = new Vector2(13, 24);

    [SerializeField] TextMeshPro scoreText;
    [SerializeField] TextMeshPro gameOverText;
    [SerializeField] Canvas restartGame;

    [SerializeField] GameObject foodPrefab;
    [SerializeField] GameObject tailPrefab;
    [SerializeField] Transform parentTail;
    [SerializeField] float moveSpeedTime = 1f;

    Vector2 direction = Vector2.down;
    List<Transform> snake = new List<Transform>();

    int score = 0;

    void Awake() 
    {
        if(gameOverText.enabled) { gameOverText.enabled = false; }
        if(restartGame.enabled) { restartGame.enabled = false; }
    }

    void Start() 
    {
        ChangeFoodPos();
        StartCoroutine(Move());
        snake.Add(transform);
    }

    void Update() 
    {
        scoreText.text = score.ToString();

        if(Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
        direction = Vector2.down;
        else if(Input.GetKeyDown(KeyCode.W) && direction != Vector2.down)
            direction = Vector2.up;
        else if(Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
            direction = Vector2.left;
        else if(Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
            direction = Vector2.right;
    }

    IEnumerator Move()
    {
        while(true)
        {
            for(int i = snake.Count-1; i > 0; i--)
            {
                snake[i].position = snake[i-1].position; 
            }

            var position = transform.position;
            position += (Vector3)direction;
            position.x = Mathf.RoundToInt(position.x);
            position.y = Mathf.RoundToInt(position.y);
            transform.position = position;

            yield return new WaitForSeconds(moveSpeedTime);
        }

    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Food"))
        {
            Grow();
        }    
        else if(other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Tail"))
        {
            GameOver();
        }
    }

    void Grow()
    {
        ChangeFoodPos();
        score++;

        GameObject tail = Instantiate(tailPrefab, transform.position, Quaternion.identity, parentTail);
        tail.transform.position = new Vector3(99,99,0);
        snake.Add(tail.transform);
        
    }

    private void ChangeFoodPos()
    {
        Vector3 randomPosForFood = new Vector3((int)Random.Range(1, _areaLimit.x), (int)Random.Range(1, _areaLimit.y),0f);
        foreach(var item in snake)
        {
            if(item.position == randomPosForFood)
            {
                randomPosForFood = new Vector3((int)Random.Range(1, _areaLimit.x), (int)Random.Range(1, _areaLimit.y),0f);
            }
        }
        foodPrefab.transform.position = randomPosForFood;
    }

    void GameOver()
    {
        Time.timeScale = 0;
        gameOverText.enabled = true;
        restartGame.enabled = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }

}
