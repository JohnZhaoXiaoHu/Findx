﻿using Findx.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Findx.ImageSharp
{
    public static class ImageSharpExtension
    {
        public static IImageProcessingContext DrawingEnText(this IImageProcessingContext processingContext, int containerWidth, int containerHeight, string text, string[] colorHexArr, Font[] fonts)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var textWidth = (containerWidth / text.Length);
                var img2Size = Math.Min(textWidth, containerHeight);
                var fontMiniSize = (int)(img2Size * 0.9);
                var fontMaxSize = (int)(img2Size * 1.37);
                Array fontStyleArr = Enum.GetValues(typeof(FontStyle));

                for (int i = 0; i < text.Length; i++)
                {
                    using (Image<Rgba32> img2 = new Image<Rgba32>(img2Size, img2Size))
                    {
                        Font scaledFont = new Font(fonts[RandomUtil.RandomInt(0, fonts.Length)], RandomUtil.RandomInt(fontMiniSize, fontMaxSize), (FontStyle)fontStyleArr.GetValue(RandomUtil.RandomInt(fontStyleArr.Length)));
                        var point = new Point(i * textWidth, (containerHeight - img2.Height) / 2);
                        var colorHex = colorHexArr[RandomUtil.RandomInt(0, colorHexArr.Length)];
                        var drawingOptions = new DrawingOptions { TextOptions = new TextOptions { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top } };

                        img2.Mutate(ctx => ctx
                                        .DrawText(drawingOptions, text[i].ToString(), scaledFont, Rgba32.ParseHex(colorHex), new Point(0, 0))
                                        .DrawingGrid(containerWidth, containerHeight, Rgba32.ParseHex(colorHex), 6, 1)
                                        .Rotate(RandomUtil.RandomInt(-45, 45))
                                    );
                        processingContext.DrawImage(img2, point, 1);
                    }
                }
            }

            return processingContext;
        }

        /// <summary>
        /// 画杂线
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="processingContext"></param>
        /// <param name="containerWidth"></param>
        /// <param name="containerHeight"></param>
        /// <param name="color"></param>
        /// <param name="count"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static IImageProcessingContext DrawingGrid(this IImageProcessingContext processingContext, int containerWidth, int containerHeight, Color color, int count, float thickness)
        {

            var points = new List<PointF> { new PointF(0, 0) };
            for (int i = 0; i < count; i++)
            {
                GetCirclePoginF(containerWidth, containerHeight, 9, ref points);
            }
            points.Add(new PointF(containerWidth, containerHeight));

            processingContext.DrawLines(color, thickness, points.ToArray());

            return processingContext;
        }

        /// <summary>
        /// 散 随机点
        /// </summary>
        /// <param name="containerWidth"></param>
        /// <param name="containerHeight"></param>
        /// <param name="lapR"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static PointF GetCirclePoginF(int containerWidth, int containerHeight, double lapR, ref List<PointF> list)
        {
            Random random = new Random();
            PointF newPoint = new PointF();
            int retryTimes = 10;
            double tempDistance = 0;

            do
            {
                newPoint.X = random.Next(0, containerWidth);
                newPoint.Y = random.Next(0, containerHeight);
                bool tooClose = false;
                foreach (var p in list)
                {
                    tooClose = false;
                    tempDistance = Math.Sqrt((Math.Pow((p.X - newPoint.X), 2) + Math.Pow((p.Y - newPoint.Y), 2)));
                    if (tempDistance < lapR)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose == false)
                {
                    list.Add(newPoint);
                    break;
                }
            }
            while (retryTimes-- > 0);

            if (retryTimes <= 0)
            {
                list.Add(newPoint);
            }
            return newPoint;
        }

        /// <summary>
        /// 保存指定格式图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="stream"></param>
        /// <param name="format"></param>
        public static void SaveImage(this Image image, Stream stream, IImageFormat format)
        {
            if (format is SixLabors.ImageSharp.Formats.Jpeg.JpegFormat)
                image.SaveAsJpeg(stream);

            if (format is SixLabors.ImageSharp.Formats.Png.PngFormat)
                image.SaveAsPng(stream);

            if (format is SixLabors.ImageSharp.Formats.Bmp.BmpFormat)
                image.SaveAsBmp(stream);

            if (format is SixLabors.ImageSharp.Formats.Gif.GifFormat)
                image.SaveAsGif(stream);
        }


        /// <summary>
        /// 图像灰度处理
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Image<Rgba32> ToGray(this Image<Rgba32> img)
        {
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    var color = img[i, j];
                    // 计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(gray, gray, gray);
                    // 修改该像素点的RGB的颜色
                    img[i, j] = new Rgba32(newColor.R, newColor.G, newColor.B, newColor.A);
                }
            }
            return img;
        }

        /// <summary>
        /// 黑白二值化
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Image<Rgba32> Binaryzation(this Image<Rgba32> image)
        {
            image = image.ToGray();//先灰度处理
            int threshold = 180;//定义阈值
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    //获取该像素点的RGB的颜色
                    var color = image[i, j];
                    //计算颜色,大于平均值为黑,小于平均值为白
                    System.Drawing.Color newColor = color.B < threshold ? System.Drawing.Color.FromArgb(0, 0, 0) : System.Drawing.Color.FromArgb(255, 255, 255);
                    //修改该像素点的RGB的颜色
                    image[i, j] = new Rgba32(newColor.R, newColor.G, newColor.B, newColor.A);
                }
            }
            return image;
        }

    }
}
