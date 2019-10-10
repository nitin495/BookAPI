using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Dtos;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetBooks()
        {
            var books = _bookRepository.GetBooks();
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var booksDto = new List<BookDto>();

            foreach (var book in books)
            {
                booksDto.Add(new BookDto
                {
                    ISBN=book.Isbn,
                    Title=book.Title,
                    DatePublished=book.DatePublished
                });
            }
            return Ok(booksDto);
        }

        [HttpGet("{bookId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetBook(int bookId)
        {
            if (!_bookRepository.BookExist(bookId))
                NotFound();

            var book = _bookRepository.GetBook(bookId);
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var bookDto = new BookDto()
            {
                ISBN = book.Isbn,
                Title = book.Title,
                DatePublished = book.DatePublished
            };
            
            return Ok(bookDto);
        }

        [HttpGet("ISBN/{bookISBN}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetBook(string bookISBN)
        {
            if (!_bookRepository.BookExist(bookISBN))
                NotFound();

            var book = _bookRepository.GetBook(bookISBN);
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var bookDto = new BookDto()
            {
                ISBN = book.Isbn,
                Title = book.Title,
                DatePublished = book.DatePublished
            };

            return Ok(bookDto);
        }

        [HttpGet("rating/{bookId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetBookRating(int bookId)
        {
            if (!_bookRepository.BookExist(bookId))
                NotFound();
             
            var bookRating = _bookRepository.GetBookRating(bookId);

            if (!ModelState.IsValid)
                BadRequest(ModelState);
            return Ok(bookRating);
        }
    }
}