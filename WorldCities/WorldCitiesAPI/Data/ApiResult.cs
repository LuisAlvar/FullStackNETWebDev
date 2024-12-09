using Microsoft.EntityFrameworkCore;

namespace WorldCitiesAPI.Data;

/// <summary>
/// Data wrapper object for Pagination and Sorting
/// <br />
/// Utilized to wrap a list of data objects with other information for pagination and sorting.
/// </summary>
/// <typeparam name="T">An EF data type of an object</typeparam>
public class ApiResult<T>
{
  #region Properties
  
  /// <summary>
  /// The data result
  /// </summary>
  public List<T> Data { get; private set; }

  /// <summary>
  /// Zero-based index of current page.
  /// </summary>
  public int PageIndex { get; private set; }

  /// <summary>
  /// Number of items contained in each page.
  /// </summary>
  public int PageSize { get; private set; }

  /// <summary>
  /// Total items count
  /// </summary>
  public int TotalCount { get; private set; }

  /// <summary>
  /// Total pages count
  /// </summary>
  public int TotalPages { get; private set; }

  /// <summary>
  /// TRUE if the current page has a previous page,
  /// <br />
  /// FALSE otherwise.
  /// </summary>
  public bool HasPreviousPage
  {
    get
    {
      return (PageIndex > 0);
    }
  }

  /// <summary>
  /// TRUE if the current page has a next page, 
  /// <br />
  /// FALSE otherwise.
  /// </summary>
  public bool HasNextPage
  {
    get
    {
      return ((PageIndex + 1) < TotalPages);
    }
  }

  #endregion

  /// <summary>
  /// Private constructor called by the CreateAsync method.
  /// </summary>
  /// <param name="data"></param>
  /// <param name="count"></param>
  /// <param name="pageIndex"></param>
  /// <param name="pageSize"></param>
  private ApiResult(List<T> data, int count, int pageIndex, int pageSize) {
    Data = data;
    PageIndex = pageIndex;
    PageSize = pageSize;
    TotalCount = count;
    TotalPages = (int)Math.Ceiling(count / (double)pageSize);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="source"></param>
  /// <param name="pageIndex"></param>
  /// <param name="pageSize"></param>
  /// <returns></returns>
  public static async Task <ApiResult<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
  {
    int count = await source.CountAsync();
    source = source.Skip(pageIndex * pageSize).Take(pageSize);
    List<T> data = await source.ToListAsync();
    return new ApiResult<T>(data, count, pageIndex, pageSize);
  }
}

