using System;
using UnityEngine;

public static class C32Util
{
    public static bool Equal(Color32 j, Color32 k) {
        return j.r == k.r && j.g == k.g && j.b == k.b && j.a == k.a;
    }

    public static Texture2D blankTextureDimensionsFrom(Texture2D reference) {
        return blankTexture(reference.width, reference.height);
    }

    public static Texture2D blankTexture(int w, int h, TextureFormat format = TextureFormat.ARGB32) {
        Texture2D blank = new Texture2D(w, h, format, false);
        Color32[] cols = new Color32[blank.width * blank.height];
        for (int i = 0; i < cols.Length; ++i) {
            cols[i] = new Color32(255, 255, 255, 255);
        }
        blank.SetPixels32(cols);
        return blank;
    }

    public static Color randomColor(float highTone = .4f) {
        return new Color(UnityEngine.Random.Range(0f, highTone), UnityEngine.Random.Range(0f, highTone), UnityEngine.Random.Range(0f, highTone));
    }

    public static Color lerpToLightGray(Color c) {
        return Color.Lerp(c, new Color(.7f, .7f, .7f), .3f);
    }

    public static Color[] basics = new Color[] {   Color.red, new Color(.8f, 1f, .4f), Color.yellow, Color.green, Color.blue,  Color.magenta , new Color(.9f, .4f, .9f)};
	public static Color[] darks = new Color[] { new Color(.5f, .3f, .3f), new Color(0f, .6f, .4f), new Color(0f, .3f, .7f), new Color(.9f, .2f, .2f), new Color(.3f, .3f, .3f) };

    public static Color basicMod(int i) { return basics[Math.Abs(i) % basics.Length]; }
	public static Color darksMod(int i) { return darks[Math.Abs(i) % darks.Length]; }

    public static Color randomBasicColor() {
        return basics[UnityEngine.Random.Range(0, basics.Length)];
    }
}

