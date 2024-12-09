using Ditzelgames;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(WaterFloat))]
public class WaterBoat : NetworkBehaviour
{
    [SerializeField] public Transform waveTransform;

    private Transform wavePrefab;

    public Text Host;
    public Text Client;

    

    //visible Properties
    public Transform Motor;
    public float SteerPower = 500f;
    public float Power = 5f;
    public float MaxSpeed = 10f;
    public float Drag = 0.1f;

    //used Components
    protected Rigidbody Rigidbody;
    protected Quaternion StartRotation;
    protected ParticleSystem ParticleSystem;
    protected Camera Camera;


    //internal Properties
    protected Vector3 CamVel;


    private NetworkObject player;


    //Network Position Variables

    float x;
    float y;
    float z;


    public void Start()
    {
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
        Rigidbody = GetComponent<Rigidbody>();
        StartRotation = Motor.localRotation;
        Camera = Camera.main;

        


    }
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            TimerUI.isTimer = true;

            /*wavePrefab = Instantiate(waveTransform);
            wavePrefab.GetComponent<NetworkObject>().Spawn(true);*/
        }
    }


    public void Update()
    {

        if (!IsOwner) return;

        x = UnityEngine.Random.Range(-30f, 30f);  // Random X value between -10 and 10
        y = 1f;  // Random Y value between -10 and 10
        z = UnityEngine.Random.Range(-30f, 30f);  // Random Z value between -10 and 10
        // Create the random Vector3
        Vector3 randomVector = new Vector3(x, y, z);

        if (TimerUI.isTimer && IsServer)
        {
            if (TimerUI.remainTime.Value > 0)
            {
                TimerUI.remainTime.Value -= Time.deltaTime;
            }
            else if (TimerUI.remainTime.Value < 0)
            {
                TimerUI.remainTime.Value = 0;
            }
        }

        
        if(BuoySpawner.isSpawned == false)
        {
           
                transform.position = randomVector;

            ClientPositionServerRpc();
            
        }


        if (Input.GetKeyDown(KeyCode.Space) && IsOwner)
            {
                int randomIndex = UnityEngine.Random.Range(0, 10);
                Vector3 randomSpawnPoint = new Vector3(UnityEngine.Random.Range(-10, 11), 5, UnityEngine.Random.Range(-10, 11));


            NetworkManager.Singleton.Shutdown();
            //Vector3 myVector = new Vector3(distance * 2, Mathf.Sin(distance), Mathf.Cos(distance));


        }

        
         
        //default direction
        var forceDirection = transform.forward;
        var steer = 0;

        //steer direction [-1,0,1]
        if (Input.GetKey(KeyCode.A))
            steer = 1;
        if (Input.GetKey(KeyCode.D))
            steer = -1;


        //Rotational Force
        Rigidbody.AddForceAtPosition(steer * transform.right * SteerPower / 100f, Motor.position);

        //compute vectors
        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);
        var targetVel = Vector3.zero;

        //forward/backward poewr
        if (Input.GetKey(KeyCode.W))
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * MaxSpeed, Power);
        if (Input.GetKey(KeyCode.S))
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * -MaxSpeed, Power);

        //Motor Animation // Particle system
        Motor.SetPositionAndRotation(Motor.position, transform.rotation * StartRotation * Quaternion.Euler(0, 30f * steer, 0));
        if (ParticleSystem != null)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                ParticleSystem.Play();
            else
                ParticleSystem.Pause();
        }

        //moving forward
        var movingForward = Vector3.Cross(transform.forward, Rigidbody.velocity).y < 0;

        //move in direction
        Rigidbody.velocity = Quaternion.AngleAxis(Vector3.SignedAngle(Rigidbody.velocity, (movingForward ? 1f : 0f) * transform.forward, Vector3.up) * Drag, Vector3.up) * Rigidbody.velocity;

        //camera position
        //Camera.transform.LookAt(transform.position + transform.forward * 6f + transform.up * 2f);
        //Camera.transform.position = Vector3.SmoothDamp(Camera.transform.position, transform.position + transform.forward * -8f + transform.up * 2f, ref CamVel, 0.05f);
    }



    [ServerRpc]
    public void ClientPositionServerRpc()
    {
        transform.position = new Vector3(x, y, z);
    }
    

   
}