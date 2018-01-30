using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

	public static bool isGameStarted = false;
	//if game play started, true
	public static bool readyToStart = true;
	//if stage created, true
	//public GameObject text;
	public static GameObject nowBlock;
	//now dropping block
	public static Vector3[] nowBlockPos;


	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void OnClick ()
	{
		if (readyToStart) {
			isGameStarted = true;
			//text.SetActive (true);
		}
	}
}
