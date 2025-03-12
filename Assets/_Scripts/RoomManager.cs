using Photon.Pun;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

	private void Awake()
	{
		if(Instance)
		{
			Destroy(this);
			return;
		}
		DontDestroyOnLoad(gameObject);
		Instance = this;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		SceneManager.sceneLoaded -= OnSceneLoaded;

	}
	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (scene.buildIndex == 1) // game scene
		{
			PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","PlayerManager"), Vector3.zero, Quaternion.identity);
		}
	}

	private void Start()
    {
           	
    }

    private void Update()
    {
        
    }
 }
