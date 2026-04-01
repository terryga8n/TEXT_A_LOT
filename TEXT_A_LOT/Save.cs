using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NfdExt;
using TEXT_A_LOT;

public   class SaveSystem


{
   public static  string outpath ="" ;
   public static  string? save ;
    public static string? b;
   public static SaveSystem s = new SaveSystem();

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

    public void saveFile()
    {
        if (outpath == "")
        {
            save = NFD.SaveDialog("C:\\", "file.txt",
              new Dictionary<string, string>
              {
                  ["Text File"] = "txt"
              });
            try
            {



                b = Text.ag.ToString();

                s.SaveDataAsync(save, b);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An I/O error occurred: {ex.Message}");
            }
        }
        else
        {
            b = Text.ag.ToString();

            s.SaveDataAsync(outpath, b);

        }
    }

    public void PickFileCrossPlatform()
    {
        // Opens the native OS file picker

        outpath = NFD.OpenDialog("C:\\",
        new Dictionary<string, string>
        {
            [outpath] = "txt"
        });

        string filePath = outpath; // Use an absolute path or a relative path
        try
        {
            string fileContents = File.ReadAllText(filePath);
            Console.WriteLine("File content:");
            Text.ag.Append(fileContents);
            
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"An I/O error occurred: {ex.Message}");
        }

    }


}
