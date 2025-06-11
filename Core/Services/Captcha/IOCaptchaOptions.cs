using System;
using SixLabors.ImageSharp;

namespace IOBootstrap.NET.Core.Services.Captcha;

public class IOCaptchaOptions
{
    public string[] FontFamilies { get; set; } = new string[] { "Courier" };
    public Color[] TextColor { get; set; } = new Color[] { Color.Blue, Color.Black, Color.Black, Color.Brown, Color.Gray, Color.Green };
    public Color[] DrawLinesColor { get; set; } = new Color[] { Color.Blue, Color.Black, Color.Black, Color.Brown, Color.Gray, Color.Green };
    public float MinLineThickness { get; set; } = 0.7f;
    public float MaxLineThickness { get; set; } = 2.0f;
    public ushort Width { get; set; } = 180;
    public ushort Height { get; set; } = 50;
    public ushort NoiseRate { get; set; } = 800;
    public Color[] NoiseRateColor { get; set; } = new Color[] { Color.Gray };
    public byte FontSize { get; set; } = 29;
    public byte DrawLines { get; set; } = 5;
    public byte MaxRotationDegrees { get; set; } = 5;
    public Color[] BackgroundColor { get; set; } = new Color[] { Color.White };
}
