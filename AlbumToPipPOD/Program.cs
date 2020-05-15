using System;
using System.IO;
using NAudio.Wave;

namespace AlbumToPipPOD
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string type = "*.mp3";
            int count = 30;
            string outputFolder = "PersonalRadio";

            try
            {
                if (args.Length == 0 || Array.Exists(args, s => s.Equals("help"))) // if help needed
                {
                    Console.WriteLine("Help:\n\tAlbumToPipPOD <folder> [--t <type>] [--c <count>] [--o <path>]");
                    Console.WriteLine("--t <type>:\n\tType of files format. [Default: mp3]\n\tFormats supported :\n\t\tmp3\n\t\twav");
                    Console.WriteLine("--c <count>:\n\tThe max of files supported by PipPOD (fixed at installation of the mod). [Default: 30]");
                    Console.WriteLine("--o <path>:\n\tSets the path output directory. [Default: <folder>/PersonalRadio]");
                }
                else
                {
                    // if the format is not mp4
                    if (Array.Exists(args, s => s.Equals("--t")))
                        type = "*." + args[Array.IndexOf(args, "--t") + 1];

                    // if the count of maximum files is not 30
                    if (Array.Exists(args, s => s.Equals("--c")))
                        count = int.Parse(args[Array.IndexOf(args, "--c") + 1]);

                    outputFolder = Path.Combine(args[0], outputFolder);
                    // if there is a custom output folder
                    if (Array.Exists(args, s => s.Equals("--o")))
                        outputFolder = args[Array.IndexOf(args, "--o") + 1];

                    string[] files = Directory.GetFiles(args[0], type); // list all files of the type
                    Directory.CreateDirectory(outputFolder); // create output directory

                    for (int i = 0; i < files.Length; i++)
                    {
                        Console.WriteLine($"Converting {Path.GetFileNameWithoutExtension(files[i])}"); // I like files[i].Split('\\')[^1].Split('.')[^2] but this one is better...
                        if (type == "*.mp3")
                            ConvertMp3ToWav(files[i], Path.Combine(outputFolder, $"{i + 1}.wav"));
                        else if (type == "*.wav")
                        {
                            using (var sourceStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                            using (var destinationStream = new FileStream(Path.Combine(outputFolder, $"{i + 1}.wav"), FileMode.Create, FileAccess.Write))
                                sourceStream.CopyTo(destinationStream);
                        }
                        else
                            throw new NotSupportedException();
                        if (i + 1 == 30)
                            break;
                    }

                    Console.WriteLine("Conversion successful");
                    if (files.Length < 30)
                        Console.WriteLine($"WARNING: Not enough files to be a full radio {files.Length}/{count}");
                    else if (files.Length > 30)
                        Console.WriteLine($"WARNING: Too much files, only the firsts {count}/{files.Length} files were converted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }
        }

        // Credits to https://stackoverflow.com/a/33088993/13257820
        private static void ConvertMp3ToWav(string inPath, string outPath)
        {
            using (var mp3 = new Mp3FileReader(inPath))
            using (var pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                WaveFileWriter.CreateWaveFile(outPath, pcm);
        }
    }
}