using AvoskaIsReal.Domain.Repositories.Abstract;

namespace AvoskaIsReal.Domain
{
    public class DataManager
    {
        public IArticlesRepository Articles { get; set; }
        public ITextFieldsRepository TextFields { get; set; }

        public DataManager(IArticlesRepository articles,
            ITextFieldsRepository textFields)
        {
            Articles = articles;
            TextFields = textFields;
        }
    }
}
