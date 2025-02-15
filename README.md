«Astana IT University»
 
 
 
 
 
 
“60 seconds”
Introduction to Game Development | Kairbayev Ernar
 
 
  
 
 
Students: Mukhanov Daniil
14.02.2025





Introduction	3
Movement realization	3
Creating a player and assigning movement keys	3
Landscape creation	3
Scripts folder creation	4
Mouse movement creation	8
Interactions	8
Interaction Events	16
Simple HUD	19
Updated landscape and Building Enemy Logic	26
Attack logic for Enemy	30

 
 Introduction
This is a 3D survival game where players must escape from a weaponized robotic monster. The goal is to survive for 60 seconds without losing all your health—otherwise, you perish. The game features interactive elements and an immersive soundtrack, enhancing the intense and thrilling atmosphere.
 Movement realization
Creating a player and assigning movement keys

First of all, I have to create a new 3d object that is a sphere that will serve as a player. Then we gonna set a camera position straight to the top of the sphere that will illustrate a first-person view. Also, we need to create a new folder called Prefabs. Navigating to window section on the top we click on Package Manager and import the Input System for future actions.
After we will create a folder input and PlayerInput using inputActions. Heading to PlayerInput there will be opened an input actions editor. In the Action Maps section, we click on “+” that creates a new action map “OnFoot” that means in this state exactly being on foot you can do next list of actions that we will set here: 
-create 1 action movement with composite setting up/down/right/left. 
-assign to each action a key as a default: WASD
-create 2 action jump and assign a space keyboard button
Save asset and mark Generate C# class  apply

Landscape creation
Create a 3d object Cube called Floor setting (0, -1.2,0) position settings and scale settings (20,0.1,20) by X Y Z coordinates. Additionally create couple of cubes and place them randomly.
After what you will get sort of this result:
![image](https://github.com/user-attachments/assets/fb591576-5f5d-48c7-9f10-192d0d38e0f2)


 
Scripts folder creation
Heading back to the assets create a folder for Scripts. Inside of it create a new C# script called InputManager after what open it and copy paste and save this code: 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    // private PlayerMotor motor;
    // private PlayerLook look;
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        // motor = GetComponent<PlayerMotor>();
        // look = GetComponent<PlayerLook>();

        // onFoot.Jump.performed += ctx => motor.Jump();
    }

    void FixedUpdate()
    {
        // motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        // look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}
Pull out the Prefabs folder out of Scenes back to Assets. Next to the first script create another one called PlayerMotor that will contain all player movement functionality:
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    // private bool isGrounded;
    public float speed = 5f;
    // public float gravity = -9.8f;
    // public float jumpHeight = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        // isGrounded = controller.isGrounded;
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        // playerVelocity.y += gravity * Time.deltaTime;
        // if (isGrounded && playerVelocity.y < 0)
        //     playerVelocity.y = -2f;

        // controller.Move(playerVelocity * Time.deltaTime);
        // Debug.Log(playerVelocity.y);

    }

    // public void Jump ()
    // {
    //     if (isGrounded)
    //     {
    //         playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    //     }
    // }

}

Get back to the InputManager and uncomment:
 motor = GetComponent<PlayerMotor>();
 private PlayerMotor motor;
 motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());


In the Unity open your Player and drag there these two scripts to add components. Then by opening Input Debugger click Keyboard and run the scene. You will see that you can move and see keys that you pressing on that means everything is ok.
![image](https://github.com/user-attachments/assets/e1a5beb7-fa1f-4a5b-a230-ad7c18815449)

 

Get back to the PlayerMotor uncomment:
private bool isGrounded;
public float gravity = -9.8f;
playerVelocity.y += gravity * Time.deltaTime;
controller.Move(playerVelocity * Time.deltaTime);
Debug.Log(playerVelocity.y);
isGrounded = controller.isGrounded;
if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
public float jumpHeight = 3f;

 public void Jump ()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

Uncomment in InputManager:
onFoot.Jump.performed += ctx => motor.Jump();

Mouse movement creation
Open PlayerInput and add action to the second section under Jump. Set value action type, choose Vector 2 and assign delta mouse  save assets. In scripts folder create another C# script PlayerLook and drag it to player. Copy paste and save this code:
 using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;

    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //rotate player tp look  left/right
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);

    }
}


Back to InputManager uncomment next code:
private PlayerLook look;
look = GetComponent<PlayerLook>();
look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
and set to cam section the main camera from Player.

Interactions

Let’s start with a new C# script in our scripts folder called Interactable: 
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //message displayed to player when looking at an interactable subject
    public string promptMessage;
    public void BaseInteract()
    {
        Interact();
    }
    protected virtual void Interact()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

Then create another C# script PlayerInteract which will contain all of the logic to detect interactables:
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
    
    void Start()
    {
        cam = GetComponent<PlayerLook>()?.cam;
       
    }

    void Update()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawLine(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
           
        }
    }
}


I decided to choose a ray logic to interact and on this picture you can notice a ray going out of the center of camera:
![image](https://github.com/user-attachments/assets/1aac97fd-363e-402f-8213-1c981b76a469)

 
Then we create a new layer specifically for our interactables: go to inspector section a bit below you will see Layer  Add Layer… Name it as Interactable. Then you have to create a new 3d object Keypad that will serve as interactable button and set its layer as Interactable. After that set Mask in PlayerInteract section among the components on the right side to Interactable as well. Add to PlayerInteract script inside if section:
if(hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Debug.Log(hitInfo.collider.GetComponent<Interactable>().promptMessage);
            }

Then in scripts section create a folder Interactables and C# script Keypad. Drag Keypad to Keypad 3d Object and drag this script to folder Interactables. Copy and paste this code to Keypad: 
using UnityEngine;

public class Keypad : Interactable
{

    private GameObject door;
    private bool doorOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {
        Debug.Log("Interacted with "+ gameObject.name);
    }
}

Then run scene and go to this keypad and aim on it. In the console you will see message that means everything works correctly.
![image](https://github.com/user-attachments/assets/eec57477-91af-48aa-913e-3c4732d5794d)

 

Next thing we will do is adding Text mesh pro by clicking + on the top left corner and rename it to PromptText. Then we click on event system  defaultInputActions  copy UI section and paste it to our InputActionsEditor and save. In addition, I created a crosshair in Canvas section and placed it on the center with dark green color. After we created a Player folder where we can hold all player…. Scripts what we did. Create there a PlayerUI C# script and drag it to Player.
Save this code for PlayerUI:
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI promptText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }
}


Then update your PlayerInteract:
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
    private PlayerUI playerUI;
    
    void Start()
    {
        cam = GetComponent<PlayerLook>()?.cam;
        playerUI = GetComponent<PlayerUI>();
       
    }

    void Update()
    {
        playerUI.UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawLine(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if(hitInfo.collider.GetComponent<Interactable>() != null)
            {
                playerUI.UpdateText(hitInfo.collider.GetComponent<Interactable>().promptMessage);
            }
        }

           
        }
    }

Drag you PromptText to PlayerUI components section and running the scene you have to aim to your Keypad object and see the Text: ”Use Keypad”:
![image](https://github.com/user-attachments/assets/c0985e69-b6ca-440b-be55-5c6317302e69)

 

After add “E” button for interact action and change PlayerInteract code:
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
    private PlayerUI playerUI;
    private InputManager inputManager;
    
    void Start()
    {
        cam = GetComponent<PlayerLook>()?.cam;
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
       
    }

    void Update()
    {
        playerUI.UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawLine(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if(hitInfo.collider.GetComponent<Interactable>()!=null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                if (inputManager.OnFoot.Interact.triggered){
                    interactable.BaseInteract();
                }
            }
        }

           
        }
    }

Then we modify Keypad:
using UnityEngine;

public class Keypad : Interactable
{

    [SerializeField]
    private GameObject door;
    private bool doorOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {
        doorOpen = !doorOpen;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
    }
}


After we created animation using animator and 2 states of doors which are Closed and Opened:
![image](https://github.com/user-attachments/assets/0acf990e-a901-42af-9721-e8024c7a4575)
![image](https://github.com/user-attachments/assets/4f8163d6-6c1b-4647-8cfb-8dd35d163d75)


   

Interaction Events
In the scripts create a new C# file called InteractionEvent:
using UnityEngine.Events;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    public UnityEvent OnInteract;
    
}


Make changes in Interactable script: 

using UnityEditor;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool useEvents;
    [SerializeField]
    public string promptMessage;

    public virtual string OnLook()
    {
        return promptMessage;
    }

    public void BaseInteract()
    {
        Interact();
    }
    protected virtual void Interact()
    {

    }
}    

Create Folder Editor and inside of it another script InteractableEditor:
using UnityEditor;

[CustomEditor(typeof(Interactable),true)]

public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Interactable interactable = (Interactable)target;
        base.OnInspectorGUI();
        if(interactable.useEvents)
        {
            if(interactable.GetComponent<InteractionEvent>() == null)
            interactable.gameObject.AddComponent<InteractionEvent>();
        }
        else
        {
            if(interactable.GetComponent<InteractionEvent>() != null)
            DestroyImmediate(interactable.GetComponent<InteractionEvent>());

        }
    }
}


Change InteractionEvent like here: 
using UnityEditor;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool useEvents;
    [SerializeField]
    public string promptMessage;

    public virtual string OnLook()
    {
        return promptMessage;
    }

    public void BaseInteract()
    {
        if(useEvents)
        GetComponent<InteractionEvent>().OnInteract.Invoke();
        Interact();
    }
    protected virtual void Interact()
    {

    }
}    


When we use Keypad so the half of the door gets material to be changed to Gold:
![image](https://github.com/user-attachments/assets/829a421b-b4f7-47f2-bb00-febe7d83c79d)

 

Edit InteractableEditor:
using UnityEditor;

[CustomEditor(typeof(Interactable),true)]

public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Interactable interactable = (Interactable)target;
        if(target.GetType()== typeof (EventOnlyInteractable))
        {
            interactable.promptMessage = EditorGUILayout.TextField("Prompt Message", interactable.promptMessage);
            EditorGUILayout.HelpBox("EventOnlyInteract can ONLY use UnityEvents.", MessageType.Info);
            if(interactable.GetComponent<InteractionEvent>() == null){
                interactable.useEvents = true;
                interactable.gameObject.AddComponent<InteractionEvent>(); 
            }

        }
        base.OnInspectorGUI();
        if(interactable.useEvents)
        {
            if(interactable.GetComponent<InteractionEvent>() == null)
            interactable.gameObject.AddComponent<InteractionEvent>();
        }
        else
        {
            if(interactable.GetComponent<InteractionEvent>() != null)
            DestroyImmediate(interactable.GetComponent<InteractionEvent>());

        }
    }
}



Simple HUD
First of all, download a zip file heading to this link: https://www.youtube.com/redirect?event=video_description&redir_token=QUFFLUhqa25mVTVaQkFBM0ZTWmdEbVE4bG93b2I0VWlYZ3xBQ3Jtc0ttX0JuU0ppVHdwQkFEclZmR2JIM1d1R0dYWllBWWdJNVR2RlQ1azU5dzRTQzlIY2F0dndpMlFVenlYRWMyUHJaeXV0YXZfVnFXYl8wNWVEMzVVZmZzUzllR194UldZa2haaDhRTi1jM1NxS0xDZ3hEdw&q=http%3A%2F%2Fwww.mediafire.com%2Ffile%2Fcl63d6ydeyp8ii5%2FHealthBarAssets.zip%2Ffile&v=CFASjEuhyf4.  It will contain all needed HUD elements for a future HealthBar. Extract zip file and drag it all to Assets
 ![image](https://github.com/user-attachments/assets/0e3aba12-3050-469a-87a5-5358c43264ca)

Set up each Texture Type to be Sprite 2D and UI and change Sprite mode to Single. All these things will allows us to apply the HUD elements for the HealthBar. Here is what it will look like:
![image](https://github.com/user-attachments/assets/2f7ccf7c-e6e7-4091-a222-ff20d275c891)

 

Create a C# script called PlayerHealth and paste this code:
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    public float maxHealth = 100;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;

    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health,0,maxHealth);
        UpdateHealthUI();
        if(Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(Random.Range(5,10));
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            RestoreHealth(Random.Range(5,10));
        }
    }

    public void UpdateHealthUI()
    {
        Debug.Log(health);
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health/maxHealth;
        if(fillB>hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer/chipSpeed;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);

        }
        if(fillF<hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer/chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f; 

    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f; 

    }


}


Running the scene, you will get ability to take damage and restore health by pressing A and S.
![image](https://github.com/user-attachments/assets/29fffc00-5722-4d65-8040-60c77d9b590c)
![image](https://github.com/user-attachments/assets/3bcfff4e-110b-417a-ae47-1dcf4315723f)


 
 

Update player health;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
 
    private float health;
    private float lerpTimer;
    [Header("Health Bar")]
    public float maxHealth = 100;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public TextMeshProUGUI healthText;
    [Header("Damage Overlay")]
    public Image overlay;
    public float duration;
    public float fadeSpeed;
    private float durationTimer;
    void Start()
    {
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }
   
    void Update()
    {
        health = Mathf.Clamp(health,0,maxHealth);
        UpdateHealthUI();
        if(overlay.color.a>0)

        if(health<30)
        {
            return;
        }
        {
            durationTimer += Time.deltaTime;
            if(durationTimer>duration)
            {
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
       
    }

    public void UpdateHealthUI()
    {
        Debug.Log(health);
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health/maxHealth;
        if(fillB>hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer/chipSpeed;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);

        }
        if(fillF<hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer/chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
         healthText.text = health +"/"+maxHealth;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f; 
        durationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);

    }
    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f; 
    }


}


And we get damage effect:
![image](https://github.com/user-attachments/assets/cf91920e-b4b0-480d-8b88-2c4ecaf95bd7)

 

Updated landscape and Building Enemy Logic
I have created a new room, an enemy on the center, columns around and painted all this stuff to not be bored:
![image](https://github.com/user-attachments/assets/01094c82-add2-435e-8019-58309fbd163d)

 
 Create enemy folder in scripts  states folder  C# script BaseState:

public abstract class BaseState
{
    public Enemy enemy;
    public StateMachine stateMachine;

    public abstract void Enter();

    public abstract void Perform();

    public abstract void Exit();

}



Then back and StateMachine C# script:
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState activeState;
    public PatrolState patrolState;

    public void Initialise()
    {
        patrolState = new PatrolState();
        ChangeState(patrolState);

    }

    void Update()
    {
        if(activeState != null)
        {
            activeState.Perform(); 
        }
    }

    public void ChangeState(BaseState newState)
    {
        if(activeState != null)
        {
            activeState.Exit();
        }
        activeState = newState;
        if(activeState != null)
        {
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();
        }
    }
}


Path script:
using System.Collections.Generic;

using UnityEngine;

public class Path : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}



Enemy script:
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }

    [SerializeField]
    private string currentState;
    public Path path;

    
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialise();
    }
    void Update()
    {
        
    }
}

PatrolState script:
using UnityEngine;

public class PatrolState : BaseState
{
    public int waypointIndex;
    public override void Enter()
    {
    }
    public override void Perform()
    {
        PatrolCycle();

    }
    public override void Exit()
    {

    }
    public void PatrolCycle()
    {
        if(enemy.Agent.remainingDistance < 0.2f)
        {
            if(waypointIndex < enemy.path.waypoints.Count - 1)
            waypointIndex++;
            else
            waypointIndex = 0;
            enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
        }
    }
}


In combination all it gives us a result of movement of the enemy
![image](https://github.com/user-attachments/assets/4fc0d886-e64f-4cd1-9f55-d2c3f3e6bc7d)


 


Attack logic for Enemy
In this section I will show how I realized another state for enemy. 
Update all these scripts:
StateMachine:
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState activeState;
    

    public void Initialise()
    {
        
        ChangeState(new PatrolState());

    }

    void Update()
    {
        if(activeState != null)
        {
            activeState.Perform(); 
        }
    }

    public void ChangeState(BaseState newState)
    {
        if(activeState != null)
        {
            activeState.Exit();
        }
        activeState = newState;
        if(activeState != null)
        {
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();
        }
    }
}



Enemy:
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;
    public NavMeshAgent Agent { get => agent; }
    public GameObject Player {get => player;}

    public Path path;
    [Header("Sight Values")]

    
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight; 
    [Header("Weapon Values")]
    public Transform gunBarrel;
    [Range(0.1f, 10)]
    public float fireRate;

    [SerializeField]
    private string currentState;

 
    
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialise();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
    }
    public bool CanSeePlayer()
    {
        if(player != null)
        {
            if(Vector3.Distance(transform.position, player.transform.position)<sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if(angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
                {
                    
                    Ray ray = new Ray(transform.position + Vector3.up * eyeHeight, targetDirection);
                    RaycastHit hitInfo = new RaycastHit();
                    if(Physics.Raycast(ray,out hitInfo, sightDistance))
                    {
                        if(hitInfo.transform.gameObject == player)
                        {
                            return true;    
                        }
                    }
                    Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                }
            }
        }
        return false; 
    }
}


AttackState:
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float shorTimer;

    public override void Enter()
    {
        
    }
    public override void Exit()
    {
        
    }
    public override void Perform()
    {
       if( enemy.CanSeePlayer())
       {
        losePlayerTimer = 0;
        moveTimer += Time.deltaTime;
        shorTimer+= Time.deltaTime;
        enemy.transform.LookAt(enemy.Player.transform);

        if(shorTimer > enemy.fireRate)
        {
            Shoot();
        }

        if(moveTimer > Random.Range(3,7))
        {
            enemy.Agent.SetDestination(enemy.transform.position +(Random.insideUnitSphere * 5));
            moveTimer = 0;
        }
       }
       else
       {
        losePlayerTimer += Time.deltaTime;
        if(losePlayerTimer >8)
        {
            stateMachine.ChangeState(new PatrolState());
        }
       }
    }
    public void Shoot(){
        

        Transform gunbarrel = enemy.gunBarrel;

        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, gunbarrel.position, enemy.transform.rotation);
        Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.transform.position).normalized;
        bullet.GetComponent<Rigidbody>().linearVelocity = Quaternion.AngleAxis(Random.Range(-3f,3f),Vector3.up)*shootDirection * 40;
        Debug.Log("Shoot");
        shorTimer = 0;
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

Bullet:
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision){
        Transform hitTransform = collision.transform;
        if(hitTransform.CompareTag("Player"))
        {
            Debug.Log("Hit player");
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(10);
        }
        Destroy(gameObject);

    }
}

Path:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Path : MonoBehaviour
{

    public List<Transform> waypoints;
    [SerializeField]
    private bool alwaysDrawPath;
    [SerializeField]
    private bool drawAsLoop;
    [SerializeField]
    private bool drawNumbers;
    public Color debugColour = Color.white;

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (alwaysDrawPath)
        {
            DrawPath();
        }
    }
    public void DrawPath()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 30;
            labelStyle.normal.textColor = debugColour;
            if (drawNumbers)
                Handles.Label(waypoints[i].position, i.ToString(), labelStyle);
            //Draw Lines Between Points.
            if (i >= 1)
            {
                Gizmos.color = debugColour;
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);

                if (drawAsLoop)
                    Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);

            }
        }
    }
    public void OnDrawGizmosSelected()
    {
        if (alwaysDrawPath)
            return;
        else
            DrawPath();
    }
#endif
}

Patrol State:
using UnityEngine;

public class PatrolState : BaseState
{
    public int waypointIndex;
    public override void Enter()
    {
    }
    public override void Perform()
    {
        PatrolCycle();
        if(enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new AttackState());
        }

    }
    public override void Exit()
    {

    }
    public void PatrolCycle()
    {
        if(enemy.Agent.remainingDistance < 0.2f)
        {
            if(waypointIndex < enemy.path.waypoints.Count - 1)
            waypointIndex++;
            else
            waypointIndex = 0;
            enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
        }
    }
}

PlayerHealth: using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;

    [Header("Health Bar")]
    public float maxHealth = 100;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public TextMeshProUGUI healthText;

    [Header("Damage Overlay")]
    public Image overlay;
    public float duration;
    public float fadeSpeed;
    private float durationTimer;

    void Start()
    {
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        if (health <= 0)
        {
            GameOver();
        }

        if (overlay.color.a > 0)
        {
            if (health < 30)
            {
                return;
            }
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }

    public void UpdateHealthUI()
    {
        Debug.Log(health);
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        
        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }

        healthText.text = health + "/" + maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    void GameOver()
    {
        Debug.Log("You DIED");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;  // Остановка в редакторе
        #else
            Application.Quit(); // Закрытие в билде
        #endif
    }
}


Resposobilities
I, Mukhanov Daniil, have independently developed this entire project in Unity. I was responsible for all aspects of the game, including coding, level design, HUD creation, enemy implementation, and overall game logic. Every element of the project was built solely by me.
Results and demo
Gameplay screen with the fact of getting damage
 ![image](https://github.com/user-attachments/assets/7a0de235-e7f8-428c-8196-f7a8892db046)

Another screenshot demonstrating, we can move on the map
![image](https://github.com/user-attachments/assets/06d1982c-c990-494f-89c5-f997d4e75738)

 

Animation of the door by pressing button
![image](https://github.com/user-attachments/assets/1a4524ee-3b4f-4341-b50a-e932a0ed5a48)

 
Link to video demo:
https://youtu.be/B-guIRqBorg?si=1OqnGIkMNuqW0fdX


Future plans
Variety of Levels
•	Add new locations: forest, desert, abandoned city.
•	Change the design of maps and obstacles in each level.
•	Introduce new enemies with different tactics.
Increasing Gameplay Difficulty
•	With each new level, enemies become stronger and smarter.
•	The player has limited time to complete the game.
•	Add new mechanics: traps, puzzles, hidden zones.
References
That material helped me to build this project:
https://youtu.be/rJqP5EesxLk?si=_vSnooBEJS97APFJ
https://www.youtube.com/redirect?event=video_description&redir_token=QUFFLUhqbDM0a3NjRWNpZXlSUGx3SmxzRnF0VVZBM3hOUXxBQ3Jtc0trOU1WRzdHTHctUEFQbmd4NVVrZHREUHR2Y3pCMXFGd2NHVE1UbDJnbWxsdWxrMjBtZzF3empxN01LM2dJZHk4TkdLanNNR0JtNWVQRWlKZzBuWU1QNWJzaUNPYXEtR3JwbTh0ZkpIclF3cmJOY0Faaw&q=https%3A%2F%2Fwww.mediafire.com%2Ffile%2Fgth5615w2gccj48%2FAssaultRifle.fbx%2Ffile&v=wNdi5hc0anY
https://www.youtube.com/redirect?event=video_description&redir_token=QUFFLUhqa1pSekY5Q0MtQ2IxV0s5dElwcXQ3aVg2NUx2d3xBQ3Jtc0tuSnRZRVFYbkVMaVNCTkV4X28tWkJ0aEQ0Z0xSWlRYMzJHYm5LUDM4cWJxNGh5UUdUU0VIbzR5U0hVMnlLY1d1Mng0WHZGU09UTFpKUE5XSFVJb0Fib09FV29Qb2l6NmhBWUNvR21wNWJqVWdrRmMwQQ&q=http%3A%2F%2Fwww.mediafire.com%2Ffile%2Fcl63d6ydeyp8ii5%2FHealthBarAssets.zip%2Ffile&v=CFASjEuhyf4










