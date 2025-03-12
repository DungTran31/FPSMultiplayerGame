using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;


public class Customizer : MonoBehaviour
{
    [SerializeField] TMP_Dropdown colorDropdown;
	[SerializeField] private List<Material> colorMaterials;
    private List<string> materialNames = new List<string>();
    public static List<Material> Materials;
    [HideInInspector] public int matIndex;
    public string currentMaterialName;
	private PhotonHashtable hash = new PhotonHashtable();

	private void Start()
    {
		Materials = colorMaterials;
        colorDropdown.ClearOptions();
		foreach (Material material in colorMaterials)
		{
			materialNames.Add(material.name);
		}
	    colorDropdown.AddOptions(materialNames);
        colorDropdown.RefreshShownValue();
		SetColor(matIndex);
	}

    private void Update()
    {
        
    }

    public void SetColor(int index)
    {
		matIndex = index;
		currentMaterialName = materialNames[index];
		if(PhotonNetwork.IsConnected)
		{
			SetHash();
		}
	}

	public void SetHash()
	{
		hash["Material"] = currentMaterialName;
		PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
	}

 }
