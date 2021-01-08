﻿using ATA.Library.Client.Web.Service.Book.Contracts;
using ATA.Library.Client.Web.Service.Category.Contracts;
using ATA.Library.Client.Web.UI.Extensions;
using ATA.Library.Shared.Core;
using ATA.Library.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATA.Library.Client.Web.UI.Pages
{
    public partial class Books
    {
        private List<CategoryDto> _categories;

        private List<BookDto> _books;

        [Parameter]
        public int? CategoryId { get; set; }

        [Parameter]
        public string CategoryTitle { get; set; }

        [Inject]
        private ICategoryWebService CategoryWebService { get; set; }

        [Inject]
        private IBookWebService BookWebService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await JsRuntime.SetLayoutTitle(AppStrings.AppPersianFullName);

            _categories = await CategoryWebService.GetCategories();

            if (_categories.Count == 0)
                return;

            if (CategoryId == null)
            {
                var defaultCategory = _categories.First();

                CategoryId = defaultCategory.Id;

                var newUrl = $"/books/{CategoryId}/{defaultCategory.CategoryName?.Replace(" ", "-")}";

                await JsRuntime.ChangeAddressBarUrl(newUrl);
            }

            _books = await BookWebService.GetBooksByCategory((int)CategoryId);

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (CategoryId != null)
                    await JsRuntime.AddClassToElementById($"categoryId{CategoryId}", "active");
            }
        }

        private async Task StateChangeRequested()
        {
            _books = await BookWebService.GetBooksByCategory((int)CategoryId!);

            StateHasChanged();
        }
    }
}