using BookAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPI.Services
{
   public interface IBookRepository
    {
        ICollection<Book> GetBooks();
        Book GetBook(int bookId);

        Book GetBook(string ISBN);
        decimal GetBookRating(int BookId);
        bool BookExist(int BookId);
        bool BookExist(string ISBN);
        bool IsDuplicateISBN(int BookId, string ISBN);
    }
}
