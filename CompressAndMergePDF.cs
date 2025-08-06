using DotNetEnv;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;

namespace GenerateAndCompressPDF
{
    public class CompressAndMergePDF
    {
        private readonly string? _ghostscriptPath;
        public CompressAndMergePDF()
        {
            var ghostscriptPath = FindGhostscriptPath();
            _ghostscriptPath = !string.IsNullOrEmpty(ghostscriptPath) ? FindGhostscriptPath() : null;
        }


        public string? FindGhostscriptPath()
        {
            Env.Load();
            string ghostscriptExecutable =  "gswin64c.exe";

            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "where",
                    Arguments = ghostscriptExecutable,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(processStartInfo))
                {
                    if (process == null)
                    {
                        return null;
                    }

                    string ghostScriptPath = process.StandardOutput.ReadToEnd().Trim();
                    return !string.IsNullOrEmpty(ghostScriptPath) && File.Exists(ghostScriptPath) ? ghostScriptPath : null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>>FindGhostscript: {ex.Message}");
                return null;
            }
        }
        public void MergePdfFile(List<byte[]> pdfBuffers, string outputPath)
        {
            PdfDocument mergedDocument = new PdfDocument();

            foreach (var pdfBuffer in pdfBuffers)
            {
                using (MemoryStream memoryStrem = new MemoryStream(pdfBuffer))
                {
                    PdfDocument inputDocument = PdfReader.Open(memoryStrem, PdfDocumentOpenMode.Import);

                    for (int i = 0; i < inputDocument.PageCount; i++)
                    {
                        PdfPage page = inputDocument.Pages[i];
                        mergedDocument.AddPage(page);
                    }
                }
            }

            mergedDocument.Save(outputPath);
        }
        public void CompressPdfFile (string inputFilePath, string outputFilePath)
        {

            try
            {
                string args = $"-sDEVICE=pdfwrite -dCompatibilityLevel=1.4 " +
                      "-dPDFSETTINGS=/ebook -dNOPAUSE -dQUIET -dBATCH " +
                      $"-sOutputFile=\"{outputFilePath}\" \"{inputFilePath}\"";

                ProcessStartInfo startProcess = new ProcessStartInfo
                {
                    FileName = _ghostscriptPath,
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                using (Process? process = Process.Start(startProcess))
                {
                    process!.WaitForExit();

                    if (process.ExitCode != 0 || !File.Exists(outputFilePath))
                    {
                        Console.WriteLine($">>> Ghostscript failed or file missing. ExitCode: {process.ExitCode}");
                        throw new Exception(">>>Ghostscript compression failed");
                    }

                }
                    Console.WriteLine($">>>Successfully compressed and merged PDFs: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>>Error compressing and merging PDF: {ex.Message}");
                try
                {

                    File.Copy(inputFilePath, outputFilePath, overwrite: true);
                    Console.WriteLine($">>>Fallback successful: {inputFilePath} copied to {outputFilePath}");
                }
                catch (Exception copyEx)
                {
                    Console.WriteLine($">>>Error during fallback copy: {copyEx.Message}");
                }
            }
           


        }

    }
}
