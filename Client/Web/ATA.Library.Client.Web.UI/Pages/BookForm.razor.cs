﻿using ATA.Library.Client.Service.HostServices.Category.Contracts;
using ATA.Library.Server.Model.Book;
using ATA.Library.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATA.Library.Client.Web.UI.Pages
{
    public partial class BookForm
    {
        [Inject]
        private ICategoryHostService CategoryHostService { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        private List<CategoryDto> _categories;

        [Parameter]
        public int? BookId { get; set; } = 1;

        private BookDto _book = new BookDto();

        private string _coverImagePreview;

        protected override async Task OnInitializedAsync()
        {
            var categoriesResponse = await CategoryHostService.GetCategories();

            if (categoriesResponse == null)
            {
                ToastService.ShowError("خطا در ارتباط با سرور");
                return;
            }

            if (!categoriesResponse!.IsSuccess)
            {
                ToastService.ShowError(categoriesResponse.Message);
                return;
            }

            _categories = categoriesResponse.Data;

            if (_categories?.Count == 0)
            {
                var errorMessage = "هیچ دسته‌ی مجازی برای شما وجود ندارد. با پشتیبانی تماس بگیرید";
                ToastService.ShowError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }

        private Task HandleBookSubmit()
        {
            throw new NotImplementedException();
        }

        private async Task OnCoverImageFileSelection(InputFileChangeEventArgs e)
        {
            if (e.File.Size > 500000)
            {
                ToastService.ShowError("حجم فایل بیشتر از مقدار مجاز می‌باشد");
                return;
            }

            IBrowserFile coverImgFile = e.File;

            var buffers = new byte[coverImgFile.Size];

            await coverImgFile.OpenReadStream().ReadAsync(buffers);

            string imgMimeType = coverImgFile.ContentType;

            _coverImagePreview = $"data:{imgMimeType};base64,{Convert.ToBase64String(buffers)}";

            Console.WriteLine(coverImgFile.Size);
        }

        private Task OnBookFileSelection(InputFileChangeEventArgs arg)
        {
            throw new NotImplementedException();
        }
    }
}
