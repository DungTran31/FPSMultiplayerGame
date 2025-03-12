using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
	[SerializeField] Image healthBarImage;
	[SerializeField] GameObject ui;

	[SerializeField] GameObject cameraHolder;
	[SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
	[SerializeField] Item[] items;

	[SerializeField] List<Material> materials;
	[SerializeField] string materialName;

	int itemIndex;
	int previousItemIndex = -1;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	Rigidbody rb;

	PhotonView PV;

	const float maxHealth = 100f;
	float currentHealth = maxHealth;

	PlayerManager playerManager;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
		materials = Customizer.Materials;
		materialName = (string)photonView.Owner.CustomProperties["Material"];
		foreach (Material material in materials)
		{
			if (material.name == materialName)
			{
				GetComponent<Renderer>().material = material;
				break;
			}
		}
	}

	private void Start()
	{
		if (PV.IsMine)
		{
			EquipItem(0);
		}
		else
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
			Destroy(ui);
		}
	}

	private void Update()
	{
		if (!PV.IsMine) return;

		// Look rotation:
		Look();

		// Calculate movement:
		Move();

		// Jump
		Jump();

		//// Grounded check (instead use box collider)
		//Ray ray = new Ray(transform.position, -transform.up);
		//RaycastHit hit;
		//if (Physics.Raycast(ray, out hit, 1.1f))
		//{
		//	grounded = true;
		//}
		//else
		//{
		//	grounded = false;
		//}

		for (int i = 0; i < items.Length; i++)
		{
			if (Input.GetKeyDown((i + 1).ToString()))
			{
				EquipItem(i);
				break;
			}
		}

		if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
		{
			if(itemIndex >= items.Length - 1)
			{
				EquipItem(0);
			}
			else
			{
				EquipItem(itemIndex + 1);
			}
		}
		else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
		{
			if (itemIndex <= 0)
			{
				EquipItem(items.Length - 1);
			}
			else
			{
				EquipItem(itemIndex - 1);
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			items[itemIndex].Use();
		}

		if(transform.position.y < -10f) // fall off the map
		{
			Die();
		}
	}

	private void Jump()
	{
		if (Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			rb.AddForce(transform.up * jumpForce);
		}
	}

	private void Move()
	{
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		Vector3 targetMoveAmount = moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed);
		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, smoothTime);
	}

	private void Look()
	{
		transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
		verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
	}

	private void EquipItem(int _index)
	{
		if (_index == previousItemIndex)
		{
			return;
		}

		itemIndex = _index;
		items[itemIndex].itemGameObject.SetActive(true);

		if (previousItemIndex != -1)
		{
			items[previousItemIndex].itemGameObject.SetActive(false);
		}

		previousItemIndex = itemIndex;

		if (PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
		{
			EquipItem((int)changedProps["itemIndex"]);
		}
	}

	public void SetGroundedState(bool _grounded)
	{
		grounded = _grounded;
	}

	private void FixedUpdate()
	{
		if (!PV.IsMine) return;

		rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	public void TakeDamage(float damage)
	{
		PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
	}

	[PunRPC]
	private void RPC_TakeDamage(float damage, PhotonMessageInfo info)
	{
		Debug.Log("Player " + PV.Owner.NickName + " has taken damage: " + damage);
		currentHealth -= damage;
		healthBarImage.fillAmount = currentHealth / maxHealth;
		if (currentHealth <= 0)
		{
			Die();
			PlayerManager.Find(info.Sender).GetKill();
		}
	}

	private void Die()
	{
		playerManager.Die();
	}
}
