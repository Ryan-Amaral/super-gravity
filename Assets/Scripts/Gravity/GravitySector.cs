using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySector{

    public Vector3 position; // position in the Game World

    public List<Vector3> gravityVectors;

    public float mass = 0f; // mass is 0 unless sector in gravity source

    Vector3 gravityVectorSum;
    Vector3 gravityVectorMax;
    Vector3 gravityVectorAvg;


    /*
    Creates a gravity sector at the desired world position.
    */
    public GravitySector(Vector3 position){
        this.position = position;
        gravityVectors = new List<Vector3>();
    }

    /*
    Combines gravity vectors into one.
    */
    public void CombineGravityVectors(float minMag, float maxMag, bool viewVector){
        // return zero vec if None,
        if(gravityVectors.Count == 0){
            gravityVectorSum = Vector3.zero;
            gravityVectorMax = Vector3.zero;
            gravityVectorAvg = Vector3.zero;
            return;
        }


        gravityVectorSum = Vector3.zero;
        float highestMag = 0f;
        List<Vector3> maxVecs = new List<Vector3>(); // list to average tied max

        foreach(Vector3 vec in gravityVectors){
            gravityVectorSum += vec; // add to sum
            if(vec.sqrMagnitude > highestMag){
                maxVecs.Clear();
                highestMag = vec.sqrMagnitude; // set max
                maxVecs.Add(vec);
            }else if(vec.sqrMagnitude == highestMag){
                maxVecs.Add(vec);
            }
        }
        gravityVectorAvg = gravityVectorSum/gravityVectors.Count; // set avg

        // average max
        gravityVectorMax = Vector3.zero;
        foreach(Vector3 vec in maxVecs){
            gravityVectorMax += vec;
        }
        gravityVectorMax /= maxVecs.Count;

        gravityVectorSum = ClampMagnitude(gravityVectorSum, minMag, maxMag);
        gravityVectorMax = ClampMagnitude(gravityVectorMax, minMag, maxMag);
        gravityVectorAvg = ClampMagnitude(gravityVectorAvg, minMag, maxMag);

        gravityVectors = null; // clear some space

        if(viewVector){
            Debug.DrawLine(position, position+gravityVectorSum, Color.red, 1000, false); // debug, delete later
        }
    }

    /*
    Returns a combined gravity vector of this sector.
    */
    public Vector3 GetGravity(GravityField.GravityType gravityType){
        switch(gravityType){
            case GravityField.GravityType.Sum:
                return gravityVectorSum;
            case GravityField.GravityType.Max:
                return gravityVectorMax;
            case GravityField.GravityType.Average:
                return gravityVectorAvg;
            default:
                return Vector3.zero;
        }
    }

    // https://forum.unity.com/threads/clampmagnitude-why-no-minimum.388488/
    Vector3 ClampMagnitude(Vector3 v, float max, float min){
        double sm = v.sqrMagnitude;
        if(max > 0 && sm > (double)max * (double)max) return v.normalized * max;
        else if(min > 0 && sm < (double)min * (double)min) return v.normalized * min;
        return v;
    }
}
