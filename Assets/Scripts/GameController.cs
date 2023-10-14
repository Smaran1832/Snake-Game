using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance=null;

    const float width = 3.7f;
    const float height = 7f;

    public float snakeSpeed = 1;

    public BodyPart bodyPrefab = null;
    public GameObject rockPrefab = null;
    public GameObject eggPrefab = null;
    public GameObject goldEggPrefab = null;
    public GameObject spikePrefab = null;

    public Sprite tailsprite = null;
    public Sprite bodysprite = null;

    public SnakeHead snakeHead = null;

    public bool alive = true;

    public bool waitingToPlay=true;

    List<Egg> eggs = new List<Egg>();
    List<GameObject> spikes = new List<GameObject>();

    int level = 0;
    int NumberOfEggsForNextLevel = 0;

    public int highScore = 0;
    public int Score = 0;

    public Text Scoretext = null;
    public Text HighScoretext = null;
    public Text TapToPlayText = null;
    public Text GameOverText = null;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Debug.Log("Starting Snake Game");
        CreateWalls();
        alive = false;
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
             QualitySettings.vSyncCount = 0;
             Application.targetFrameRate = 60;
        #endif

        if(waitingToPlay)
        {
            foreach(Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    StartGamePlay();
                }

            }

            if (Input.GetMouseButtonUp(0))
                StartGamePlay();
        }
    }

    void StartGamePlay()
    {
        Score = 0;
        level = 0;
        Scoretext.text = "Score = " + Score;
        HighScoretext.text = "HighScore = " + highScore;
        GameOverText.gameObject.SetActive(false);
        TapToPlayText.gameObject.SetActive(false);
        waitingToPlay = false;
        alive = true;
        
        KillOldSpikes();
        KillOldEggs();
        LevelUp();
    }

    void LevelUp()
    {
        level++;

        NumberOfEggsForNextLevel = 4 + (level * 2); //6 8 ...
        snakeSpeed = 1.5f * (level / 2f);
        if (snakeSpeed > 6) snakeSpeed = 6;

        snakeHead.ResetSnake();
        KillOldSpikes();
        CreateEgg();
        for(int i=0;i<level;i++)
        {
            createSpike();
        }
    }
    public void GameOver()
    {
        alive = false;
        waitingToPlay = true;
        GameOverText.gameObject.SetActive(true);
        TapToPlayText.gameObject.SetActive(true);
    }

    public void EggEaten(Egg egg)
    {
        Score++;

        NumberOfEggsForNextLevel--;
        if (NumberOfEggsForNextLevel == 0)
        {
            LevelUp();
            Score += 10;

        }
        else if (NumberOfEggsForNextLevel == 1)
            CreateEgg(true);
        else
            CreateEgg(false);

        if (Score > highScore)
        {
            highScore = Score;
            HighScoretext.text = "HighScore =" + highScore;
        }
           

        Scoretext.text = "Score =" + Score;
       

        eggs.Remove(egg);
        Destroy(egg.gameObject);
    }
    void CreateWalls()
    {
        Vector3 start = new Vector3(-width, -height, 0);
        Vector3 finish = new Vector3(-width, +height, 0);
        CreateWall(start, finish);
        start = new Vector3(+width, -height, 0);
        finish = new Vector3(+width, +height, 0);
        CreateWall(start, finish);
        start = new Vector3(-width, -height, 0);
        finish = new Vector3(+width, -height, 0);
        CreateWall(start, finish);
        start = new Vector3(-width, +height, 0);
        finish = new Vector3(+width, +height, 0);
        CreateWall(start, finish);
    }

    void CreateWall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start, finish);
        int noOfRocks = (int)(distance * 3f);
        Vector3 delta = (finish - start) / noOfRocks;

        Vector3 position = start;
        for (int rock = 0; rock <= noOfRocks; rock++)
        {
            float rotation = Random.Range(0, 360f);
            float scale = Random.Range(1.5f, 2f);
            CreateRock(position, scale, rotation);
            position = position + delta;
        }
    }

    void CreateRock(Vector3 position,float Scale,float rotation)
    {
        GameObject rock = Instantiate(rockPrefab, position, Quaternion.Euler(0, 0, rotation));
        rock.transform.localScale = new Vector3(Scale, Scale, 1);
    }

    void CreateEgg(bool gold=false)
    {
        Vector3 position;
        position.x=-width+ Random.Range(3f,(width*2))-2f;
        position.y = -height + Random.Range(3f, (height * 2)) - 2f;
        position.z = 0;
        Egg egg = null;
        if(gold)
           egg= Instantiate(goldEggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        else
           egg= Instantiate(eggPrefab,position, Quaternion.identity).GetComponent<Egg>();

        eggs.Add(egg);
    }

    void KillOldEggs()
    {
        foreach(Egg egg in eggs)
        {
            Destroy(egg.gameObject);
        }
        eggs.Clear();
    }

    void createSpike()
    {
        Vector3 position;
        position.x = -width + Random.Range(2f, (width * 2)) - 2f;
        position.y = -height + Random.Range(2f, (height * 2)) - 2f;
        position.z = 0;
        GameObject spike = Instantiate(spikePrefab, position, Quaternion.identity);
        spikes.Add(spike);
    }

    void KillOldSpikes()
    {
        foreach (GameObject spike in spikes)
        {
            Destroy(spike.gameObject);
        }
        spikes.Clear();
    }
}
