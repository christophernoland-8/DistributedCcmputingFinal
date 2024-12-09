using Ditzelgames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFloat : MonoBehaviour
{
    public float AirDrag = 1;
    public float WaterDrag = 10;
    public Transform[] FloatPoints;
    public bool AttachToSurface;


    protected Rigidbody Rigidbody;
    protected WaveNetwork Waves;

    protected float WaterLine;
    protected Vector3[] WaterLinePoints;

    protected Vector3 centerOffset;
    protected Vector3 smoothVectorRotation;
    protected Vector3 TargetUp;

    public Vector3 Center { get { return transform.position + centerOffset; } }

    // Start is called before the first frame update
    void Awake()
    {

        Waves = FindObjectOfType<WaveNetwork>();
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.useGravity = false;

        WaterLinePoints = new Vector3[FloatPoints.Length];
        for (int i = 0; i < FloatPoints.Length; i++)
        {
            WaterLinePoints[i] = FloatPoints[i].position;
        }
        centerOffset = PhysicsHelper.GetCenter(WaterLinePoints) - transform.position;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        var newWaterLine = 0f;
        var pointUnderWater = false;

        for (int i = 0; i < FloatPoints.Length; i++)
        {

            WaterLinePoints[i] = FloatPoints[i].position;
            WaterLinePoints[i].y = Waves.GetHeight(FloatPoints[i].position);
            newWaterLine += WaterLinePoints[i].y / FloatPoints.Length;
            if (WaterLinePoints[i].y > FloatPoints[i].position.y)
            {
                pointUnderWater = true;
            }
        }

            var waterLineDelta = newWaterLine - WaterLine;
            WaterLine = newWaterLine;

        

        var gravity = Physics.gravity;
        Rigidbody.drag = AirDrag;
        if (WaterLine > Center.y)
        {
            Rigidbody.drag = WaterDrag;

            if (AttachToSurface)
            {
                Rigidbody.position = new Vector3(Rigidbody.position.x, WaterLine - centerOffset.y, Rigidbody.position.z);
            }
            else
            {
                gravity = -Physics.gravity;
                transform.Translate(Vector3.up * waterLineDelta * 0.9f);
            }

            gravity = -Physics.gravity;
            transform.Translate(Vector3.up * waterLineDelta * 0.9f);
           
        }
        Rigidbody.AddForce(gravity * Mathf.Clamp(Mathf.Abs(WaterLine - Center.y), 0,1));

        TargetUp = PhysicsHelper.GetNormal(WaterLinePoints);

        if (pointUnderWater)
        {
            TargetUp = Vector3.SmoothDamp(transform.up, TargetUp, ref smoothVectorRotation, 0.2f);
            Rigidbody.rotation = Quaternion.FromToRotation(transform.up, TargetUp) * Rigidbody.rotation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (FloatPoints == null)
            return;

        for (int i = 0; i < FloatPoints.Length; i++)
        {
            if (FloatPoints[i] == null)
                continue;

            if (Waves != null)
            {

                //draw cube
                Gizmos.color = Color.red;
                Gizmos.DrawCube(WaterLinePoints[i], Vector3.one * 0.3f);
            }

            //draw sphere
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(FloatPoints[i].position, 0.1f);

        }

        //draw center
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(Center.x, WaterLine, Center.z), Vector3.one * 1f);
            Gizmos.DrawRay(new Vector3(Center.x, WaterLine, Center.z), TargetUp * 1f);
        }
    }
}
