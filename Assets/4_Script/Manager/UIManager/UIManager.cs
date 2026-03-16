using UnityEngine;

namespace Defense.Manager
{
	public class UIManager : MonoBehaviour
	{
		#region Singleton
		private static UIManager s_instance;
		public static UIManager Instance { get { return s_instance; } }
		public void Init()
		{
			if (s_instance == null)
			{
				s_instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
			else
			{
				Destroy(this.gameObject);
				return;
			}
		}
		#endregion

		[SerializeField] private GameUIManager gameUI;

		public static GameUIManager GameUI => s_instance.gameUI;


		private void Awake()
		{
			Init();
		}
	}
}