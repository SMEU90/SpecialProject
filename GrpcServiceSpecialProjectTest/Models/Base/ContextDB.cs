using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GrpcServiceSpecialProjectTest.Models.Base
{
    public class ContextDB : DbContext
    {
        //Коллекция сущности Questionnaire
        public DbSet<Questionnaire> Questionnaires { get; set; }
        //Коллекция сущности Country
        public DbSet<Country> Countrys { get; set; }
        //Создание БД
        public ContextDB(DbContextOptions<ContextDB> opt) : base(opt)
        {
            Database.EnsureCreated();
        }
        //Получение контекса
        public static ContextDB GetContext()
        {
            var configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();

            var db_options = new DbContextOptionsBuilder<ContextDB>()
                .UseNpgsql(configuration.GetConnectionString("Default"))
                .Options;

            return new ContextDB(db_options);
        }
    }
}
