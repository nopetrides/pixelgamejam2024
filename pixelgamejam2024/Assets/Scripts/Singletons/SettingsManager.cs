using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
	[SerializeField] private GameObject SettingsMenuCanvas;
	[SerializeField] private SettingsMenuUI SettingsMenu;
	
	public void ShowSettingsMenu()
	{
		SettingsMenuCanvas.SetActive(true);
		SettingsMenu.OpenMenu(OnSettingsMenuClosed);
	}

	private void OnSettingsMenuClosed()
	{
		SettingsMenuCanvas.SetActive(false);
	}
}