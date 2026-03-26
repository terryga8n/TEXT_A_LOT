using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class SaveSystem
{
    public async Task SaveDataAsync(string filename, string  content)
    {
        // Get path to local app data folder (works on Windows, Linux, macOS)
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appFolder = Path.Combine(folder, "YourAppName");

        // Create directory if it doesn't exist
        Directory.CreateDirectory(appFolder);

        // Combine path safely
        string filePath = Path.Combine(appFolder, filename);

        // Save file asynchronously
        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
        Console.WriteLine($"Saved to: {filePath}");
    }
}
