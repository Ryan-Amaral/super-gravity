using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Defines a gravity field which is comprised of many sectors, each with its own
gravity vector. Only use 1 instance per scene.
*/
public class GravityField : MonoBehaviour{

    public enum GravityType {Sum, Max, Average};

    // bounds where gravity exists
    public Vector3 boundsStart;
    public Vector3 boundsEnd;

    public float sectorSize = 10; // size on each axis of each cube of gravity field
    float sectorDiagonal;

    public GravityType gravityType; // how to aggregate gravity vectors within sector

    public string gravityTag = "Gravity"; // the tag for objects that affect gravity

    GravitySector[,,] gravitySectors; // make up the gravity field

    Vector3 fieldCenter; // center of field to draw to if OOB

    public static GravityField instance;

    /*
    Create the gravity sectors with just positions for now.
    */
    void Start(){
        instance = this;

        // amount of sectors to place along each axis
        int numX = Mathf.CeilToInt((boundsEnd.x - boundsStart.x)/sectorSize);
        int numY = Mathf.CeilToInt((boundsEnd.y - boundsStart.y)/sectorSize);
        int numZ = Mathf.CeilToInt((boundsEnd.z - boundsStart.z)/sectorSize);
        gravitySectors = new GravitySector[numX,numY,numZ];

        for(int x=0; x<numX; x++){
            for(int y=0; y<numY; y++){
                for(int z=0; z<numZ; z++){
                    Vector3 sectorPosition = new Vector3(
                        boundsStart.x + (x*sectorSize) + sectorSize/2,
                        boundsStart.y + (y*sectorSize)+ sectorSize/2,
                        boundsStart.z + (z*sectorSize)+ sectorSize/2
                    );
                    gravitySectors[x,y,z] = new GravitySector(sectorPosition);

                    // debug delete later
                    //GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //temp.transform.position = sectorPosition;
                    //temp.GetComponent<Collider>().enabled = false;
                }
            }
        }

        fieldCenter = new Vector3((boundsEnd.x - boundsStart.x)/2,
                                  (boundsEnd.y - boundsStart.y)/2,
                                  (boundsEnd.z - boundsStart.z)/2);

        CreateGravityField();

        sectorDiagonal = Mathf.sqrt(Mathf.Pow(sectorSize,2)*2);
    }

    // Update is called once per frame
    void Update(){

    }

    /*
    Gives gravity vectors to sectors and combines them.
    */
    void CreateGravityField(){
        // create gravity vectors
        for(int x=0; x<gravitySectors.GetLength(0); x++){
            for(int y=0; y<gravitySectors.GetLength(1); y++){
                for(int z=0; z<gravitySectors.GetLength(2); z++){
                    // if sector is in collider, update gravity
                    GravitySource gravSrc = GetGravitySource(gravitySectors[x,y,z].position);
                    if(gravSrc != null){ // is a gravity source
                        UpdateGravitySectors(gravitySectors[x,y,z], gravSrc.density);
                        //Debug.Log("Gravity Source at: " + gravitySectors[x,y,z].position);
                    }
                }
            }
        }

        // combine vectors within each sector
        for(int x=0; x<gravitySectors.GetLength(0); x++){
            for(int y=0; y<gravitySectors.GetLength(1); y++){
                for(int z=0; z<gravitySectors.GetLength(2); z++){
                    gravitySectors[x,y,z].CombineGravityVectors();
                }
            }
        }
    }

    /*
    Checks if the point is within some collider that causes gravity, and returns
    that object if so. Would be cleaner if can hit backfaces.
    */
    GravitySource GetGravitySource(Vector3 point){
        RaycastHit[] hits = Physics.RaycastAll(point, Vector3.down);

        float minDist = -1;
        Vector3 closestPoint = Vector3.zero;
        bool hitFound = false;
        foreach(RaycastHit hit in hits){
            // only count gravity causing objects
            if(hit.transform.gameObject.tag.Equals(gravityTag)){
                // find nearest hit
                float curDist = hit.distance;
                if(minDist == -1 || curDist < minDist){
                    hitFound = true;
                    minDist = hit.distance;
                    closestPoint = hit.point;
                }
            }
        }

        // raycast back to origin point, from either out of field or closestPoint
        if(!hitFound){
            closestPoint = new Vector3(point.x, boundsStart.y, point.z);
        }

        hits = Physics.RaycastAll(closestPoint, Vector3.up,
                                        (point - closestPoint).magnitude);
        // any number of hits going back means point is inside
        foreach(RaycastHit hit in hits){
            // only count gravity causing objects
            if(hit.transform.gameObject.tag.Equals(gravityTag)){
                return hit.transform.gameObject.GetComponent<GravitySource>();
            }
        }

        return null;
    }

    /*
    Updates all gravity sectors with the gravity from the current point.
    */
    void UpdateGravitySectors(GravitySector sourceSector, float sourceDensity){
        for(int x=0; x<gravitySectors.GetLength(0); x++){
            for(int y=0; y<gravitySectors.GetLength(1); y++){
                for(int z=0; z<gravitySectors.GetLength(2); z++){
                    gravitySectors[x,y,z].gravityVectors.Add(
                            CreateGravityVector(sourceSector.position,
                                                gravitySectors[x,y,z].position,
                                                sourceDensity));
                }
            }
        }
    }

    /*
    Creates a gravity vector of appropriate direction and strength.
    */
    Vector3 CreateGravityVector(Vector3 source, Vector3 dest, float sourceDensity){
        if(Vector3.Distance(source, dest) == 0){
            return Vector3.zero;
        }else{
            Vector3 dirVect = source - dest; // vector pointing from dest to source
            Vector3 dirVectNorm = dirVect.normalized; // unit length
            float gravStrength = sourceDensity/dirVect.sqrMagnitude;

            return dirVectNorm*gravStrength;
        }
    }

    /*
    Gets the gravity vector at the reqeusted position.
    */
    public Vector3 GetGravity(Vector3 position){
        // get sector coordinates within gravity field
        float coordX = (position.x - boundsStart.x)/sectorSize;
        float coordY = (position.y - boundsStart.y)/sectorSize;
        float coordZ = (position.z - boundsStart.z)/sectorSize;

        // indices for looking up gravity sectors
        // make idx_1s valid
        int idxX1 = (int)Mathf.Clamp(coordX, 0, gravitySectors.GetLength(0)-1);
        int idxX2 = idxX1;
        int idxY1 = (int)Mathf.Clamp(coordY, 0, gravitySectors.GetLength(1)-1);
        int idxY2 = idxY1;
        int idxZ1 = (int)Mathf.Clamp(coordZ, 0, gravitySectors.GetLength(2)-1);
        int idxZ2 = idxZ1;

        // determine if idx_2s go before or after
        if(idxX1 == Mathf.Floor(coordX + 0.5f)){
            idxX2 = idxX1 - 1;
        }else{
            idxX2 = idxX1 + 1;
        }
        if(idxY1 == Mathf.Floor(coordY + 0.5f)){
            idxY2 = idxY1 - 1;
        }else{
            idxY2 = idxY1 + 1;
        }
        if(idxZ1 == Mathf.Floor(coordZ + 0.5f)){
            idxZ2 = idxZ1 - 1;
        }else{
            idxZ2 = idxZ1 + 1;
        }

        // store surrounding
        List<float> distances = new List<float>();
        List<Vector3> gravityVectors = new List<Vector3>();
        float totalDist = 0f;

        // get nearest surrounding gravity sectors vectors
        // first one guaranteed
        Vector3 vec = gravitySectors[idxX1,idxY1,idxZ1].GetGravity(gravityType);
        float dist = (position-vec).sqrMagnitude;
        distances.Add(dist);
        totalDist += dist;
        gravityVectors.Add(vec);
        // 1,1,2
        if(idxZ2 >= 0 || idxZ2 < gravitySectors.GetLength(2)){
            vec = gravitySectors[idxX1,idxY1,idxZ2].GetGravity(gravityType);
            dist = (position-vec).sqrMagnitude;
            distances.Add(dist);
            totalDist += dist;
            gravityVectors.Add(vec);
        }
        // 1,2,1
        if(idxY2 >= 0 || idxY2 < gravitySectors.GetLength(1)){
            vec = gravitySectors[idxX1,idxY2,idxZ1].GetGravity(gravityType);
            dist = (position-vec).sqrMagnitude;
            distances.Add(dist);
            totalDist += dist;
            gravityVectors.Add(vec);
        }
        // 1,2,2
        if((idxY2 >= 0 || idxY2 < gravitySectors.GetLength(1)) &&
                (idxZ2 >= 0 || idxZ2 < gravitySectors.GetLength(2))){
            vec = gravitySectors[idxX1,idxY2,idxZ2].GetGravity(gravityType);
            dist = (position-vec).sqrMagnitude;
            distances.Add(dist);
            totalDist += dist;
            gravityVectors.Add(vec);
        }
        // 2,1,1
        if(idxX2 >= 0 || idxX2 < gravitySectors.GetLength(0)){
            vec = gravitySectors[idxX2,idxY1,idxZ1].GetGravity(gravityType);
            dist = (position-vec).sqrMagnitude;
            distances.Add(dist);
            totalDist += dist;
            gravityVectors.Add(vec);
        }
        // 2,1,2
        if((idxX2 >= 0 || idxX2 < gravitySectors.GetLength(0)) &&
                (idxZ2 >= 0 || idxZ2 < gravitySectors.GetLength(2))){
            vec = gravitySectors[idxX2,idxY1,idxZ2].GetGravity(gravityType);
            dist = (position-vec).sqrMagnitude;
            distances.Add(dist);
            totalDist += dist;
            gravityVectors.Add(vec);
        }
        // 2,2,1
        if((idxX2 >= 0 || idxX2 < gravitySectors.GetLength(0)) &&
                (idxY2 >= 0 || idxY2 < gravitySectors.GetLength(1))){
            vec = gravitySectors[idxX2,idxY2,idxZ1].GetGravity(gravityType);
            dist = (position-vec).sqrMagnitude;
            distances.Add(dist);
            totalDist += dist;
            gravityVectors.Add(vec);
        }
        // 2,2,2
        if((idxX2 >= 0 || idxX2 < gravitySectors.GetLength(0)) &&
                (idxY2 >= 0 || idxY2 < gravitySectors.GetLength(1)) &&
                (idxZ2 >= 0 || idxZ2 < gravitySectors.GetLength(2))){
            vec = gravitySectors[idxX2,idxY2,idxZ2].GetGravity(gravityType);
            dist = (position-vec).sqrMagnitude;
            distances.Add(dist);
            totalDist += dist;
            gravityVectors.Add(vec);
        }

        // combine gravity vectors weighted by distance to position
        Vector3 finalVector = Vector3.zero;
        int numVecs = gravityVectors.Count;

        while(gravityVectors.Count > 0){
            Vector3 closestVec = gravityVectors[0];
            float closestDist = distances[0];
            int removeIndex = 0;
            for(int i=1; i < gravityVectors.Count; i++){
                if(distances[i] < closestDist){
                    closestDist = distances[i];
                    closestVec = gravityVectors[i];
                    removeIndex = i;
                }
            }

            // weight higher with smaller distance
            finalVector += closestVec*((1f/numVecs)*(sectorDiagonal-(sectorDiagonal-closestDist)));
            totalDist = totalDist-closestDist;

            gravityVectors.RemoveAt(removeIndex);
            distances.RemoveAt(removeIndex);
        }

        return finalVector;
    }

}
