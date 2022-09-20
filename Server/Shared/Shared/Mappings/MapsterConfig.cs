namespace Shared.Mappings
{
    using Mapster;

    using Domain.Entities;

    using Models.User;

    public class MapsterConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<User, UserDto>.NewConfig()
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName);
        }
    }
}
