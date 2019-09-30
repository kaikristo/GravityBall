using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float RotationSpeed = 1f;

    public float minAngle = -30.0f;
    public float maxAngle = 30.0f;

    public float minGravity = -9.81f;
    public float maxGravity = 9.81f;

    bool reseting = false;






    private void Start()
    {

        Physics.gravity = new Vector3(0, minGravity * 2, 0);
    }

    // Start is called before the first frame update



    private float AngleToGravity(float angle)
    {
        if (angle > 180f) angle -= 360f;
        float range = Mathf.Abs(minAngle) + Mathf.Abs(maxAngle);
        float anglePercent = (angle + range / 2) / range;


        float physRange = Mathf.Abs(minGravity) + Mathf.Abs(maxGravity);
        float gravity = (anglePercent * physRange) - physRange / 2;

        return gravity;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        if (!LevelCtrl.fly)
        {
          
#if UNITY_EDITOR || UNITY_STANDALONE
  if (Input.GetMouseButton(0))
            {

                RotateObject();
            }

#elif UNITY_ANDROID
if (Input.touchCount > 0  &&Input.touches[0].phase == TouchPhase.Moved)
            {
               var i = Input.GetTouch(0).deltaPosition;
                RotateObject();
            }
        
#endif
        }


    }

    private IEnumerator AnimateRotationTowards(Transform target, Quaternion rot, float dur)
    {
        reseting = true;
        float t = 0f;
        Quaternion start = target.rotation;
        while (t < dur)
        {
            target.rotation = Quaternion.Slerp(start, rot, t / dur);
            yield return null;
            t += Time.deltaTime;
        }
        reseting = false;
        target.rotation = rot;
    }

    public void ResetCamera()
    {
        if (reseting) return;
        StartCoroutine(AnimateRotationTowards(this.transform, Quaternion.identity, 1f));
        Physics.gravity = new Vector3(0, minGravity * 4, 0);
    }

    private float ClampAngle(float angle, float from, float to)
    {

        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }
    private void RotateObject()
    {




#if UNITY_EDITOR || UNITY_STANDALONE
        float x = (Input.GetAxis("Mouse Y") * RotationSpeed * 30f * -Time.deltaTime);
        float z = (Input.GetAxis("Mouse X") * RotationSpeed * 30f * Time.deltaTime);
#elif UNITY_ANDROID
        var pos = Input.GetTouch(0).deltaPosition;
        Debug.Log(pos);
        float x = (pos.y * RotationSpeed * -Time.deltaTime);
        float z = (pos.x * RotationSpeed * Time.deltaTime);
#endif


        transform.Rotate(x, 0, z, Space.World);

        x = transform.eulerAngles.x;
        z = transform.eulerAngles.z;

        x = ClampAngle(x, minAngle, maxAngle);
        z = ClampAngle(z, minAngle, maxAngle);
        transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, z);

        x = AngleToGravity(x);
        z = AngleToGravity(z);

        Physics.gravity = new Vector3(z, 4 * minGravity, -x);
    }
}
