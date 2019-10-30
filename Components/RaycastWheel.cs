using UnityEngine;
using System.Collections;

// Do test the code! You usually need to change a few small bits.

public class RaycastWheel : MonoCached {

    public float motorTorque {get; set;}
    public float steerAngle { get; set; }

	public float wheelRadius  = 0.5f;
	public float suspensionRange  = 0.10f;
	public float suspensionForce  = 0.00f;
	public float suspensionDamp  = 0.00f;
	public float compressionFrictionFactor  = 0.00f;

	public float sidewaysFriction  = 0.00f;
	public float sidewaysDamp  = 0.00f;

	public float forwardFriction  = 0.00f;

   // public bool isSteeringWheel = false;

	private float speed  = 0.00f;

    private bool isGrounded = false;

    public bool IsGrounded { get { return isGrounded; } }

	private RaycastHit hit;
    public RaycastHit Hit { get { return hit; } }

	private Rigidbody parentRigidbody;

	private float wheelCircumference = 0.00f;

    private Vector3 wheelDownDirection, wheelPointVelocity, wheelPointInversedVelocity, relativeSuspensionForce, shockDrag;
    private Vector3 forwardForce, sideForce, sideDrag, motorForce, resultForce;
    
    private Vector3 forwardDifferenceDirection = Vector3.zero, sideDifferenceDirection = Vector3.zero;

    public Transform _transform { get; set; }

    private void OnEnable()
    {
        allFixedTicks.Add(this);
    }
    private void OnDisable()
    {
        allFixedTicks.Remove(this);
    }
    void Start ()
	{
		wheelCircumference = wheelRadius * Mathf.PI * 2;
        _transform = transform;
        parentRigidbody = _transform.parent.parent.GetComponent<Rigidbody>();
    }

    public override void OnFixedTick()
    {
        _transform.localRotation = Quaternion.AngleAxis(steerAngle, _transform.up);

        speed = parentRigidbody.velocity.magnitude;

        wheelDownDirection = -_transform.up; 

		if(Physics.Raycast (_transform.position, wheelDownDirection, out hit, suspensionRange + wheelRadius) && !ReferenceEquals(hit.transform.root, _transform.root))
		{
            isGrounded = true;
			wheelPointVelocity = parentRigidbody.GetPointVelocity(hit.point);

			float compression = hit.distance / (suspensionRange + wheelRadius);
			compression = -compression + 1;

			relativeSuspensionForce = -wheelDownDirection * compression * suspensionForce;
            
			wheelPointInversedVelocity = _transform.InverseTransformDirection(wheelPointVelocity);
			wheelPointInversedVelocity.z = 0;
			wheelPointInversedVelocity.x = 0;
            
			shockDrag = _transform.TransformDirection(wheelPointInversedVelocity) * -suspensionDamp;
            // __________________z-friction__________________

            float forwardDifference = _transform.InverseTransformDirection(wheelPointVelocity).z - speed;
            forwardDifferenceDirection.z = -forwardDifference;

            float newForwardFriction = forwardFriction;
            
			newForwardFriction = Mathf.Lerp(newForwardFriction, newForwardFriction * compression * 1, compressionFrictionFactor);

			forwardForce = _transform.TransformDirection(forwardDifferenceDirection) * newForwardFriction;
            // __________________x-friction__________________

            float sidewaysDifference = _transform.InverseTransformDirection(wheelPointVelocity).x;
            sideDifferenceDirection.x = -sidewaysDifference;

			float newSideFriction = sidewaysFriction ;
			newSideFriction = Mathf.Lerp(newSideFriction, newSideFriction * compression * 1, compressionFrictionFactor);
            

			sideForce = _transform.TransformDirection(sideDifferenceDirection) * newSideFriction;

			wheelPointInversedVelocity = _transform.InverseTransformDirection(wheelPointVelocity);
			wheelPointInversedVelocity.z = 0;
			wheelPointInversedVelocity.y = 0;

			sideDrag = _transform.TransformDirection(wheelPointInversedVelocity) * -sidewaysDamp;

            motorForce = _transform.forward * motorTorque;

            resultForce = motorForce + relativeSuspensionForce + shockDrag - forwardForce + sideForce + sideDrag;

            // By the some of all you combined, I become CAPTAIN FORCE
            parentRigidbody.AddForceAtPosition(resultForce, _transform.position);

			speed += forwardDifference;
		}
		else
		{
            isGrounded = false;
		}
    }

    private Vector3 newVisualPosition = Vector3.zero;

    public void VisualWheelSync(Transform visualWheel)
    {
        if(isGrounded)
        {
            visualWheel.position = _transform.position + (wheelDownDirection * (hit.distance - wheelRadius));
            //newVisualPosition = _transform.position + (wheelDownDirection * (hit.distance - wheelRadius));
        }
        else
        {
           visualWheel.position = _transform.position + (wheelDownDirection * suspensionRange);
            //newVisualPosition = _transform.position + (wheelDownDirection * suspensionRange);
        }

        //newVisualPosition.x = visualWheel.position.x;
        //newVisualPosition.z = visualWheel.position.z;

        //visualWheel.position = newVisualPosition;

        visualWheel.Rotate(360 * (-speed / wheelCircumference) * Time.deltaTime, 0, 0);
    }
    void  OnDrawGizmosSelected ()
	{
		Gizmos.color = new Color(0,1,0,1);
		Vector3 direction = transform.TransformDirection (-Vector3.up) * (this.wheelRadius);
		Gizmos.DrawRay(transform.position,direction);

		Gizmos.color = new Color(0,0,1,1);
		direction = transform.TransformDirection (-Vector3.up) * (this.suspensionRange);
		Gizmos.DrawRay(new Vector3(transform.position.x,transform.position.y - wheelRadius,transform.position.z),direction);
	}
}