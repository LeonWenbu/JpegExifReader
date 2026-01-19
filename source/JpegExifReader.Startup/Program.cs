using JpegExifReader.Common;
using MetadataExtractor.Formats.Jpeg;
using System.IO;
using System.Linq;

namespace JpegExifReader.Startup
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("没有指定jpg文件。");
                return;
            }

            string imagePath = args[0];
            // for test
            //string imagePath = "C:\\Users\\lhuo\\OneDrive - SS&C Technologies, Inc\\Pictures\\AI method.png";

            var fileInfo = new FileInfo(imagePath);


            try
            {
                var directories = ExifReader.ReadDirectories(imagePath);
                var jpegDirectory = directories.OfType<JpegDirectory>().FirstOrDefault();


                var exifData = ExifReader.ReadExifData(directories).ToList();

                var width = ExifReader.GetWidth(exifData);
                var height = ExifReader.GetHeight(exifData);

                if (exifData.Count == 0)
                {
                    Console.WriteLine("没有找到 EXIF 数据。");
                }
                else
                {
                    Console.WriteLine("====================================================");
                    Console.WriteLine("文件名: " + imagePath);
                    Console.WriteLine("====================================================");

                    Console.WriteLine("JPEG图像数据:");
                    if (width.HasValue && height.HasValue)
                    {
                        Console.WriteLine($"图像尺寸: {width.Value} x {height.Value} 像素");
                    }
                    else
                    {
                        Console.WriteLine("图像尺寸: 不可用");
                    }

                    var readableSize = FileInfoReader.GetFileSizeWithUnits(imagePath);
                    Console.WriteLine($"文件大小: {readableSize}");

                    var bitDepth = ExifReader.GetBitDepth(directories);
                    if (bitDepth.HasValue)
                    {
                        Console.WriteLine($"图像位深: {bitDepth.Value} 位颜色");
                    }
                    else
                    {
                        Console.WriteLine("图像位深: 不可用。");
                    }

                    var compressionRatio = ExifReader.EstimateCompressionRatio(directories, fileInfo.Length);
                    if (compressionRatio.HasValue)
                    {
                        Console.WriteLine($"压缩因子 (未压缩大小:压缩后大小) = {compressionRatio.Value:F2}:1");
                    }
                    else
                    {
                        Console.WriteLine("压缩因子: 不可用。");
                    }

                    const string columnFormat = "{0,-15} | {1,-35} | {2}";
                    Console.WriteLine(columnFormat, "Directory", "Tag", "Value");
                    Console.WriteLine(new string('-', 90));

                    foreach (var (directory, tagName, tagDescription) in exifData)
                    {
                        Console.WriteLine(columnFormat, directory, tagName, tagDescription ?? string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"出现错误: {ex.Message}");
            }

            Console.WriteLine("====================================================");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }
    }
}