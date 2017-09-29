using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public static class PoolBossInspectorResources {
	public const string PoolBossFolderPath = "PoolBoss";

	public static Texture DeleteTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/deleteIcon.png", PoolBossFolderPath)) as Texture;
	public static Texture LeftArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_left.png", PoolBossFolderPath)) as Texture;
	public static Texture RightArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_right.png", PoolBossFolderPath)) as Texture;
	public static Texture UpArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_up.png", PoolBossFolderPath)) as Texture;
	public static Texture DownArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_down.png", PoolBossFolderPath)) as Texture;
	public static Texture SettingsTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/gearIcon.png", PoolBossFolderPath)) as Texture;
	public static Texture CancelTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/cancel.png", PoolBossFolderPath)) as Texture;
	public static Texture SaveTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/save.png", PoolBossFolderPath)) as Texture;
	public static Texture CopyTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/copyIcon.png", PoolBossFolderPath)) as Texture;

	private static readonly string SkinColor = DTPoolBossInspectorUtility.IsDarkSkin ? "Dark" : "Light";
	public static Texture PrefabTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/prefabIcon{1}.png", PoolBossFolderPath, SkinColor)) as Texture;
}
