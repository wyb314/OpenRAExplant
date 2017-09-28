using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public Transform tran;

    public Transform targetTran;

    public Vector3 DefaultOffset = new Vector3(0, 8, -4);

    void Start()
    {
        this.tran = transform;
    }


    void FixedUpdate()
    {
        if (targetTran == null)
        {
            return;
        }

        Vector3 offsetPos = DefaultOffset;

        Vector3 goalPosition = offsetPos*0.9f + this.targetTran.position;

        this.tran.position = Vector3.Lerp(this.tran.position, goalPosition, Time.deltaTime * 4);

        Vector3 dir = this.tran.forward;
        dir.y = 0;
        dir.Normalize();
        Vector3 t = this.targetTran.position;
        t += dir * 0.7f;

        Vector3 lookAt = t - this.tran.position;
        lookAt.Normalize();

        this.tran.forward = Vector3.Lerp(this.tran.forward, lookAt, Time.deltaTime * 4);
    }


}
