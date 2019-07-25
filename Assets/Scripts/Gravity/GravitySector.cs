using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySector{

    public Vector3 position; // position in the Game World

    public float mass = 0f; // mass is 0 unless sector in gravity source

    // types of final vectors
    Vector3 gravityVectorSum;
    Vector3 gravityVectorMax;
    Vector3 gravityVectorAvg;

    // to help build gravity vectors
    int totalVecs = 0;
    int totalMaxVecs = 0;
    float maxSqrMag = 0f;


    /*
    Creates a gravity sector at the desired world position.
    */
    public GravitySector(Vector3 position){
        this.position = position;

        gravityVectorSum = Vector3.zero;
        gravityVectorMax = Vector3.zero;
        gravityVectorAvg = Vector3.zero;
    }

    /*
    Adds the vector to the aggregate vectors, except for avg which is done after.
    */
    public void AddGravityVector(Vector3 gravVec, bool maxOnly=false){
        if(!maxOnly){
            gravityVectorSum += gravVec;
            totalVecs++;
        }

        float curSqrMag = gravVec.sqrMagnitude;
        if(!Mathf.Approximately(curSqrMag, maxSqrMag) && curSqrMag > maxSqrMag){
            gravityVectorMax = gravVec;
            totalMaxVecs = 1;
            maxSqrMag = curSqrMag;
        }else if(Mathf.Approximately(curSqrMag, maxSqrMag)){
            gravityVectorMax += gravVec;
            totalMaxVecs += 1;
        }
    }

    /*
    Combines gravity vectors into one.
    */
    public void CombineGravityVectors(float minMag, float maxMag, bool viewVector){
        // sum is already taken care of, create max and avg
        if(totalVecs > 0 && totalMaxVecs > 0){
            gravityVectorMax /= totalMaxVecs;
            gravityVectorAvg = gravityVectorSum / totalVecs;
        }

        // clamp values if applicable
        gravityVectorSum = ClampMagnitude(gravityVectorSum, minMag, maxMag);
        gravityVectorMax = ClampMagnitude(gravityVectorMax, minMag, maxMag);
        gravityVectorAvg = ClampMagnitude(gravityVectorAvg, minMag, maxMag);

        if(viewVector){
            Debug.DrawLine(position, position+gravityVectorSum, Color.red, 1000, false);
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
