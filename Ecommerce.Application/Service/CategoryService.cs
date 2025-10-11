using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Mapster;
using Slugify;

namespace Ecommerce.Application.Service
{
    public class CategoryService(IUnitOfWork unit,SlugHelper slugHelper,ICategoryRepository categoryRepo)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly SlugHelper _slugHelper = slugHelper;
        private readonly ICategoryRepository _categoryRepo = categoryRepo;

        public async Task<List<CategoryResponse>?> GetAllCategories()
        {
            var categories = await _categoryRepo.GetAllCategoriesWithProducts();
            return categories?.Adapt<List<CategoryResponse>>();
        }

        public async Task<CategoryResponse?> GetCategoryById(Guid id)
        {
            var category = await _categoryRepo.GetCategoryWithProductsById(id);
            return category?.Adapt<CategoryResponse>();
        }

        public async Task CreateNewCategory(CategoryRequest request)
        {
            var category = request.Adapt<Category>();
            category.Slug = slugHelper.GenerateSlug(request.Name);
            await _unit.Categories.AddAsync(category);
            await _unit.SaveChangesAsync();
        }
        public async Task UpdateCategory(Guid id, CategoryRequest request)
        {
            var category = request.Adapt<Category>();
            category.Slug = slugHelper.GenerateSlug(request.Name);
            _unit.Categories.Update(id, category);
            await _unit.SaveChangesAsync();
        }

        public async Task DeleteCategory(Guid id)
        {
            _unit.Categories.Delete(id);
            await _unit.SaveChangesAsync();
        }

        public async Task<CategoryResponse?> GetCategoryBySlug(string slug)
        {
            var category = await _categoryRepo.GetCategoryBySlug(slug);
            return category?.Adapt<CategoryResponse>();
        }

    }
}
