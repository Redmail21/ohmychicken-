using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class scriptCube : MonoBehaviour
{

    GameObject directionsContainer;  //We access this in order to use the direction sprites
    private int laps = 0;

    public enum Level
    {
        UP, OG, DOWN, BAIT
    }

    public Vector3 og_pos; //{ get; private set; }
    Vector3 down_pos;
    Vector3 up_pos;

    Renderer gm_rend;
    public float speed = 100;
    public float smoothing = .10f;
    public float waitTime { get; private set; } = 0;
    public float expectedTimeToUp { get; private set; } = 7;

    public bool isVanished { get; private set; } = false;
    public bool isVanishing { get; private set; } = false;
    private Level level = Level.OG;

    private void Awake()
    {
        gm_rend = GetComponent<Renderer>();
        directionsContainer = transform.GetChild(0).gameObject;
    }
    void Start()
    {           
        Init_pos(gameObject.transform.position);        
    }

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {        
        //printInfo();
    }

    private void Update()
    {
        if(this.isVanished == true && this.isVanishing == false)
        {
            
            this.waitTime += Time.deltaTime;
            if(this.waitTime >= expectedTimeToUp)  //If the time waiting while vanished surpasses the expected time, it goes back to the og position
            {                                        
                ToggleVanish();                                
            }
        }
       
    }

    public void ToggleVanish()
    {

        
        if(this.isVanished == false) //If not Vanished
        {
            
            this.isVanished = true;  //Set all the further stuff needed
            this.isVanishing = true;
            StartCoroutine(FlickerDangerMove());


        }                 
        else
        {
            
            StopAllCoroutines();
            StartCoroutine(Move(Level.OG));
            
            

        }




    }

    public IEnumerator Move(Level val)
    {
        //EXPERIMENTAL use ENUMS                                 
        float total = 1000;
        var curr = transform.position;
        Vector3 destiny_pos;
        waitTime = 0;

        if (val == Level.UP || val == Level.DOWN)
        {

            
            if (val == Level.UP) destiny_pos = up_pos; else destiny_pos = down_pos;

            float t = 0;

            while (Vector3.Distance(destiny_pos, transform.position) > .5f)
            {

                transform.position = Vector3.Lerp(transform.position, destiny_pos, t/total);

                t += Time.deltaTime * speed;


                if (Vector3.Distance(destiny_pos, transform.position) < .5f)
                {
                    this.isVanishing = false;
                    transform.position = destiny_pos;
                    this.level = val;
                    this.isVanished = true;
                    laps++;
                    yield break;
                }

                yield return null;


            }            

        }
        else if (val == Level.OG)
        {
            
            this.isVanishing = true;             
            float t = 0;
            
            
            while (Vector3.Distance(og_pos, curr) > .5f)
            {                
                transform.position = Vector3.Lerp(transform.position, og_pos, t / total); //changed from transform.position
                t += Time.deltaTime * speed;                



                if (Vector3.Distance(og_pos, transform.position) <= .5f)
                {
                    this.level = Level.OG;
                    ColourToggler();
                    this.isVanishing = false;
                    
                    this.isVanished = false;
                    transform.position = og_pos;
                    laps++;
                    yield break;
                }

                yield return null;
            }

            
        } else if (val == Level.BAIT)
        {
            float t = 0;
            var randomDestiny = UnityEngine.Random.Range(0, 1);
            this.level = Level.BAIT;

            if (randomDestiny == 0) destiny_pos = up_pos; else destiny_pos = down_pos;

            this.waitTime = expectedTimeToUp - 1f;  //Little time until it goes back to Level.OG

            while (Vector3.Distance(destiny_pos, transform.position) > .5f)
            {
                transform.position = Vector3.Lerp(curr, destiny_pos, t / total); //changed from transform.position
                t += Time.deltaTime * speed * 50;                

                yield return null;
            }



        } 


        



    }


    private void Init_pos(Vector3 argpos)
    {
        this.og_pos = argpos;
        this.down_pos = new(argpos.x, -200, argpos.z);
        this.up_pos = new(argpos.x, 200, argpos.z);
    }

    

    private IEnumerator FlickerDangerMove()  //Changed it from colours to working with the sprites that tell its direction
    {
        GameObject spriteDirectionFlicker = null;

        Level localDirection = RandomLevelEnum();


        switch (localDirection)
        {
            case Level.UP:
                //Debug.Log("Case: UP");
                spriteDirectionFlicker = this.directionsContainer.transform.Find("up").gameObject;
                //spriteDirectionFlicker = this.directionsContainer.transform.GetChild(0).gameObject;
                break;

            case Level.DOWN:
                //Debug.Log("Case: DOWN");
                spriteDirectionFlicker = this.directionsContainer.transform.Find("down").gameObject;
                //spriteDirectionFlicker = this.directionsContainer.transform.GetChild(1).gameObject;
                break;

            case Level.BAIT:
                //Debug.Log("Case: BAIT");
                spriteDirectionFlicker = this.directionsContainer.transform.Find("random").gameObject;
                //spriteDirectionFlicker = this.directionsContainer.transform.GetChild(2).gameObject;
                break;


            default:
                Debug.Log("Case: Default");
                break;


        }

        

        expectedTimeToUp = UnityEngine.Random.Range(1f, 2f); //Originally max 6
        int cond = 0;

        while ( cond < 3)
        {

            //gm_rend.material.SetColor("_Color", Color.yellow); Changed into turning on the sprite
            if(spriteDirectionFlicker!=null) spriteDirectionFlicker.SetActive(true);


            yield return new WaitForSeconds(0.3f);
            //gm_rend.material.SetColor("_Color", Color.white); Changed into turning off the sprite
            if (spriteDirectionFlicker != null) spriteDirectionFlicker.SetActive(false);

            yield return new WaitForSeconds(0.3f);
            cond++;
            yield return null;

        }


        if (this.isVanished) gm_rend.material.SetColor("_Color", Color.red); else gm_rend.material.SetColor("_Color", Color.green);



       

        StartCoroutine(Move(localDirection));

        yield break;

    }



    Level RandomLevelEnum()
    {
        Array values = Enum.GetValues(typeof(Level));
        System.Random random = new System.Random();
        Level randomFinal = (Level)values.GetValue(random.Next(values.Length));

        if(randomFinal != Level.OG)
        {
            return randomFinal;

        } else
        {
            return RandomLevelEnum();
        }

        
    }

    void ColourToggler()
    {
        switch (this.level)
        {
            case Level.OG:
                this.gm_rend.material.SetColor("_Color", Color.white);
                break;
            case Level.UP:
                this.gm_rend.material.SetColor("_Color", Color.red);
                break;
            case Level.DOWN:
                this.gm_rend.material.SetColor("_Color", Color.red);
                break;
            case Level.BAIT:
                this.gm_rend.material.SetColor("_Color", Color.red);
                break;


            default:
                break;
        }
    }

    void printInfo()
    {
        print("IsVanished, " + this.isVanished);
        print("IsVanishing, " + this.isVanishing);                              
        print("Level, " + this.level);
        print("waitTime, " + this.waitTime);
    }

}//End of Class
