using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AssetGenerator
{
    internal static class NpmHelper
    {
        /// <summary>
        /// Checks the generated models for errors with the glTF-Validator. 
        /// https://github.com/KhronosGroup/glTF-Validator
        /// </summary>
        public static void ValidateModels(string manifestPath, string outputFolder)
        {
            var projectDirectory = Directory.GetParent(outputFolder).ToString();
            var validatorGeneratorDirectory = Path.Combine(projectDirectory, "Validator");
            var logFile = Path.Combine(outputFolder, "ValidationResults.log");

            // Runs the ScreenshotGenerator.
            Console.WriteLine();
            Console.WriteLine("Validating models...");
            var psiNpmStartValidator = new ProcessStartInfo
            {
                FileName = NpmPath(),
                WorkingDirectory = validatorGeneratorDirectory,
                Arguments = $"start -- manifest={manifestPath}",
                RedirectStandardOutput = true,
            };
            var pNpmStartValidator = Process.Start(psiNpmStartValidator);
            string output = pNpmStartValidator.StandardOutput.ReadToEnd();
            pNpmStartValidator.WaitForExit();

            // Write the output to a log.
            File.WriteAllText(logFile, output, Encoding.UTF8);
        }

        /// <summary>
        /// Pulls the npm directory from the PATH environment variable, so npm can be run cross-plat and regardless of installation directory.
        /// </summary>
        /// <returns>Path to npm program.</returns>
        static string NpmPath()
        {
            var pathsArray = Environment.GetEnvironmentVariable("Path").Split(';');
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(pathsArray.First(x => x.Contains("nodejs")), "npm.cmd");
            }
            else
            {
                // Should be /usr/local/bin/npm by default
                return pathsArray.First(x => x.Contains("npm"));
            }
        }
    }
}
