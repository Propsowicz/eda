namespace eda.api.Services.EmailService
{
    public static class MockedEmailList
    {
        public static List<EmailEntity> Get()
        {
            return new List<EmailEntity>
            {
                new EmailEntity { Name = "John Snow", Email = "js@visit.com"},
                new EmailEntity { Name = "Eddy Stark", Email = "eddy@smile.com"},
                new EmailEntity { Name = "Mountain", Email = "mm@mm.com"}
            };
        }
    }

    public class EmailEntity
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
