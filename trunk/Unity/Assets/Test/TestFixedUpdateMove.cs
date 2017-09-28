using System.Collections;
using System.Collections.Generic;
using Engine;
using Engine.ComponentsAI;
using Engine.Primitives;
using UnityEngine;

public class TestFixedUpdateMove : MonoBehaviour {

    GameInputerGetter inputGetter = new GameInputerGetter();

    public float posDamp = 8;

    // Use this for initialization
    void Start ()
    {
        this.curPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		this.GetJoystickInput();


        //Vector3 pos = Vector3.Lerp(this.transform.position, this.curPos, Time.deltaTime * posDamp);

        //this.transform.position = pos;
    }

    private ushort data;
    private int facing;

    private int lastRot = int.MaxValue;
    private WPos lastPos = new WPos(0, 0, 0);
    private Vector3 curPos = Vector3.zero;
    void FixedUpdate()
    {
        //this.GetJoystickInput();
        
        int angle = ((data & 0xFF00) >> 8);
        int force = data & 0x00ff;

        if (force != 0)
        {
            facing = AIUtils.TickFacing(facing, angle, 4);
            
            float rad = facing * Mathf.PI / 128;

            this.transform.eulerAngles = new Vector3(0, -rad * Mathf.Rad2Deg, 0);

            var dir = new WVec(0, -1024, 0).Rotate(WRot.FromFacing(this.facing));
           
            WVec v = 1024 * 3 * dir * Game.Timestep / (1024 * 1024);
            //Debug.Log("dir length->" + dir.Length + " length-> " + v.Length);
            this.lastPos += v;
            curPos = new Vector3(((float)this.lastPos.X) / 1024, 0, -((float)this.lastPos.Y) / 1024);

            //curPos = Vector3.MoveTowards(curPos, pos, Time.deltaTime * 15);

            //this.lastPos = curPos;

            this.transform.position = curPos;
        }
    }

    private void GetJoystickInput()
    {
        float h = this.inputGetter.GetAxis("Horizontal");
        float v = this.inputGetter.GetAxis("Vertical");

        float sqrLength = h * h + v * v;
        if (sqrLength > 0)
        {
            float rad = MathUtils.Atan2(v, h) - MathUtils.Half_PI;

            if (rad < 0)
            {
                rad = 2*MathUtils.PI + rad;
            }

            int val = (int) (rad*128/MathUtils.PI);

            val = MathUtils.Min(val, byte.MaxValue); //normaized to 0 - 255;
            ushort angle = (ushort) val;

            int length = (int) (256*MathUtils.Sqrt(sqrLength));
            length = MathUtils.Min(length, byte.MaxValue);
            ushort force = (ushort) length;

            data = (ushort) ((angle << 8) | force);

        }
        else
        {
            data = 0;
        }
    }
}
