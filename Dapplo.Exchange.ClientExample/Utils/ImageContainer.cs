using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Xmp;
using Microsoft.Exchange.WebServices.Data;

namespace Dapplo.Exchange.ClientExample.Utils
{
    /// <summary>
    /// Container for images, supporting Exif orientation
    /// </summary>
    public class ImageContainer
    {
        /// <summary>
        /// Tests if the supplied attachment can be seen as an image
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public static bool IsImage(Attachment attachment)
        {
            if (attachment.IsInline)
            {
                return false;
            }
            if (attachment.ContentType == null)
            {
                if (!attachment.Name.EndsWith(".jpg") && !attachment.Name.EndsWith(".png") && !attachment.Name.EndsWith(".jpeg") && !attachment.Name.EndsWith(".bmp"))
                {
                    return false;
                }
            } else if (!attachment.ContentType.Contains("image"))
            {
                return false;
            }
            var fileAttachment = attachment as FileAttachment;
            return fileAttachment != null;
        }

        /// <summary>
        /// Name of the file
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// The actual image
        /// </summary>
        public ImageSource ImageData { get; private set; }

        /// <summary>
        /// Exif information
        /// </summary>
        public IEnumerable<MetadataExtractor.Directory> Metadata { get; private set; } = Enumerable.Empty<MetadataExtractor.Directory>();

        /// <summary>
        /// Dummy
        /// </summary>
        public ImageContainer()
        {
            ImageData = new BitmapImage();
        }

        /// <summary>
        /// Dummy
        /// </summary>
        public ImageContainer(string filename)
        {
            ReadFrom(filename);
        }

        /// <summary>
        /// handle reading from a memorystream
        /// </summary>
        private void ReadFrom(MemoryStream memoryStream)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);

            Metadata = ImageMetadataReader.ReadMetadata(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var bitmapImage = new BitmapImage();
            ImageData = bitmapImage;
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
        }

        /// <summary>
        /// handle reading from a file
        /// </summary>
        public void ReadFrom(string filename)
        {
            using (var filestream = File.OpenRead(filename))
            {
                var memoryStream = new MemoryStream();
                filestream.CopyTo(memoryStream);
                ReadFrom(memoryStream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attachment"></param>
        public ImageContainer(Attachment attachment)
        {
            Filename = attachment.Name;

            var fileAttachment = attachment as FileAttachment;

            var memoryStream = new MemoryStream();
            fileAttachment?.Load(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            ReadFrom(memoryStream);
        }

        /// <summary>
        /// Returns the rotation
        /// </summary>
        public int Rotation
        {
            get
            {
                var orientation = (ushort?)Metadata?.OfType<ExifIfd0Directory>().FirstOrDefault()?.GetObject(ExifDirectoryBase.TagOrientation);
                if (!orientation.HasValue)
                {
                    return 0;
                }
                switch (orientation.Value)
                {
                    case 6:
                        return 90;
                    case 8:
                        return 270;
                    case 3:
                        return 180;
                    default:
                        return 0;
                }
            }
        }
    }
}
