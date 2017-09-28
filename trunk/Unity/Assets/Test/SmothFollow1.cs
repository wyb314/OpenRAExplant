using UnityEngine;
using System.Collections;

public class SmothFollow1 : MonoBehaviour {



    

    // The target we are following
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 10.0f;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    // How much we 
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    public float posDamping = 10f;


    public float maxAngleSpeed = 30f;

    public float wantedAngle = 0;

    public float rot_y_scale = 1f;

    public float rot_y_speed = 0;

    // Update is called once per frame
    public void FixedUpdate()
    {
        // Early out if we don't have a target
        if (!target)
            return;

        //Debug.Log("rot_y_speed : " + rot_y_speed);
        Quaternion rot0 = Quaternion.Euler(0,rot_y_speed * rot_y_scale * Time.deltaTime,0) * transform.rotation;

        rot0 = Quaternion.Slerp(transform.rotation,rot0,rotationDamping * Time.deltaTime);

        // Calculate the current rotation angles
        //var wantedRotationAngle = target.eulerAngles.y + wantedAngle;
        //var wantedRotationAngle = transform.eulerAngles.y + rot_y_speed * 2 * Time.deltaTime;

        var wantedHeight = target.position.y + height;

        //var currentRotationAngle = transform.eulerAngles.y;
        var currentHeight = transform.position.y;


        //float temRotAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);


        //currentRotationAngle = temRotAngle;

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);


        //var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        Vector3 targetPos = target.position;

        targetPos -= rot0 * Vector3.forward * distance;

        targetPos.y = currentHeight;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * posDamping);

        //targetPos = target.position;
        //targetPos.y = 0;

        //transform.LookAt(targetPos);

        /*
        targetPos = target.position;
        //targetPos.y = 0;

        Vector3 dir = targetPos - transform.position;

        Quaternion rot = Quaternion.LookRotation(dir);


        transform.rotation = rot;
         * 
         * */

        //transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotationDamping);

        //Vector3 eulerAngles = transform.eulerAngles;
        //eulerAngles.y = currentRotationAngle;

        //Quaternion rot = Quaternion.Euler(eulerAngles);

        transform.rotation = rot0;

        //transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotationDamping);

    }
}
