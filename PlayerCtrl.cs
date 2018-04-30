using UnityEngine;
using System.Collections;

//挂载动画片段
[System.Serializable]

public class Anim {
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}

public class PlayerCtrl : MonoBehaviour {

    private float h = 0.0f;
    private float v = 0.0f;
    private Transform tr;
    public float moveSpeed = 8.0f;
    public float rotSpeed = 100.0f;

    public Anim anim;
    public Animation _animation;

    // Use this for initialization
    void Start () {
        tr = GetComponent<Transform>();
        _animation = GetComponentInChildren<Animation>();
        _animation.clip = anim.idle;
        _animation.Play();
	}
	
	// Update is called once per frame
	void Update () {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Debug.Log("H=" + h.ToString());
        Debug.Log("V=" + v.ToString());

        //计算前后左右移动方向向量
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        //移动
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

        //镜头跟随目标移动
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        //平滑动画
        if(v >= 0.1f)
        {
            //前进动画
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if (v <= -0.1f)
        {
            //后退动画
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            //左移动画
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            //右移动画
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else
        {
            //待机动画
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }
}
