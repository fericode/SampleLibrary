﻿using ATA.Library.Server.Model.Entities.Category;
using ATA.Library.Server.Service.Book.Contracts;
using ATA.Library.Server.Service.Category.Contracts;
using ATA.Library.Shared.Dto;
using ATA.Library.Shared.Service.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ATA.Library.Server.Api.Controllers.api.Category
{
    /// <summary>
    /// Book Categories based on subjects and related units
    /// </summary>
    [ApiVersion("1")]
    public class CategoryController : BaseApiController
    {
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        #region Constructor Injections

        public CategoryController(ICategoryService categoryService, IMapper mapper, IBookService bookService)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _bookService = bookService;
        }

        #endregion

        /// <summary>
        /// Get all authorized categories
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAsync(cancellationToken);
            return Ok(_mapper.Map<List<CategoryDto>>(categories));
        }

        /// <summary>
        /// Add a category to the categories list
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<IActionResult> AddCategory(CategoryDto dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = _mapper.Map<CategoryEntity>(dto);

            var addedEntity = await _categoryService.AddAsync(entity, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, _mapper.Map<CategoryDto>(addedEntity));
        }

        /// <summary>
        /// Edit an already existing category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("edit")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> EditCategory(int categoryId, CategoryDto dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _categoryService.GetByIdAsync(categoryId, cancellationToken);

            if (entity == null)
                return NotFound($"هیچ دسته‌ای با این شناسه پیدا نشد. شناسه‌ی ارسالی = {categoryId}");

            dto.CreatedAt = entity.CreatedAt; //fix CreateAt mapping issue

            _mapper.Map(dto, entity);

            await _categoryService.UpdateAsync(entity, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete a category from list of categories
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> DeleteCategory(int categoryId, CancellationToken cancellationToken)
        {
            var entity = await _categoryService.GetByIdAsync(categoryId, cancellationToken);

            if (entity == null)
                return NotFound($"هیچ دسته‌ای با این شناسه پیدا نشد. شناسه‌ی ارسالی = {categoryId}");

            var hasBooks = await _bookService.GetAll()
                .AnyAsync(b => b.CategoryId == categoryId, cancellationToken: cancellationToken);

            if (hasBooks)
                throw new DomainLogicException("ابتدا کتاب‌های موجود در این دسته را حذف نمایید");


            await _categoryService.DeleteAsync(categoryId, cancellationToken);

            return NoContent();
        }
    }
}