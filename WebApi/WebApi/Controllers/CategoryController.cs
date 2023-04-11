using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.DAL;
using WebApi.Dtos.CategoryDtos;
using WebApi.Dtos.ProductDtos;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public CategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        [HttpGet]
        public IActionResult GetAll(string search, int page = 1) // category uzre pagination lazim olsa rahat istifade ede bilecek
        {
            var query = _appDbContext.Categories
                .Where(c => !c.IsDelete);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(q => q.Name.Contains(search));

            var categories = query.Skip((page - 1) * 2)
            .Take(2)
            .ToList();

            CategoryListDto categoryListDto = new();
            categoryListDto.TotalCount = query.Count();
            categoryListDto.CurrentPage = page;
            categoryListDto.Items = categories.Select(c => new CategoryListItemDto
            {
                Name = c.Name,
                CreatedDate = c.CreatedDate,
                UpdatedDate = c.UpdatedDate
            }).ToList();

            return StatusCode(200, categoryListDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            Category category = _appDbContext.Categories
                .Where(c => !c.IsDelete)
                .FirstOrDefault(x => x.Id == id);
            if (category == null) return StatusCode(StatusCodes.Status404NotFound);

            CategoryReturnDto categoryReturnDto = new()
            {
                Name = category.Name,
                UpdatedDate = category.UpdatedDate,
                CreatedDate = category.CreatedDate
            };

            return Ok(categoryReturnDto);
        }


        [HttpPost]
        public IActionResult AddCategory(CategoryCreateDto categoryCreateDto)
        {
            Category newCategory = new()
            {
                Name = categoryCreateDto.Name,
                IsDelete = categoryCreateDto.IsDelete,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            _appDbContext.Categories.Add(newCategory);
            _appDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created, newCategory);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _appDbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            _appDbContext.Categories.Remove(category);
            _appDbContext.SaveChanges();

            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, CategoryUpdateDto categoryUpdateDto)
        {
            var existCatgeory = _appDbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (existCatgeory == null) return NotFound();

            existCatgeory.Name = categoryUpdateDto.Name;
            existCatgeory.IsDelete = categoryUpdateDto.IsDelete;
            _appDbContext.SaveChanges();

            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpPatch]
        public IActionResult ChangeStatus(int id, bool isActive)
        {
            var existCatgeory = _appDbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (existCatgeory == null) return NotFound();

            existCatgeory.IsDelete = isActive;
            _appDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
