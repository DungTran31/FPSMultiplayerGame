using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

	private void Start()
	{
		if (PlayerPrefs.HasKey("username"))
		{
			usernameInput.text = PlayerPrefs.GetString("username");
			PhotonNetwork.NickName = PlayerPrefs.GetString("username");
		}
		else
		{
			usernameInput.text = "Player " + Random.Range(0, 1000).ToString("0000");
		}
	}

	public void OnUsernameInputValueChanged()
	{
		PhotonNetwork.NickName = usernameInput.text;
		PlayerPrefs.SetString("username", usernameInput.text);
	}
}
