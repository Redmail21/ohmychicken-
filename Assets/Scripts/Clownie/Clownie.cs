using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using static UnityEngine.GraphicsBuffer;

public class Clownie : MonoBehaviour
{

    List<Vector3> clownSpawnsPosses = new() { new(-80f, 30, 0), new (0,0,80f), new(70f, 0, 0) , new(0, 0, -50f) }; //Four Possible Positions where the clown could spawn

    [SerializeField] private GameObject Player; //We keep the Player here in order to track it
    [SerializeField] public float clownSpeed { get; private set; } = 3f;
    [SerializeField] public float clownDashSpeed { get; private set; } = 35f;

    public float clownLifetime = 15f; //Will be increased as the game gets harder
    public float clownLifetimeCount = 0f; //Will be increased as the game goes on, (after it reaches the clownlifetime it goes back to 0 and clown leaves the scene)
    public bool isClownDashing = false; //This determines if the clown is gonna throw itself in a torpedo like type of dash
    private bool isClownDashInProcess = false; //Determines if he is doing the dash (Avoiding multiple dashes at same time)s

    private bool canRotate = true;
    private float timeForDashingCount = 0f; // count for how long until it decides to try to dash in
    private float timeFordashingThreshold = 5f; // how long until it decides to try to dash in

    private bool hasSpookySound = false;
    private float timeUntilSpookySound = 0;


    Vector3 exitDestination;
    Vector3 SpawnPos;

    private float timeDashingPreparation = 0; //Time countup until it stops rotating itself and dashes, this is for when it has decided to dash

    void Start()
    {   
        exitDestination = clownSpawnsPosses[UnityEngine.Random.Range(0, clownSpawnsPosses.Count - 1)];
        SpawnPos = clownSpawnsPosses[UnityEngine.Random.Range(0, clownSpawnsPosses.Count-1)];
        transform.position = SpawnPos;
        
    }




    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {

            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            print("hit you!");
            Vector3 hitDirection = this.transform.forward;

            hitDirection += Vector3.up * 10f;

            playerRb.AddForce( hitDirection* 50f, ForceMode.Impulse);
            GameManager.isChickenAlive = false;
            //Add force to the enemy player 

            



        }

    }

    private void OnCollisionExit(Collision collision)
    {
        
    }




    // Update is called once per frame
    void Update()
    {

        
        if(GameManager.isClownHere)
        {
            

            if(canRotate) TrackRotation(Player.transform.position);

            if (!isClownDashing) {  //If not dashing, chase player

                ChaseTarget(Time.deltaTime, Player.transform.position); 
                timeForDashingCount += Time.deltaTime; //print("chasing");
            }


            ClownNoise();

            if (timeForDashingCount >= 5f) isClownDashing = true;


            if (isClownDashing)
            {
                
                timeDashingPreparation += Time.deltaTime; //Stops looking at the player while standing

                if(timeDashingPreparation >= 2f && !isClownDashInProcess) //Make sure it only dashes once
                {                    

                    Vector3 playerLastPos = Player.transform.position;
                    StartCoroutine(clownTorpedoDash(playerLastPos));
                }
            }

            this.clownLifetimeCount += Time.deltaTime;

            if(this.clownLifetimeCount >= clownLifetime && !this.isClownDashing && !this.isClownDashInProcess)
            {
                StopAllCoroutines();
                //print("Clownie is leaving the scene");
                GameManager.isClownHere = false;                
                this.clownLifetimeCount = 0;
            }
                


        } else
        {

            if (GameManager.ClownSpawnInstancesToDate >= 1)  //After its first apparition he chooses a new start point.
            {
                //print("go home");

                //Make it go somewhere else            
                if (exitDestination == null) exitDestination = clownSpawnsPosses[UnityEngine.Random.Range(0, clownSpawnsPosses.Count - 1)];
                
                
                TrackRotation(exitDestination);               
                ChaseTarget(Time.deltaTime * 10, exitDestination); //Move to exitDestination

            }
            

        }


        



    }//End of Update

    void ClownNoise()
    {


        if (timeUntilSpookySound >= 0 && (hasSpookySound = true))  //Reduce the timer until it is allowed to make a creepy noise
        {
            timeUntilSpookySound -= Time.deltaTime;
        }
        else
        {
            hasSpookySound = false;
        }

        if (!hasSpookySound)  //If the bool is false, it can make a noise, after it does, it assigns a down timer
        {
            //print("Playing Evil Clown Sound");

            FindObjectOfType<SAudioManager>().Play("EvilLaugh1"); //Randomize the laugh

            hasSpookySound = true;

            timeUntilSpookySound = 8f;
        }


    }

    void ClownieLifeTimeIncrease()
    {

    }

    void TrackRotation(Vector3 target)
    {
        if (Player == null) return;
        
            // Get the direction from this object to the target
        Vector3 direction = target - transform.position;

        // Make the object rotate to face the target


        if (direction.magnitude > Mathf.Epsilon)
        { 
            Quaternion rot = Quaternion.LookRotation(direction);
        
            transform.rotation = rot;
        }

        
    }

    void ChaseTarget(float time, Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, clownSpeed * time);
    }

    IEnumerator clownTorpedoDash(Vector3 destiny_pos)
    {
        
        isClownDashInProcess = true;
        canRotate = false;
        float counter = 0f;
        var curr = transform.position;

        while (Vector3.Distance( transform.position , destiny_pos) > .5f)
        {
            //print("dashing " + counter); 
            transform.position = Vector3.Lerp(curr, destiny_pos, counter/50f);
            counter += Time.deltaTime * clownDashSpeed;
            yield return null;
        }


        timeDashingPreparation = 0f;  //Returning everything to normal

        isClownDashing = GenericScripts.GetRandomBool(0.45f); //Made this random, we could make it dash more than once in a single sequence

        isClownDashInProcess = false;
        canRotate = true;
        timeForDashingCount = 0f;
        timeFordashingThreshold = UnityEngine.Random.Range(5, 13);  //Decide new random thresholds to make it feel dynamic


        yield return null;
    }


    

}//End Of Class
