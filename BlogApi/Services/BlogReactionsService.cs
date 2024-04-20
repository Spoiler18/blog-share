using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class BlogReactionsService : IBlogReactionService
    {
        private readonly BlogContext _blogContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BlogReactionsService(BlogContext blogContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _blogContext = blogContext;
        }

        public async Task<List<DetailedReaction>> GetBlogReaction(int? blogId)
        {
            var data = (from reactions in _blogContext.BlogReactions
                        join user in _blogContext.UserDetail
                        on reactions.UserId equals user.UserId
                        where reactions.BlogId == blogId
                        select new DetailedReaction
                        {
                            ReactionId = reactions.ReactionId,
                            UserId = reactions.UserId,
                            BlogId = reactions.BlogId,
                            UserReaction = reactions.UserReaction,
                            UserReactionFullName = user.FirstName + " " + user.LastName,
                        }).ToList();

            return data;
        }

        public async Task<ResponseModel> AddBlogReaction(BlogReaction reaction)
        {
            ResponseModel response = new ResponseModel();

            if (reaction.UserReaction != null)
            {
                BlogReaction newReaction = new BlogReaction
                {
                    UserId = reaction.UserId,
                    BlogId = reaction.BlogId,
                    UserReaction = reaction.UserReaction,
                    CreatedOn = DateTime.Now,
                    CreatedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext),
                };

                try
                {
                    _blogContext.BlogReactions.Add(newReaction);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Reaction Added Successfully!!!";

                }
                catch (Exception ex)
                {
                    response.isError = true;
                    response.isSuccess = false;
                    response.message = "Something Went Wrong!!!";
                }
            }
            else
            {
                response.isError = false;
                response.isSuccess = true;
                response.message = "Empty Reaction!!!";
            }

            return response;
        }

        public async Task<ResponseModel> EditBlogReaction(BlogReaction reaction)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                BlogReaction? editReaction = await _blogContext.BlogReactions.Where(item => item.ReactionId == reaction.ReactionId).FirstOrDefaultAsync();

                if (editReaction != null)
                {
                    editReaction.UserReaction = reaction.UserReaction;
                    editReaction.ModifiedOn = DateTime.Now;
                    editReaction.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);

                    _blogContext.BlogReactions.Update(editReaction);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Reaction Edited Successfully!!!";
                }
                else
                {
                    response.isError = true;
                    response.isSuccess = false;
                    response.message = "Reaction Not Found!!!";
                }
            }
            catch (Exception ex)
            {
                response.isError = true;
                response.isSuccess = false;
                response.message = "Something Went Wrong!!!";
            }

            return response;
        }

        public async Task<ResponseModel> DeleteBlogReaction(int? reactionId)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var reaction = await _blogContext.BlogReactions.Where(reaction => reaction.ReactionId == reactionId).FirstOrDefaultAsync();
                _blogContext.BlogReactions.Remove(reaction);
                await _blogContext.SaveChangesAsync();

                response.isError = false;
                response.isSuccess = true;
                response.message = "Reaction Deleted Successfully!!!";
            }
            catch
            {
                response.isError = false;
                response.isSuccess = true;
                response.message = "Something Went Wrong!!!";
            }

            return response;
        }
    }
}
