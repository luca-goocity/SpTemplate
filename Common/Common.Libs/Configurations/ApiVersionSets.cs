using Asp.Versioning.Builder;

namespace Common.Libs.Configurations;

public static class ApiVersionSets
{
    public static readonly ApiVersionSet V1 = new ApiVersionSetBuilder("v1")
        .HasApiVersion(new(1))
        .Build();
    
    public static readonly ApiVersionSet V2 = new ApiVersionSetBuilder("v2")
        .HasApiVersion(new(2))
        .Build();
}