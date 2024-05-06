﻿namespace Nursing_Home.Application.Common.Pages;
public class PaginatedResponse<T> where T : class
{
    public PaginatedResponse(PaginatedList<T> paginatedList)
    {
        Page = paginatedList.PageIndex;
        Size = paginatedList.PageSize;
        TotalPages = paginatedList.TotalPages;
        TotalItems = paginatedList.TotalItems;
        HasPreviousPage = paginatedList.HasPreviousPage;
        HasNextPage = paginatedList.HasNextPage;
        Contents = paginatedList;
    }

    public int Page { get; }
    public int Size { get; }
    public int TotalPages { get; }
    public int TotalItems { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }
    public IList<T> Contents { get; }
}

public static class PaginatedResponseExtensions
{
    public static PaginatedResponse<T> ToPaginatedResponse<T>(this PaginatedList<T> source) where T : class
    {
        return new PaginatedResponse<T>(source);
    }

    public static Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(this PaginatedList<T> source) where T : class
    {
        return Task.FromResult(new PaginatedResponse<T>(source));
    }
}
