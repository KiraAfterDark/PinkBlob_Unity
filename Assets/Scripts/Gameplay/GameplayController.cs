using System.Collections.Generic;
using PinkBlob.Gameplay.Ability.Properties;
using PinkBlob.Gameplay.Enemy;
using PinkBlob.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PinkBlob.Gameplay
{
	public class GameplayController : MonoBehaviour
	{
		public static GameplayController Instance { get; private set; }

		[Title("World Properties")]

		[Required]
		[AssetsOnly]
		[SerializeField]
		private PinkBlobPhysics physics;

		public PinkBlobPhysics Physics => physics;

		[Title("Ability Properties")]
		
		[Required]
		[AssetsOnly]
		[SerializeField]
		private AbilityPropertyGroup abilityPropertyGroup;

		public AbilityPropertyGroup AbilityPropertyGroup => abilityPropertyGroup;

		[Title("Player")]

		[Required]
		[SerializeField]
		private PlayerController playerControllerPrefab;

		public PlayerController PlayerController { get; private set; }

		[Title("Stage")]

		[SerializeField]
		private string greyboxStageScene;
		
		public StageController Stage { get; private set; }

		// Enemies
		public List<EnemyController> Enemies => enemies;
		private List<EnemyController> enemies = new List<EnemyController>();

		private void Awake()
		{
			DontDestroyOnLoad(transform);

			if (Instance != null)
			{
				Destroy(Instance);
			}

			Instance = this;

			if (!SceneHandler.IsStageLoaded())
			{
				SceneManager.LoadSceneAsync(greyboxStageScene, LoadSceneMode.Additive);
			}
			
			GameplayEvents.OnStageLoaded += OnStageLoaded;
		}

		private void OnStageLoaded(StageController stage)
		{
			GameplayEvents.OnStageLoaded -= OnStageLoaded;

			Stage = stage;

			PlayerController = Instantiate(playerControllerPrefab, Vector3.zero, Quaternion.identity);
			PlayerController.transform.position = Stage.PlayerSpawn.position;
		}

		public void SpawnEnemy(EnemyController enemy)
		{
			enemies.Add(enemy);
		}
	}
}
