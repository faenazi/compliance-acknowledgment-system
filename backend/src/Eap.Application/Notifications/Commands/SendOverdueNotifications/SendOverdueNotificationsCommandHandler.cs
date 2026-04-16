using Eap.Application.Notifications.Abstractions;
using Eap.Application.Notifications.Models;
using Eap.Domain.Notifications;
using Eap.Domain.Requirements;
using Eap.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Eap.Application.Notifications.Commands.SendOverdueNotifications;

internal sealed class SendOverdueNotificationsCommandHandler
    : IRequestHandler<SendOverdueNotificationsCommand, NotificationResultDto>
{
    private readonly EapDbContext _db;
    private readonly IEmailSender _emailSender;
    private readonly INotificationRepository _notificationRepo;
    private readonly ILogger<SendOverdueNotificationsCommandHandler> _logger;

    public SendOverdueNotificationsCommandHandler(
        EapDbContext db,
        IEmailSender emailSender,
        INotificationRepository notificationRepo,
        ILogger<SendOverdueNotificationsCommandHandler> logger)
    {
        _db = db;
        _emailSender = emailSender;
        _notificationRepo = notificationRepo;
        _logger = logger;
    }

    public async Task<NotificationResultDto> Handle(
        SendOverdueNotificationsCommand request, CancellationToken ct)
    {
        var candidates = await (
            from r in _db.UserActionRequirements
            where r.IsCurrent && r.Status == UserActionRequirementStatus.Overdue
            join u in _db.Users on r.UserId equals u.Id
            join av in _db.AcknowledgmentVersions on r.AcknowledgmentVersionId equals av.Id
            join ad in _db.AcknowledgmentDefinitions on av.AcknowledgmentDefinitionId equals ad.Id
            select new { Requirement = r, User = u, Definition = ad }
        ).ToListAsync(ct);

        int sent = 0, failed = 0, skipped = 0;

        foreach (var c in candidates)
        {
            if (string.IsNullOrWhiteSpace(c.User.Email)) { skipped++; continue; }

            var alreadySent = await _notificationRepo.ExistsAsync(
                c.User.Id, NotificationType.Overdue, c.Requirement.Id, ct);
            if (alreadySent) { skipped++; continue; }

            var subject = $"إجراء متأخر: {c.Definition.Title}";
            var body = $"""
                <div dir="rtl" style="font-family: 'Segoe UI', sans-serif;">
                  <h2>إجراء متأخر</h2>
                  <p>لديك إجراء تجاوز الموعد النهائي: <strong>{c.Definition.Title}</strong></p>
                  {(c.Requirement.DueDate.HasValue ? $"<p>كان الموعد النهائي: {c.Requirement.DueDate:yyyy-MM-dd}</p>" : "")}
                  <p>يرجى إتمام الإجراء في أقرب وقت ممكن.</p>
                </div>
                """;

            var notification = new Notification(
                c.User.Id, c.User.Email, NotificationType.Overdue,
                "UserActionRequirement", c.Requirement.Id, subject, body);

            var (success, failureReason) = await _emailSender.SendAsync(
                c.User.Email, subject, body, ct);

            notification.RecordAttempt(success, failureReason);

            if (success) { notification.MarkSent(DateTimeOffset.UtcNow); sent++; }
            else
            {
                notification.MarkFailed(DateTimeOffset.UtcNow);
                failed++;
                _logger.LogWarning("{AuditEvent} OverdueNotificationFailed {UserId} {Reason}",
                    "OverdueNotificationFailed", c.User.Id, failureReason);
            }

            await _notificationRepo.AddAsync(notification, ct);
        }

        await _notificationRepo.SaveChangesAsync(ct);

        return new NotificationResultDto
        {
            TotalProcessed = candidates.Count,
            Sent = sent, Failed = failed, Skipped = skipped,
        };
    }
}
