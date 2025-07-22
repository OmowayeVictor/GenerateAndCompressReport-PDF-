

namespace GenerateAndCompressPDF
{
    public class Queries
    {
        public string GetBonusSummaryQualifiers( string reportPeriod, string currencyCode)
        {
            return $"SELECT DISTINCT CustomerID FROM combonussummaries WHERE CommissionRunID = (SELECT MAX(CommissionRunID) FROM ComVolumeSummaries WHERE Period = {reportPeriod} ) AND hcurrencycode = '{currencyCode}'";
        }

        public string GetWTMBussinessSummaryQualifiers(string subregion, string period)
        {
            return $@"
                     WITH bizpro AS (
                     SELECT CustomerID 
                     FROM PeriodVolumes pv  
                     WHERE periodtypeid = 1 AND periodid = {period}
                     AND pv.Volume97 >=70 AND pv.Volume97 < 110 )
                     SELECT b.CustomerID FROM bizpro b 
                     JOIN customers c ON c.CustomerID = b.CustomerID 
                     JOIN CustomServicesContext.NeolifeCountries nc ON nc.CountryCode = c.MainCountry 
                     WHERE SubRegion = '{subregion}'
                     ORDER BY CustomerID";
        }

        public string GetPTMBussinessSummaryQualifiers(string period)
        {
            return @$"
                        WITH bizpro AS (
                         SELECT pv.CustomerID
                         FROM PeriodVolumes pv
                         WHERE periodtypeid = 1
                         AND periodid = {period}
                         AND pv.Volume81 >= 110 )
                         SELECT  b.CustomerID FROM bizpro b
                         JOIN customers c ON b.CustomerID = c.CustomerID
                         JOIN CustomServicesContext.NeolifeCountries nc ON nc.CountryCode = c.MainCountry 
                         WHERE region = 'AFR'
                         ORDER By CustomerID";
        }
    }
}
