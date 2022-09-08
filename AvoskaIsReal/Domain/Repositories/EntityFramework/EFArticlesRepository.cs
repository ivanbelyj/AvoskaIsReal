using AvoskaIsReal.Domain.Entities;
using AvoskaIsReal.Domain.Repositories.Abstract;

namespace AvoskaIsReal.Domain.Repositories.EntityFramework
{
    public class EFArticlesRepository : IArticlesRepository
    {
        private readonly AppDbContext _appDbContext;
        public EFArticlesRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void DeleteArticle(Guid id)
        {
            Article? article = GetArticleById(id);

            if (article is null)
                return;
            
            _appDbContext.Articles.Remove(article);
            _appDbContext.SaveChanges();
        }

        public Article? GetArticleById(Guid id)
        {
            return _appDbContext.Articles.FirstOrDefault(item => item.Id == id);
        }

        public IQueryable<Article> GetArticles()
        {
            return _appDbContext.Articles;
        }

        public void SaveArticle(Article entity)
        {
            _appDbContext.Entry(entity).State = entity.Id == default(Guid) ?
                    Microsoft.EntityFrameworkCore.EntityState.Added
                    : Microsoft.EntityFrameworkCore.EntityState.Modified;
            _appDbContext.SaveChanges();
        }
    }
}
