using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance { get; private set; }

	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip shootClip;
	[SerializeField] private AudioClip reloadClip;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void PlayShootSound()
	{
		audioSource.PlayOneShot(shootClip);
	}

	public void PlayReloadSound()
	{
		audioSource.PlayOneShot(reloadClip);
	}
}
