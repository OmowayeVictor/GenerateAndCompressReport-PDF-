// See https://aka.ms/new-console-template for more information
using DotNetEnv;
using GenerateAndCompressPDF;
GetReportAndCompress getReport = new GetReportAndCompress();


Env.Load();
Console.WriteLine();
Console.WriteLine(">>>>> WELCOME <<<<<");
Console.WriteLine();
Console.WriteLine(">>> We’d love for you to feel at home here! Just answer a few quick questions — and if you need anything, feel free to reach out to any of our colleagues");
Console.WriteLine();
Console.WriteLine(">>> What Report Type are we running today? BP [Bussiness Profile] or BS [Bonus Summary]");
string reportType = Console.ReadLine()!.ToUpper();
while (!new[] { "BP", "BS" }.Contains(reportType))
{
    Console.WriteLine(">>> Invalid input. What Report Type are we running today? BP [Bussiness Profile] or BS [Bonus Summary]");
    reportType = Console.ReadLine()?.ToUpper()!;
}
if (reportType == "BS")
{
    await getReport.GetBonusSummaryReportAsync();
}
else
{
    await getReport.GetBusinessProfileReportAsync();
}
