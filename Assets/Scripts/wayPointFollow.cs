using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wayPointFollow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject[] wayPoints;//定义一个GameObject数组变量wayPoints，用于存储路径点对象
    private int currentWayPointIndex = 0;
    [SerializeField] private float speed = 2f;
    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(wayPoints[currentWayPointIndex].transform.position, transform.position) < 0.1f)
        {
            currentWayPointIndex++;
            if(currentWayPointIndex >= wayPoints.Length)
            {
                currentWayPointIndex = 0;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, wayPoints[currentWayPointIndex].transform.position, speed * Time.deltaTime);
    }
}
