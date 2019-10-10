using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Services;
using BookAPI.Dtos;
using BookAPI.Models;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private ICategoryRepository _categoryRepository;
        private IBookRepository _bookRepository;
        public CategoriesController(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
    }
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(ICollection<CategoryDto>))]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetAllCategoies();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoriesDtos = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDtos.Add(new CategoryDto
                { Id = category.Id,
                    Name = category.Name
                }
                                    );
            }
            return Ok(categoriesDtos);

        }
        [HttpGet(Name = "GetCategory")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        [Route("{categoryId}")]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExist(categoryId))
                NotFound();
            var category = _categoryRepository.GetCategory(categoryId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryDtos = new CategoryDto(){Id= category.Id,Name= category.Name};
            return Ok(categoryDtos);

        }

        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(ICollection<CategoryDto>))]
        public IActionResult GetAllCategoiesForBook(int bookId)
        {
            if (!_bookRepository.BookExist(bookId))
                return NotFound();

            var categories = _categoryRepository.GetAllCategoiesForBook(bookId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoriesDtos = new List<CategoryDto>();
            foreach (var categoryDto in categories)
            {
                categoriesDtos.Add(new CategoryDto()
                    {
                        Id =categoryDto.Id
                        ,Name=categoryDto.Name
                    }
                );
            }
            return Ok(categoriesDtos);

        }
        [HttpGet("{categoryId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(ICollection<CategoryDto>))]
        public IActionResult GetAllBooksForCategory(int categoryId)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (!_categoryRepository.CategoryExist(categoryId))
                return NotFound();

            var books = _categoryRepository.GetAllBooksForCategory(categoryId);
           
            IList<BookDto> booksDto = new List<BookDto>();
            foreach (var bookDto in books)
            {
                booksDto.Add(new BookDto()
                {
                    ISBN = bookDto.Isbn,
                    Title=bookDto.Title,
                    DatePublished=bookDto.DatePublished
                }
                ) ;
            }
            return Ok(booksDto);

        }
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(222)]
        [ProducesResponseType(500)]
        [ProducesResponseType(201,Type =typeof(Category))]
        public IActionResult CreateCategory([FromBody]Category category)
        {
            if (category == null)
                return BadRequest(ModelState);
            var categoryExist = _categoryRepository.GetAllCategoies().Where(c => c.Name.Trim().ToUpper() == category.Name.Trim().ToUpper()).FirstOrDefault();
            if (categoryExist!=null)
            {
                ModelState.AddModelError("",$"Category name {category.Name} already exists.");
                return StatusCode(222,ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("", $"While creating Category {category.Name} some error occur.");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategory",new { categoryId = category.Id},category);

        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]//no content 
        [ProducesResponseType(400)]
        [ProducesResponseType(222)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCategory(int categoryId, [FromBody]Category categoryToUpdate)
        {
            if (categoryToUpdate == null)
                return BadRequest(ModelState);
            if (categoryId != categoryToUpdate.Id)
                return BadRequest(ModelState);
            if (!_categoryRepository.CategoryExist(categoryId))
                return NotFound();
            if (_categoryRepository.IsDuplicateCategoryName(categoryId, categoryToUpdate.Name))
            {
                ModelState.AddModelError("", $"Category with name {categoryToUpdate.Name} already exist.");
                return StatusCode(222, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_categoryRepository.UpdateCategory(categoryToUpdate))
            {
                ModelState.AddModelError("", $"Error occur while updating country {categoryToUpdate.Name}.");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]//no content 
        [ProducesResponseType(400)]
        [ProducesResponseType(222)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExist(categoryId))
                return BadRequest(ModelState);
            
            var category = _categoryRepository.GetCategory(categoryId);
            
            if (_categoryRepository.GetAllBooksForCategory(categoryId).Count() > 0)
            {
                ModelState.AddModelError("", $"Books exist for category {category.Name} category can't be delete.");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("", $"some error occur while delting category {category.Name}.");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }
    }
}
