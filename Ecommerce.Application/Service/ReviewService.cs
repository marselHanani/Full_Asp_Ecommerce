using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Ecommerce.Identity.Service.interfaces;
using Mapster;

namespace Ecommerce.Application.Service
{
    public class ReviewService(IUnitOfWork unit, IReviewRepository reviewRepo, IInformationUserService userService)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly IReviewRepository _reviewRepo = reviewRepo;
        private readonly IInformationUserService _userService = userService;
        
        private async Task<List<ReplyResponse>> MapRepliesRecursively(IEnumerable<Review> replies)
        {
            var result = new List<ReplyResponse>();
            
            foreach (var reply in replies)
            {
                var replyResponse = new ReplyResponse
                {
                    Id = reply.Id,
                    Content = reply.Content,
                    AuthorName = await _userService.GetUserNameByIdAsync(reply.UserId),
                    CreatedAt = reply.CreatedAt
                };

                if (reply.Reviews.Any())
                {
                    replyResponse.Replies = await MapRepliesRecursively(reply.Reviews);
                }
                
                result.Add(replyResponse);
            }
            
            return result;
        }

        private async Task<ReviewResponse> CreateReviewResponse(Review review)
        {
            var response = review.Adapt<ReviewResponse>();
            response.AuthorName = await _userService.GetUserNameByIdAsync(review.UserId);
            
            return response;
        }

        public async Task<IEnumerable<ReviewResponse>> GetAllReviewsByProductId(Guid productId)
        {
            var reviews = await _reviewRepo.GetAllReviewsByProductId(productId);
            var result = new List<ReviewResponse>();
            
            foreach (var review in reviews)
            {
                result.Add(await CreateReviewResponse(review));
            }
            return result;
        }

        public async Task<IEnumerable<ReviewResponse>> GetAllReviewsByUserId(Guid userId)
        {
            var reviews = await _reviewRepo.GetAllReviewsByUserId(userId);
            var result = new List<ReviewResponse>();
            foreach (var review in reviews)
            {
                result.Add(await CreateReviewResponse(review));
            }
            return result;
        }

        public async Task<IEnumerable<ReviewResponse>> GetAllReviews()
        {
            var reviews = await _reviewRepo.GetAllReviews();
            var result = new List<ReviewResponse>();
            
            foreach (var review in reviews)
            {
                result.Add(await CreateReviewResponse(review));
            }
            
            return result;
        }

        public async Task<ReviewResponse> GetReviewById(Guid id)
        {
            var review = await _unit.Reviews.GetByIdAsync(id);
            if (review == null) 
                throw new KeyNotFoundException($"Review with ID {id} not found");
            
            return await CreateReviewResponse(review);
        }

        public async Task AddReview(ReviewRequest request, Guid userId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Review request cannot be null");
                
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Review title cannot be empty", nameof(request.Title));
                
            if (string.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentException("Review content cannot be empty", nameof(request.Content));
                
            var review = request.Adapt<Review>();
            review.UserId = userId;
            await _unit.Reviews.AddAsync(review);
            await _unit.SaveChangesAsync();
        }

        public async Task UpdateReview(Guid id, ReviewRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Review request cannot be null");
                
            var review = await _unit.Reviews.GetByIdAsync(id);
            if (review == null) 
                throw new KeyNotFoundException($"Review with ID {id} not found");

            review.Title = request.Title;
            review.Content = request.Content;
            review.Rating = request.Rating;

            await _unit.SaveChangesAsync();
        }

        public async Task DeleteReview(Guid id)
        {
            var review = await _unit.Reviews.GetByIdAsync(id);
            if (review == null) 
                throw new KeyNotFoundException($"Review with ID {id} not found");
                
            _unit.Reviews.Delete(id);
            await _unit.SaveChangesAsync();
        }

        public async Task AddReplyToReview(ReplyReviewRequest request, Guid userId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Reply request cannot be null");
                
            if (string.IsNullOrWhiteSpace(request.Comment))
                throw new ArgumentException("Reply comment cannot be empty", nameof(request.Comment));

            var parentReview = await _unit.Reviews.GetByIdAsync(request.ParentId);
            if (parentReview == null)
                throw new KeyNotFoundException($"Parent review with ID {request.ParentId} not found");
            var replyReview = new Review
            {
                Content = request.Comment,
                Title = "Reply",
                Rating = 1,
                ProductId = parentReview.ProductId,
                UserId = userId,
                ParentReviewId = request.ParentId
            };
            await _unit.Reviews.AddAsync(replyReview);
            await _unit.SaveChangesAsync();
        }
    }
}
