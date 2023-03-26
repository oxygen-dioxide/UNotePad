// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Text;
using UNotePad;

string DefaultTextEditor = "notepad";

try {
    var AppVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version;
    Console.WriteLine("Edit Lyric In Notepad version " + AppVersion.ToString());

    // Load Config
    var configPath = Path.Join(
        Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
        "config.yaml");
    Config config;
    try
    {
        config = Yaml.DefaultDeserializer.Deserialize<Config>(File.ReadAllText(configPath));
    }
    catch (Exception e) {
        config = new Config();
    }
    if (config.TextEditor == "")
    {
        config.TextEditor = DefaultTextEditor;
    }
    
    // Load temp ust file
    if(args.Length == 0)
    {
        Console.WriteLine("Please Launch this plugin from OpenUtau");
        Console.ReadLine();
        return;
    }
    var ustPath = args[0];
    var ustLines = File.ReadAllLines(ustPath, Encoding.UTF8);
    var lyricLineIndices = new List<int>();
    var lyrics = new StringBuilder();
    foreach (int i in Enumerable.Range(0,ustLines.Length))
    {
        string line = ustLines[i];
        if (line.StartsWith("Lyric="))
        {
            string lyric = line.Substring(6);
            if (lyric != "R")
            {
                lyricLineIndices.Add(i);
                lyrics.AppendLine(lyric);
            }
        }
    }
    //Write into temp txt file
    var txtPath = Path.GetFullPath("temp.txt");
    File.WriteAllText(txtPath, lyrics.ToString(),Encoding.UTF8);

    //Launch Text Editor
    var startInfo = new ProcessStartInfo()
    {
        FileName = config.TextEditor,
        Arguments = config.Argument+" "+txtPath,
        UseShellExecute = config.UseShell
    };
    using (var process = Process.Start(startInfo))
    {
        process.WaitForExit();
    }

    //Parse edited txt file
    var editedLyrics = File.ReadAllLines(txtPath, Encoding.UTF8);
    Enumerable.Zip(editedLyrics, 
        lyricLineIndices,
        (lyric, id) =>
        {
            ustLines[id] = "Lyric=" + lyric;
            return true;
        }
        ).Last();

    //write temp ust file
    File.WriteAllLines(ustPath, ustLines, Encoding.UTF8);
}
catch(Exception e) {
    Console.WriteLine(e);
    Console.ReadLine();
    return;
}