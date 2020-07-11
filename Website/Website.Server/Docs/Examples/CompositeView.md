<if view>
##### CompositeView.razor

```jsx
@inject IStylesheet Stylesheet

<VMContext VM="CompositeViewVM" TState="object">
    <d-panel css="@Stylesheet["CompositeView"]" horizontal="true">
        <section>
            <VMContext VM="FilterableMovieTableVM" TState="object">
                <MovieTable />
            </VMContext>
        </section>
        <aside>
            <MovieDetails />
            <MovieFilter />
        </aside>
    </d-panel>
</VMContext>
```

##### MovieTable.razor

```jsx
<VMContext VM="MovieTableVM" TState="IMovieTableState" OnStateChange="UpdateState">
@if (state != null)
{
    <div>
        <table>
            <thead>
                <tr>
                    @foreach (var header in state.Headers)
                    {
                        <th @key="header">@header</th>
                    }
                </tr>
            </thead>
            <tbody>
                @foreach (var data in state.Data)
                {
                    <tr @key="data.Rank"
                        class="@(state.SelectedKey == data.Rank ? "selected" : "")"
                        @onclick="_ => state.SelectedKey = data.Rank">
                        <td>@data.Rank</td>
                        <td>@data.Movie</td>
                        <td>@data.Year</td>
                        <td>@data.Director</td>
                    </tr>
                }
            </tbody>
        </table>
        <div class="pagination">
            @foreach (var page in state.Pagination)
            {
                <div @key="page"
                     class="@(state.SelectedPage == page ? "current" : "")"
                     @onclick="_ => state.SelectedPage = page">
                    @(page)
                </div>
            }
        </div>
    </div>
}
</VMContext>

@code {
   private IMovieTableState state;

   public interface IMovieTableState
   {
       string[] Headers { get; set; }
       MovieRecord[] Data { get; set; }
       int[] Pagination { get; set; }
       [Watch] int SelectedKey { get; set; }
       [Watch] int SelectedPage { get; set; }
   }

   public class MovieRecord
   {
       public int Rank { get; set; }
       public string Movie { get; set; }
       public int Year { get; set; }
       public string Director { get; set; }
       public string Cast { get; set; }
   }

   private void UpdateState(IMovieTableState state)
   {
       this.state = state;
       StateHasChanged();
   }
}
```

##### MovieDetails.razor

```jsx
<VMContext VM="MovieDetailsVM" TState="IMovieDetailsState" OnStateChange="UpdateState">
@if (state != null)
{
    <div class="card">
        <header class="card-header">
            <b>@state.Movie.Movie</b>
            <div>@state.Movie.Year</div>
        </header>
        <section class="card-body">
            <b>Director</b>
            <p>@state.Movie.Director</p>
            <b>Cast</b>
            <div>
                @foreach (var cast in state.Movie.Cast.Split(","))
                {
                    <div>@cast</div>
                }
            </div>
        </section>
    </div>
}
</VMContext>

@code {
   private IMovieDetailsState state;

   public interface IMovieDetailsState
   {
       MovieTable.MovieRecord Movie { get; set; }
   }

   private void UpdateState(IMovieDetailsState state)
   {
       this.state = state;
       StateHasChanged();
   }
}
```

##### MovieFilter.razor

```jsx
<VMContext VM="FilterableMovieTableVM.MovieFilterVM" TState="IMovieFilterState" OnStateChange="UpdateState">
@if (state != null)
{
    <form>
        <div class="filter card">
            <div class="card-header">Filters</div>
            <div class="card-body">
                <select class="form-control"
                        value="@filter"
                        @onchange="e => UpdateFilterDropdown(e.Value.ToString())">
                    @foreach (var text in movieProps)
                    {
                        <option value="@text">@text</option>
                    }
                </select>
                <div class="operation">
                    @foreach (var op in operations)
                    {
                        <div class="form-check">
                            <input class="form-check-input"
                                    type="radio"
                                    id="@op"
                                    value="@op"
                                    checked="@(operation == op)"
                                    @onchange="_ => operation = op">
                            <label class="form-check-label" for="@op">@op</label>
                        </div>
                    }
                </div>
                <input class="form-control" @bind="text" @bind:event="oninput" >
                <div>
                    @foreach (var filter in filters)
                    {
                        <div class="chip" @key="filter.Id">
                            @filter.Property @filter.Operation @filter.Text
                            <div @onclick="_ => HandleDelete(filter.Id)">
                                <i class="material-icons">clear</i>
                            </div>
                        </div>
                    }
                </div>
            </div>
            <div class="card-footer">
                <button class="btn btn-primary" @onclick="HandleApply" disabled="@string.IsNullOrWhiteSpace(text)">Apply</button>
            </div>
        </div>
    </form>
}
</VMContext>

@code {
   private IMovieFilterState state;
   private List<Filter> filters = new List<Filter>();
   private int filterId = 0;
   private string filter = "Any";
   private string[] movieProps = new string[] { "Any", "Rank", "Movie", "Year", "Cast", "Director" };
   private string operation = "contains";
   private string[] operations = new string[] { "contains" };
   private string text = "";

   public interface IMovieFilterState
   {
       void Apply(Filter filter);
       void Delete(int filterId);
   }

   public class Filter
   {
       public int Id { get; set; }
       public string Property { get; set; }
       public string Operation { get; set; }
       public string Text { get; set; }
   }

   private void HandleApply()
   {
       var newId = filterId + 1;
       var newFilter = new Filter { Id = newId, Property = filter, Operation = operation, Text = text };

       filterId = newId;
       filters.Add(newFilter);
       text = "";

       state.Apply(newFilter);
       UpdateFilterDropdown("Any"); // Reset filter dropdown.
   }

   private void HandleDelete(int filterId)
   {
       state.Delete(filterId);
       filters = filters.Where(filter => filter.Id != filterId).ToList();
   }

   private void UpdateFilterDropdown(string filter)
   {
       this.filter = filter;
       if (filter == "Rank" || filter == "Year")
       {
           this.operations = new string[] { "equals", ">=", "<=" };
           this.operation = "equals";
       }
       else
       {
           this.operations = new string[] { "contains" };
           this.operation = "contains";
       }
   }

   private void UpdateState(IMovieFilterState state)
   {
       this.state = state;
       StateHasChanged();
   }
}
```

</if>
<if viewmodel>
##### CompositeView.cs

```csharp
public class CompositeViewVM : BaseVM
{
   private readonly IMovieService _movieService;

   private event EventHandler<int> Selected;

   public CompositeViewVM(IMovieService movieService)
   {
      _movieService = movieService;
   }

   public override void OnSubVMCreated(BaseVM subVM)
   {
      if (subVM is FilterableMovieTableVM)
         InitMovieTableVM(subVM as FilterableMovieTableVM);
      else if (subVM is MovieDetailsVM)
         InitMovieDetailsVM(subVM as MovieDetailsVM);
   }

   private void InitMovieTableVM(FilterableMovieTableVM vm)
   {
      // Set the movie table data source to AFI Top 100 movies.
      vm.DataSource = () => _movieService.GetAFITop100();

      // When movie table selection changes, raise a private Selected event.
      vm.Selected += (sender, rank) => Selected?.Invoke(this, rank);
   }

   private void InitMovieDetailsVM(MovieDetailsVM vm)
   {
      // Set default details to the highest ranked movie.
      vm.SetByAFIRank(1);

      // When the Selected event occurs, update the movie details.
      Selected += (sender, rank) => vm.SetByAFIRank(rank);
   }
}
```

##### MovieTable.cs

```csharp
public interface IPaginatedTable<T>
{
   IEnumerable<string> Headers { get; }
   IEnumerable<T> Data { get; }
   int SelectedKey { get; set; }
   int[] Pagination { get; }
   int SelectedPage { get; set; }
}

public class MovieTableVM : BaseVM, IPaginatedTable<MovieRecord>
{
   private int _recordsPerPage = 10;
   private Func<IEnumerable<MovieRecord>> _dataSourceFunc;

   public IEnumerable<string> Headers => new string[] { "Rank", "Movie", "Year", "Director" };

   public Func<IEnumerable<MovieRecord>> DataSource
   {
      set
      {
         _dataSourceFunc = value;
         Changed(nameof(Data));
      }
   }

   public IEnumerable<MovieRecord> Data => GetData();

   public int SelectedKey
   {
      get => Get<int>();
      set
      {
         Set(value);
         Selected?.Invoke(this, value);
      }
   }

   public int[] Pagination
   {
      get => Get<int[]>();
      set
      {
         Set(value);
         SelectedPage = 1;
      }
   }

   public int SelectedPage
   {
      get => Get<int>();
      set
      {
         Set(value);
         Changed(nameof(Data));
      }
   }

   public event EventHandler<int> Selected;

   private IEnumerable<MovieRecord> GetData()
   {
      if (_dataSourceFunc == null)
         return null;

      var data = _dataSourceFunc();
      if (!data.Any(i => i.Rank == SelectedKey))
         SelectedKey = data.Count() > 0 ? data.First().Rank : -1;

      return Paginate(data);
   }

   private IEnumerable<MovieRecord> Paginate(IEnumerable<MovieRecord> data)
   {
      // ChangedProperties is a base class property that contains a list of changed properties.
      // Here it's used to check whether user has changed the SelectedPage property value by clicking a pagination button.
      if (this.HasChanged(nameof(SelectedPage)))
         return data.Skip(_recordsPerPage * (SelectedPage - 1)).Take(_recordsPerPage);
      else
      {
         var pageCount = (int)Math.Ceiling(data.Count() / (double)_recordsPerPage);
         Pagination = Enumerable.Range(1, pageCount).ToArray();
         return data.Take(_recordsPerPage);
      }
   }
}
```

##### FilterableMovieTable.cs

```csharp
public class FilterableMovieTableVM : BaseVM
{
   private Func<IEnumerable<MovieRecord>> _dataSourceFunc;
   private Action _updateData;
   private string _query;

   public event EventHandler<int> Selected;

   public Func<IEnumerable<MovieRecord>> DataSource
   {
      set
      {
         _dataSourceFunc = value;
         _updateData?.Invoke();
      }
   }

   // This method is called when an instance of a view model inside this view model's scope is being created.
   // It provides a chance for this view model to initialize them.
   public override void OnSubVMCreated(BaseVM subVM)
   {
      if (subVM is MovieTableVM)
         InitMovieTableVM(subVM as MovieTableVM);
      else if (subVM is MovieFilterVM)
         InitMovieFilterVM(subVM as MovieFilterVM);
   }

   private void InitMovieTableVM(MovieTableVM vm)
   {
      // Forward the movie table's Selected event.
      vm.Selected += (sender, key) => Selected?.Invoke(this, key);

      // Create an action to update the movie table with the filtered data.
      _updateData = () => vm.DataSource = GetFilteredData;
      _updateData();
   }

   private void InitMovieFilterVM(MovieFilterVM vm)
   {
      // If a filter is added, set the filter query and update the movie table data.
      vm.FilterChanged += (sender, query) =>
      {
         _query = query;
         _updateData?.Invoke();
      };
   }

   private IEnumerable<MovieRecord> GetFilteredData()
   {
      try
      {
         return !string.IsNullOrEmpty(_query) ?
         _dataSourceFunc().AsQueryable().Where(_query) : _dataSourceFunc();
      }
      catch (Exception)
      {
         return new List<MovieRecord>();
      }
   }
}
```

##### MovieDetails.cs

```csharp
public class MovieDetailsVM : BaseVM
{
   private readonly IMovieService _movieService;

   public MovieRecord Movie
   {
      get { return Get<MovieRecord>(); }
      set { Set(value); }
   }

   public MovieDetailsVM(IMovieService movieService)
   {
      _movieService = movieService;
   }

   public void SetByAFIRank(int rank) => Movie = _movieService.GetMovieByAFIRank(rank);
}
```

##### MovieFilter.cs

```csharp
public class MovieFilterVM : BaseVM
{
   private List<MovieFilter> _filters = new List<MovieFilter>();

   public class MovieFilter
   {
      public int Id { get; set; }
      public string Property { get; set; }
      public string Operation { get; set; }
      public string Text { get; set; }

      public string ToQuery()
      {
         if (Operation == "contains")
            return Property == "Any" ? $"( Movie + Cast + Director ).toLower().contains(\"{Text.ToLower()}\")"
               : $"{Property}.toLower().contains(\"{Text.ToLower()}\")";
         else
         {
            int intValue = int.Parse(Text);
            if (Operation == "equals")
               return $"{Property} == {intValue}";
            else
               return $"{Property} {Operation} {intValue}";
         }
      }

      public static string BuildQuery(IEnumerable<MovieFilter> filters) => string.Join(" and ", filters.Select(i => i.ToQuery()));
   }

   public Action<MovieFilter> Apply => arg =>
   {
      _filters.Add(arg);
      FilterChanged?.Invoke(this, MovieFilter.BuildQuery(_filters));
   };

   public Action<int> Delete => id =>
   {
      _filters = _filters.Where(i => i.Id != id).ToList();
      FilterChanged?.Invoke(this, MovieFilter.BuildQuery(_filters));
   };

   public event EventHandler<string> FilterChanged;
}
```

</if>
