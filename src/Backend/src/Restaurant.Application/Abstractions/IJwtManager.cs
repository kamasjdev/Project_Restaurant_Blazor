namespace Restaurant.Application.Abstractions
{
    public interface IJwtManager
    {
        string CreateToken(Guid userId, string role);
    }
}
