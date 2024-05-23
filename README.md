## What is QueryMaker.Grid?

QueryMaker.Grid is a datagrid component built using [QuickGrid](https://aspnet.github.io/quickgridsamples/) which adds integrated functionality for **filtering**, **pagination**, **sorting** using [QueryMaker](https://github.com/PRLL/QueryMaker) with **virtualization** for showing the column options.

<br />



## Installation

Install [QueryMaker.Grid](https://www.nuget.org/packages/QueryMaker.Grid/) from the package manager console:

  ```
  PM> Install-Package QueryMaker.Grid
  ```

  Or from the .NET CLI as:

  ```powershell
  dotnet add package QueryMaker.Grid
  ```

<br />



## How To Use

First, on our .NET API project we create an endpoint which receives an instance of [QueryMaker](https://github.com/PRLL/QueryMaker) and returns the filtered resuts of some entity for our grid with both the array of paginated items and the total ammount using [QueryMaker.GetDataAsync](https://www.nuget.org/packages/QueryMaker.GetDataAsync/):

  ```csharp
  using QueryMakerLibrary;
  using QueryMakerLibrary.GetDataAsync;

  [HttpPost(Name = "getGridData")]
  public async Task<IActionResult> GetGridData([FromBody] QueryMaker query)
  {
    // first, we make the queries using QueryMaker to get the data asynchronously
    QueryMakerData<T> data = await _dbContext.SomeEntity.MakeQueryResult(queryMaker).GetDataAsync();

    // then, we return the resulting Items and TotalAmmount
    return Ok(new
    {
      data.Items,
      data.TotalAmmount
    });
  }
  ```

Now, on our Blazor project we can use the `QueryMakerGrid` component to consume this endpoint and populate our grid:

  ```razor
  @using QueryMakerLibrary.Grid.Components
  @using QueryMakerLibrary.Grid.Providers
  @using QueryMakerLibrary.Grid.Common

  @inject HttpClient Http

  @* we instantiate our QueryMakerGrid component with its columns and DataProvider *@
  <QueryMakerGrid DataProvider="@(async request => await GetData(request))" PaginationState="@PaginationState">
    <QueryMakerGridColumn Virtualize="true" Sortable="true" Title="Name" Property="@(x => x.Name)" />
    <QueryMakerGridColumn SearchWhileTyping="false" Title="Status" Property="@(x => x.Status)" />
  </QueryMakerGrid>

  @* to add pagination we need to use the QueryMakerGridPaginator component *@
  @* and provide the PaginationState to the grid *@
  <QueryMakerGridPaginator State=@PaginationState />

  @code
  {
    // property for providing pagination to the grid initialized with a count of 25 items per page
    // and optionally using an 'Id' property as index for faster pagination
    private QueryMakerGridPaginationState PaginationState = new(25, "Id");

    // method for getting the data from our API sending a QueryMaker instance
    public async Task<QueryMakerGridItemsProviderResult<YourGridType>> GetData(
      QueryMakerGridItemsProviderRequest request, CancellationToken cancellationToken = default)
      {
        return (await (await Http.PostAsJsonAsync("{myAPIUrl}/GetGridData",
          request.Query, cancellationToken))
          .Content.ReadFromJsonAsync<QueryMakerGridItemsProviderResult<YourGridType>>());
      }
  }
  ```

And that's it! The grid will now perform **filtering**, **pagination**, **sorting**, **virtualization** all by itself without the need for any more code on our API:

![sample](https://i.imgur.com/eGAQbxp.png)

<br />



## License

Distributed under the GNU General Public License v3.0 License. See `LICENSE.md` for more information.

<br />



## Contact

LinkedIn: [Jose Toyos](https://www.linkedin.com/in/josetoyosvargas/)

Email: josemoises.toyosvargas@hotmail.com

Project Link: [https://github.com/PRLL/QueryMaker.Grid](https://github.com/PRLL/QueryMaker.Grid)

<br />



## Copyright

©Jose Toyos 2024
