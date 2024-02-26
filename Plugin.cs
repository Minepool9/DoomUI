using BepInEx;
using ABLoader;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Configgy;

namespace CRuSUI;

[BepInPlugin(PluginData.GUID, PluginData.Name, PluginData.Version)]
[BepInDependency("Hydraxous.ULTRAKILL.Configgy")]
public class Plugin : BaseUnityPlugin
{
	[Configgable(displayName: "Is on Death mode")]
	public static ConfigToggle death = new(true);
	private void Awake()
	{
		ConfigBuilder config = new ConfigBuilder(PluginData.GUID, PluginData.Name);
		config.BuildAll();
	}
	private void SetLife(bool on)
	{
		Texture[] all = default;
		if (on) {
			AssetBundle death_bundle = Loader.LoadBundle("crus-death-tex");
			all = death_bundle.LoadAllAssets<Texture>();
		} else {
			AssetBundle life_bundle = Loader.LoadBundle("crus-life-tex");
			all = life_bundle.LoadAllAssets<Texture>();
		}
		shared_hp.tex = all;
	}
	private void Start()
	{
		AssetBundle ui_bundle = Loader.LoadBundle("crus");

		Texture[] all = default; 
		if (death.Value) {
			AssetBundle death_bundle = Loader.LoadBundle("crus-death-tex");
			all = death_bundle.LoadAllAssets<Texture>();
		} else {
			AssetBundle life_bundle = Loader.LoadBundle("crus-life-tex");
			all = life_bundle.LoadAllAssets<Texture>();
		}
		GameObject ui = (GameObject)ui_bundle.LoadAsset("CRUSUI.prefab");
		GameObject ui_object = Instantiate(ui);

		SquadUIReferences ui_ref = ui_object.transform.Find("Canvas").gameObject.AddComponent<SquadUIReferences>();
		ui_ref.border = ui_ref.transform.Find("Border").GetComponent<RawImage>();
		ui_ref.health = ui_ref.transform.Find("h").GetComponent<RawImage>();
		ui_ref.ammo_ind = ui_ref.transform.Find("ammo").GetComponent<RawImage>();
		ui_ref.health_text = ui_ref.transform.Find("h_text").GetComponent<TextMeshProUGUI>();

		ui_ref.border1 = (Texture)ui_bundle.LoadAsset("border.png");
		ui_ref.border2 = (Texture)ui_bundle.LoadAsset("border2.png");
		ui_ref.border3 = (Texture)ui_bundle.LoadAsset("border3.png");
		ui_ref.border4 = (Texture)ui_bundle.LoadAsset("border4.png");

		shared_ui_ref = ui_ref;

		HealthPulsator hp = ui_ref.health.gameObject.AddComponent<HealthPulsator>();
		shared_hp = hp;
		hp.tex = all;

		crus_ui = ui_object;

		DontDestroyOnLoad(ui_object);

		death.OnValueChanged += SetLife;
	}

	private void Update()
	{
		if (!crus_ui)
			return;
		if (NewMovement.Instance && SceneHelper.CurrentScene != "Main Menu")
		{
			crus_ui.SetActive(true);
			NewMovement.Instance.screenHud.GetComponent<HudController>().gunCanvas.SetActive(false);
			NewMovement.Instance.screenHud.transform.Find("StyleCanvas").gameObject.SetActive(false);

			int h = NewMovement.Instance.hp;
			if (death.Value)
				h += 250;

			shared_ui_ref.health_text.text = Convert.ToString(NewMovement.Instance.hp);
			shared_ui_ref.health.color = Color.HSVToRGB((h * 0.01f) * 0.25f, 1, 1);
			if (InputManager.Instance.InputSource.Fire1.IsPressed || InputManager.Instance.InputSource.Fire2.WasPerformedThisFrame || InputManager.Instance.InputSource.Punch.WasPerformedThisFrame) {
				rot_spd -= 2f;
			}

			shared_ui_ref.ammo_ind.transform.Rotate(0, 0, rot_spd * 57.29578f);

			int diff = PrefsManager.Instance.GetInt("difficulty");

			if (diff == 3) {
				shared_ui_ref.border.texture = shared_ui_ref.border2;
			} else if (diff > 3) {
				shared_ui_ref.border.texture = shared_ui_ref.border4;
			} else if (diff >= 1) {
				shared_ui_ref.border.texture = shared_ui_ref.border1;
			} else {
				shared_ui_ref.border.texture = shared_ui_ref.border3;
			}

			shared_hp.freq_timer = 1f / Mathf.Clamp(100 - NewMovement.Instance.hp, 24, 100);

			rot_spd = Mathf.Lerp(rot_spd, 0, 0.1f);
		}
		else
		{
			crus_ui.SetActive(false);
		}
	}
	GameObject crus_ui;
	SquadUIReferences shared_ui_ref;
	HealthPulsator shared_hp;
	float rot_spd = 0f;
}
