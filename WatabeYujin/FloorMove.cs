using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMove : MonoBehaviour {
	const float spawnRotate = 40f;
	const float returnRotate =-23f;
	const float speedAdjust =5;
    [SerializeField]
    private bool mode=true;

	float nowRotateX = 0f;

	void Awake(){
		nowRotateX = transform.eulerAngles.x;
	}

	void Update () {
		nowRotateX = RotateAdjust (nowRotateX);
		if (ReturnRotateCheck (nowRotateX))
			nowRotateX = spawnRotate+ (nowRotateX- returnRotate);
		FloorRotate();
	}

	void FloorRotate(){
        float m_speed = -0.5f;
        if (mode) nowRotateX += PlaySceneManager.SceneManager.GetSpeed() * speedAdjust;
        //if (mode) nowRotateX += PlaySceneManager.SceneManager.GetSpeed();
        else nowRotateX += m_speed;
        transform.eulerAngles = Vector3.right*nowRotateX;
	}
	
	/// <summary>
	/// 角度がマイナスにならないのを調節してマイナスになるようにする
	/// 今回は180度以上行かないため、180度以上は考慮しない
	/// </summary>
	/// <returns>調整後の角度</returns>
	/// <param name="rotate">調整する角度</param>
	private float RotateAdjust(float rotate){
		if (rotate > 180) rotate -= 360;
		return rotate;
	}


	bool ReturnRotateCheck(float rotate){
		if (rotate <= returnRotate)
			return true;
		else
			return false;
	}
}
