using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RayList : MonoBehaviour
{
    private Ray[] _rays;
    private List<RaycastHit> _allRayHits;

    private float _rotationHitX;
    
    [SerializeField] private float minHookDistance;
    
    private void Start()
    {
        SetRays();
    }



    public RaycastHit? GetTheBestRayHit()
    {
        _allRayHits = new List<RaycastHit>();

        for (int i = 0; i < _rays.Length; i++)
        {
            RaycastHit? tempRay = _rays[i].GetRay();
            if (tempRay != null)
            {
                _allRayHits.Add(tempRay.Value);
            }
        }
            
        if (_allRayHits.Count > 0)
        {
            var rayhit = CalculateTheBestRayHit();

            return rayhit;
        }
        return null;
    }

    
    private RaycastHit? CalculateTheBestRayHit()
    {
        var ray = GetBestDistanceRay();
        return ray;
    }
    
    private RaycastHit? GetBestDistanceRay()
    {
        List<float> raysDistance = new List<float>();
        for (int i = 0; i < _allRayHits.Count; i++)
        {
            float distance = Mathf.Abs( _allRayHits[i].point.magnitude - transform.position.magnitude);
            raysDistance.Add(distance);
        }

        if (raysDistance.Max() <= minHookDistance)
        {
            return null; 
        }
            
        int maxRayHitDistanceIndex = raysDistance.IndexOf(raysDistance.Max());
        return _allRayHits[maxRayHitDistanceIndex];
    }


    private void SetRays()
    {
        _rays = GetComponentsInChildren<Ray>();
    }

    public float GetTheBestRayHitRotationX()
    {
        return _rotationHitX;
    }

}
