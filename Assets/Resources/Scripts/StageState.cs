using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageState : MonoBehaviour {

	public static int[,,] stage;	//それぞれx, y, zを表している
	const int MAXIMUM_LENGTH = 6;	//ブロックが入れる部分
	const int ARRAY_SIZE = 8;		//配列のサイズ



	//stage init (all status are 0)
	void Start () {
		stage = new int[ARRAY_SIZE, ARRAY_SIZE-1, ARRAY_SIZE];
		for (int i = 0; i < ARRAY_SIZE; i++) {
			for (int j = 0; j < ARRAY_SIZE-1; j++) {
				for (int k = 0; k < ARRAY_SIZE; k++) {
					if(i == 0 || i == ARRAY_SIZE || j == 0 || k == 0 || k == ARRAY_SIZE){
						stage[i, j, k] = 1;		//外壁はalways1
					}
					else {
						stage[i, j, k] = 0;		//内側はまだ何も入っていないので0
					}
				}
			}
		}
	}
}
