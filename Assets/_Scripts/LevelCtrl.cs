using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCtrl : MonoBehaviour
{
    public static LevelCtrl instance;


    public static bool fly = false;

 

    public GameObject[] levels;

    public GameObject Ball;
  

    public float movementSpeed = 10.0f;
    private GameObject startPos, endPos;




    private GameObject curLevel = null;
    private GameObject nextLevel = null;
    private GameObject prevLevel = null;

    private CameraRotator cameraRotator;
    private readonly string start = "Respawn";
    private readonly string end = "Finish";
    private readonly string death = "Death";



    private Rigidbody ballRb;
    private int levelNumber = 0;

    internal void Again()
    {
        Ball.transform.position = new Vector3(-5f, 9, 0);
        fly = true;
        SpawnLevel();

    }

    public string StartTag => start;

    public string EndTag => end;

    public string DeathTag => death;



   

    public void SpawnLevel()
    {
       


        ballRb.velocity = Vector3.zero;
        if (curLevel == null)
        {
            curLevel = Instantiate(levels[levelNumber++ % 2]);
            curLevel.transform.position = Vector3.zero;
            nextLevel = Instantiate(levels[levelNumber++ % 2]);
        }
        else
        {

            prevLevel = curLevel;

            curLevel = nextLevel;
            nextLevel = Instantiate(levels[levelNumber++ % 2]);
        }



        foreach (var i in curLevel.GetComponentsInChildren<Transform>())
        {
            if (i.tag == StartTag)
                startPos = i.gameObject;
            else
                if (i.tag == EndTag)
                endPos = i.gameObject;
        }
        // добавить сдвиг, чтоб старт поинт была под энд поинтом.
        nextLevel.transform.position = new Vector3
            (
            endPos.transform.position.x,
            endPos.transform.position.y - 40,
            endPos.transform.position.z
            );
        Debug.Log(startPos);
        Debug.Log(endPos);



        // StartCoroutine(MoveTo(startPos.transform.position));
        
     
        //destroy previously level

    }

    public void Death()
    {
        ballRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        ballRb.isKinematic = true;
   
        Destroy(curLevel);
        Destroy(nextLevel);
        curLevel = null;
        nextLevel = null;
        prevLevel = null;
        cameraRotator.ResetCamera();
        Again();       
    }

    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    private void Start()
    {
        ballRb = Ball.GetComponent<Rigidbody>();
       
        cameraRotator = FindObjectOfType<CameraRotator>();

        ReachEnd();
        

    }


    public void ReachEnd()
    {
         SpawnLevel();
        fly = true;
        ballRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        ballRb.isKinematic = true;

        cameraRotator.ResetCamera();
    }
    public void ReachStart()
    {
       
        fly = false;
        ballRb.isKinematic = false;
        ballRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        DestroyPrev();
    }


    private void DestroyPrev()
    {
        if (prevLevel != null)
            Destroy(prevLevel);
    }

    private void Update()
    {
        if (fly)
        {
            ballRb.isKinematic = true;
            Vector3 direction = (startPos.transform.position - Ball.transform.position).normalized;
            ballRb.MovePosition(Ball.transform.position + direction * movementSpeed * Time.deltaTime);

        }

    }


}
