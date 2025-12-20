using StartupStarter.Core.Model.DashboardAggregate.Entities;

namespace StartupStarter.Api.Features.DashboardManagement.Dtos;

public static class DashboardExtensions
{
    public static DashboardDto ToDto(this Dashboard dashboard)
    {
        return new DashboardDto
        {
            DashboardId = dashboard.DashboardId,
            ProfileId = dashboard.ProfileId,
            DashboardName = dashboard.DashboardName,
            Layout = dashboard.LayoutType.ToString(),
            IsDefault = dashboard.IsDefault,
            CreatedAt = dashboard.CreatedAt,
            UpdatedAt = dashboard.UpdatedAt,
            DeletedAt = dashboard.DeletedAt
        };
    }

    public static DashboardCardDto ToDto(this DashboardCard card)
    {
        return new DashboardCardDto
        {
            CardId = card.CardId,
            DashboardId = card.DashboardId,
            CardType = card.CardType,
            Configuration = card.ConfigurationJson,
            Position = card.Position.Row,
            Size = card.Position.Width,
            CreatedAt = card.CreatedAt,
            UpdatedAt = card.UpdatedAt
        };
    }

    public static DashboardShareDto ToDto(this DashboardShare share)
    {
        return new DashboardShareDto
        {
            ShareId = share.DashboardShareId,
            DashboardId = share.DashboardId,
            SharedWithUserId = share.SharedWithUserId,
            PermissionLevel = share.PermissionLevel.ToString(),
            CreatedAt = share.SharedAt
        };
    }
}
