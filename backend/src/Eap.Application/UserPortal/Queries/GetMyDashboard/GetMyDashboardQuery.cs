using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Queries.GetMyDashboard;

/// <summary>Returns the dashboard summary for the authenticated user (Sprint 6).</summary>
public sealed record GetMyDashboardQuery(int PendingLimit = 5, int RecentLimit = 5)
    : IRequest<MyDashboardDto>;
