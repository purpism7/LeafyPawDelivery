using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BezierCurves : MonoBehaviour
{
    [SerializeField] private Vector3 startPos = Vector3.zero; 
    [SerializeField] private Vector3 targetPos = Vector3.zero;
    [SerializeField] private Vector3 wayPointPos = Vector3.zero;
    [SerializeField] private float duration = 1f;

    private List<Vector3> _gizmoPointList = new(); 
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(Vector3 startPos, Vector3 targetPos)
    {
        wayPointPos = new Vector3(Random.Range(startPos.x, targetPos.x), Random.Range(10f, 50f), 0);
    }

    private void OnDrawGizmos()
    {
        _gizmoPointList.Clear();

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(startPos, 100f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wayPointPos, 100f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetPos, 100f);
        
        for (int i = 0; i < duration; ++i)
        {
            float t = (i / duration);
            
            var pos1 = Vector3.Lerp(startPos, wayPointPos, t);
            var pos2 = Vector3.Lerp(wayPointPos, targetPos, t);
            
            _gizmoPointList.Add(Vector3.Lerp(pos1, pos2, t));
        }

        for (int i = 0; i < _gizmoPointList.Count - 1; ++i)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_gizmoPointList[i], _gizmoPointList[i + 1]);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Vector3.Lerp()
    }
}
