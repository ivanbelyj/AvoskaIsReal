using AvoskaIsReal.Domain.Entities;
namespace AvoskaIsReal.Domain.Repositories.Abstract
{
    public interface IArticlesRepository
    {
        IQueryable<Article> GetArticles();
        Article? GetArticleById(Guid id);
        void DeleteArticle(Guid id);
        void SaveArticle(Article entity);
    }
}
