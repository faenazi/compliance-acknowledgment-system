using Eap.Application.Admin.Models;
using MediatR;

namespace Eap.Application.Admin.Queries.GetAdminDashboard;

/// <summary>Returns the operational admin dashboard summary.</summary>
public sealed record GetAdminDashboardQuery(int RecentLimit = 5) : IRequest<AdminDashboardDto>;
