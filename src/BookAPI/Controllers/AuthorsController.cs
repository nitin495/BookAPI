using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Dtos;
using BookAPI.Models;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : Controller
    {
        IAuthorRepository _authorRepository;
        IBookRepository _bookRepository;
        ICountryRepository _countryRepository;
        public AuthorsController(IAuthorRepository authorRepository,IBookRepository bookRepository, ICountryRepository countryRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _countryRepository = countryRepository;
        }
        //api/Authors//
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetAuthors()
        {
            var authors = _authorRepository.GetAuthors();
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var authorsDtos = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorsDtos.Add(new AuthorDto
                {
                    FirstName=author.FirstName,
                    LastName=author.LastName
                });
            }
            return Ok(authorsDtos);
        }

        //api/Authors/authorid
        [HttpGet("{authorid}",Name = "GetAuthor") ]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetAuthor(int authorid)
        {
            if (_authorRepository.AuthorExists(authorid))
                return NotFound();

            var author = _authorRepository.GetAuthor(authorid);

            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var authorDtos = new AuthorDto()
            {
                FirstName = author.FirstName,
                LastName = author.LastName
            };
            
            return Ok(authorDtos);
        }

        //api/Authors/authorid/books
        [HttpGet("{authorid}/books")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetBooksByAuthor(int authorid)
        {
            if (_authorRepository.AuthorExists(authorid))
                return NotFound();

            var booksOfAuthor = _authorRepository.GetBooksByAuthor(authorid);

            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var booksDto = new List<BookDto>();

            foreach (var book in booksOfAuthor)
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


        //api/Authors/authorid/books
        [HttpGet("{bookId}/authors")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetAuthorsOfABook(int bookId)
        {
            if (!_bookRepository.BookExist(bookId))
                return NotFound();

            var authorsOfABook = _authorRepository.GetAuthorsofABook(bookId);

            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var authorDto = new List<AuthorDto>();

            foreach (var author in authorsOfABook)
            {
                authorDto.Add(new AuthorDto
                {
                    FirstName=author.FirstName,
                    LastName=author.LastName
                });

            }
            return Ok(authorDto);

        }


        [HttpPost]
        [ProducesResponseType(400)]//bad request
        [ProducesResponseType(404)]//not found
        [ProducesResponseType(500)]
        [ProducesResponseType(201, Type = typeof(Author))]
        public IActionResult CreateAuthor([FromBody]Author authorToCerate)
        {
            if (authorToCerate == null)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExists(authorToCerate.Country.Id))
            {
                ModelState.AddModelError("", $"The country with Id {authorToCerate.Country.Id} doesn't exist.");
                return NotFound(ModelState);
            }
           
            authorToCerate.Country = _countryRepository.GetCountry(authorToCerate.Country.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (!_authorRepository.CreateAuthor(authorToCerate))
            {
                ModelState.AddModelError("", $"While creating author some error occur.please try again.");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetAuthor", new { authorId = authorToCerate.Id }, authorToCerate);

        }

        [HttpPut("{authorId}")]
        [ProducesResponseType(204)]//no content
        [ProducesResponseType(400)]//bad request
        [ProducesResponseType(404)]//not found
        [ProducesResponseType(500)]//internal server error

        public IActionResult UpdateReview(int authorId, [FromBody]Author authorToUpdate)
        {
            if (authorToUpdate == null)
                return BadRequest(ModelState);
            if (authorId != authorToUpdate.Id)
                return BadRequest(ModelState);

            if (_authorRepository.AuthorExists(authorId))
                ModelState.AddModelError("", "The author doesn't exist.");
            if (_countryRepository.GetCountry(authorToUpdate.Country.Id) == null)
                ModelState.AddModelError("", $"The country with Id {authorToUpdate.Id} doesn't exist.");
            if (_bookRepository.GetBook(reviewToUpdate.Id) == null)
                ModelState.AddModelError("", $"The book with Id {reviewToUpdate.Book.Id} doesn't exist.");
            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);
            reviewToUpdate.Reviewer = _reviewerRepository.GetReviewer(reviewToUpdate.Reviewer.Id);
            reviewToUpdate.Book = _bookRepository.GetBook(reviewToUpdate.Book.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (!_reviewRepository.UpdateReview(reviewToUpdate))
            {
                ModelState.AddModelError("", $"While creating review some error occur.please try again.");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

    }
}
