using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageState : MonoBehaviour
{

	public static int[,,] stage;
	//それぞれx, y, zを表している
	const int STAGE_SIZE_X = 8;
	//stageのサイズ(8,7,8)
	const int STAGE_SIZE_Y = 7;
	const int STAGE_SIZE_Z = 8;


	public static string charPos = "forward";
	//自分がどこに今いるのか
	//正面か後ろ: forward
	//脇: side
	//上下: updown

	//(0,0,0)はステージの手前左下



	//stage init (all status are 0)
	void Start ()
	{
		stage = new int[STAGE_SIZE_X, STAGE_SIZE_Y, STAGE_SIZE_Z];
		for (int i = 0; i < STAGE_SIZE_X; i++) {
			for (int j = 0; j < STAGE_SIZE_Y; j++) {
				for (int k = 0; k < STAGE_SIZE_Z; k++) {
					if (i == 0 || i == STAGE_SIZE_X - 1 || j == 0 || k == 0 || k == STAGE_SIZE_Z - 1) {
						stage [i, j, k] = 1;		//外壁はalways1
					} else {
						stage [i, j, k] = 0;		//内側はまだ何も入っていないので0
					}
				}
			}
		}
	}

	//指定した方向に動かせるならtrueを返す
	//動かせないならnowBlockPosを初期に戻してfalseを返す
	//動ける場合は、centerposも更新
	public static bool CouldMoveBlock (string str)
	{
		//一度tmpに保存
		Vector3[] tmpBlockPos = new Vector3[GameController.nowBlockPos.Length];
		for (int i = 0; i < tmpBlockPos.Length; i++) {
			tmpBlockPos [i] = GameController.nowBlockPos [i];
		}
		for (int i = 0; i < tmpBlockPos.Length; i++) {
		
			switch (str) {
			case "drop":
				GameController.nowBlockPos [i] += new Vector3 (0, -1, 0);
				break;
			case "left":
				GameController.nowBlockPos [i] += new Vector3 (-1, 0, 0);
				break;
			case "right":
				GameController.nowBlockPos [i] += new Vector3 (1, 0, 0);
				break;
			case "up":
				GameController.nowBlockPos [i] += new Vector3 (0, 0, 1);
				break;
			case "down":
				GameController.nowBlockPos [i] += new Vector3 (0, 0, -1);
				break;
			case "roll_right":
				//Debug.Log ("right");
				rotateBlock (charPos, i);
				break;
			case "roll_left":
				//Debug.Log ("left");
				rotateBlock (charPos, i);
				rotateBlock (charPos, i);
				rotateBlock (charPos, i);
				break;

			default:
				break;
			}
			int posx = (int)GameController.nowBlockPos [i].x;
			int posy = (int)GameController.nowBlockPos [i].y;
			int posz = (int)GameController.nowBlockPos [i].z;

			//もし動かせないならもどす
			if (posx < 0 || posy < 0 || posz < 0) {
				for (int j = 0; j < tmpBlockPos.Length; j++) {
					GameController.nowBlockPos [j] = tmpBlockPos [j];
				}
				return false;
			}
			if (stage [posx, posy, posz] == 1) {
				//ダメなら保存したtmpを入れて終わり
				for (int j = 0; j < tmpBlockPos.Length; j++) {
					GameController.nowBlockPos [j] = tmpBlockPos [j];
				}
				return false;
			}

		}

		string s = "";
		for (int j = STAGE_SIZE_Y - 1; j >= 0; j--) {
			for (int l = 0; l < STAGE_SIZE_X; l++) {

				string minis = StageState.stage [l, j, 1] + " ";
				for (int i = 0; i < 4; i++) {
					if (GameController.nowBlockPos[i].x == l && GameController.nowBlockPos[i].y == j) {
						minis = "1 ";
						break;
					}
				}
				s += minis;
			}
			Debug.Log (s);
			s = "";
		}
		Debug.Log ("----------------------");

		//動かせたらnowBlockPosはそのままにしてtrue
		return true;
	}

	//システムの座標の移動
	//ブロックの座標の移動はmoveBlock
	public static void confirm_stage ()
	{
		for (int i = 0; i < GameController.nowBlockPos.Length; i++) {
			int posx = (int)GameController.nowBlockPos [i].x;
			int posy = (int)GameController.nowBlockPos [i].y;
			int posz = (int)GameController.nowBlockPos [i].z;
			StageState.stage [posx, posy, posz] = 1;
		}
	}
		

	//ブロックの座標移動
	//システムの座標は移動しない
	//システムの座標の移動はconfirm_stage
	public static void MoveBlock (string direction)
	{
		switch (direction) {
		case "drop":
			GameController.nowBlock.transform.position += new Vector3 (0f, -BlockGenerator.move_amount, 0f);
			break;
		case "left":
			GameController.nowBlock.transform.position += new Vector3 (-BlockGenerator.move_amount, 0f, 0f);
			break;
		case "right":
			GameController.nowBlock.transform.position += new Vector3 (BlockGenerator.move_amount, 0f, 0f);
			break;
		case "up":
			GameController.nowBlock.transform.position += new Vector3 (0f, 0f, BlockGenerator.move_amount);
			break;
		case "down":
			GameController.nowBlock.transform.position += new Vector3 (0f, 0f, -BlockGenerator.move_amount);
			break;
		case "roll_right":
			Vector3 centerPos = new Vector3 ();
			foreach (Transform t in GameController.nowBlock.transform) {
				if (t.gameObject.tag == "Center") {
					centerPos = t.position;
				}
			}
			GameController.nowBlock.transform.RotateAround (centerPos, Vector3.forward, -90);

			break;
		case "roll_left":
			centerPos = new Vector3 ();
			foreach (Transform t in GameController.nowBlock.transform) {
				if (t.gameObject.tag == "Center") {
					centerPos = t.position;
				}
			}
			GameController.nowBlock.transform.RotateAround (centerPos, Vector3.forward, 90);

			break;
		default:
			break;
		}
	}




	//一列揃ってるかどうかの判定
	//return listに揃ってる行数を入れる
	public static List<int> findFill ()
	{
		//ブロックの置けるエリア内が全て1の時はlistにappend
		List<int> filledList = new List<int> ();
		for (int i = STAGE_SIZE_Y - 1; i >= 1; i--) {
			int count = 0;
			for (int j = 0; j < STAGE_SIZE_X; j++) {
				for (int k = 0; k < STAGE_SIZE_Z; k++) {
					if (stage [j, i, k] == 1)
						count++;
				}
			}
			if (count /*== STAGE_SIZE_X * STAGE_SIZE_Z*/ > 30)
				filledList.Add (i);
		}
		return filledList;
	}


	//引数で指定された行数一列を削除する
	//objは削除しない
	public static void DeleteRaw (int raw)
	{
		for (int i = 1; i < STAGE_SIZE_X - 1; i++) {
			for (int j = 1; j < STAGE_SIZE_Z - 1; j++) {
				for (int k = raw; k < STAGE_SIZE_Y - 1; k++) {
					stage [i, k, j] = stage [i, k + 1, j];
				}
			}
		}
		for (int i = 1; i < STAGE_SIZE_X - 1; i++) {
			for (int j = 1; j < STAGE_SIZE_Z - 1; j++) {
				stage [i, STAGE_SIZE_Y - 1, j] = 0;
			}
		}
	}

	//nowBlockPosを右に一回回転させる
	public static void rotateBlock (string direction, int t)
	{
		
		int centerx = (int)GameController.nowBlockPos [0].x;
		int centery = (int)GameController.nowBlockPos [0].y;
		int centerz = (int)GameController.nowBlockPos [0].z;

		int rerativex = (int)GameController.nowBlockPos [t].x - centerx;
		int rerativey = (int)GameController.nowBlockPos [t].y - centery;
		int rerativez = (int)GameController.nowBlockPos [t].z - centerz;
		int ansx = 0, ansy = 0, ansz = 0;

		if (direction == "forward") {
			ansx = rerativey;
			ansy = -rerativex;
			ansz = rerativez;
		}

		GameController.nowBlockPos [t] = GameController.nowBlockPos [0] + new Vector3 (ansx, ansy, ansz);
//		Debug.Log (GameController.nowBlockPos [0] + new Vector3 (ansx, ansy, ansz));
	}
		
}
