using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Models;

namespace BookAPI.Services
{
    public class ReviewRepository : IReviewRepository
    {

        private BookDbContext _reviewDbContext;
        public ReviewRepository(BookDbContext reviewDbContext)
        {
            _reviewDbContext = reviewDbContext;
        }

        public bool CreateReview(Review review)
        {
            _reviewDbContext.Add(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _reviewDbContext.Remove(review);
            return Save();
        }

        public bool DeleteReviews(IList<Review> reviews)
        {
            _reviewDbContext.RemoveRange(reviews);
            return Save();
        }

        public Book GetBookofReview(int reviewId)
        {
            var bookId= _reviewDbContext.Reviews.Where(r=>r.Id== reviewId).Select(b=>b.Book.Id).FirstOrDefault();
            return _reviewDbContext.Books.Where(b=>b.Id== bookId).FirstOrDefault();
        }

        public Review GetReview(int reviewId)
        {
            return _reviewDbContext.Reviews.FirstOrDefault(r => r.Id == reviewId);
        }

        public ICollection<Review> GetReviews()
        {
            return _reviewDbContext.Reviews.Select(r => r).ToList();

        }

        public ICollection<Review> GetReviewsByBook(int bookId)
        {
            return _reviewDbContext.Books.FirstOrDefault(b => b.Id == bookId).Reviews.Select(r=>r).ToList();
        }

        public bool ReviewExist(int reviewId)
        {
            return _reviewDbContext.Reviews.Any(r=>r.Id== reviewId);
        }

        public bool Save()
        {
            int changes=_reviewDbContext.SaveChanges();
            return changes >= 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            _reviewDbContext.Update(review);
            return Save();
        }
    }
}
