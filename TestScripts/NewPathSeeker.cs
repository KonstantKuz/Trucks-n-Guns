using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPathSeeker : MonoBehaviour
{
    [System.Serializable]
    public class SeekPoint
    {
        public Vector3 pointPosition;
        public bool isClosed;
    }

    public LayerMask obstacleMsk;

    private Truck truck;
    private SeekPoint currentSeekPoint;
    private SeekPoint[] seekPoints;

    private bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        truck = GetComponent<Truck>();
        truck.SetUpTruck();
        StartCoroutine(latestart());
    }

    private IEnumerator latestart()
    {
        yield return new WaitForSeconds(2f);
        seekPoints = new SeekPoint[3];
        for (int i = 0; i < seekPoints.Length; i++)
        {
            seekPoints[i] = new SeekPoint();
            seekPoints[i].pointPosition = new Vector3();
        }
        currentSeekPoint = new SeekPoint();
        currentSeekPoint.pointPosition = new Vector3(); 
        for (int i = 0; i < seekPoints.Length; i++)
        {
            seekPoints[i].pointPosition.y = 0;
            seekPoints[i].pointPosition.z = 15;
        }
        seekPoints[0].pointPosition.x = (truck._transform.right * 14).x;
        seekPoints[2].pointPosition.x = -(truck._transform.right * 14).x;
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(started == true)
        {

            truck.MoveTruck(2);
            truck.SteeringWheels(SeekPointFollow());

            for (int i = 0; i < seekPoints.Length; i++)
            {
                seekPoints[i].pointPosition.z = truck._transform.position.z + 25f + truck.CurrentSpeed()*0.05f;
            }
            for (int i = 0; i < seekPoints.Length; i++)
            {

                if (Physics.CheckSphere(seekPoints[i].pointPosition, 5, obstacleMsk))
                {
                    seekPoints[i].isClosed = true;
                }
                else
                {
                    seekPoints[i].isClosed = false;
                }
            }
            for (int i = 0; i < seekPoints.Length; i++)
            {
                if (seekPoints[i].isClosed == false)
                {
                    currentSeekPoint = seekPoints[i];
                    return;
                }
            }

        }
        
    }

    public float SeekPointFollow()
    {
        Vector3 relativeToCurrentSeekPoint = truck._transform.InverseTransformPoint(currentSeekPoint.pointPosition);
        float newsteer = (relativeToCurrentSeekPoint.x / relativeToCurrentSeekPoint.magnitude);
        return newsteer;
    }

    private void OnDrawGizmos()
    {
        if(seekPoints!= null)
        {
            for (int i = 0; i < seekPoints.Length; i++)
            {
                if (seekPoints[i].isClosed == true)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawWireSphere(seekPoints[i].pointPosition, 5);
            }

        }
    }
}
