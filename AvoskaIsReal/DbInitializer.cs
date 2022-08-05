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
        Авоська - ужасное и загадочное существо, живущее в Гавнистане.Она
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
Авоська - реальное существо, живущее в Хавнистане.
Вес: ~600 кг +- 70 кг.Пол: предположительно женский.
Раса: смесь медведя, разумной хавнистанской кисы и других.
Дата рождения: 2000 г.
Диагноз: параноидальная шизофрения, копрофилия,
параноидальный мазахизм (отсюда и копрофилия) и множество других
психических заболеваний.Родилась в семье сбежавших из дома 6 и 7
летних Юноны и Вельсура.Малый возраст родителей -
одна из причин отклонений.
Обычно Авоська пребывает на мусорках, канализациях,
общественных туалетах.
Пьёт, курит, частенько ворует. Ходит нагой, либо в одежде из рваных
мешков. Никто точно не знает, кто назвал её Авоськой и почему, что
творилось и творится в её жизни.В одно время встречалась с 49 -
летним хавнистанцем Хунгрыжей (который одновременно
встречался и с её сёстрами и матерью). Имеет в своем распоряжении
странное существо, называемое говнёнцем, которое способно
перевозить Авоську и изменять свой размер.Представляет угрозу
не только для прохожих и исследователей, но и для всего Хавнистана.
Из большой хавнистанской энциклопедии.
Издательство Загадки Хавнистана. 2019г.";

        public static async Task InitializeAsync(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, DataManager dataManager,
            IConfiguration configuration)
        {
            // Add roles
            await HasRole(roleManager, "moderator");
            await HasRole(roleManager, "admin");
            await HasRole(roleManager, "owner");

            // Create owner's account
            string ownerName = configuration.GetSection("Project")["OwnerUserName"];
            string ownerEmail = configuration.GetSection("Project")["OwnerEmail"];
            string ownerPassword =
                configuration.GetSection("Project")["OwnerInitialPassword"];
            User owner = new User()
            {
                UserName = ownerName,
                Email = ownerEmail,
            };
            IdentityResult createRes = await userManager.CreateAsync(owner, ownerPassword);
            if (createRes.Succeeded)
            {
                await userManager.AddToRoleAsync(owner, "moderator");
                await userManager.AddToRoleAsync(owner, "admin");
                await userManager.AddToRoleAsync(owner, "owner");
            }
            else
            {
                // Todo: log error
            }

            // Add text fields
            TextField[] textFields = new TextField[]
            {
                new TextField() {
                    CodeWord = "Index", Text = defaultIndexText,
                    Title = "Все об Авоське"
                },
                new TextField() {
                    CodeWord = "Contact", Text = defaultContactText,
                    Title = "Связаться с нами"
                },
                new TextField() {
                    CodeWord = "AllAboutAvoska", Text = defaultAllAboutAvoskaText,
                    Title = "Все об Авоське"
                },
            };
            foreach (TextField textField in textFields)
            {
                TextField? tf = dataManager.TextFields.GetTextFieldByCodeWord(textField.CodeWord);
                if (tf == null)
                    dataManager.TextFields.SaveTextField(textField);
            }
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


    }
}
