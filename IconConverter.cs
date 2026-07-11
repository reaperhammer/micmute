// IconConverter.cs — Multi-resolution ICO generator
// Converts single-size .ico files into multi-resolution .ico files
// containing 16x16, 24x24, 32x32, 48x48, 64x64, and 256x256 entries.
//
// Uses progressive multi-pass downscaling (halving) for small sizes
// to produce sharp icons instead of blurry single-step resizes.
//
// Small sizes (<=64) are stored as BMP/DIB entries for maximum
// compatibility with Windows Explorer, taskbar, and csc.exe /win32icon.
// The 256x256 entry is stored as PNG per the ICO specification.
//
// Usage:
//   IconConverter.exe <icon1.ico> [icon2.ico] [icon3.ico] ...
//
// Each file is converted in-place. Back up your originals if needed.
//
// Build:
//   csc.exe /out:IconConverter.exe /nologo IconConverter.cs

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

class IconConverter
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("IconConverter — Multi-resolution ICO generator");
            Console.WriteLine();
            Console.WriteLine("Usage: IconConverter.exe <icon1.ico> [icon2.ico] ...");
            Console.WriteLine();
            Console.WriteLine("Converts single-size .ico files into multi-resolution .ico files");
            Console.WriteLine("containing 16, 24, 32, 48, 64, and 256 pixel entries.");
            Console.WriteLine("Files are converted in-place.");
            return 1;
        }

        int errors = 0;
        foreach (string arg in args)
        {
            if (!ConvertIcon(arg))
                errors++;
        }

        Console.WriteLine();
        Console.WriteLine(errors == 0 ? "Done!" : "Finished with " + errors + " error(s).");
        return errors;
    }

    static bool ConvertIcon(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("  ERROR: Not found: " + path);
            return false;
        }

        if (!path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("  ERROR: Not an .ico file: " + path);
            return false;
        }

        try
        {
            // Read raw ICO bytes and extract the largest image
            byte[] icoBytes = File.ReadAllBytes(path);

            // Validate ICO header: reserved (2 bytes) + type (2 bytes) + count (2 bytes)
            if (icoBytes.Length < 6 ||
                BitConverter.ToInt16(icoBytes, 0) != 0 ||
                BitConverter.ToInt16(icoBytes, 2) != 1)
            {
                Console.WriteLine("  ERROR: Invalid ICO header: " + path);
                return false;
            }

            int count = BitConverter.ToInt16(icoBytes, 4);
            if (count == 0)
            {
                Console.WriteLine("  ERROR: ICO contains no images: " + path);
                return false;
            }

            // Find the largest entry
            int bestIdx = -1;
            int bestSize = 0;
            for (int i = 0; i < count; i++)
            {
                int entryOffset = 6 + i * 16;
                int w = icoBytes[entryOffset];
                if (w == 0) w = 256;
                if (w > bestSize) { bestSize = w; bestIdx = i; }
            }

            // Extract the image data for the largest entry
            int bestEntryOffset = 6 + bestIdx * 16;
            int imgDataSize = BitConverter.ToInt32(icoBytes, bestEntryOffset + 8);
            int imgDataOffset = BitConverter.ToInt32(icoBytes, bestEntryOffset + 12);

            byte[] imgData = new byte[imgDataSize];
            Array.Copy(icoBytes, imgDataOffset, imgData, 0, imgDataSize);

            Bitmap srcBitmap;
            using (MemoryStream ms = new MemoryStream(imgData))
            {
                srcBitmap = new Bitmap(ms);
            }
            Console.WriteLine("  " + Path.GetFileName(path) + ": source=" + srcBitmap.Width + "x" + srcBitmap.Height);

            // Target sizes for the output ICO
            int[] sizes = { 16, 24, 32, 48, 64, 256 };
            byte[][] imageData = new byte[sizes.Length][];

            for (int i = 0; i < sizes.Length; i++)
            {
                int size = sizes[i];
                Bitmap resized;

                if (size == srcBitmap.Width)
                {
                    resized = (Bitmap)srcBitmap.Clone();
                }
                else if (size <= 48)
                {
                    // Multi-pass downscale for small sizes (halve progressively)
                    resized = MultiPassResize(srcBitmap, size);
                }
                else
                {
                    resized = SingleResize(srcBitmap, size);
                }

                if (size == 256)
                {
                    // 256x256 stored as PNG per ICO spec
                    using (MemoryStream ms = new MemoryStream())
                    {
                        resized.Save(ms, ImageFormat.Png);
                        imageData[i] = ms.ToArray();
                    }
                }
                else
                {
                    // Smaller sizes stored as BMP/DIB for compatibility
                    imageData[i] = CreateDibData(resized, size);
                }

                if (resized != srcBitmap)
                    resized.Dispose();
            }

            srcBitmap.Dispose();

            // Write the multi-resolution ICO file (in-place)
            using (FileStream fs = File.Create(path))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                // ICO Header
                bw.Write((short)0);            // Reserved
                bw.Write((short)1);            // Type: 1 = ICO
                bw.Write((short)sizes.Length);  // Number of images

                int dataOff = 6 + sizes.Length * 16;

                // Directory entries
                for (int i = 0; i < sizes.Length; i++)
                {
                    int size = sizes[i];
                    bw.Write((byte)(size >= 256 ? 0 : size));  // Width
                    bw.Write((byte)(size >= 256 ? 0 : size));  // Height
                    bw.Write((byte)0);                          // Color palette
                    bw.Write((byte)0);                          // Reserved
                    bw.Write((short)1);                         // Color planes
                    bw.Write((short)32);                        // Bits per pixel
                    bw.Write(imageData[i].Length);               // Image data size
                    bw.Write(dataOff);                          // Offset
                    dataOff += imageData[i].Length;
                }

                // Image data
                for (int i = 0; i < sizes.Length; i++)
                    bw.Write(imageData[i]);
            }

            Console.WriteLine("    -> " + sizes.Length + " sizes, " + new FileInfo(path).Length + " bytes");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("  ERROR processing " + path + ": " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Multi-pass resize: halve the image repeatedly until within 2x of the
    /// target, then do a final resize. This produces much sharper small icons
    /// than a single jump from e.g. 256 to 16.
    /// </summary>
    static Bitmap MultiPassResize(Bitmap src, int targetSize)
    {
        Bitmap current = (Bitmap)src.Clone();

        while (current.Width > targetSize * 2)
        {
            int halfW = current.Width / 2;
            int halfH = current.Height / 2;
            Bitmap half = new Bitmap(halfW, halfH, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(half))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(current, 0, 0, halfW, halfH);
            }
            current.Dispose();
            current = half;
        }

        Bitmap result = new Bitmap(targetSize, targetSize, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(result))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(current, 0, 0, targetSize, targetSize);
        }
        current.Dispose();

        return result;
    }

    static Bitmap SingleResize(Bitmap src, int size)
    {
        Bitmap result = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(result))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(src, 0, 0, size, size);
        }
        return result;
    }

    /// <summary>
    /// Creates a BMP/DIB byte array for an ICO entry (BITMAPINFOHEADER + XOR mask + AND mask).
    /// </summary>
    static byte[] CreateDibData(Bitmap bmp, int size)
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            int andMaskRowSize = ((size + 31) / 32) * 4;
            int andMaskSize = andMaskRowSize * size;

            // BITMAPINFOHEADER (40 bytes)
            bw.Write(40);                                    // biSize
            bw.Write(size);                                  // biWidth
            bw.Write(size * 2);                              // biHeight (doubled: XOR + AND)
            bw.Write((short)1);                              // biPlanes
            bw.Write((short)32);                             // biBitCount
            bw.Write(0);                                     // biCompression = BI_RGB
            bw.Write(size * size * 4 + andMaskSize);         // biSizeImage
            bw.Write(0);                                     // biXPelsPerMeter
            bw.Write(0);                                     // biYPelsPerMeter
            bw.Write(0);                                     // biClrUsed
            bw.Write(0);                                     // biClrImportant

            // XOR mask — pixel data in bottom-up row order, BGRA format
            for (int y = size - 1; y >= 0; y--)
                for (int x = 0; x < size; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    bw.Write(c.B);
                    bw.Write(c.G);
                    bw.Write(c.R);
                    bw.Write(c.A);
                }

            // AND mask — all zeros (fully opaque for 32-bit ARGB)
            bw.Write(new byte[andMaskSize]);

            return ms.ToArray();
        }
    }
}
