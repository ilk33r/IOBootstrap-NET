using System;
using System.Numerics;
using IOBootstrap.NET.Common.Utilities;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace IOBootstrap.NET.Core.Services.Captcha;

public class IOCaptchaModule : IIOCaptchaModule
{
    private readonly IOCaptchaOptions Options;
    public IOCaptchaModule(IOCaptchaOptions options) => Options = options;

    public byte[] Generate(string stringText, string projectDir)
    {
        byte[] result;

        using (var imgText = new Image<Rgba32>(Options.Width, Options.Height))
        {
            float position = 0;
            var random = new Random();
            var startWith = (byte)IORandomUtilities.GenerateRandomNumber(5, 10);
            imgText.Mutate(ctx => ctx.BackgroundColor(Color.Transparent));

            var fontName = Options.FontFamilies[IORandomUtilities.GenerateRandomNumber(0, Options.FontFamilies.Length)];
            string fontsPath = Path.Combine(projectDir, "Fonts");

            FontCollection collection = new();
            collection.Add(Path.Combine(fontsPath, fontName));

            var fontFamily = collection.Families.First();
            Font font = fontFamily.CreateFont(Options.FontSize, FontStyle.Regular);

            foreach (var c in stringText)
            {
                var location = new PointF(startWith + position, IORandomUtilities.GenerateRandomNumber(7, 13));
                imgText.Mutate(ctx => ctx.DrawText(c.ToString(), font, Options.TextColor[IORandomUtilities.GenerateRandomNumber(0, Options.TextColor.Length)], location));
                position += TextMeasurer.MeasureSize(c.ToString(), new TextOptions(font)).Width;
            }

            // add rotation
            var rotation = getRotation();
            imgText.Mutate(ctx => ctx.Transform(rotation));

            // add the dynamic image to original image
            var size = (ushort)TextMeasurer.MeasureSize(stringText, new TextOptions(font)).Width;
            var img = new Image<Rgba32>(size + 10 + 5, Options.Height);
            img.Mutate(ctx => ctx.BackgroundColor(Options.BackgroundColor[IORandomUtilities.GenerateRandomNumber(0, Options.BackgroundColor.Length)]));


            Parallel.For(0, Options.DrawLines, i =>
            {
                var x0 = IORandomUtilities.GenerateRandomNumber(0, IORandomUtilities.GenerateRandomNumber(0, 30));
                var y0 = IORandomUtilities.GenerateRandomNumber(10, img.Height);
                var x1 = IORandomUtilities.GenerateRandomNumber(img.Width - IORandomUtilities.GenerateRandomNumber(0, (int)(img.Width * 0.25)), img.Width);
                var y1 = IORandomUtilities.GenerateRandomNumber(0, img.Height);
                img.Mutate(ctx =>
                    ctx.DrawLine(Options.DrawLinesColor[IORandomUtilities.GenerateRandomNumber(0, Options.DrawLinesColor.Length)],
                                  (float)IORandomUtilities.GenerateRandomNumber(Options.MinLineThickness, Options.MaxLineThickness),
                                  new PointF[] { new(x0, y0), new(x1, y1) })
                    );
            });

            img.Mutate(ctx => ctx.DrawImage(imgText, 0.80f));

            Parallel.For(0, Options.NoiseRate, i =>
            {
                var x0 = IORandomUtilities.GenerateRandomNumber(0, img.Width);
                var y0 = IORandomUtilities.GenerateRandomNumber(0, img.Height);
                img.Mutate(
                        ctx => ctx
                            .DrawLine(Options.NoiseRateColor[IORandomUtilities.GenerateRandomNumber(0, Options.NoiseRateColor.Length)],
                            (float)IORandomUtilities.GenerateRandomNumber(0.5, 1.5), new PointF[] { new Vector2(x0, y0), new Vector2(x0, y0) })
                    );
            });

            img.Mutate(x => x.Resize(Options.Width, Options.Height));

            using (var ms = new MemoryStream())
            {
                IImageEncoder encoder = new JpegEncoder();
                img.Save(ms, encoder);
                result = ms.ToArray();
            }
        }

        return result;
    }

    private AffineTransformBuilder getRotation()
    {
        var random = new Random();
        var builder = new AffineTransformBuilder();
        var width = IORandomUtilities.GenerateRandomNumber(10, Options.Width);
        var height = IORandomUtilities.GenerateRandomNumber(10, Options.Height);
        var pointF = new PointF(width, height);
        var rotationDegrees = IORandomUtilities.GenerateRandomNumber(0, Options.MaxRotationDegrees);
        var result = builder.PrependRotationDegrees(rotationDegrees, pointF);
        return result;
    }
}
