﻿using ATA.Library.Client.Service.HostServices.Book.Contracts;
using ATA.Library.Shared.Dto;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ATA.Library.Client.Service.HostServices.Book
{
    public class BookHostService : IBookHostService
    {
        private readonly HttpClient _hostClient;

        #region Constructor Injections

        public BookHostService(HttpClient hostClient)
        {
            _hostClient = hostClient;
        }

        #endregion

        public async Task<ApiResult<BookDto>?> GetBookById(int bookId)
        {
            return await _hostClient.GetFromJsonAsync<ApiResult<BookDto>?>($"api/v1/book/get-by-id?bookId={bookId}");
        }

        public async Task<ApiResult<List<BookDto>>?> GetBooksByCategory(int categoryId)
        {
            return await _hostClient.GetFromJsonAsync<ApiResult<List<BookDto>>>($"api/v1/book/get-by-category?categoryId={categoryId}");
        }

        public async Task<ApiResult<string>?> PostUploadBookFile(MultipartFormDataContent fileContent)
        {
            var httpResponseMessage = await _hostClient.PostAsync("api/v1/book/file-upload", fileContent);

            return await httpResponseMessage.Content.ReadFromJsonAsync<ApiResult<string>>();
        }

        public async Task<ApiResult?> PostAddBook(BookDto book)
        {
            var httpResponseMessage = await _hostClient.PostAsJsonAsync("api/v1/book/add", book);

            return await httpResponseMessage.Content.ReadFromJsonAsync<ApiResult>();
        }

        public async Task<ApiResult?> PutEditBook(BookDto book)
        {
            var httpResponseMessage =
                await _hostClient.PutAsJsonAsync($"api/v1/book/edit?bookId={book.Id}", book);

            if (httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
                return await httpResponseMessage.Content.ReadFromJsonAsync<ApiResult?>();

            return new ApiResult { IsSuccess = true };
        }

        public async Task<ApiResult?> DeleteBook(int bookId)
        {
            var httpResponseMessage = await _hostClient.DeleteAsync($"api/v1/book/delete?bookId={bookId}");

            if (httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
                return await httpResponseMessage.Content.ReadFromJsonAsync<ApiResult?>();

            return new ApiResult { IsSuccess = true };
        }
    }
}