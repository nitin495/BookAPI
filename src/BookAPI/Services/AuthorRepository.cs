using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Models;

namespace BookAPI.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        BookDbContext _authorRepository;
        public AuthorRepository(BookDbContext authorRepository)
        {
            _authorRepository = authorRepository;

        }
        public bool AuthorExists(int authorId)
        {
            return _authorRepository.Authors.Any(a=>a.Id== authorId);
        }

        public bool CreateAuthor(Author author)
        {
            _authorRepository.Add(author);
            return Save();
        }

        public bool DeleteAuthor(Author author)
        {
            _authorRepository.Remove(author);
            return Save();
        }

        public Author GetAuthor(int authorId)
        {
            return _authorRepository.Authors.Where(a=>a.Id== authorId).FirstOrDefault();
        }

        public ICollection<Author> GetAuthors()
        {
            return _authorRepository.Authors.Select(a=>a).ToList();

        }

        public ICollection<Author> GetAuthorsofABook(int bookId)
        {
            return _authorRepository.BookAuthors.Where(ba => ba.BookId == bookId).Select(a=>a.Author).ToList();
            
        }

        public ICollection<Book> GetBooksByAuthor(int authorId)
        {
            return _authorRepository.BookAuthors.Where(ba => ba.AuthorId == authorId).Select(b => b.Book).ToList();
        }

        public bool Save()
        {
            int saveChanges=_authorRepository.SaveChanges();
            return saveChanges >= 0 ? true : false;
        }

        public bool UpdateAuthor(Author author)
        {
            _authorRepository.Update(author);
            return Save();
        }
    }
}
