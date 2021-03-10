using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }
    public RotationAxes axes = RotationAxes.MouseXAndY;

    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    private float _rotationX = 0;
    // Start is called before the first frame update
    void Start()
    {
        /*
         * Controlla se la rotazione dello script deve essere applicata o meno,
         * i gameObject che sono rigidBody vengono ignorati dallo script.
         *
         */
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
            body.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (axes == RotationAxes.MouseX)
        {
            //rotazione della camera sull'asse x
            transform.Rotate(0,Input.GetAxis("Mouse X") * sensitivityHor, 0);
            
        } 
        else if (axes == RotationAxes.MouseY)
        {
            //rotazione della camera sull'asse y
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert; // la sottrazione previene l'inversione di camera
            //clamping: costringo la camera in un intervallo di rotazione per evitare la rotrazione oltre i 45 gradi up&down
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
            //la rotazione attorno l'asse x (look up&down) necessita frame by frame dell'ultima posizione dell'asse y
            float rotationY = transform.localEulerAngles.y;
            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        }
        else
        {
            //rotazione della camera su entrambi gli assi (somma vettoriale y + deltaX)
            //rotazione della camera sull'asse y
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert; // la sottrazione previene l'inversione di camera
            //clamping: costringo la camera in un intervallo di rotazione per evitare la rotrazione oltre i 45 gradi up&down
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

            float delta = Input.GetAxis("Mouse X") * sensitivityHor;
            float rotationY = transform.localEulerAngles.y + delta;

            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        }
    }
}
