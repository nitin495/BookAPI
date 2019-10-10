using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Models;

namespace BookAPI.Services
{
    public class BookRepository : IBookRepository
    {
        private BookDbContext _bookDbContext;
        public BookRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }
        public bool BookExist(int bookId)
        {
           return _bookDbContext.Books.Any(b => b.Id == bookId);
        }

        public bool BookExist(string ISBN)
        {
            return _bookDbContext.Books.Any(b => b.Isbn == ISBN);
        }

        public Book GetBook(int bookId)
        {
            return _bookDbContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Book GetBook(string ISBN)
        {
            return _bookDbContext.Books.Where(b => b.Isbn == ISBN).FirstOrDefault();
        }

        public decimal GetBookRating(int bookId)
        {
            var reviews= _bookDbContext.Reviews.Where(r => r.Book.Id == bookId);
            return reviews.Count() == 0 ? 0 : (decimal)reviews.Sum(r=>r.Rating) / reviews.Count();
        }

        public ICollection<Book> GetBooks()
        {
            return _bookDbContext.Books.Select(b=>b).ToList();
        }

        public bool IsDuplicateISBN(int BookId, string ISBN)
        {
            var Isbn = _bookDbContext.Books.Where(b => b.Isbn.Trim().ToUpper() == ISBN.Trim().ToUpper() && b.Id!=BookId).FirstOrDefault();

            return Isbn == null ? false : true;
        }
    }
}
