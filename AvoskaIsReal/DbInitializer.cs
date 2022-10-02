using Microsoft.AspNetCore.Identity;
using AvoskaIsReal.Domain;
using AvoskaIsReal.Domain.Entities;

namespace AvoskaIsReal
{
    public class DbInitializer
    {
        private const string defaultIndexText = @"
<div class=""main-page__text-block"">
    <h2 class=""title_2 gray-color"">Кто это?</h2>
    <p class=""main-page__p"">
        Авоська - ужасное и загадочное существо.Она
        вселяет страх и уважение жителей страны, она - самое омерзительное,
        что только есть на свете.
    </p>
</div>
<div class=""main-page__text-block"">
    <h2 class=""title_2 gray-color"">Наша миссия</h2>
    <p class=""main-page__p"">
        Мы собираем всю информацию об Авоське, а также доказательства ее
        существования
    </p>
</div>
";
        private const string defaultContactText = @"
<div class=""contact-page__text-block"">
    <h2 class=""title_1 contact-page__title"">
        Вы знаете что-то об Авоське?
    </h2>
    <p>
        Если вы узнали хоть что-то новое, мы будем признательны, если вы
        сообщите об этом нам.Также принимаются намеки и теории об Авоське и
        ее родственниках
    </p>
</div>
<div class=""contact-page__text-block"">
    <h2 class=""title_1 contact-page__title"">
        Хотите вступить в команду добровольцев?
    </h2>
    <p>
        Вы можете один раз отправить нам письмо, а можете включиться в
        дискуссии об Авоське и стать модератором сайта.В любом случае
        добровольцев ничто не обязывает, так что заходите на чай!
    </p>
</div>

<div class=""contact-page__text-block"">
    <h2 class=""title_1"">По всем вопросам по проекту</h2>
    <ul class=""links-list contact-page__links-list"">
        <li><a class=""link"" href="""">example_email @mail.com</a></li>
        <li><a class=""link"" href="""">www.vk.com/some_vk_id</a></li>
        <li><a class=""link"" href="""">www.other_contacts.com</a></li>
    </ul>
    <ul class=""links-list contact-page__links-list"">
        <li><a class=""link"" href="""">example_email @mail.com</a></li>
        <li><a class=""link"" href="""">www.vk.com/some_vk_id</a></li>
        <li><a class=""link"" href="""">www.other_contacts.com</a></li>
    </ul>
</div>
<div class=""contact-page__text-block"">
    <h2 class=""title_1"">По всем вопросам по Авоське</h2>
    <ul class=""links-list contact-page__links-list"">
        <li><a class=""link"" href="""">example_email @mail.com</a></li>
        <li><a class=""link"" href="""">www.vk.com/some_vk_id</a></li>
        <li><a class=""link"" href="""">www.other_contacts.com</a></li>
    </ul>
</div>";
        private const string defaultAllAboutAvoskaText = @"
Авоська - реальное существо.
Вес: ~600 кг +- 70 кг.Пол: предположительно женский.
Дата рождения: 2000 г.
Диагнозы: параноидальная шизофрения, ...
Обычно Авоська пребывает на мусорках, канализациях,
общественных туалетах.
Пьёт, курит, частенько ворует. Ходит в одежде из рваных
мешков. Никто точно не знает, кто назвал её Авоськой и почему, что
творилось и творится в её жизни. Представляет угрозу
не только для прохожих и исследователей, но и для всей страны.";

        public static async Task InitializeAsync(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, DataManager dataManager,
            IConfiguration configuration)
        {
            // Add roles
            await HasRole(roleManager, "moderator");
            await HasRole(roleManager, "admin");
            await HasRole(roleManager, "owner");

            await HasOwner(configuration, userManager);

            await OwnerHasArticle(dataManager, userManager, configuration);


            // Add text fields
            TextField[] textFields = new TextField[]
            {
                new TextField() {
                    CodeWord = "Index", Text = defaultIndexText,
                    Title = "Все об Авоське",
                    MetaDescription = "MetaDescriptionTest", MetaKeywords = "MetaKeywordsTest"
                },
                new TextField() {
                    CodeWord = "Contact", Text = defaultContactText,
                    Title = "Связаться с нами",
                    MetaDescription = "MetaDescriptionTest", MetaKeywords = "MetaKeywordsTest"
                },
                new TextField() {
                    CodeWord = "AllAboutAvoska", Text = defaultAllAboutAvoskaText,
                    Title = "Все об Авоське",
                    MetaDescription = "MetaDescriptionTest", MetaKeywords = "MetaKeywordsTest"
                },
                new TextField() {
                    CodeWord = "TheoriesAndEvidences", Text = "",
                    Title = "Теории и доказательства",
                    MetaDescription = "MetaDescriptionTest", MetaKeywords = "MetaKeywordsTest"
                },
            };
            foreach (TextField textField in textFields)
            {
                TextField? tf = dataManager.TextFields.GetTextFieldByCodeWord(textField.CodeWord);
                if (tf == null)
                    dataManager.TextFields.SaveTextField(textField);
            }
        }

        private static async Task OwnerHasArticle(DataManager dataManager,
            UserManager<User> userManager, IConfiguration config)
        {
            if (dataManager.Articles.GetArticles().Count() > 0)
                return;

            string ownerEmail = config.GetSection("Project")["OwnerEmail"];
            User author = await userManager.FindByEmailAsync(ownerEmail);
            if (author is null)
                throw new Exception("Невозможно создать статью: " +
                    "не найден автор");  // TODO: logging

            Article article = new Article()
            {
                Title = "Авоська уже здесь!",
                SubTitle = "Авоську видели на Ленинградской",
                MetaDescription = "Авоська уже здесь!",
                MetaKeywords = "авоська, ужас, ленинградская, новости",
                Text = "Недавно в городе Воронеж на улице Ленинградская видели" +
                " Авоську. Есть ли пострадавшие, точно не известно. По словам "
                + "очевидцев, Авоська лазила на мусорке и искала еду.",
                // User = author,
                UserId = author.Id,
                // TitleImageUrl = "nature.jpg"
            };
            dataManager.Articles.SaveArticle(article);
        }

        private static async Task HasRole(RoleManager<IdentityRole> roleManager,
            string roleName)
        {
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                IdentityResult createRes =
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                if (createRes.Errors.Count() != 0)
                {
                    // Todo: log error
                }
            }
        }

        private static async Task HasOwner(IConfiguration configuration,
            UserManager<User> userManager)
        {
            string ownerEmail = configuration.GetSection("Project")["OwnerEmail"];
            // Если пользователь с указанным email уже существует, добавление не требуется
            if ((await userManager.FindByEmailAsync(ownerEmail)) != null)
            {
                return;
            }

            string ownerName = configuration.GetSection("Project")["OwnerUserName"];
            string ownerPassword =
                configuration.GetSection("Project")["OwnerInitialPassword"];
            User owner = new User()
            {
                UserName = ownerName,
                Email = ownerEmail,
            };
            await CreateOwner(userManager, owner, ownerPassword);
        }

        private static async Task CreateOwner(UserManager<User> userManager, User user,
            string userPassword)
        {
            IdentityResult createRes = await userManager.CreateAsync(user, userPassword);
            if (createRes.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "moderator");
                await userManager.AddToRoleAsync(user, "admin");
                await userManager.AddToRoleAsync(user, "owner");
            }
            else
            {
                // Todo: log error
                throw new ApplicationException("Не удалось создать пользователя-владельца.");
            }
        }
    }
}
