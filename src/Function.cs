using System.Collections;
using System.Diagnostics;
using System.IO.Compression;

namespace multiver_archiver {
    internal class function {

        public static void Run() {

            string programPath = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Documents\\MultiverArchives";
            string lunarDir = Environment.GetEnvironmentVariable("USERPROFILE") + "\\.lunarclient\\offline\\multiver";
            string zipName = $"multiver_archive_{DateTime.Now.ToString("yyyy-MM-dd")}.zip";
            string zipPath = Path.Combine(programPath, zipName);

            if (!Directory.Exists(programPath)) Directory.CreateDirectory(programPath);

            if (Directory.Exists(lunarDir)) {

                Console.WriteLine("Found multiver folder at " + lunarDir + "\nContents:");

                Array readfiles = Directory.GetFiles(lunarDir);
                Array readdirectories = Directory.GetDirectories(lunarDir);

                ArrayList files = new ArrayList();
                ArrayList directories = new ArrayList();

                foreach (string file in readfiles) {
                    if (file.Contains(".log")) Console.WriteLine(file + " (Ignored)");
                    else { Console.WriteLine(file); files.Add(file); }
                }

                foreach (string directory in readdirectories) {
                    if (directory.Contains("logs") || directory.Contains("cache") || directory.Contains("ichor") || directory.Equals("overrides")) Console.WriteLine(directory + "\\ (Ignored)");
                    else { Console.WriteLine(directory + "\\"); directories.Add(directory); }
                }

                Console.WriteLine("Filtered files contents:");

                foreach (string directory in directories) Console.WriteLine(directory);
                foreach (string file in files) Console.WriteLine(file);

                CreateZipFile(zipPath, directories, files);
                Console.WriteLine("SUCCESS: Press any key to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else {
                Console.WriteLine("ERROR: Could not find multiver at " + lunarDir + "[Do you have lunar installed?]\n");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        private static void CreateZipFile(String writeDirectory, ArrayList directories, ArrayList files) {
            try {
                using (ZipArchive archive = ZipFile.Open(writeDirectory, ZipArchiveMode.Create)) {
                    foreach (string file in files) archive.CreateEntryFromFile(file, Path.GetRelativePath(writeDirectory, file));
                    foreach (string directory in directories) AddDirectoryToZip(archive, directory, writeDirectory);
                }
                Console.WriteLine("Successfully compressed to " + writeDirectory);
            }
            catch (Exception e) { Console.WriteLine("EXCEPTION " + e.ToString()); }
        }

        private static void AddDirectoryToZip(ZipArchive archive, string directory, string writeDirectory) {
            string[] subDirectories = Directory.GetDirectories(directory);
            foreach (string subDir in subDirectories) {
                AddDirectoryToZip(archive, subDir, writeDirectory);
            }

            string[] files = Directory.GetFiles(directory);
            foreach (string file in files) {
                archive.CreateEntryFromFile(file, Path.GetRelativePath(writeDirectory, file));
            }
        }
    }
}
