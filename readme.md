# SecuritiesExchangeCommission.Edgar
.NET class library for accessing the Security Exchange Commission's EDGAR database. This library allows you to access over twenty years worth of financial data that has been reported to the SEC, mostly by publicly traded companies.

## Delcare a User-Agent!
The SEC requires all automated tools to declare their traffic by specifying a user agent in each HTTP request header. This library is designed to do that and will pass the User-Agent that you provide. To set your User-Agent:
```
SecuritiesExchangeCommission.Edgar.SecRequestManager.Instance.UserAgent = "MyCompany/4.1.0";
```
In the above example, `MyCompany` is the name of your company/service (declaring who you are) and the `4.1.0` declares the version of the service. The name and version must be separated by the forward slash. More on User-Agents an be read [here](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent).

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