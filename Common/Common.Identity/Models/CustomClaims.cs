using System.Security.Claims;

namespace Common.Identity.Models;

public static class CustomClaims
{
    public static readonly Claim CanDelete = new("SignorPrestito.CanDelete", "true");
    public static readonly Claim Scope = new("scope", "SpWebsite");
    public static readonly Claim CanAdd = new("SignorPrestito.CanAdd", "true");
    public static readonly Claim CanEdit = new("SignorPrestito.CanEdit", "true");
}