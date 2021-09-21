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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal BitmapSource orirginalSourceBitmap = null;
        //internal BitmapSource transformedSourceBitmap = null;
        internal BitmapSource displayedSourceBitmap = null;
        //public static UInt32[,] pixel;
        public static Bitmap bitmap = null;
        //public BlurEffect myBlur = new BlurEffect();

        //размытие
        public const int matrixSize = 3;
        public static double[,] blur = new double[matrixSize, matrixSize] {{0.111, 0.111, 0.111},
                                                                           {0.111, 0.111, 0.111},
                                                                           {0.111, 0.111, 0.111}};
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

            //int width, height;

            //if (transformedSourceBitmap == null)
            //{
            //    width = orirginalSourceBitmap.PixelWidth;
            //    height = orirginalSourceBitmap.PixelHeight;
            //}
            //else
            //{
            //    width = transformedSourceBitmap.PixelWidth;
            //    height = transformedSourceBitmap.PixelHeight;
            //}
            //UInt32[,] rotatedPixels = new UInt32[width, height];
            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        //rotatedPixels[width - x - 1, y] = pixel[y, x];
            //        rotatedPixels[x, height - y - 1] = pixel[y, x];
            //    }
            //}
            //pixel = rotatedPixels;
            //bitmap = new Bitmap(height, width, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            //for (int y = 0; y < bitmap.Height; y++)
            //    for (int x = 0; x < bitmap.Width; x++)
            //        bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb((int)pixel[y, x]));

            //transformedSourceBitmap = ConvertToBitmapImage(bitmap);
            //image.Source = transformedSourceBitmap;
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
            //myBlur.Radius += 5;
            //myBlur.KernelType = KernelType.Gaussian;
            //image.Effect = myBlur;



            //bitmap = ConvertToBitmap(orirginalSourceBitmap);

            //pixel = new UInt32[bitmap.Height, bitmap.Width];
            //for (int y = 0; y < bitmap.Height; y++)
            //    for (int x = 0; x < bitmap.Width; x++)
            //        pixel[y, x] = (UInt32)(bitmap.GetPixel(x, y).ToArgb());

            int height = displayedSourceBitmap.PixelHeight;
            int width = displayedSourceBitmap.PixelWidth;

            var bytesPerPixel = displayedSourceBitmap.Format.BitsPerPixel / 8;
            var stride = width * bytesPerPixel;

            byte[] bytes = new byte[height * stride];
            displayedSourceBitmap.CopyPixels(bytes, stride, 0);

            UInt32[,] pixels = new UInt32[height, width];

            for (int h = 0; h < height; h++)
                for (int w = 0; w < width; w++)
                    pixels[h, w] = BitConverter.ToUInt32(bytes, i);

            for (int i = 0, j = 0; i < bytes.Length; i += bytesPerPixel, j++)
            {
                pixels[i, j] = BitConverter.ToUInt32(bytes, i);
            }
            UInt32[] transformedPixels = new UInt32[pixels.Length];



        }
        //public static Bitmap ConvertToBitmap(BitmapSource bitmapSource)
        //{
        //    var width = bitmapSource.PixelWidth;
        //    var height = bitmapSource.PixelHeight;
        //    var stride = width * (bitmapSource.Format.BitsPerPixel / 8);
        //    var memoryBlockPointer = Marshal.AllocHGlobal(height * stride);
        //    bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), memoryBlockPointer, height * stride, stride);
        //    var bitmap = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, memoryBlockPointer);
        //    return bitmap;
        //}

        //public BitmapImage ConvertToBitmapImage(Bitmap bitmap)
        //{
        //    using (MemoryStream memory = new MemoryStream())
        //    {
        //        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        //        memory.Position = 0;
        //        BitmapImage bitmapimage = new BitmapImage();
        //        bitmapimage.BeginInit();
        //        bitmapimage.StreamSource = memory;
        //        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapimage.EndInit();
        //        return bitmapimage;
        //    }
        //}

    }
}
