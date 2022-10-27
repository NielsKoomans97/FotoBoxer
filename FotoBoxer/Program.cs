using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Text;

internal class Program
{
    public static Dictionary<DateTime, string> Log = new Dictionary<DateTime, string>();

    private static async Task Main(string[] args)
    {
        if (!args.Any())
        {
            Console.WriteLine("No arguments were passed");
            return;
        }

        var dir = args[0];
        var editDir = $"{dir.UpDir()}\\boxed";

        string[] images = new string[0];
        images = Directory.GetFiles(dir, "*.jpg");

        try
        {
            Directory.CreateDirectory(editDir);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[{DateTime.Now}] - {ex.Message}\n");
            Log.Add(DateTime.Now, ex.ToString());
            Console.ReadKey();

            return;
        }

        foreach (var image in images)
        {
            try
            {
                Console.Write($"Processing {image}... \r");
                await Task.Run(() => MakeBoxedEdit(image, editDir.CreateFileName(image)));
                Log.Add(DateTime.Now, $"Processed {image}\n");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[{DateTime.Now}] - {ex.Message}\n");
                Log.Add(DateTime.Now, ex.ToString());
            }
        }

        Console.WriteLine("\n\nDone processing images!");

        File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\log", JsonConvert.SerializeObject(Log));
        Console.ReadKey();
    }

    private static void MakeBoxedEdit(string fileName, string newFileName)
    {
        var origBitmap = (Bitmap)Image.FromFile(fileName);              // Open stream to original photo
        var corrRect = origBitmap.GetCorrectRect();                     // Get a rectangle with even width/height ratio

        var newBitmap = new Bitmap(corrRect.Width, corrRect.Height);    // Create new bitmap with dimensions from corrected rectangle
        var graphics = Graphics.FromImage(newBitmap);                   // Create graphics object with new bitmap
        var centRect = origBitmap.GetCenteredRect(corrRect);            // Create rectangle where the bounds of the original photo are centered

        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

        graphics.FillRectangle(Brushes.White, corrRect);                // Fill corrected rectangle with solid background White
        graphics.DrawImage(origBitmap, centRect);                       // Draw photo to newly created bitmap

        newBitmap.Save(newFileName);                                    // Save new bitmap
    }

    public static bool Check(object value)
    {
        return value == null;
    }
}

public static class StringExtensions
{
    public static string UpDir(this string dir)
    {
        var parts = dir.Split('\\');
        var builder = new StringBuilder();

        Array.ForEach(parts[0..(parts.Length - 1)],
                      a => builder.Append($"{a}\\"));

        return builder.ToString();
    }

    public static bool IsDirNameCorrect(this string dir)
    {
        var invalidDirChars = Path.GetInvalidPathChars();

        Array.ForEach(dir.ToCharArray(), chr =>
        {
            if (invalidDirChars.Any(ichr => ichr == chr))
                return;
        });

        return false;
    }

    public static string CreateFileName(this string newDir, string fileName)
    {
        return $"{newDir}\\{Path.GetFileName(fileName)}";
    }
}

public static class BitmapExtensions
{
    public static Rectangle GetCenteredRect(this Bitmap bitmap, Rectangle correctedRect)
    {
        return new Rectangle((correctedRect.Width / 2) - (bitmap.Width / 2), (correctedRect.Height / 2) - (bitmap.Height / 2), bitmap.Width, bitmap.Height);
    }

    public static Rectangle GetCorrectRect(this Bitmap bitmap)
    {
        if (bitmap.Width > bitmap.Height)
        {
            return new Rectangle(0, 0, bitmap.Width, bitmap.Width);
        }
        else
        {
            return new Rectangle(0, 0, bitmap.Height, bitmap.Height);
        }
    }
}