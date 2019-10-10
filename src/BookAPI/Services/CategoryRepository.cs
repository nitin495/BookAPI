using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Models;

namespace BookAPI.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private BookDbContext _categoryContext;
        public CategoryRepository(BookDbContext bookDbContext)
        {
            _categoryContext = bookDbContext;
        }

        public bool CategoryExist(int categoryId)
        {
            return _categoryContext.Categories.Any(ca => ca.Id == categoryId);   
        }

        public bool CreateCategory(Category category)
        {
            _categoryContext.Add(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _categoryContext.Remove(category);
            return Save();
        }

        public ICollection<Book> GetAllBooksForCategory(int categoryId)
        {
            return _categoryContext.BookCategories.Where(bc => bc.CategoryId == categoryId).Select(b => b.Book).ToList();
        }

        public ICollection<Category> GetAllCategoies()
        {
           return _categoryContext.Categories.OrderBy(c => c.Name).ToList();
        }

        public ICollection<Category> GetAllCategoiesForBook(int bookId)
        {
            return _categoryContext.BookCategories.Where(bc => bc.BookId == bookId).Select(c=>c.Category).ToList();
        }

        public Category GetCategory(int categoryId)
        {
            return _categoryContext.Categories.Where(ca => ca.Id == categoryId).FirstOrDefault();

        }

        public bool IsDuplicateCategoryName(int categoryId, string categoryName)
        {
            var category = _categoryContext.Categories.Where(c => c.Name.Trim().ToUpper() == categoryName.Trim().ToUpper() && c.Id != categoryId).FirstOrDefault();

            return category == null ? false : true;
        }

        public bool Save()
        {
            var saveChanges=_categoryContext.SaveChanges();
            return  saveChanges > 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _categoryContext.Update(category);
            return Save();
        }
    }
}
