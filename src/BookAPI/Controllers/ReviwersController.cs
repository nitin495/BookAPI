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
    [Route("api/[Controller]")]
    [ApiController]
    public class ReviwersController : Controller
    {
        IReviewerRepository _reviewerRepository;
        IReviewRepository _reviewRepository;
        public ReviwersController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
        }
        //api//reviwers
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviwers()
        {

            var reviwers = _reviewerRepository.GetReviewers();
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            var reviewerDtos = new List<ReviewerDto>();

            foreach (var reviwer in reviwers)
            {
                reviewerDtos.Add(new ReviewerDto
                {
                    Lastname = reviwer.LastName,
                    Firstname = reviwer.FirstName
                });
            }
            return Ok(reviewerDtos);
        }

        //api//reviwers/reviwerid
        [HttpGet("{reviwerId}")]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviwer(int reviwerId)
        {

            if (!_reviewerRepository.ReviewerExists(reviwerId))
                NotFound();

            var reviwers = _reviewerRepository.GetReviewer(reviwerId);
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            ReviewerDto reviewerDto = new ReviewerDto()
            {
                Lastname = reviwers.LastName,
                Firstname = reviwers.FirstName
            };

            return Ok(reviewerDto);
        }


        //api//reviwers/reviwerid/reviwes
        [HttpGet("{reviwerId}/reviwes")]
        [ProducesResponseType(200, Type = typeof(IEnumerator<ReviewDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviwesByReviewer(int reviwerId)
        {

            if (!_reviewerRepository.ReviewerExists(reviwerId))
                NotFound();

            var reviwes = _reviewerRepository.GetReviewsByReviewer(reviwerId);
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            IList<ReviewDto> reviewDtos = new List<ReviewDto>();

            foreach (var review in reviwes)
            {
                reviewDtos.Add(new ReviewDto {
                    HeadLine = review.Headline,
                    ReviewText = review.ReviewText,
                    Rating = review.Rating
                });

            }
            return Ok(reviewDtos);
        }
        //api/reviewers/reviewid/reviewer
        [HttpGet("{reviewid}/reviewer")]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewerByReview(int reviewId)
        {
            if (_reviewRepository.ReviewExist(reviewId))
                return NotFound();

            var reviwer = _reviewerRepository.GetReviewerByReview(reviewId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            ReviewerDto reviwerDto = new ReviewerDto() {
                Firstname = reviwer.FirstName,
                Lastname = reviwer.LastName
            };

            return Ok(reviwerDto);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(222)]
        [ProducesResponseType(500)]
        [ProducesResponseType(201)]
        public IActionResult CreateReviewer([FromBody]Reviewer reviewerToCreate)
        {
            if (reviewerToCreate == null)
                return BadRequest(ModelState);

            var reviwer = _reviewerRepository.GetReviewers().Where(r => r.FirstName == reviewerToCreate.FirstName
                                                                  && r.LastName == reviewerToCreate.LastName).FirstOrDefault();
            if (reviwer != null)
            {
                ModelState.AddModelError("", $"The reviwer with name {reviewerToCreate.FirstName} {reviewerToCreate.LastName} already exists.");
                return StatusCode(222, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.CreateReviewer(reviewerToCreate))
            {
                ModelState.AddModelError("", $"Some error occur while creating reviewer.");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetReviewer", new { reviwerId = reviewerToCreate.Id }, reviewerToCreate);

        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(222)]
        [ProducesResponseType(500)]
        [ProducesResponseType(201)]
        public IActionResult UpdateReviewer(int reviewId,[FromBody]Reviewer reviewerToUpdate)
        {
            if (reviewerToUpdate == null)
                return BadRequest(ModelState);
            if (reviewId != reviewerToUpdate.Id)
                return BadRequest(ModelState);
            if (!_reviewerRepository.ReviewerExists(reviewId))
                return NotFound(ModelState);
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.UpdateReviewer(reviewerToUpdate))
            {
                ModelState.AddModelError("", $"Some error occur while creating reviewer.");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }
        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]//bad request
        [ProducesResponseType(404)]//Not found
        [ProducesResponseType(500)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();
            var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);
            var reviewsToDelete = _reviewerRepository.GetReviewsByReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
            {
                ModelState.AddModelError("", $"There is some error occur while deleting reviwer {reviewerToDelete.FirstName} {reviewerToDelete.LastName}.");
                return StatusCode(500, ModelState);
            }

            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", $"There is some error occur while deleting review for reviwer {reviewerToDelete.FirstName} {reviewerToDelete.LastName}.");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }


    }
}