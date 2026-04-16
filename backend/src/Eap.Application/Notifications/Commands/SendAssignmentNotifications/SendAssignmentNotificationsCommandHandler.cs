using Eap.Application.Notifications.Abstractions;
using Eap.Application.Notifications.Models;
using Eap.Domain.Notifications;
using Eap.Domain.Requirements;
using Eap.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Eap.Application.Notifications.Commands.SendAssignmentNotifications;

internal sealed class SendAssignmentNotificationsCommandHandler
    : IRequestHandler<SendAssignmentNotificationsCommand, NotificationResultDto>
{
    private readonly EapDbContext _db;
    private readonly IEmailSender _emailSender;
    private readonly INotificationRepository _notificationRepo;
    private readonly ILogger<SendAssignmentNotificationsCommandHandler> _logger;

    public SendAssignmentNotificationsCommandHandler(
        EapDbContext db,
        IEmailSender emailSender,
        INotificationRepository notificationRepo,
        ILogger<SendAssignmentNotificationsCommandHandler> logger)
    {
        _db = db;
        _emailSender = emailSender;
        _notificationRepo = notificationRepo;
        _logger = logger;
    }

    public async Task<NotificationResultDto> Handle(
        SendAssignmentNotificationsCommand request, CancellationToken ct)
    {
        // Find pending requirements that haven't been notified yet.
        var requirementsQuery = from r in _db.UserActionRequirements
                                where r.IsCurrent && r.Status == UserActionRequirementStatus.Pending
                                join u in _db.Users on r.UserId equals u.Id
                                join av in _db.AcknowledgmentVersions on r.AcknowledgmentVersionId equals av.Id
                                join ad in _db.AcknowledgmentDefinitions on av.AcknowledgmentDefinitionId equals ad.Id
                                select new { Requirement = r, User = u, Version = av, Definition = ad };

        if (request.AcknowledgmentVersionId.HasValue)
        {
            requirementsQuery = requirementsQuery.Where(x =>
                x.Version.Id == request.AcknowledgmentVersionId.Value);
        }

        var candidates = await requirementsQuery.ToListAsync(ct);

        int sent = 0, failed = 0, skipped = 0;

        foreach (var c in candidates)
        {
            if (string.IsNullOrWhiteSpace(c.User.Email))
            {
                skipped++;
                continue;
            }

            // Skip if an assignment notification already exists for this requirement.
            var alreadySent = await _notificationRepo.ExistsAsync(
                c.User.Id, NotificationType.Assignment, c.Requirement.Id, ct);
            if (alreadySent)
            {
                skipped++;
                continue;
            }

            var subject = $"إجراء جديد مطلوب: {c.Definition.Title}";
            var body = BuildAssignmentBody(c.Definition.Title, c.Requirement.DueDate);

            var notification = new Notification(
                c.User.Id,
                c.User.Email,
                NotificationType.Assignment,
                "UserActionRequirement",
                c.Requirement.Id,
                subject,
                body);

            var (success, failureReason) = await _emailSender.SendAsync(
                c.User.Email, subject, body, ct);

            notification.RecordAttempt(success, failureReason);

            if (success)
            {
                notification.MarkSent(DateTimeOffset.UtcNow);
                sent++;
            }
            else
            {
                notification.MarkFailed(DateTimeOffset.UtcNow);
                failed++;
                _logger.LogWarning(
                    "{AuditEvent} NotificationFailed {UserId} {Email} {Reason}",
                    "NotificationFailed", c.User.Id, c.User.Email, failureReason);
            }

            await _notificationRepo.AddAsync(notification, ct);
        }

        await _notificationRepo.SaveChangesAsync(ct);

        return new NotificationResultDto
        {
            TotalProcessed = candidates.Count,
            Sent = sent,
            Failed = failed,
            Skipped = skipped,
        };
    }

    private static string BuildAssignmentBody(string actionTitle, DateOnly? dueDate)
    {
        var dueLine = dueDate.HasValue
            ? $"<p>الموعد النهائي: {dueDate.Value:yyyy-MM-dd}</p>"
            : string.Empty;
        return $"""
            <div dir="rtl" style="font-family: 'Segoe UI', sans-serif;">
              <h2>إجراء جديد مطلوب</h2>
              <p>تم تعيين إجراء جديد لك: <strong>{actionTitle}</strong></p>
              {dueLine}
              <p>يرجى الدخول إلى المنصة لإتمام الإجراء المطلوب.</p>
            </div>
            """;
    }
}
