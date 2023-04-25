using UnityEngine;

namespace Data.Cars
{
    public class FlyAlongLine : MonoBehaviour
    {
        [SerializeField] private LineRenderer path;
        

        private int _currentPointIndex = 0;

        [SerializeField] private float speed = 5f;

        private void Update()
        {
            Vector3 targetPosition = path.GetPosition(_currentPointIndex);
            
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
            transform.LookAt(targetPosition);
            
            if (transform.position == targetPosition)
            {
              
                _currentPointIndex++;
                
                if (_currentPointIndex >= path.positionCount)
                {
                    _currentPointIndex = 0;
                }
            }
        }
    }
}