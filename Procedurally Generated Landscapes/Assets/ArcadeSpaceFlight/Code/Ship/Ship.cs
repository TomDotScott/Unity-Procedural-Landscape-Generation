using UnityEngine;

/// <summary>
/// Ties all the primary ship components together.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ShipPhysics))]
[RequireComponent(typeof(ShipInput))]
public class Ship : MonoBehaviour
{    
    public bool isPlayer = false;

    private ShipInput input;
    private ShipPhysics physics;    

    // Getters for external objects to reference things like input.
    public bool UsingMouseInput { get { return input.useMouseInput; } }
    public Vector3 Velocity { get { return physics.Rigidbody.velocity; } }
    public float Throttle { get { return input.throttle; } }

    private void Awake()
    {
        input = GetComponent<ShipInput>();
        physics = GetComponent<ShipPhysics>();
    }

    private void Update()
    {
        // Pass the input to the physics to move the ship.
        physics.SetPhysicsInput(new Vector3(input.strafe, 0.0f, input.throttle), new Vector3(input.pitch, input.yaw, input.roll));
    }
}
