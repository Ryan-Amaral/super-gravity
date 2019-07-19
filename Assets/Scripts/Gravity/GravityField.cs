using System.Collections;
using System.Collections.Generic;
using System;
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

    public GravityType gravityType; // how to aggregate gravity vectors within sector

    public string gravityTag = "Gravity"; // the tag for objects that affect gravity

    public float minMagnitude = -1f;
    public float maxMagnitude = -1f;

    public bool viewVectors = false; // view in editor, expensive with many vectors

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
    }

    // Update is called once per frame
    void Update(){

    }

    /*
    Gives gravity vectors to sectors and combines them.
    */
    void CreateGravityField(){
        // find all mass causing sectors, and all others
        List<GravitySector> sourceSectors = new List<GravitySector>();
        List<GravitySector> masslessSectors = new List<GravitySector>();

        for(int x=0; x<gravitySectors.GetLength(0); x++){
            for(int y=0; y<gravitySectors.GetLength(1); y++){
                for(int z=0; z<gravitySectors.GetLength(2); z++){
                    GravitySource gravSrc = GetGravitySource(gravitySectors[x,y,z].position);
                    if(gravSrc != null){ // is a gravity source
                        // update sector with mass from source
                        gravitySectors[x,y,z].mass = gravSrc.density;
                        // combine now, as nothing to add
                        gravitySectors[x,y,z].CombineGravityVectors(
                                minMagnitude, maxMagnitude, viewVectors);
                        sourceSectors.Add(gravitySectors[x,y,z]);
                    }else{
                        masslessSectors.Add(gravitySectors[x,y,z]);
                    }
                }
            }
        }

        // for each massless sector, update with gravity vectors from all source sectors
        foreach(GravitySector noMassSect in masslessSectors){
            foreach(GravitySector srcSect in sourceSectors){
                noMassSect.gravityVectors.Add(
                        CreateGravityVector(srcSect.position,
                                            noMassSect.position,
                                            srcSect.mass));
            }
            noMassSect.CombineGravityVectors(minMagnitude, maxMagnitude, viewVectors);
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
    Creates a gravity vector of appropriate direction and strength.
    */
    Vector3 CreateGravityVector(Vector3 source, Vector3 dest, float sourceMass){
        if(Vector3.Distance(source, dest) == 0){
            return Vector3.zero;
        }else{
            Vector3 dirVect = source - dest; // vector pointing from dest to source
            Vector3 dirVectNorm = dirVect.normalized; // unit length
            float gravStrength = sourceMass/dirVect.sqrMagnitude;

            return dirVectNorm*gravStrength;
        }
    }

    /*
    Gets the gravity vector at the reqeusted position.
    Using Inverse Distance Weighting to interpolate between surrounding vectors.
    https://mgimond.github.io/Spatial/spatial-interpolation.html
    */
    public Vector3 GetGravity(Vector3 position){
        // get sector coordinates within gravity field
        float coordX = (position.x - boundsStart.x)/sectorSize;
        float coordY = (position.y - boundsStart.y)/sectorSize;
        float coordZ = (position.z - boundsStart.z)/sectorSize;

        // clamp values to make valid
        int x = (int)Mathf.Clamp(coordX, 0, gravitySectors.GetLength(0)-1);
        int y = (int)Mathf.Clamp(coordY, 0, gravitySectors.GetLength(1)-1);
        int z = (int)Mathf.Clamp(coordZ, 0, gravitySectors.GetLength(2)-1);

        // build up numerator and denominator of IDW
        Vector3 idwNum = Vector3.zero;
        float idwDen = 0f;

        int[,] allIndices = GetSurroundingIndices3D(x,y,z);

        try{
            // interpolate between all surrounding
            for(int i = 0; i < allIndices.GetLength(0); i++){
                int iX = allIndices[i,0];
                int iY = allIndices[i,1];
                int iZ = allIndices[i,2];
                if(iX < 0 || iX >= gravitySectors.GetLength(0) ||
                        iY < 0 || iY >= gravitySectors.GetLength(1) ||
                        iZ < 0 || iZ >= gravitySectors.GetLength(2)){
                    continue; // don't take out of bounds ones
                }else{
                    GravitySector gravSect = gravitySectors[iX,iY,iZ];
                    float dist = (position-gravSect.position).sqrMagnitude;
                    idwNum += gravSect.GetGravity(gravityType)/dist;
                    idwDen += 1f/dist;
                }
            }
        }catch(Exception e){
            Debug.Log(e);
            // if dist is zero, will cause error, return center vect
            return gravitySectors[x,y,z].GetGravity(gravityType);
        }

        return idwNum/idwDen; // return the interpolated ones
    }

    /*
    Gets the surrounding cube of indices of the given 3D index.
    */
    int[,] GetSurroundingIndices3D(int x, int y, int z, bool includeMiddle=true){
        int[,] surInds;
        // different size if including middle
        if(includeMiddle){
            surInds = new int[27,3];
        }else{
            surInds = new int[26,3];
        }

        int count = 0;

        for(int iX = x-1; iX < x+2; iX++){
            for(int iY = y-1; iY < y+2; iY++){
                for(int iZ = z-1; iZ < z+2; iZ++){
                    // don't include middle unless we are
                    if(includeMiddle || !(iX == x && iY == y && iZ == z)){
                        surInds[count,0] = iX;
                        surInds[count,1] = iY;
                        surInds[count,2] = iZ;
                        count++;
                    }
                }
            }
        }

        return surInds;
    }

}
