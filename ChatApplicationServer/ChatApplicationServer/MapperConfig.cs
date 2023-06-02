using AutoMapper;
using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;

namespace ChatApplicationServer
{
    public class MapperConfig
    {
        public static Mapper InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();

            });
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
