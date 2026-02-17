using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Authentication;

[SuppressMessage("Roslynator", "RCS1194:Implement exception constructors")]
public sealed class UnauthorizedException(string message) : Exception(message);