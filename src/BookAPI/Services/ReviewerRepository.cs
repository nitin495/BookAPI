using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Models;

namespace BookAPI.Services
{
    public class ReviewerRepository : IReviewerRepository
    {
        private BookDbContext _reviewerDbContext;
        public ReviewerRepository(BookDbContext reviewerDbContext)
        {
            _reviewerDbContext = reviewerDbContext;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            _reviewerDbContext.Add(reviewer);
            return Save();
        }

        public bool DeleteReviewer(Reviewer reviewer)
        {
            _reviewerDbContext.Remove(reviewer);
            return Save();
        }

        public Reviewer GetReviewer(int reviwerId)
        {
           return _reviewerDbContext.Reviewers.FirstOrDefault(r => r.Id == reviwerId);
            
        }

        public Reviewer GetReviewerByReview(int reviewId)
        {
           return _reviewerDbContext.Reviews.FirstOrDefault(r => r.Id == reviewId).Reviewer;
        }

        public ICollection<Reviewer> GetReviewers()
        {
          return _reviewerDbContext.Reviewers.OrderBy(r=>r.LastName).ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviwerId)
        {
            return _reviewerDbContext.Reviews.Where(r=>r.Reviewer.Id== reviwerId).ToList();
        }

        public bool ReviewerExists(int reviwerId)
        {
            return _reviewerDbContext.Reviewers.Any(r => r.Id == reviwerId);
        }

        public bool Save()
        {
            int saveRecord=  _reviewerDbContext.SaveChanges();
            return saveRecord >= 0 ? true : false;
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            _reviewerDbContext.Update(reviewer);
            return Save();
        }
    }
}
