using AvoskaIsReal.Domain.Entities;
using AvoskaIsReal.Domain.Repositories.Abstract;

namespace AvoskaIsReal.Domain.Repositories.EntityFramework
{
    public class EFTextFieldsRepository : ITextFieldsRepository
    {
        private readonly AppDbContext _appDbContext;
        public EFTextFieldsRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public TextField? GetTextFieldByCodeWord(string codeWord)
        {
            var list = _appDbContext.TextFields.ToArray();
            var res = _appDbContext.TextFields
                .FirstOrDefault(item => item.CodeWord == codeWord);
            return res;
        }

        public TextField? GetTextFieldById(Guid id)
        {
            return _appDbContext.TextFields
                .FirstOrDefault(item => item.Id == id);
        }

        public IQueryable<TextField> GetTextFields()
        {
            return _appDbContext.TextFields;
        }

        public void SaveTextField(TextField entity)
        {
            _appDbContext.Entry(entity).State = entity.Id == default(Guid) ?
                Microsoft.EntityFrameworkCore.EntityState.Added
                : Microsoft.EntityFrameworkCore.EntityState.Modified;
            _appDbContext.SaveChanges();
        }
    }
}
