using UnityEngine;

public static class ColorHelper
{
    /// <summary>
    /// Brighten
    /// </summary>
    /// <param name="c">a color</param>
    /// <param name="by">range -1 to 1 (negative darkens)</param>
    /// <returns></returns>
    public static Color Brighten(Color c, float by) {
        HSVColor result = HSVColor.FromRGBColor(c);
        result.brighten(by);
        return result.toRGBColor();
    }
}

public struct HSVColor
{
    public float h, s, v;

    public HSVColor(float h, float s, float v) {
        this.h = h; this.s = s; this.v = v;
    }

    public static HSVColor FromRGBColor(Color rgbColor) {
        float h, s, v;
        Color.RGBToHSV(rgbColor, out h, out s, out v);
        return new HSVColor(h, s, v);
    }

    public void brighten(float by) {
        v = Mathf.Clamp01(v + by);
    }

    public Color toRGBColor() {
        return Color.HSVToRGB(h, s, v);
    }
}
