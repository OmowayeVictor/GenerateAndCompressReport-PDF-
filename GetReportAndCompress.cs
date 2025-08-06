

namespace GenerateAndCompressPDF
{
    public class GetReportAndCompress
    {
        private readonly SSRSReport ssrsReport = new();
        private readonly CompressAndMergePDF compressAndMergePdf = new();
        private readonly Queries queries = new();

        /// <summary> Generate Bonus Summary Report </summary>
        public async Task GetBonusSummaryReportAsync()
        {
            Console.WriteLine();
            Console.WriteLine(">>>>>>>>>> Bonus Summary <<<<<<<<<<");
            Console.WriteLine();
            string? retry = null;
            do
            {
                Console.WriteLine(">>> CountryCode ? E.g ngn, php e.t.c...");
                string currencyCode = Console.ReadLine()!.ToLower();
                Console.WriteLine();
                Console.WriteLine(">>> Period ? (YYYYMM)");
                string reportPeriod = Console.ReadLine()!;

                Console.WriteLine();
                try
                {
                    var qualifiers = new Database(queries.GetBonusSummaryQualifiers(reportPeriod, currencyCode)).GetDetailsFromDB();
                    Console.WriteLine($">>> Reports are been pulled for total of {qualifiers.Count} Customers");
                    var pdfBuffers = new List<byte[]>();
                    int counter = 1;
                    foreach (var qualifier in qualifiers)
                    {
                        Console.WriteLine($">>> {counter}. Fetching Report for {qualifier["CustomerID"]}");
                        var customerID = qualifier["CustomerID"].ToString();
                        string? isMexico = currencyCode == "mxn" ? "MX" : null;
                        byte[] pdfBuffer = await ssrsReport.FetchBufferAsync(isMexico!, customerID!, reportPeriod!, reportType: "Bonus Summary");
                        counter++;
                        pdfBuffers.Add(pdfBuffer);
                    }
                    string tempMergedPdfPath = Path.Combine(Path.GetTempPath(), "merged.pdf");
                    string downloadsFolder = Path.Combine(
                                   Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                   "Downloads"
                               );
                    Directory.CreateDirectory(downloadsFolder);
                    string outputPath = Path.Combine(downloadsFolder, $"BonusSummaryReport_{currencyCode}.pdf");
                    compressAndMergePdf.MergePdfFile(pdfBuffers, tempMergedPdfPath);
                    compressAndMergePdf.CompressPdfFile(tempMergedPdfPath, outputPath);
                    Console.WriteLine($">>>>> PDF saved to {outputPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    string tempMergedPdfPath = Path.Combine(Path.GetTempPath(), "merged.pdf");
                    File.Delete(tempMergedPdfPath);
                }
                Console.WriteLine(">>>>> Would you like to run another Bonus Summary Report ? (y/n)");

                retry = Console.ReadLine()?.ToLower();

            }
            while (retry?.ToLower() is "yes" or "y");
            Console.WriteLine(">>>>> Press any key to close this window . . .");
            Console.ReadKey();
        }

        /// <summary> Generate Bussiness Profile Report </summary>
        public async Task GetBusinessProfileReportAsync()
        {
            Console.WriteLine();
            Console.WriteLine(">>>>>>>>>> Business Profile <<<<<<<<<<");
            Console.WriteLine();
            string? retry = null;

            do
            {;
                Console.WriteLine(">>> PeriodID ? E.g 112,113 e.t.c... check periods table");
                string? period = Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine(">>> Whose report ? WTM or PTM ?");
                string? getReportType = Console.ReadLine()?.ToUpper();
                while (!new[] { "WTM", "PTM" }.Contains(getReportType))
                {
                    Console.WriteLine(">>> Invalid input. Please enter either 'WTM' or 'PTM' without space please.");
                    getReportType = Console.ReadLine()?.ToUpper()!;
                }
                Console.WriteLine();
                var subregion = "AFR";
                if (getReportType == "WTM")
                {
                    Console.WriteLine(">>>Subregion ? Choose either of SAFR, EAFR, WAFR, NGR");
                    subregion = Console.ReadLine()?.ToUpper()!;
                    while (!new[] { "SAFR", "WAFR", "EAFR", "NGR" }.Contains(subregion))
                    {
                        Console.WriteLine(">>>Invalid input. Please enter  either of SARF, EAFR, WAFR, NG without space please.");
                        subregion = Console.ReadLine()?.ToUpper()!;
                    }
                }

                Console.WriteLine();
                Console.WriteLine(">>Report Period  (YYYYMM) ? ");
                var reportPeriod = Console.ReadLine();

                Console.WriteLine();
                var query = getReportType == "WTM" ? queries.GetWTMBussinessSummaryQualifiers(subregion, period!) : queries.GetPTMBussinessSummaryQualifiers(period!);
                try
                {
                    var qualifiers = new Database(query).GetDetailsFromDB();

                  
                    Console.WriteLine($">>>Total Customers to pull reports for : {qualifiers.Count()}");
                    var pdfBuffers = new List<byte[]>();
                    int counter = 1;
                    foreach (var qualifier in qualifiers)
                    {
                        Console.WriteLine($">>>{counter}. Fetching Report for {qualifier["CustomerID"]}");
                        var CustomerID = qualifier["CustomerID"].ToString();
                        byte[] pdfBuffer = await ssrsReport.FetchBufferAsync(isMexico: null, CustomerID!, reportPeriod!, reportType: "Business Profile");
                        counter++;
                        pdfBuffers.Add(pdfBuffer);

                    }
                    string tempMergedPdfPath = Path.Combine(Path.GetTempPath(), "merged.pdf");
                    string downloadsFolder = Path.Combine(
                                   Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                   "Downloads"
                               );
                    Directory.CreateDirectory(downloadsFolder);
                    string outputPath = Path.Combine(downloadsFolder, $"BusinessProfileReport_{subregion}.pdf");
                    Console.WriteLine("Compressing... ");
                    compressAndMergePdf.MergePdfFile(pdfBuffers, tempMergedPdfPath);
                    compressAndMergePdf.CompressPdfFile(tempMergedPdfPath, outputPath);
                    Console.WriteLine($"PDF saved to {outputPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($">>>Error: {ex.Message}");
                }
                finally
                {
                    string tempMergedPdfPath = Path.Combine(Path.GetTempPath(), "merged.pdf");
                    File.Delete(tempMergedPdfPath);
                }
                Console.WriteLine(">>>Do You Want To Run Again ? (y/n)");
                retry = Console.ReadLine()?.Trim().ToLower()!;
            }
            while(retry?.ToLower() is "yes" or "y");

            Console.WriteLine("Press any key to close this window . . .");
            Console.ReadKey();

        }
    }
}
