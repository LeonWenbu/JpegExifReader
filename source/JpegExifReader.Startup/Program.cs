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
            //if (args.Length == 0)
            //{
            //    Console.WriteLine("Please provide the path to a JPEG image file.");
            //    return;
            //}

            //string imagePath = args[0];
            // for test
            string imagePath = "C:\\Users\\lhuo\\OneDrive - SS&C Technologies, Inc\\Pictures\\1379178870.jpg";

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
                        Console.WriteLine("图像位深: 不可用");
                    }

                    var compressionRatio = ExifReader.EstimateCompressionRatio(directories, fileInfo.Length);
                    if (compressionRatio.HasValue)
                    {
                        Console.WriteLine($"Estimated compression ratio (uncompressed:compressed) = {compressionRatio.Value:F2}:1");
                    }
                    else
                    {
                        Console.WriteLine("Estimated compression ratio: unavailable.");
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
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("====================================================");
            Console.WriteLine("Press Enter to exit...");
            Console.ReadKey();
        }
    }
}