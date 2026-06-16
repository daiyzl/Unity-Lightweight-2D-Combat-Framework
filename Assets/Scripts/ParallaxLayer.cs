using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("视觉差,0为不跟随，1为完全跟随")]
    [Range(0f, 1f)]//滑动条0到1
    public float parallaxFactory;//移动系数
    private Transform cam;//获取相机位置
    private Vector3 previousCamPos;//记录相机前一时刻的三位坐标
    private void Start()
    {
        //获取主相机位置
        cam=UnityEngine.Camera.main.transform;
        //记录相机初始位置
        previousCamPos = cam.position;

    }
    private void LateUpdate()
    {
        //计算相机移动了多少
        float deltaX=cam.position.x - previousCamPos.x;
        float deltaY=cam.position.y-previousCamPos.y;

        //背景按系数进行相对移动
        transform.position += new Vector3(deltaX * parallaxFactory, 0, 0);
        //更新相机位置
        previousCamPos=cam.position;
    }
}
