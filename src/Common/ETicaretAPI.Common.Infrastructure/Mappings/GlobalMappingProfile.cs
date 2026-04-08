using AutoMapper;

namespace ETicaretAPI.Common.Infrastructure.Mappings;

public class GlobalMappingProfile : Profile
{
    public GlobalMappingProfile()
    {
        CreateMap<DateOnly, DateTime>()
            .ConvertUsing(d => d.ToDateTime(TimeOnly.MinValue));

        CreateMap<DateTime, DateOnly>()
            .ConvertUsing(dt => DateOnly.FromDateTime(dt));

        CreateMap<DateOnly?, DateTime?>()
            .ConvertUsing(d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null);

        CreateMap<DateTime?, DateOnly?>()
            .ConvertUsing(dt => dt.HasValue ? DateOnly.FromDateTime(dt.Value) : (DateOnly?)null);
    }
}