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
using Color = System.Drawing.Color;

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
        public const int matrixSize = 3;
        //public static double[,] blur;
        //public static double[,] blur = new double[matrixSize, matrixSize] {{0.111, 0.111, 0.111},
        //                                                                   {0.111, 0.111, 0.111},
        //                                                                   {0.111, 0.111, 0.111}};
        public static double[,] blur = new double[matrixSize, matrixSize] {{-1, -1, -1},
                                                                           {-1, 9, -1},
                                                                           {-1, -1, -1}};
        //public static double[,] blur = new double[matrixSize, matrixSize] {{0.000789, 0.006581, 0.013347, 0.006581, 0.000789},
        //                                                                   {0.006581, 0.054901, 0.111345, 0.054901, 0.006581},
        //                                                                   {0.013347, 0.111345, 0.225821, 0.111345, 0.013347},
        //                                                                   {0.006581, 0.054901, 0.111345, 0.054901, 0.006581},
        //                                                                   {0.000789, 0.006581, 0.013347, 0.006581, 0.000789}};


        //public static double[,] blur = new double[matrixSize, matrixSize] {{0.00000067, 0.00002292, 0.00019117, 0.00038771, 0.00019117, 0.00002292, 0.00000067},
        //                                                                   {0.00002292, 0.00078633, 0.00655965, 0.01330373, 0.00655965, 0.00078633, 0.00002292},
        //                                                                   {0.00019117, 0.00655965, 0.05472157, 0.11098164, 0.05472157, 0.00655965, 0.00019117},
        //                                                                   {0.00038771, 0.01330373, 0.11098164, 0.22508352, 0.11098164, 0.01330373, 0.00038771},
        //                                                                   {0.00019117, 0.00655965, 0.05472157, 0.11098164, 0.05472157, 0.00655965, 0.00019117},
        //                                                                   {0.00002292, 0.00078633, 0.00655965, 0.01330373, 0.00655965, 0.00078633, 0.00002292},
        //                                                                   {0.00000067, 0.00002292, 0.00019117, 0.00038771, 0.00019117, 0.00002292, 0.00000067}};
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

            //GussianBlur();
            // return;

            //if (displayedSourceBitmap.Format != PixelFormats.Bgra32)
            //    displayedSourceBitmap = new FormatConvertedBitmap(displayedSourceBitmap, PixelFormats.Bgra32, null, 0);

            //if (blur == null)
            //{
            //    blur = GetMatrix(0.6, 5);
            //}

            //int matrixSize = blur.GetUpperBound(0);

            int height = displayedSourceBitmap.PixelHeight;
            int width = displayedSourceBitmap.PixelWidth;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            byte[] bytes = new byte[height * stride];
            displayedSourceBitmap.CopyPixels(bytes, stride, 0);

            int[,] pixels = new int[height, width];
            int[,] transformedPixels = new int[height, width];

            int index = 0;
            for (int h = 0; h < height; h++)
                for (int w = 0; w < width; w++, index += bytesPerPixel)
                    pixels[h, w] = BitConverter.ToInt32(bytes, index);
            // ---------------------------------------------------------------------------------
            int i, j, k, m, gap = (int)(matrixSize / 2);
            int tmpH = height + 2 * gap, tmpW = width + 2 * gap;
            int[,] tmppixel = new int[tmpH, tmpW];

            //int height = displayedSourceBitmap.PixelHeight;
            //int width = displayedSourceBitmap.PixelWidth;

            //var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            //var stride = width * bytesPerPixel;

            //byte[] bytes = new byte[height * stride];
            //displayedSourceBitmap.CopyPixels(bytes, stride, 0);

            //UInt32[,] pixels = new UInt32[height, width];
            //UInt32[,] transformedPixels = new UInt32[height, width];

            //int index = 0;
            //for (int h = 0; h < height; h++)
            //    for (int w = 0; w < width; w++, index += bytesPerPixel)
            //        pixels[h, w] = BitConverter.ToUInt32(bytes, index);

            //// ---------------------------------------------------------------------------------

            //int i, j, k, m, gap = (int)(matrixSize / 2);
            //int tmpH = height + 2 * gap, tmpW = width + 2 * gap;
            //UInt32[,] tmppixel = new UInt32[tmpH, tmpW];

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

            for (i = gap; i < tmpH - gap; i++)
                for (j = gap; j < tmpW - gap; j++)
                {
                    double[] sum = new double[4];
                    for (k = 0; k < matrixSize; k++)
                        for (m = 0; m < matrixSize; m++)
                        {
                            Color color = Color.FromArgb(tmppixel[i - gap + k, j - gap + m]);
                            sum[0] = sum[0] + color.A * blur[k, m];
                            sum[1] = sum[1] + color.R * blur[k, m];
                            sum[2] = sum[2] + color.G * blur[k, m];
                            sum[3] = sum[3] + color.B * blur[k, m];
                        }
                    if (sum[0] < 0) sum[0] = 0;
                    if (sum[0] > 255) sum[0] = 255;
                    if (sum[1] < 0) sum[1] = 0;
                    if (sum[1] > 255) sum[1] = 255;
                    if (sum[2] < 0) sum[2] = 0;
                    if (sum[2] > 255) sum[2] = 255;
                    if (sum[3] < 0) sum[3] = 0;
                    if (sum[3] > 255) sum[3] = 255;

                    Color rc = Color.FromArgb((int)sum[0], (int)sum[1], (int)sum[2], (int)sum[3]);
                    transformedPixels[i - gap, j - gap] = rc.ToArgb();
                }
            byte[] transformedBytes = new byte[height * stride];
            ////применение ядра свертки
            //RGB ColorOfPixel = new RGB();
            //RGB ColorOfCell = new RGB();
            //for (i = gap; i < tmpH - gap; i++)
            //    for (j = gap; j < tmpW - gap; j++)
            //    {
            //        ColorOfPixel.R = 0;
            //        ColorOfPixel.G = 0;
            //        ColorOfPixel.B = 0;
            //        for (k = 0; k < matrixSize; k++)
            //            for (m = 0; m < matrixSize; m++)
            //            {
            //                ColorOfCell = CalculationOfColor(tmppixel[i - gap + k, j - gap + m], blur[k, m]);
            //                ColorOfPixel.R += ColorOfCell.R;
            //                ColorOfPixel.G += ColorOfCell.G;
            //                ColorOfPixel.B += ColorOfCell.B;
            //            }
            //        //контролируем переполнение переменных
            //        if (ColorOfPixel.R < 0) ColorOfPixel.R = 0;
            //        if (ColorOfPixel.R > 255) ColorOfPixel.R = 255;
            //        if (ColorOfPixel.G < 0) ColorOfPixel.G = 0;
            //        if (ColorOfPixel.G > 255) ColorOfPixel.G = 255;
            //        if (ColorOfPixel.B < 0) ColorOfPixel.B = 0;
            //        if (ColorOfPixel.B > 255) ColorOfPixel.B = 255;

            //        transformedPixels[i - gap, j - gap] = Build(ColorOfPixel);
            //    }
            //byte[] transformedBytes = new byte[height * stride];

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
        ////сборка каналов
        //public static UInt32 Build(RGB ColorOfPixel)
        //{
        //    UInt32 Color;
        //    Color = 0xFF000000 | ((UInt32)ColorOfPixel.R << 16) | ((UInt32)ColorOfPixel.G << 8) | ((UInt32)ColorOfPixel.B);
        //    return Color;
        //}
        ////вычисление нового цвета
        //public static RGB CalculationOfColor(UInt32 pixel, double coefficient)
        //{
        //    RGB Color = new RGB();
        //    Color.R = (float)(coefficient * ((pixel & 0x00FF0000) >> 16));
        //    Color.G = (float)(coefficient * ((pixel & 0x0000FF00) >> 8));
        //    Color.B = (float)(coefficient * (pixel & 0x000000FF));
        //    return Color;
        //}

        public static double[,] GetMatrix(double sigma, int W)
        {
            double[,] kernel = new double[W, W];
            double mean = W / 2;
            double sum = 0.0; // For accumulating the kernel values

            double c_part = 1 / (2 * Math.PI * sigma * sigma);

            for (int x = 0; x < W; ++x) 
                for (int y = 0; y < W; ++y) {

                    kernel[x, y] = c_part * Math.Exp(-(Math.Pow(x - mean, 2.0) + Math.Pow(y - mean, 2.0)) / (2 * Math.Pow(sigma, 2.0)));

                    //kernel[x, y] = Math.Exp(-0.5 * (Math.Pow((x - mean) / sigma, 2.0) + Math.Pow((y - mean) / sigma, 2.0)))
                    //             / (2 * Math.PI * sigma * sigma);

                    // Accumulate the kernel values
                    sum += kernel[x, y];
            }

            // Normalize the kernel
            for (int x = 0; x < W; ++x) 
                for (int y = 0; y < W; ++y)
                    kernel[x, y] /= sum;

            return kernel;
        }

        private static int sigma = 5;
        private static int radius = 3 * sigma;
        private static double[] kernel = new double[radius + 1];

        public void GussianBlur()
        {
            initKernel();

            if (displayedSourceBitmap.Format != PixelFormats.Bgra32)
                displayedSourceBitmap = new FormatConvertedBitmap(displayedSourceBitmap, PixelFormats.Bgra32, null, 0);

            int h = displayedSourceBitmap.PixelHeight;
            int w = displayedSourceBitmap.PixelWidth;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = w * bytesPerPixel;

            byte[] bytes = new byte[h * stride];
            displayedSourceBitmap.CopyPixels(bytes, stride, 0);

            int[] pix = new int[bytes.Length / bytesPerPixel];
            for (int i = 0, j = 0; i < bytes.Length; i += bytesPerPixel, j++)
            {
                pix[j] = BitConverter.ToInt32(bytes, i);
            }

            int[] tmp = new int[bytes.Length / bytesPerPixel];
            int[] result = new int[bytes.Length / bytesPerPixel];

            for (int i = 0; i < w * h; i++)
            {
                int x = i % w;
                int y = i / w;
                double[] sum = new double[4];
                for (int j = -radius; j <= radius; j++)
                {
                    int currentX = Math.Min(Math.Max(x + j, 0), w - 1);
                    int index = y * w + currentX;
                    Color color = Color.FromArgb(pix[index]);
                    sum[0] = sum[0] + color.A * kernel[Math.Abs(j)];
                    sum[1] = sum[1] + color.R * kernel[Math.Abs(j)];
                    sum[2] = sum[2] + color.G * kernel[Math.Abs(j)];
                    sum[3] = sum[3] + color.B * kernel[Math.Abs(j)];
                }
                Color rc = Color.FromArgb((int)sum[0], (int)sum[1], (int)sum[2], (int)sum[3]);
                tmp[i] = rc.ToArgb();
            }
            for (int i = 0; i < w * h; i++)
            {
                int x = i % w;
                int y = i / w;
                double[] sum = new double[4];
                for (int j = -radius; j <= radius; j++)
                {
                    int currentY = Math.Min(Math.Max(y + j, 0), h - 1);
                    int index = currentY * w + x;
                    Color color = Color.FromArgb(tmp[index]);
                    sum[0] = sum[0] + color.A * kernel[Math.Abs(j)];
                    sum[1] = sum[1] + color.R * kernel[Math.Abs(j)];
                    sum[2] = sum[2] + color.G * kernel[Math.Abs(j)];
                    sum[3] = sum[3] + color.B * kernel[Math.Abs(j)];
                }
                Color rc = Color.FromArgb((int)sum[0], (int)sum[1], (int)sum[2], (int)sum[3]);
                result[i] = rc.ToArgb();
            }
            byte[] transformedBytes = new byte[h * stride];
            for (int i = 0, j = 0; i < result.Length; i++, j += bytesPerPixel)
            {
                transformedBytes[j] = (byte)result[i];
                transformedBytes[j + 1] = (byte)(result[i] >> 8);
                transformedBytes[j + 2] = (byte)(result[i] >> 16);
                transformedBytes[j + 3] = (byte)(result[i] >> 24);
            }
            displayedSourceBitmap = BitmapSource.Create(w, h, displayedSourceBitmap.DpiX,
                displayedSourceBitmap.DpiY, displayedSourceBitmap.Format, null, transformedBytes, stride);
            image.Source = displayedSourceBitmap;
        }
        private void initKernel()
        {
            double sum = 0.0;
            for (int i = 0; i < kernel.Length; i++)
            {
                kernel[i] = 1/Math.Sqrt(2 * Math.PI) * Math.Exp(-(i * i * 1.0) / (2.0 * sigma * sigma)) / sigma;
                if (i > 0)
                {
                    sum = sum + kernel[i] * 2.0;
                }
                else
                {
                    sum = sum + kernel[i];
                }
            }
            for (int i = 0; i < kernel.Length; i++)
            {
                kernel[i] = kernel[i] / sum;
            }
        }

        private void Sharp_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Median_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
