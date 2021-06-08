# SecuritiesExchangeCommission.Edgar
.NET class library for accessing the Security Exchange Commission's EDGAR database. This library allows you to access over twenty years worth of financial data that has been reported to the SEC, mostly by publicly traded companies.

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