using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using TrueSync;

public class NewEditModeTest {

	[Test]
	public void NewEditModeTestSimplePasses() {
		// Use the Assert class to test conditions.\
        
        float[] fGroup0 = new Single[10];
        
        float[] fGroup1 = new Single[10];

	    GerateRamSum(fGroup0);
        GerateRamSum(fGroup1);

	    FP result0 = 0;
	    FP sum0 = 0;
	    FP aa = FP.EN3;
	    foreach (var val0 in fGroup0)
	    {
	        sum0 += val0;
	        result0 += val0* aa;
	    }
        
        FP result1 = 0;
        FP sum1 = 0;
        foreach (var val0 in fGroup1)
        {
            sum1 += val0;
            result1 += val0 * aa;
        }

        //Debug.LogError("sum0->" + sum0.AsFloat() + " sum1->" + sum1.AsFloat());
        Debug.LogError("result0->" + result0._serializedValue + " result1->" + result1._serializedValue);

    }

    private void GerateRamSum(float[] fGroup0)
    {

        //UnityEngine.Random.InitState(3);

        int i = 0;
        float sum = 0;
        while (i <= 8)
        {
            fGroup0[i] = UnityEngine.Random.Range(0, 0.1f);
            sum += fGroup0[i];
            if (i == 8)
            {
                fGroup0[i + 1] = 2 - sum;
                //sum += fGroup0[i + 1];
                break;
            }
            else
            {
                fGroup0[i + 1] = UnityEngine.Random.Range(0, 0.1f);
                sum += fGroup0[i + 1];
            }
            i+= 2;
        }
        
    }

    // A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	//[UnityTest]
	public IEnumerator NewEditModeTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
