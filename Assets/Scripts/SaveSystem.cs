using UnityEngine;

public static class SaveSystem
{
    public enum PrefsField
    {
        Master,
        Music,
        Fx,
        Level,
        SubLevel,
    }

    public static float GetVolume(PrefsField field)
    {
        if (field != PrefsField.Fx && field != PrefsField.Music && field != PrefsField.Master) return 10f;
        if (!PlayerPrefs.HasKey(field.ToString())) return 10;
        return PlayerPrefs.GetFloat(field.ToString());
    }

    public static void SetVolume(PrefsField field, float value)
    {
        if (field != PrefsField.Fx && field != PrefsField.Music && field != PrefsField.Master) return;
        PlayerPrefs.SetFloat(field.ToString(), value);
    }

    // TODO: Check for level progress.
    public static Vector2Int GetProgress()
    {
        var x = PlayerPrefs.HasKey(PrefsField.Level.ToString()) ? PlayerPrefs.GetInt(PrefsField.Level.ToString()) : 0;
        var y = PlayerPrefs.HasKey(PrefsField.SubLevel.ToString())
            ? PlayerPrefs.GetInt(PrefsField.SubLevel.ToString())
            : 0;
        return new Vector2Int(x, y);
    }

    public static void SetProgress(Vector2Int progress)
    {
        PlayerPrefs.SetInt(PrefsField.Level.ToString(), progress.x);
        PlayerPrefs.SetInt(PrefsField.SubLevel.ToString(), progress.y);
    }
}