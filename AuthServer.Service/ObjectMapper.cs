using AutoMapper;
using System;

namespace AuthServer.Service
{
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> Lazy = new(() =>
             new MapperConfiguration(cfg => cfg.AddProfile<DtoMapper>()).CreateMapper()
        );

        public static IMapper Mapper => Lazy.Value;
    }
}
