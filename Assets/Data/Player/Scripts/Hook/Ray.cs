using UnityEngine;

public class Ray : MonoBehaviour
{


    [SerializeField] private float height = 15;
    [SerializeField] private float rotationX = -50f;
    [SerializeField] private Transform rayStartPoint;
    
    
    public LayerMask whatIsGrapple;
    
    private UnityEngine.Ray _ray;
    
    private RaycastHit _hitPoint;


    private void Awake()
    {
        transform.rotation = Quaternion.Euler(rotationX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        
        _ray = new UnityEngine.Ray(rayStartPoint.position, transform.forward);
    }
    
    void Update()
    {
        _ray.origin = rayStartPoint.position;
        _ray.direction = transform.forward;
        
        DebugRay();
    }
    
    private void DebugRay()
    {
        Debug.DrawRay(_ray.origin, _ray.direction * height, Color.blue); 
    }
    
    public RaycastHit? GetRay()
    {
        if (Physics.Raycast(_ray.origin, _ray.direction, out var hit, height, whatIsGrapple))
        {
            return hit;
        }

        return null;
    }
}
