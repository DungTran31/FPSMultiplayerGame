using Photon.Pun;
using System.Collections;
using UnityEngine;

public class SingleShotGun : Gun
{
	[SerializeField] Camera cam;

	PhotonView PV;
	private Animator animator;

	private void Awake()
	{
		PV = GetComponent<PhotonView>();
		animator = GetComponentInChildren<Animator>();
	}

	public override void Use()
	{
		switch (shootingMode)
		{
			case ShootingMode.Single:
				Shoot();
				break;
			case ShootingMode.Burst:
				StartCoroutine(BurstShoot());
				break;
			case ShootingMode.Auto:
				StartCoroutine(AutoShoot());
				break;
		}
	}

	private void Shoot()
	{
		// Trigger the RECOIL animation
		if (animator != null)
		{
			animator.SetTrigger("RECOIL");
		}

		// Play shoot sound
		if (PV.IsMine)
		{
			SoundManager.Instance.PlayShootSound();
			PV.RPC("RPC_PlayShootSound", RpcTarget.Others);
		}

		// Spawn muzzle effect
		if (muzzleEffectPrefab != null)
		{
			muzzleEffectPrefab.GetComponent<ParticleSystem>().Play(); // Play the muzzle effect
		}

		Ray ray = new Ray(cam.transform.position, cam.transform.forward);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			hit.collider.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
			PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
		}
	}

	[PunRPC]
	private void RPC_PlayShootSound()
	{
		SoundManager.Instance.PlayShootSound();
	}

	private IEnumerator BurstShoot()
	{
		for (int i = 0; i < burstCount; i++)
		{
			Shoot();
			yield return new WaitForSeconds(autoFireRate);
		}
	}

	private IEnumerator AutoShoot()
	{
		while (Input.GetMouseButton(0))
		{
			Shoot();
			yield return new WaitForSeconds(autoFireRate);
		}
	}

	[PunRPC]
	public void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
	{
		Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
		if (colliders.Length != 0)
		{
			GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
			Destroy(bulletImpactObj, 10f);
			bulletImpactObj.transform.SetParent(colliders[0].transform);
		}
	}
}
