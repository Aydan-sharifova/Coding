using FluentValidation;
using MediatR;

namespace Coding.Application.Features.Search;

public enum SearchResultType { Project, File, User, Task }

public sealed record SearchResultDto(
    SearchResultType Type,
    Guid Id,
    string Title,
    string Subtitle,
    Guid? ProjectId,
    string MatchedText,
    string NavigationUrl,
    decimal Rank);

public sealed record SearchGroupDto(SearchResultType Type, IReadOnlyList<SearchResultDto> Items, bool HasMore);
public sealed record GlobalSearchResponse(string Query, int Page, int PageSize, IReadOnlyList<SearchGroupDto> Groups);
public sealed record GlobalSearchQuery(string Query, SearchResultType? Type = null, Guid? ProjectId = null, int Page = 1, int PageSize = 5)
    : IRequest<GlobalSearchResponse>;

public sealed class GlobalSearchQueryValidator : AbstractValidator<GlobalSearchQuery>
{
    public GlobalSearchQueryValidator()
    {
        RuleFor(x => x.Query).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 20);
    }
}
