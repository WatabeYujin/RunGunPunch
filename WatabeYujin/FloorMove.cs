using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMove : MonoBehaviour {
	const float spawnRotate = 50f;
	const float returnRotate = -18;
	const float speedAdjust = 6;

	float nowRotateX = 0;

	void Awake(){
		nowRotateX = transform.eulerAngles.x;
	}

	void Update () {
		nowRotateX = RotateAdjust (nowRotateX);
		if (ReturnRotateCheck (nowRotateX))
			nowRotateX = spawnRotate;
		FloorRotate();
	}

	void FloorRotate(){
		nowRotateX += PlaySceneManager.SceneManager.GetSpeed() * speedAdjust;
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
