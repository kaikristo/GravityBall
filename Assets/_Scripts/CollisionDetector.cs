using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{

    private LevelCtrl levelCtrl;
    private void Start()
    {
        levelCtrl = LevelCtrl.instance;
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == levelCtrl.EndTag)
        {
            levelCtrl.ReachEnd();
        }
        else
            if (other.tag == levelCtrl.StartTag)
        {
            levelCtrl.ReachStart();
        }
        else
            if(other.tag == levelCtrl.DeathTag && !LevelCtrl.fly)
        {
            levelCtrl.Death();
        }
    }
}
