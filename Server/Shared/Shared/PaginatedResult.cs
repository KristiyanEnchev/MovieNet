﻿namespace Shared
{
    using Newtonsoft.Json;

    public class PaginatedResult<T> : Result<T>
    {
        private new List<T> _data;

        public PaginatedResult() : base()
        {
            _data = new List<T>();
        }

        public PaginatedResult(List<T> data) : base()
        {
            _data = data;
        }

        [JsonConstructor]
        public PaginatedResult(bool succeeded, List<T> data = default!, int count = 0, int pageNumber = 1, int pageSize = 10)
            : base()
        {
            _data = data ?? new List<T>();
            Success = succeeded;
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
        }

        [JsonProperty(Order = -2)]
        public new List<T> Data
        {
            get => _data;
            set
            {
                _data = value;
                base.Data = default;
            }
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }

        [JsonIgnore]
        public bool HasPreviousPage => CurrentPage > 1;

        [JsonIgnore]
        public bool HasNextPage => CurrentPage < TotalPages;

        public static PaginatedResult<T> Create(List<T> data, int count, int pageNumber, int pageSize)
        {
            return new PaginatedResult<T>(true, data, count, pageNumber, pageSize);
        }
    }
}
