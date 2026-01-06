using System.IO;

namespace JpegExifReader.Common
{
    public static class FileInfoReader
    {
        private const long BytesPerKilobyte = 1024;
        private const long BytesPerMegabyte = BytesPerKilobyte * 1024;
        private const long BytesPerGigabyte = BytesPerMegabyte * 1024;

        public static long GetFileSize(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        public static string GetFileSizeWithUnits(string filePath)
        {
            var length = GetFileSize(filePath);
            return FormatFileSize(length);
        }

        public static string FormatFileSize(long fileSize)
        {
            if (fileSize >= BytesPerGigabyte)
            {
                return $"{fileSize / (double)BytesPerGigabyte:F2} GB";
            }

            if (fileSize >= BytesPerMegabyte)
            {
                return $"{fileSize / (double)BytesPerMegabyte:F2} MB";
            }

            if (fileSize >= BytesPerKilobyte)
            {
                return $"{fileSize / (double)BytesPerKilobyte:F2} KB";
            }

            return $"{fileSize} bytes";
        }
    }
}
