using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stickPlantform : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);//当玩家进入平台的触发器时，将玩家对象设置为平台对象的子对象，使玩家能够跟随平台移动
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);//当玩家离开平台的触发器时，将玩家对象的父对象设置为null，使玩家不再跟随平台移动
        }
    }
}
