using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq.Dynamic.Core;

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

  /// <summary>
  /// Sorting Column name (or null if none set)
  /// </summary>
  public string? SortColumn { get; set; }


  /// <summary>
  /// Sorting Order ("ASC","DESC" or null if none set)
  /// </summary>
  public string? SortOrder { get; set; }

  /// <summary>
  /// Filter Column name (or null if none set)
  /// </summary>
  public string? FilterColumn { get; set; }

  /// <summary>
  /// FilterQuery string
  /// (to be used within the given FilterColumn)
  /// </summary>
  public string? FilterQuery { get; set; }

  #endregion

  /// <summary>
  /// Private constructor called by the CreateAsync method.
  /// </summary>
  /// <param name="data"></param>
  /// <param name="count"></param>
  /// <param name="pageIndex"></param>
  /// <param name="pageSize"></param>
  private ApiResult(
    List<T> data,
    int count,
    int pageIndex,
    int pageSize,
    string? sortColumn,
    string? sortOrder,
    string? filterColumn,
    string? filterQuery) 
  {
    Data = data;
    PageIndex = pageIndex;
    PageSize = pageSize;
    TotalCount = count;
    TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    SortColumn = sortColumn;
    SortOrder = sortOrder;
    FilterColumn = filterColumn;
    FilterQuery = filterQuery;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="source"></param>
  /// <param name="pageIndex"></param>
  /// <param name="pageSize"></param>
  /// <returns></returns>
  public static async Task <ApiResult<T>> CreateAsync(
    IQueryable<T> source,
    int pageIndex,
    int pageSize,
    string? sortColumn=null,
    string? sortOrder=null,
    string? filterColumn=null, 
    string? filterQuery=null)
  {

    if (!string.IsNullOrEmpty(filterColumn) 
      && !string.IsNullOrEmpty(filterQuery)
      && IsValidProperty(filterColumn))
    {
      source = source.Where(string.Format("{0}.StartsWith(\"{1}\")", filterColumn, filterQuery));
    }

    int count = await source.CountAsync();

    if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
    {
      sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
      source = source.OrderBy(string.Format("{0} {1}", sortColumn, sortOrder));
    }
    source = source.Skip(pageIndex * pageSize).Take(pageSize);

    List<T> data = await source.ToListAsync();
    return new ApiResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
  }

  #region Methods
  /// <summary>
  /// Method to ensure any sortColumn value is actually from the template class
  /// <br />
  /// Checks if the given property name exists to protect against SQL injection attacks.
  /// </summary>
  /// <param name="propertyName">A property name of the template class</param>
  /// <param name="throwExceptionIfNotFound">Option to throw an error if user attempts to use a property not within the template class</param>
  /// <returns></returns>
  /// <exception cref="NotSupportedException">Exception to notify user that the given property name is not valided.</exception>
  public static bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = true)
  {
    var prop = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

    if (prop == null && throwExceptionIfNotFound)
    {
      throw new NotSupportedException(string.Format($"ERROR: Property '{propertyName}' does not exist."));
    }
    return prop != null;
  }
  #endregion
}

