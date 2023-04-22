using Gifter.Models;

namespace Gifter.Repositories
{
    public interface IPostRepository
    {
        bool Add(Post post);
        bool Delete(int id);
        List<Post> GetAll(bool includeComments = false);
        Post? GetById(int id, bool includeComments = false);
        bool Update(Post post);
    }
}