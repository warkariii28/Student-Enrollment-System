using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace BrightPath.DTOs;

public class PagedResultDto<T>
{
    public List<T> Items {get; set;} = [];
    public int TotalCount {get; set;}
    public int Page{get; set;}
    public int PageSize{get; set;}
}