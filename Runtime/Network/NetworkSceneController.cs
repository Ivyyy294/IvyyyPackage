using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ivyyy.Network
{
	public class NetworkSceneController : NetworkBehaviour
	{
		public static NetworkSceneController Me {get; private set;}
		private int currentScene;
		private NetworkManager networkManager;

		public void LoadScene (int index)
		{
			if (Owner)
				currentScene = index;
		}

		protected override void SetPackageData()
		{
			networkPackage.AddValue (new NetworkPackageValue (currentScene));
		}

		private void Awake()
		{
			if (Me == null)
			{
				Me = this;
				DontDestroyOnLoad (gameObject);
			}
			else
				Destroy (this);
		}

		private void Start()
		{
			networkManager = NetworkManager.Me;
		}

		// Update is called once per frame
		void Update()
		{
			if (!Owner && networkPackage.Available)
				currentScene = networkPackage.Value(0).GetInt32();

			if (currentScene != SceneManager.GetActiveScene().buildIndex)
				SceneManager.LoadScene (currentScene);
		}

	}
}

