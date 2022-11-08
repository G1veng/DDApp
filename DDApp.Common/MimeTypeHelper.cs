using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common
{
    public class MimeTypeHelper
    {
        private static readonly byte[] BMP = { 66, 77 };
        private static readonly byte[] GIF = { 71, 73, 70, 56 };
        private static readonly byte[] ICO = { 0, 0, 1, 0 };
        private static readonly byte[] JPG = { 255, 216, 255 };
        private static readonly byte[] PNG = { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 };
        private static readonly byte[] TIFF = { 73, 73, 42, 0 };
        private static readonly byte[] MP4 = { 0, 0, 0, 24, 102, 116, 121, 112, 52, 50, 0, 0, 0, 0, 105, 115, 111, 109, 109, 112, 52, 20};

        private static readonly List<string> videoTypes = new List<string>
        {
            "video/mp4",
        };

        ///<summary>
        ///Проверяет, явялется ли файл типа изображения
        ///</summary>
        public static bool CheckImageMimeType(byte[] file)
        {
            if (file.Take(2).SequenceEqual(BMP))
            {
                return true;
            }
            if (file.Take(4).SequenceEqual(GIF))
            {
                return true;
            }
            if (file.Take(4).SequenceEqual(ICO))
            {
                return true;
            }
            if (file.Take(3).SequenceEqual(JPG))
            {
                return true;
            }
            if (file.Take(16).SequenceEqual(PNG))
            {
                return true;
            }
            if (file.Take(4).SequenceEqual(TIFF))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверяет, является ли файл видео файлом (работает только с mp4)
        /// </summary>
        public static bool CheckVideoMimeType(byte[] file)
        {
            if (file.Take(23).SequenceEqual(MP4))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверяет является ли файл видео файлом, пока только mp4
        /// </summary>
        public static bool CheckVideoMimeTypeByMimeType(string mimeType)
        {
            if (videoTypes.Contains(mimeType))
            {
                return true;
            }

            return false;
        }
    }
}
