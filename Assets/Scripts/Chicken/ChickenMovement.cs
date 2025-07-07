using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class ChickenMovement : MonoBehaviour
{
    

    Rigidbody rb;
    [SerializeField] private float frontRaycastRange;
    [SerializeField] private float isStandingRaycastRange;
    [SerializeField] private float jumpForce;
    [SerializeField] private float movSpeed = 4;
    [SerializeField] private float movSpeedMax = 7; // Max movement speed
    public float accelerationRate = 0.05f; // Rate of acceleration
    public float decelerationRate = 0.2f; // Rate of deceleration
    private float movTime = 0; 

    [SerializeField] private float hValue;
    [SerializeField] private float vValue;
    
    private float rotSpeed;
    private bool canMove;
    Quaternion originalRot;
    

    Vector3 inputDir = Vector3.zero;
    
    public int jumpCount; //number of times you can jump
    public float jumpCountRefreshRate; //JumpCount Refresh rate
    public float jumpCountRefreshRateCount; //JumpCount Refresh rate count, use it with delta time

    private bool isCollidingWithWall;
    private bool isItJumping;
    private bool canJump;    
    private bool hasJumped;
    private bool isItStanding;    

    public GameObject floorUnderneathNotSafe { get; private set; }


    private ChickenTouch chickenInputSystem;
    private InputAction inputActionChickenMovement;
    private InputAction inputActionJump;    

    // Start is called before the first frame update

    private void Awake()
    {
        chickenInputSystem = new ChickenTouch();


    }


    private void OnEnable()
    {
        inputActionChickenMovement = chickenInputSystem.Player.Move;
        inputActionChickenMovement.Enable();

        inputActionJump = chickenInputSystem.Player.Jump;   
        inputActionJump.Enable();

        inputActionJump.performed += ctx => JumpChickenGeneral();
        //inputActionJump.performed += ctx => print("beniscock");
    }

    private void OnDisable()
    {
        inputActionChickenMovement.Disable();
        inputActionJump.Disable();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpForce = 7;                        
        isItJumping = false;
        canJump = true;
        frontRaycastRange = 4;
        isStandingRaycastRange = 2.8f;
        
        rotSpeed = 6;
        canMove = true;
        originalRot = rb.rotation;

        jumpCount = 2;
        jumpCountRefreshRate = 6;
        jumpCountRefreshRateCount = 0;
        
        Quaternion upRot = Quaternion.FromToRotation(transform.up, Vector3.up);

    }


    private void Update(){

        FloorUnderneathNotSafe();
        //if(floorUnderneathNotSafe != null) print("The position of the tile underneath you rn: " + floorUnderneathNotSafe.transform.position);
        //print("Scale "+Time.timeScale); 

        //Debug.Log("Jump count: " + jumpCount);
        //print("can move: " + canMove);

        //hValue = Input.GetAxis("Horizontal");
        //vValue = -1* Input.GetAxis("Vertical");
        //inputDir = new Vector3(vValue, 0, hValue);        

        Vector2 chickenVector2d = inputActionChickenMovement.ReadValue<Vector2>();
        inputDir = new Vector3(-1 *  chickenVector2d.y, 0,  chickenVector2d.x);
        

        


        if (chickenVector2d.magnitude > 0)                        //Lerps the chicken speed the longer it moves
        {
            movTime += Time.deltaTime;
            movSpeed = Mathf.Lerp(movSpeed, movSpeedMax, accelerationRate * Time.deltaTime);
        }
        else
        {
            movTime = 0f;

            if(movSpeed > 10f) movSpeed = Mathf.Lerp(movSpeed, 1f, decelerationRate * Time.deltaTime); else movSpeed = 10f;

        }


        IsItStanding();

        ChickenJumpCountCoolDown(Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space)) JumpChickenGeneral();




    }

    void FixedUpdate()
    {

        

        MoveChicken(inputDir);


        //Makes the chicken stand
        //
        //
        float yRotation = rb.rotation.eulerAngles.y;


        // Interpolate the rotation for smooth movement, adjust the speed as needed
        if (rb.velocity.magnitude == 0 && GameManager.isChickenAlive && isItStanding)
        {
         
            Quaternion newRotation = Quaternion.Euler(originalRot.eulerAngles.x, yRotation, originalRot.eulerAngles.z);
            float rotationSpeed = 5f;
            rb.rotation = Quaternion.Slerp(rb.rotation, newRotation, Time.deltaTime * rotationSpeed);

        }


        //RagdollToStanding(); //Might implement it, is a static chicken really a good thing?




    }

    //Custom methods
    //
    //
    //

    private void RagdollToStanding()
    {
        if (isItStanding)  //Freeze the rigidbody rotation constraints only if it has touch ground and is at a standing position or not too far from (0, any, 0)
        {
            if ((Mathf.Abs(transform.rotation.eulerAngles.x) < 0.05f) && (Mathf.Abs(transform.rotation.eulerAngles.z) < 0.05f))
            {

                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void ChickenJumpCountCoolDown(float time)
    {
        if (this.jumpCount < 2)
        {
            if (this.jumpCountRefreshRateCount < this.jumpCountRefreshRate) {

                this.jumpCountRefreshRateCount += time;




                
            }
            else
            {
                jumpCount += 1;

                this.jumpCountRefreshRateCount = 0;

                var randomRangeStart = Random.Range(9, 15);
                jumpCountRefreshRate = Random.Range(randomRangeStart, 30);

            }




        }
    }

    public void MoveChicken(Vector3 inputDir) {
       
        if (inputDir != Vector3.zero)
        {
            
            if(this.canMove) rb.MovePosition(transform.position + (movSpeed * Time.deltaTime * inputDir));


            transform.forward = inputDir;
            Quaternion toRotation = Quaternion.LookRotation(inputDir, Vector3.up);                        
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotSpeed); //Tried rb.MoveRotation, aint that good


            //Write code that balances the chicken -- R/ Or just make it kinematic and toggle the ragdoll when dead or bouncing


        }

        //Ray ray = new Ray(transform.position, transform.TransformDirection(transform.forward * raycastRange));
        //Debug.DrawRay(transform.position, transform.TransformDirection(transform.forward * raycastRange));

        Ray ray = new(transform.position, transform.forward * frontRaycastRange);
        Debug.DrawRay(transform.position, transform.forward * frontRaycastRange);

        if (Physics.Raycast(ray, out RaycastHit hit, isStandingRaycastRange))
        {
            if (hit.collider.CompareTag("Enviroment"))
            {

                this.canMove = false;

            }

        }
        else this.canMove = true;


    }
    public void JumpChickenGeneral() //Changed the jump logic, now jumops shall be limited, they wont refresh until after some time.
    {
        if (GameManager.isChickenAlive)
        {



            if (jumpCount <= 0) canJump = false;

            if (canJump)
            {

            
                isItJumping = true;
            
                JumpChicken();
                this.jumpCount -= 1;


            }
        }

    }
    public void JumpChicken()
    {
        if (jumpCount > 0)
        {
                
                rb.velocity = Vector3.up * jumpForce;

                rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);

                print("Jumped!, count: " + jumpCount);
        }
    }

    private IEnumerator JumpCoolDown()
    {
        yield return new WaitForSeconds(.3f);
        isItJumping = false;
         //Here, the cooldown shall be random, change it on the update
        canJump = true;
    }



private void IsItStanding()
    {
        Vector3 rayDirection = Vector3.down; //Making relative to the entity's transform? Not absolutely downwards?

        Ray ray1 = new(transform.position, rayDirection  * isStandingRaycastRange);//middle
        Debug.DrawRay(transform.position, rayDirection * isStandingRaycastRange);

        Vector3 rayDirection2 = Quaternion.Euler(0, 0, -45) * rayDirection; //backray
        Ray ray2 = new(transform.position, rayDirection2 * isStandingRaycastRange);
        Debug.DrawRay(transform.position, rayDirection2 * isStandingRaycastRange);

        Vector3 rayDirection3 = Quaternion.Euler(0, 0, 45) * rayDirection;  //frontray
        Ray ray3 = new (transform.position, rayDirection3 * isStandingRaycastRange);
        Debug.DrawRay(transform.position, rayDirection3 * isStandingRaycastRange);


        if (Physics.Raycast(ray1, out RaycastHit hit, isStandingRaycastRange) || Physics.Raycast(ray2, out RaycastHit hit2, isStandingRaycastRange) || Physics.Raycast(ray3, out RaycastHit hit3, isStandingRaycastRange))
        {

            
            isItStanding = true;
            //rb.useGravity = false; //This and the other one in order to avoid the chicken from falling down randomly when hit by something MIGHT-RM


            StartCoroutine(nameof(JumpCoolDown));

            

        } else
        {

            //print("in the air");
            //rb.useGravity = true; //Toggles when actually fallin MIGHT-RM
            isItStanding = false;
            

        }

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "DeadZone" || collider.name == "DeadZone2") GameManager.isChickenAlive = false;         
    }

    
    void FloorUnderneathNotSafe()
    {
        Vector3 rayDirection = Vector3.down;
        Ray ray = new(transform.position, rayDirection * isStandingRaycastRange * 6f);

        if (Physics.Raycast(ray, out RaycastHit hit, isStandingRaycastRange)){

            this.floorUnderneathNotSafe = hit.collider.gameObject; //We assign it to the variable, it will be used by the game manager in a random fashion in order to increase the difficutly.

        }
    }
}  //End of class



