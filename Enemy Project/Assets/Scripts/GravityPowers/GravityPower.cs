using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* This class handles the player gravity powers
 * Attraction:
 * Releasing: 
 * Launching: launching an entity while attracted
 */
public class GravityPower : MonoBehaviour
{
    //Keycodes
    public KeyCode attractionKey;
    public KeyCode launchingKey;

    //GravityPowers status
    private bool attracting = false;
    private bool releasing = false;
    private bool launching = false;

    //ReactiveObject target
    private ReactiveObject target;
    private Rigidbody rb;

    //Speeds
    public float attractionSpeed;
    public float launchingSpeed;

    //Range
    public float attractionRange;

    //Costs
    public float attractionCost;
    public float launchingCost;
    private PlayerStatus _playerStatus;

    //EnergyRecover
    private EnergyRecover _energyRecover;

    //Aux methods for external questioning
    public bool IsAttracting() => attracting;
    public bool IsReleasing() => releasing;
    public bool IsLaunching() => launching;

    //Animation
    PlayerAnimationController _animController;

    private Camera _camera;

    private GUIStyle style = new GUIStyle();

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _energyRecover = GetComponentInParent<EnergyRecover>();
        _playerStatus = GetComponentInParent<PlayerStatus>();
        _animController = GetComponentInParent<PlayerAnimationController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        style.fontSize = 24;
        style.normal.textColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if(!attracting)
        {
            if (Input.GetKeyDown(attractionKey) && _playerStatus.HasEnoughEnergy() && !_animController.GetIsGrabbing() && !EventSystem.current.IsPointerOverGameObject()) Attraction();       
        }
        else
        {
            if (Input.GetKeyDown(launchingKey) && _playerStatus.HasEnoughEnergy() && !_animController.GetIsThrowing() && !_animController.GetIsReleasing() && !EventSystem.current.IsPointerOverGameObject()) Launching();

            else if (Input.GetKeyDown(attractionKey) && !_animController.GetIsReleasing()) Releasing();
        }

        //Energy consuming
        if(attracting)
        {
            //The object may be destroyed
            if(!target.IsDestroyed())
            {
                _playerStatus.ConsumeEnergy(attractionCost * rb.mass * Time.deltaTime);
                if (!_playerStatus.HasEnoughEnergy())
                {
                    Releasing();
                }
            }
            else
            {
                target = null;
                attracting = false;

                _animController.StopAttracting();
            }
            
        }
    }

    private void FixedUpdate()
    {
        if (attracting)
        {
            target.ReactToAttraction(attractionSpeed);
        }
        if(releasing)
        {
            target.ReactToReleasing();
            target = null;
            releasing = false;
            attracting = false;

            _animController.StopReleasing();
        }
        if(launching)
        {
            target.ReactToLaunching(launchingSpeed);
            target = null;
            launching = false;
            attracting = false;

            _animController.StopThrowing();
        }
    }

    private void Attraction()
    { 
        target = AcquireTarget();
        if (target != null)
        {
            _animController.Attract();

            StartCoroutine(StartAttracting());
        }
    }

    private void Releasing()
    {
        _animController.Release();

        StartCoroutine(StartReleasing());
    }

    private void Launching()
    {
        _animController.Throw();

        StartCoroutine(StartThrowing());
    }

    //Acquiring the target gameobject or null
    private ReactiveObject AcquireTarget()
    {
        Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
        Ray ray = _camera.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, attractionRange))
        {
            //I suppose this item has rigidbody
            rb = hit.transform.gameObject.GetComponent<Rigidbody>();
            return hit.transform.gameObject.GetComponent<ReactiveObject>();
        }
        return null;
    }

    //Print a gui aim
    private void OnGUI()
    {
        int size = 24;
        float posX = _camera.pixelWidth / 2 - size / 4;
        float posY = _camera.pixelHeight / 2 - size / 2;

       // GUI.Label(new Rect(posX, posY, size, size), "[ o ]", style);

        //For testing purposes
        GUIStyle style2 = new GUIStyle();
        style2.fontSize = 22;
        style2.normal.textColor = Color.white;
        size = 380;
        posX = 100;
        posY = 600;
        GUI.Label(new Rect(posX, posY, size, size), "Attracting= " + attracting + "\nReleasing= " + releasing + "\nLaunching= " + launching, style2);
    }

    private IEnumerator StartAttracting()
    {
        yield return new WaitForSeconds(0.3f);

        attracting = true;
        //energy recovery
        _energyRecover.enabled = false;
    }

    private IEnumerator StartThrowing()
    {
        yield return new WaitForSeconds(0.3f);

        launching = true;
        attracting = false;

        //energy cost
        _playerStatus.ConsumeEnergy(launchingCost * rb.mass);
        _energyRecover.enabled = true;
    }

    private IEnumerator StartReleasing()
    {
        yield return new WaitForSeconds(0.2f);

        releasing = true;
        //energy recovery
        _energyRecover.enabled = true;
    }
}
