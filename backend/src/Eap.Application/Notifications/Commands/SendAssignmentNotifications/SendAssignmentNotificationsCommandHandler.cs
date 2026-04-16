using Eap.Application.Notifications.Abstractions;
using Eap.Application.Notifications.Models;
using Eap.Domain.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eap.Application.Notifications.Commands.SendAssignmentNotifications;

internal sealed class SendAssignmentNotificationsCommandHandler
    : IRequestHandler<SendAssignmentNotificationsCommand, NotificationResultDto>
{
    private readonly INotificationCandidateQuery _candidateQuery;
    private readonly IEmailSender _emailSender;
    private readonly INotificationRepository _notificationRepo;
    private readonly ILogger<SendAssignmentNotificationsCommandHandler> _logger;

    public SendAssignmentNotificationsCommandHandler(
        INotificationCandidateQuery candidateQuery,
        IEmailSender emailSender,
        INotificationRepository notificationRepo,
        ILogger<SendAssignmentNotificationsCommandHandler> logger)
    {
        _candidateQuery = candidateQuery;
        _emailSender = emailSender;
        _notificationRepo = notificationRepo;
        _logger = logger;
    }

    public async Task<NotificationResultDto> Handle(
        SendAssignmentNotificationsCommand request, CancellationToken ct)
    {
        var candidates = await _candidateQuery.GetPendingAssignmentCandidatesAsync(
            request.AcknowledgmentVersionId, ct);

        int sent = 0, failed = 0, skipped = 0;

        foreach (var c in candidates)
        {
            if (string.IsNullOrWhiteSpace(c.UserEmail))
            {
                skipped++;
                continue;
            }

            // Skip if an assignment notification already exists for this requirement.
            var alreadySent = await _notificationRepo.ExistsAsync(
                c.UserId, NotificationType.Assignment, c.RequirementId, ct);
            if (alreadySent)
            {
                skipped++;
                continue;
            }

            var subject = $"إجراء جديد مطلوب: {c.DefinitionTitle}";
            var body = BuildAssignmentBody(c.DefinitionTitle, c.DueDate);

            var notification = new Notification(
                c.UserId,
                c.UserEmail,
                NotificationType.Assignment,
                "UserActionRequirement",
                c.RequirementId,
                subject,
                body);

            var (success, failureReason) = await _emailSender.SendAsync(
                c.UserEmail, subject, body, ct);

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
                    "NotificationFailed", c.UserId, c.UserEmail, failureReason);
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
