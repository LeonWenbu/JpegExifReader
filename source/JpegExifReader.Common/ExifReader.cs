using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using MetadataDirectory = MetadataExtractor.Directory;

namespace JpegExifReader.Common
{
    public class ExifReader
    {
        public static IReadOnlyList<MetadataDirectory> ReadDirectories(string imagePath)
        {
            var directories = ImageMetadataReader.ReadMetadata(imagePath);
            return directories ?? Array.Empty<MetadataDirectory>();
        }

        public static IEnumerable<Tuple<string, string, string>> ReadExifData(string imagePath)
        {
            var directories = ReadDirectories(imagePath);
            return ReadExifData(directories);
        }

        public static IEnumerable<Tuple<string, string, string>> ReadExifData(IEnumerable<MetadataDirectory> directories)
        {
            var exifData = new List<Tuple<string, string, string>>();
            if (directories == null)
            {
                return exifData;
            }

            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                {
                    exifData.Add(Tuple.Create(directory.Name, tag.Name, tag.Description ?? string.Empty));
                }
            }

            return exifData;
        }

        public static int? GetWidth(IEnumerable<Tuple<string, string, string>> directories)
        {
            if (directories == null || !directories.Any())
            {
                return null;
            }

            var widthTag = directories.FirstOrDefault(t => ConstValues.WidthTag.Contains(t.Item2));
            if (widthTag == null)
            {
                return null;
            }

            var widthPixil = widthTag.Item3.Split(' ')[0];

            if (int.TryParse(widthPixil, out var width) && width > 0)
            {
                return width;
            }

            return null;
        }

        public static int? GetHeight(IEnumerable<Tuple<string, string, string>> directories)
        {
            if (directories == null || !directories.Any())
            {
                return null;
            }
            var heightTag = directories.FirstOrDefault(t => ConstValues.HeightTag.Contains(t.Item2));
            if (heightTag == null)
            {
                return null;
            }

            var heightPixil = heightTag.Item3.Split(' ')[0];

            if (int.TryParse(heightPixil, out var height) && height > 0)
            {
                return height;
            }
            return null;
        }

        public static int? GetDataPrecision(IEnumerable<Tuple<string, string, string>> directories)
        {
            if (directories == null || !directories.Any())
            {
                return null;
            }
            var dataPrecisionTag = directories.FirstOrDefault(t => ConstValues.DataPrecisionTag.Contains(t.Item2));
            if (dataPrecisionTag == null)
            {
                return null;
            }

            var dataPrecision = dataPrecisionTag.Item3.Split(' ')[0];

            if (int.TryParse(dataPrecision, out var precision) && precision > 0)
            {
                return precision;
            }
            return null;
        }

        public static int? GetComponentsCount(IEnumerable<Tuple<string, string, string>> directories)
        {
            if (directories == null || !directories.Any())
            {
                return null;
            }
            var componentsTag = directories.FirstOrDefault(t => ConstValues.ComponentsTag.Contains(t.Item2));
            if (componentsTag == null)
            {
                return null;
            }

            var components = componentsTag.Item3.Split(' ')[0];

            if (int.TryParse(components, out var componentCount) && componentCount > 0)
            {
                return componentCount;
            }
            return null;
        }

        public static int? GetBitDepth(IEnumerable<Tuple<string, string, string>> directories)
        {
            if (directories == null || !directories.Any())
            {
                return null;
            }

            var bitDepthTag = directories.FirstOrDefault(t => ConstValues.BitDepthTag.Contains(t.Item2));
            if (bitDepthTag == null || string.IsNullOrWhiteSpace(bitDepthTag.Item3))
            {
                return null;
            }

            return ParseFirstPositiveInteger(bitDepthTag.Item3);
        }

        public static double? EstimateCompressionRatio(IEnumerable<MetadataDirectory> directories, long compressedSizeBytes)
        {
            if (directories == null || compressedSizeBytes <= 0)
            {
                return null;
            }

            var jpegDirectory = directories.OfType<JpegDirectory>().FirstOrDefault();
            if (jpegDirectory == null)
            {
                return null;
            }

            if (!jpegDirectory.TryGetInt32(JpegDirectory.TagImageWidth, out var width) || width <= 0 ||
                !jpegDirectory.TryGetInt32(JpegDirectory.TagImageHeight, out var height) || height <= 0)
            {
                return null;
            }

            var bitsPerSample = GetPositiveTagValue(directories, ExifDirectoryBase.TagBitsPerSample, 8);
            var components = GetPositiveTagValue(directories, ExifDirectoryBase.TagSamplesPerPixel, 3);

            var uncompressedBytes = width * (double)height * bitsPerSample * components / 8D;
            if (uncompressedBytes <= 0)
            {
                return null;
            }

            return uncompressedBytes / compressedSizeBytes;
        }

        public static int? GetBitDepth(IEnumerable<MetadataDirectory> directories)
        {
            var tags = ReadExifData(directories);
            var compCount = GetComponentsCount(tags);
            var dataPrecision = GetDataPrecision(tags);

            if (compCount.HasValue && dataPrecision.HasValue)
            {
                return compCount.Value * dataPrecision.Value;
            }

            return null;
        }

        private static int GetPositiveTagValue(IEnumerable<MetadataDirectory> directories, int tagType, int defaultValue)
        {
            var value = GetPositiveTagValueOrNull(directories, tagType);
            return value ?? defaultValue;
        }

        private static int? GetPositiveTagValueOrNull(IEnumerable<MetadataDirectory> directories, int tagType)
        {
            if (directories == null)
            {
                return null;
            }

            foreach (var directory in directories)
            {
                if (directory.TryGetInt32(tagType, out var value) && value > 0)
                {
                    return value;
                }
            }

            return null;
        }

        private static int? ParseFirstPositiveInteger(string rawValue)
        {
            var parts = rawValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (int.TryParse(part, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) && value > 0)
                {
                    return value;
                }
            }

            return null;
        }
    }
}
