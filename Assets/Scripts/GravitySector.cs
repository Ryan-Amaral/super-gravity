using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySector{

    public Vector3 position; // position in the Game World

    public List<Vector3> gravityVectors;

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
    Returns a combined gravity vector of this sector.
    */
    public Vector3 GetGravity(GravityField.GravityType gravityType){
        return new Vector3();
    }

    /*
    Combines gravity vectors into one.
    */
    public void CombineGravityVectors(){
        gravityVectorSum = Vector3.zero;
        float maxMag = 0;

        foreach(Vector3 vec in gravityVectors){
            gravityVectorSum += vec; // add to sum
            if(vec.sqrMagnitude > maxMag){
                maxMag = vec.sqrMagnitude; // set max
                gravityVectorMax = vec;
            }
        }
        gravityVectorAvg = gravityVectorSum/gravityVectors.Count; // set avg

        Debug.DrawLine(position, position+gravityVectorSum, Color.red, 1000, false); // debug, delete later
    }
}
