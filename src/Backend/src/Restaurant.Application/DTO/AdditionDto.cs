namespace Restaurant.Application.DTO
{
    public class AdditionDto
    {
        public Guid Id { get; set; }
        public string AdditionName { get; set; }
        public decimal Price { get; set; }
        public string AdditionKind { get; set; }
    }
}
