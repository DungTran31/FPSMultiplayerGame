using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;

    Player player;

    public void Initialize(Player player)
    {
        this.player = player;

        usernameText.text = player.NickName;
		UpdateStats();

	}

	private void UpdateStats()
	{
		if (player.CustomProperties.TryGetValue("kills", out object kills))
		{
			killsText.text = kills.ToString();
		}
		if (player.CustomProperties.TryGetValue("deaths", out object deaths))
		{
			deathsText.text = deaths.ToString();
		}
	}


	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if (targetPlayer == player)
		{
			if (changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths"))
			{
				UpdateStats();
			}
		}
	}
}
