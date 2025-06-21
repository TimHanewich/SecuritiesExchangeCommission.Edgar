# SecuritiesExchangeCommission.Edgar
.NET class library for accessing the Security Exchange Commission's EDGAR database. This library allows you to access over twenty years worth of financial data that has been reported to the SEC, mostly by publicly traded companies.

## Make sure to Identify your Program!
The SEC requires all automated tools to declare their traffic by specifying a user agent in each HTTP request header. This library is designed to do that and will pass the User-Agent according to the SEC's expected format (`AppName/1.0 (email)`). This should be the first thing you do!

```
using SecuritiesExchangeCommission.Edgar;

IdentificationManager.Instance.AppName = "MyAppName";
IdentificationManager.Instance.AppVersion = "1.0.0";
IdentificationManager.Instance.Email = "email@gmail.com";
```

**Be sure to follow this step before doing anything with this library! If you do not, it will likely fail to return any data!**

## Querying the SEC EDGAR Database
Use the `EdgarSearch` class to query the database for filings for any publicly traded company.  
For example, requesting Microsoft's ($MSFT) latest 10-K filings:
```
EdgarSearch msft10ks = await EdgarSearch.CreateAsync("MSFT", "10-K");
```
The first parameter in the `CreateAsync` static method, `stock_symbol`, can be specified as either the company's public trading symbol (MSFT in this case) or the company's SEC-assigned CIK, or "Central Index Key". For example, Microsoft's CIK is 789019.  
The `EdgarSearch` instance is going to place the results of your query into its `Results` property. The `Results` property contains the most recent results that suite your query, but is limited of the number of results it can fit in one return. To get the next page of results, you can do something like this:
```
EdgarSearch msft10ks = await EdgarSearch.CreateAsync("MSFT", "10-K");
if (msft10ks.NextPageAvailable())
{
    EdgarSearch next_page = await msft10ks.NextPageAsync();
}
```

## Extracting Data from Filings
The `Results` property of the `EdgarSearch` instance we used to query the database will contain an array of `EdgarSearchResult` instances. These instances will contain some basic details about the filing and provide you with a method to access additional details about that particular filing.

## NEW: Interface with SEC's Data API
Recently, the SEC created a REST API service that supplies data. You can find documentation for that API [here](https://www.sec.gov/search-filings/edgar-application-programming-interfaces).

This library also allows you to interface with the two most useful endpoints of the API: `/companyconcept` and `companyfacts`.

### Company Facts
`/companyfacts` returns a list of every XBRL fact that the company has ever reported, and every discrete data point associated with each fact. It is a lot of data!

Below is an example of how to interface with the SEC's `/companyfacts` service:

```
IdentificationManager.Instance.AppName = "LApp";
IdentificationManager.Instance.AppVersion = "1.0";
IdentificationManager.Instance.Email = "chrisha@gmail.com";

CompanyFactsQuery cfq = await CompanyFactsQuery.QueryAsync(1655210); //Beyond Meat (BYND)
foreach (Fact f in cfq.Facts)
{
    Console.WriteLine("Fact: " + f.Tag);
    Console.WriteLine("Label: " + f.Label);
    Console.WriteLine("Description: " + f.Description);
    Console.WriteLine();
    Console.WriteLine("Data Points:");
    foreach (FactDataPoint fdp in f.DataPoints)
    {
        Console.WriteLine(fdp.End.ToShortDateString() + " - $" + fdp.Value.ToString("#,##0"));
    }

    Console.WriteLine();
    Console.Write("Press enter to continue onto the next fact...");
    Console.ReadLine();
    Console.WriteLine();
    Console.WriteLine();
}
```


Will result in, for example:

```
Fact: AdvertisingExpense
Label: Advertising Expense
Description: Amount charged to advertising expense for the period, which are expenses incurred with the objective of increasing revenue for a specified brand, product or product line.

Data Points:
12/31/2017 - $300,000
12/31/2018 - $62,000
12/31/2018 - $62,000
12/31/2019 - $300,000
12/31/2019 - $300,000
12/31/2019 - $300,000
12/31/2020 - $300,000
12/31/2020 - $300,000
12/31/2020 - $300,000
12/31/2021 - $12,100,000
12/31/2021 - $12,100,000
12/31/2021 - $12,100,000
12/31/2022 - $20,600,000
12/31/2022 - $20,600,000
12/31/2022 - $20,600,000
12/31/2023 - $17,200,000
12/31/2023 - $17,200,000
12/31/2024 - $8,500,000

Press enter to continue onto the next fact...
```

### Company Concept
`/companyconcept` returns a list of discrete data that a company reported for a *particular fact tag* (i.e. `AssetsCurrent`).

We can interface with the `/companyconcept` service like so:

```
IdentificationManager.Instance.AppName = "LApp";
IdentificationManager.Instance.AppVersion = "1.0";
IdentificationManager.Instance.Email = "chrisha@gmail.com";

CompanyConceptQuery ccq = await CompanyConceptQuery.QueryAsync(1655210, "RevenueFromContractWithCustomerExcludingAssessedTax");

Console.WriteLine("Company Name: " + ccq.EntityName);
Console.WriteLine("Fact: " + ccq.Result.Tag);
Console.WriteLine("Label: " + ccq.Result.Label);
Console.WriteLine("Description: " + ccq.Result.Description);
Console.WriteLine();
Console.WriteLine("Data Points:");
foreach (FactDataPoint fdp in ccq.Result.DataPoints)
{
    if (fdp.Period == FiscalPeriod.FiscalYear) //Only show year-end values (on the 10-K), not quarterly
    {
        Console.WriteLine("Year End " + fdp.End.ToShortDateString() + " - $" + fdp.Value.ToString("#,##0"));
    }
}
```

Will result in:

```
Company Name: BEYOND MEAT, INC.
Fact: RevenueFromContractWithCustomerExcludingAssessedTax
Label: Revenue from Contract with Customer, Excluding Assessed Tax
Description: Amount, excluding tax collected from customer, of revenue from satisfaction of performance obligation by transferring promised good or service to customer. Tax collected from customer is tax assessed by governmental authority that is both imposed on and concurrent with specific revenue-producing transaction, including, but not limited to, sales, use, value added and excise.

Data Points:
Year End 12/31/2017 - $32,581,000
Year End 3/31/2018 - $12,776,000
Year End 6/30/2018 - $17,367,000
Year End 9/29/2018 - $26,277,000
Year End 12/31/2018 - $87,934,000
Year End 12/31/2018 - $87,934,000
Year End 12/31/2018 - $31,514,000
Year End 3/30/2019 - $40,206,000
Year End 3/30/2019 - $40,206,000
Year End 6/29/2019 - $67,251,000
Year End 6/29/2019 - $67,251,000
Year End 9/28/2019 - $91,961,000
Year End 9/28/2019 - $91,961,000
Year End 12/31/2019 - $297,897,000
Year End 12/31/2019 - $297,897,000
Year End 12/31/2019 - $297,897,000
Year End 12/31/2019 - $98,479,000
Year End 12/31/2019 - $98,479,000
Year End 3/28/2020 - $97,074,000
Year End 6/27/2020 - $113,338,000
Year End 9/26/2020 - $94,436,000
Year End 12/31/2020 - $406,785,000
Year End 12/31/2020 - $406,785,000
Year End 12/31/2020 - $406,785,000
Year End 12/31/2020 - $101,937,000
Year End 12/31/2021 - $464,700,000
Year End 12/31/2021 - $464,700,000
Year End 12/31/2021 - $464,700,000
Year End 12/31/2022 - $418,933,000
Year End 12/31/2022 - $418,933,000
Year End 12/31/2022 - $418,933,000
Year End 12/31/2023 - $343,376,000
Year End 12/31/2023 - $343,376,000
Year End 12/31/2024 - $326,452,000
```