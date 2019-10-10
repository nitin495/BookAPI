using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Models;
namespace BookAPI.Services
{
    public interface ICategoryRepository
    {
        //get all categories
        ICollection<Category> GetAllCategoies();
        //get specific category by id
        Category GetCategory(int categoryId);
        //get all categories of a book
        ICollection<Category> GetAllCategoiesForBook(int bookId);
        //get all books in a category
        ICollection<Book> GetAllBooksForCategory(int categoryId);
        //check if category exist
        bool CategoryExist(int categoryId);
        bool IsDuplicateCategoryName(int categoryId, string categoryName);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();

    }
}
