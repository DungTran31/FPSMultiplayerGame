using UnityEngine;

public abstract class Gun : Item
{
	public abstract override void Use();

	public GameObject bulletImpactPrefab;
	public GameObject muzzleEffectPrefab; // Muzzle effect prefab
	public ShootingMode shootingMode;
	public int burstCount = 3; // Number of shots in burst mode
	public float autoFireRate = 0.1f; // Time between shots in auto mode
}

public enum ShootingMode
{
	Single,
	Burst,
	Auto
}
