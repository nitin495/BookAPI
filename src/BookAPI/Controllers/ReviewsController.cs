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
    public class ReviewsController : Controller
    {
        IReviewerRepository _reviewerRepository;
        IReviewRepository _reviewRepository;
        IBookRepository _bookRepository;
        public ReviewsController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository,IBookRepository bookRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
        }
        [HttpGet("{reviewId}" ,Name = "GetReview")]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExist(reviewId))
                return NotFound();
            var review = _reviewRepository.GetReview(reviewId);
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var reviewDtos = new ReviewDto()
            {
                HeadLine = review.Headline,
                ReviewText = review.ReviewText,
                Rating = review.Rating
            };
            return Ok(reviewDtos);
        }

        //api//reviews
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviews()
        {

            var reviews = _reviewRepository.GetReviews();
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var reviewsDtos = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                reviewsDtos.Add(new ReviewDto
                {
                    HeadLine= review.Headline,
                    ReviewText=review.ReviewText, 
                    Rating=review.Rating 
            });
            }
            return Ok(reviewsDtos);
        }
        //api//reviews/bookId/reviews
        [HttpGet("{bookId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsOfABook(int bookId)
        {

            if (!_bookRepository.BookExist(bookId))
                return NotFound();
            var reviews = _reviewRepository.GetReviewsByBook(bookId);
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var reviewsDtos = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                reviewsDtos.Add(new ReviewDto
                {
                    HeadLine = review.Headline,
                    ReviewText = review.ReviewText,
                    Rating = review.Rating
                });
            }
            return Ok(reviewsDtos);
        }
        //api//reviews/reviewId/book
        [HttpGet("{reviewId}/book")]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBookOfAReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExist(reviewId))
                NotFound();

            var book = _reviewRepository.GetBookofReview(reviewId);
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var bookDto = new BookDto()
            {
                ISBN=book.Isbn,
                Title=book.Title,
                DatePublished=book.DatePublished
            };

            
            return Ok(bookDto);
        }

        [HttpPost]
        [ProducesResponseType(404)]
        [ProducesResponseType(222)]
        [ProducesResponseType(500)]
        [ProducesResponseType(201, Type = typeof(Review))]
        public IActionResult CreateReview([FromBody]Review reviewToCerate)
        {
            if (reviewToCerate == null)
                return BadRequest(ModelState);

            if (_reviewerRepository.GetReviewer(reviewToCerate.Reviewer.Id) == null)
                ModelState.AddModelError("",$"The reviwer with Id {reviewToCerate.Id} doesn't exist.");
            if (_bookRepository.GetBook(reviewToCerate.Id) == null)
                ModelState.AddModelError("", $"The book with Id {reviewToCerate.Book.Id} doesn't exist.");
            if (!ModelState.IsValid)
                return StatusCode(404,ModelState);
            reviewToCerate.Reviewer = _reviewerRepository.GetReviewer(reviewToCerate.Reviewer.Id);
            reviewToCerate.Book = _bookRepository.GetBook(reviewToCerate.Book.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (!_reviewRepository.CreateReview(reviewToCerate))
            {
                ModelState.AddModelError("", $"While creating review some error occur.please try again.");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetReview", new { reviewId = reviewToCerate.Id }, reviewToCerate);

        }


        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]//no content
        [ProducesResponseType(400)]//bad request
        [ProducesResponseType(404)]//not found
        [ProducesResponseType(500)]//internal server error

        public IActionResult UpdateReview(int reviewId,[FromBody]Review reviewToUpdate)
        {
            if (reviewToUpdate == null)
                return BadRequest(ModelState);
            if(reviewId!= reviewToUpdate.Id)
                return BadRequest(ModelState);

            if(_reviewRepository.ReviewExist(reviewId))
                ModelState.AddModelError("", "The review doesn't exist.");
            if (_reviewerRepository.GetReviewer(reviewToUpdate.Reviewer.Id) == null)
                ModelState.AddModelError("", $"The reviwer with Id {reviewToUpdate.Id} doesn't exist.");
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

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]//no content
        [ProducesResponseType(400)]//bad request
        [ProducesResponseType(404)]//not found
        [ProducesResponseType(500)]//internal server error

        public IActionResult DeleteReview(int reviewId)
        {

            if (!_reviewRepository.ReviewExist(reviewId))
                return NotFound(ModelState);

            var review = _reviewRepository.GetReview(reviewId);

          
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_reviewRepository.DeleteReview(review))
            {
                ModelState.AddModelError("", $"Some error occur while deleting review {review.Id}.");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }
    }
}