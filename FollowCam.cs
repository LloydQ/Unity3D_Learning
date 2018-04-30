using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

    public Transform targetTr;
    public float dist = 10.0f;  //相机距离目标的距离
    public float height = 3.0f;  //相机高于目标坐标原点的高度
    public float dampTrace = 20.0f;  //平滑度

    private Transform camTr;

	// Use this for initialization
	void Start () {
        camTr = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
	    camTr.position = Vector3.Lerp(camTr.position, targetTr.position - (targetTr.forward * dist) + (Vector3.up * height), Time.deltaTime* dampTrace);

        camTr.LookAt(targetTr.position);
	}
}
