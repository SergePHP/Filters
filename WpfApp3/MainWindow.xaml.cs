using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Effects;

namespace WpfApp3
{
    public struct RGB
    {
        public float R;
        public float G;
        public float B;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        internal BitmapSource orirginalSourceBitmap = null;
        internal BitmapSource displayedSourceBitmap = null;

        //размытие
        public const int matrixSize = 7;
        //public static double[,] blur = new double[matrixSize, matrixSize] {{0.111, 0.111, 0.111},
        //                                                                   {0.111, 0.111, 0.111},
        //                                                                   {0.111, 0.111, 0.111}};
        //public static double[,] blur = new double[matrixSize, matrixSize] {{0.000789, 0.006581, 0.013347, 0.006581, 0.000789},
        //                                                                   {0.006581, 0.054901, 0.111345, 0.054901, 0.006581},
        //                                                                   {0.013347, 0.111345, 0.225821, 0.111345, 0.013347},
        //                                                                   {0.006581, 0.054901, 0.111345, 0.054901, 0.006581},
        //                                                                   {0.000789, 0.006581, 0.013347, 0.006581, 0.000789}};


        public static double[,] blur = new double[matrixSize, matrixSize] {{0.00000067, 0.00002292, 0.00019117, 0.00038771, 0.00019117, 0.00002292, 0.00000067},
                                                                           {0.00002292, 0.00078633, 0.00655965, 0.01330373, 0.00655965, 0.00078633, 0.00002292},
                                                                           {0.00019117, 0.00655965, 0.05472157, 0.11098164, 0.05472157, 0.00655965, 0.00019117},
                                                                           {0.00038771, 0.01330373, 0.11098164, 0.22508352, 0.11098164, 0.01330373, 0.00038771},
                                                                           {0.00019117, 0.00655965, 0.05472157, 0.11098164, 0.05472157, 0.00655965, 0.00019117},
                                                                           {0.00002292, 0.00078633, 0.00655965, 0.01330373, 0.00655965, 0.00078633, 0.00002292},
                                                                           {0.00000067, 0.00002292, 0.00019117, 0.00038771, 0.00019117, 0.00002292, 0.00000067}};
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == true)
            {
                orirginalSourceBitmap = new BitmapImage(new Uri(dlg.FileName));
                displayedSourceBitmap = orirginalSourceBitmap;
                image.Effect = null;
                label.Visibility = Visibility.Hidden;
                image.Source = displayedSourceBitmap;
            }
        }
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Rotate_Click(object sender, RoutedEventArgs e)
        {
            int width = displayedSourceBitmap.PixelWidth;
            int height = displayedSourceBitmap.PixelHeight;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            byte[] bytes = new byte[height * stride];
            displayedSourceBitmap.CopyPixels(bytes, stride, 0);

            UInt32[] pixels = new UInt32[bytes.Length / bytesPerPixel];
            for (int i = 0, j = 0; i < bytes.Length; i += bytesPerPixel, j++)
            {
                pixels[j] = BitConverter.ToUInt32(bytes, i);
            }
            UInt32[] transformedPixels = new UInt32[pixels.Length];

            int newWidth = height;
            int newHeight = width;

            var newStride = newWidth * bytesPerPixel;

            Button button = sender as Button;
            if (button.Name.Equals("rotate90"))
            {
                for (int y = 0, i = width - 1; y < newHeight; y++, i--)
                {
                    for (int x = 0; x < newWidth; x++)
                    {
                        transformedPixels[y * newWidth + x] = pixels[width * x + i];
                    }
                }
            }
            else
            {
                for (int y = 0; y < newHeight; y++)
                {
                    for (int x = 0; x < newWidth; x++)
                    {
                        transformedPixels[y * newWidth + x] = pixels[(height - x - 1) * width + y];
                    }
                }
            }

            byte[] transformedBytes = new byte[newHeight * newStride];
            for (int i = 0, j = 0; i < transformedPixels.Length; i++, j += bytesPerPixel)
            {
                transformedBytes[j] = (byte)transformedPixels[i];
                transformedBytes[j + 1] = (byte)(transformedPixels[i] >> 8);
                transformedBytes[j + 2] = (byte)(transformedPixels[i] >> 16);
                transformedBytes[j + 3] = (byte)(transformedPixels[i] >> 24);
            }

            displayedSourceBitmap = BitmapSource.Create(newWidth, newHeight, displayedSourceBitmap.DpiY, 
                displayedSourceBitmap.DpiX, displayedSourceBitmap.Format, null, transformedBytes, newStride);
            image.Source = displayedSourceBitmap;
        }

        private void Mirror_Click(object sender, RoutedEventArgs e)
        {
            int width = displayedSourceBitmap.PixelWidth;
            int height = displayedSourceBitmap.PixelHeight;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            byte[] bytes = new byte[height * stride];
            displayedSourceBitmap.CopyPixels(bytes, stride, 0);

            UInt32[] pixels = new UInt32[bytes.Length / bytesPerPixel];
            for (int i = 0, j = 0; i < bytes.Length; i += bytesPerPixel, j++)
            {
                pixels[j] = BitConverter.ToUInt32(bytes, i);
            }

            UInt32[] transformedPixels = new UInt32[pixels.Length];
            Button button = sender as Button;
            
            if (button.Name.Equals("mirrorv"))
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0, i = width; x < width; x++, i--)
                    {
                        transformedPixels[y * width + x] = pixels[(y * width - 1) + i];
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        transformedPixels[y * width + x] = pixels[(height - y - 1) * width + x];
                    }
                }
            }
            byte[] transformedBytes = new byte[height * stride];
            for (int i = 0, j = 0; i < transformedPixels.Length; i++, j += bytesPerPixel)
            {
                transformedBytes[j] = (byte)transformedPixels[i];
                transformedBytes[j + 1] = (byte)(transformedPixels[i] >> 8);
                transformedBytes[j + 2] = (byte)(transformedPixels[i] >> 16);
                transformedBytes[j + 3] = (byte)(transformedPixels[i] >> 24);
            }

            displayedSourceBitmap = BitmapSource.Create(width, height, displayedSourceBitmap.DpiX,
                displayedSourceBitmap.DpiY, displayedSourceBitmap.Format, null, transformedBytes, stride);
            image.Source = displayedSourceBitmap;
        }
        private void Inversion_Click(object sender, RoutedEventArgs e)
        {
            int width = displayedSourceBitmap.PixelWidth;
            int height = displayedSourceBitmap.PixelHeight;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            byte[] bytes = new byte[height * stride];
            displayedSourceBitmap.CopyPixels(bytes, stride, 0);

            byte[] transformedBytes = new byte[bytes.Length];

            for (int i = 0; i < transformedBytes.Length; i++)
            {
                transformedBytes[i] = (byte) (255 - bytes[i] & 0xFF);
            }

            displayedSourceBitmap = BitmapSource.Create(width, height, displayedSourceBitmap.DpiX,
                displayedSourceBitmap.DpiY, displayedSourceBitmap.Format, null, transformedBytes, stride);
            image.Source = displayedSourceBitmap;
        }
        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            //BlurEffect myBlur = new BlurEffect();
            //myBlur.Radius += 5;
            //myBlur.KernelType = KernelType.Gaussian;
            //image.Effect = myBlur;

            int height = displayedSourceBitmap.PixelHeight;
            int width = displayedSourceBitmap.PixelWidth;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            byte[] bytes = new byte[height * stride];
            displayedSourceBitmap.CopyPixels(bytes, stride, 0);

            UInt32[,] pixels = new UInt32[height, width];
            UInt32[,] transformedPixels = new UInt32[height, width];

            int index = 0;
            for (int h = 0; h < height; h++)
                for (int w = 0; w < width; w++, index += bytesPerPixel)
                    pixels[h, w] = BitConverter.ToUInt32(bytes, index);

            // ---------------------------------------------------------------------------------

            int i, j, k, m, gap = (int)(matrixSize / 2);
            int tmpH = height + 2 * gap, tmpW = width + 2 * gap;
            UInt32[,] tmppixel = new UInt32[tmpH, tmpW];

            //заполнение временного расширенного изображения
            //углы
            for (i = 0; i < gap; i++)
                for (j = 0; j < gap; j++)
                {
                    tmppixel[i, j] = pixels[0, 0];
                    tmppixel[i, tmpW - 1 - j] = pixels[0, width - 1];
                    tmppixel[tmpH - 1 - i, j] = pixels[height - 1, 0];
                    tmppixel[tmpH - 1 - i, tmpW - 1 - j] = pixels[height - 1, width - 1];
                }
            //крайние левая и правая стороны
            for (i = gap; i < tmpH - gap; i++)
                for (j = 0; j < gap; j++)
                {
                    tmppixel[i, j] = pixels[i - gap, j];
                    tmppixel[i, tmpW - 1 - j] = pixels[i - gap, width - 1 - j];
                }
            //крайние верхняя и нижняя стороны
            for (i = 0; i < gap; i++)
                for (j = gap; j < tmpW - gap; j++)
                {
                    tmppixel[i, j] = pixels[i, j - gap];
                    tmppixel[tmpH - 1 - i, j] = pixels[height - 1 - i, j - gap];
                }
            //центр
            for (i = 0; i < height; i++)
                for (j = 0; j < width; j++)
                    tmppixel[i + gap, j + gap] = pixels[i, j];

            //применение ядра свертки
            RGB ColorOfPixel = new RGB();
            RGB ColorOfCell = new RGB();
            for (i = gap; i < tmpH - gap; i++)
                for (j = gap; j < tmpW - gap; j++)
                {
                    ColorOfPixel.R = 0;
                    ColorOfPixel.G = 0;
                    ColorOfPixel.B = 0;
                    for (k = 0; k < matrixSize; k++)
                        for (m = 0; m < matrixSize; m++)
                        {
                            ColorOfCell = CalculationOfColor(tmppixel[i - gap + k, j - gap + m], blur[k, m]);
                            ColorOfPixel.R += ColorOfCell.R;
                            ColorOfPixel.G += ColorOfCell.G;
                            ColorOfPixel.B += ColorOfCell.B;
                        }
                    //контролируем переполнение переменных
                    if (ColorOfPixel.R < 0) ColorOfPixel.R = 0;
                    if (ColorOfPixel.R > 255) ColorOfPixel.R = 255;
                    if (ColorOfPixel.G < 0) ColorOfPixel.G = 0;
                    if (ColorOfPixel.G > 255) ColorOfPixel.G = 255;
                    if (ColorOfPixel.B < 0) ColorOfPixel.B = 0;
                    if (ColorOfPixel.B > 255) ColorOfPixel.B = 255;

                    transformedPixels[i - gap, j - gap] = Build(ColorOfPixel);
                }
            byte[] transformedBytes = new byte[height * stride];

            var bit = 0;
            for (int x = 0; x < transformedPixels.GetUpperBound(0) + 1; x++ )
                for (int y = 0; y < transformedPixels.GetUpperBound(1) + 1; y++, bit += bytesPerPixel)
                {
                    transformedBytes[bit] = (byte)transformedPixels[x, y];
                    transformedBytes[bit + 1] = (byte)(transformedPixels[x, y] >> 8);
                    transformedBytes[bit + 2] = (byte)(transformedPixels[x, y] >> 16);
                    transformedBytes[bit + 3] = (byte)(transformedPixels[x, y] >> 24);
                }

            displayedSourceBitmap = BitmapSource.Create(width, height, displayedSourceBitmap.DpiX,
                displayedSourceBitmap.DpiY, displayedSourceBitmap.Format, null, transformedBytes, stride);
            image.Source = displayedSourceBitmap;
        }
        //сборка каналов
        public static UInt32 Build(RGB ColorOfPixel)
        {
            UInt32 Color;
            Color = 0xFF000000 | ((UInt32)ColorOfPixel.R << 16) | ((UInt32)ColorOfPixel.G << 8) | ((UInt32)ColorOfPixel.B);
            return Color;
        }
        //вычисление нового цвета
        public static RGB CalculationOfColor(UInt32 pixel, double coefficient)
        {
            RGB Color = new RGB();
            Color.R = (float)(coefficient * ((pixel & 0x00FF0000) >> 16));
            Color.G = (float)(coefficient * ((pixel & 0x0000FF00) >> 8));
            Color.B = (float)(coefficient * (pixel & 0x000000FF));
            return Color;
        }

    }
}
