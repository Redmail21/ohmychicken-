using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    public GameObject floorCube; //Each of them has a total width of 6

    public List<GameObject> cubesList = new List<GameObject>();

    Vector3 cubePos;

    

    private void Awake()
    {
        cubePos = new Vector3(6, -8, 6);
        
    }

    private void Start()
    {


        CubeInstatiateAllSides();
               

    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        
    }

    //Custom functions
    //
    //


    void CubeInstatiateAllSides()
    {

        int length = 3;  //Total Grid length

       
       GameObject centerCube = GameObject.Instantiate(floorCube, new Vector3(0, -8, 0), Quaternion.identity, gameObject.transform);
        
        cubesList.Add(centerCube);

        //horizontal and vertical

        for (int i = 1; i <= length; i++)
        {

        //Horizontally
        cubePos = new Vector3(6, -8, 6);


        cubePos.x = 0;
        cubePos.z = 6*i;

        cubesList.Add(GameObject.Instantiate(floorCube, cubePos, Quaternion.identity, gameObject.transform));

        cubePos.z = 6 *-i;

        cubesList.Add(GameObject.Instantiate(floorCube, cubePos, Quaternion.identity, gameObject.transform));


        //Now vertically


        cubePos = new Vector3(6, -8, 6);

        cubePos.z = 0;
        cubePos.x = 6*i;

        cubesList.Add(GameObject.Instantiate(floorCube, cubePos, Quaternion.identity, gameObject.transform));

        cubePos.x = 6*-i;

        cubesList.Add(GameObject.Instantiate(floorCube, cubePos, Quaternion.identity, gameObject.transform));


        }

        //Diagonals
        for (int i = 0; i <= length; i++)
        {
            

            for (int j = 0; j <= length; j++)
            {   
                if (j!=0 && i!=0)
                {


                cubePos = new Vector3(6, -8, 6);

                cubePos.x *= j;
                cubePos.z *= i;

                
                cubesList.Add(GameObject.Instantiate(floorCube, cubePos, Quaternion.identity, gameObject.transform));


                cubePos.x *= -1;

                cubesList.Add(GameObject.Instantiate(floorCube, cubePos, Quaternion.identity, gameObject.transform));

                cubePos.z *= -1;

                cubesList.Add(GameObject.Instantiate(floorCube, cubePos, Quaternion.identity, gameObject.transform));

                cubePos.x *= -1;

                cubesList.Add(GameObject.Instantiate(floorCube, cubePos, Quaternion.identity, gameObject.transform));


                }

            }

        }



    }

    public void CubeVanisher(bool random, bool isVanished, int optionalIndex=0)
    {
        if (random)
        {

            int val = getCubeIndex(isVanished);

            if (val == 999999999)
            {
                print("INDEX ERROR, NOTHING TO CHOOSE FROM.");
                return; 
            }

            cubesList[val].GetComponent<scriptCube>().ToggleVanish();
        

        } else if (!random)
        {
            
            cubesList[optionalIndex].GetComponent<scriptCube>().ToggleVanish();


        }
        else
        {
            Debug.Log("Error, incoherence on banishing method!?");
        }

    }


    private int getCubeIndex(bool isVanished) //Want it vanished? True or False
    {
        for (int i = 0; i < cubesList.Count; i++)
        {

            if (cubesList[i].GetComponent<scriptCube>().isVanished == isVanished ) {

                int val = Random.Range(0, cubesList.Count);
                if (cubesList[val].GetComponent<scriptCube>().isVanished == !isVanished && cubesList[val].GetComponent<scriptCube>().isVanishing == false && cubesList[val] !=null)
                {
                    return getCubeIndex(isVanished);

                }
                else

                {
                    print("getting one");
                    return val;
                }
            }

            

        }

        return 999999999; //If everything else fails we get the index 1, hopefully we'll keep that from happening.
        
    }



    
}
