using Eap.Application.Notifications.Abstractions;
using Eap.Application.Notifications.Models;
using Eap.Domain.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eap.Application.Notifications.Commands.SendOverdueNotifications;

internal sealed class SendOverdueNotificationsCommandHandler
    : IRequestHandler<SendOverdueNotificationsCommand, NotificationResultDto>
{
    private readonly INotificationCandidateQuery _candidateQuery;
    private readonly IEmailSender _emailSender;
    private readonly INotificationRepository _notificationRepo;
    private readonly ILogger<SendOverdueNotificationsCommandHandler> _logger;

    public SendOverdueNotificationsCommandHandler(
        INotificationCandidateQuery candidateQuery,
        IEmailSender emailSender,
        INotificationRepository notificationRepo,
        ILogger<SendOverdueNotificationsCommandHandler> logger)
    {
        _candidateQuery = candidateQuery;
        _emailSender = emailSender;
        _notificationRepo = notificationRepo;
        _logger = logger;
    }

    public async Task<NotificationResultDto> Handle(
        SendOverdueNotificationsCommand request, CancellationToken ct)
    {
        var candidates = await _candidateQuery.GetOverdueCandidatesAsync(ct);

        int sent = 0, failed = 0, skipped = 0;

        foreach (var c in candidates)
        {
            if (string.IsNullOrWhiteSpace(c.UserEmail)) { skipped++; continue; }

            var alreadySent = await _notificationRepo.ExistsAsync(
                c.UserId, NotificationType.Overdue, c.RequirementId, ct);
            if (alreadySent) { skipped++; continue; }

            var subject = $"إجراء متأخر: {c.DefinitionTitle}";
            var body = $"""
                <div dir="rtl" style="font-family: 'Segoe UI', sans-serif;">
                  <h2>إجراء متأخر</h2>
                  <p>لديك إجراء تجاوز الموعد النهائي: <strong>{c.DefinitionTitle}</strong></p>
                  {(c.DueDate.HasValue ? $"<p>كان الموعد النهائي: {c.DueDate:yyyy-MM-dd}</p>" : "")}
                  <p>يرجى إتمام الإجراء في أقرب وقت ممكن.</p>
                </div>
                """;

            var notification = new Notification(
                c.UserId, c.UserEmail, NotificationType.Overdue,
                "UserActionRequirement", c.RequirementId, subject, body);

            var (success, failureReason) = await _emailSender.SendAsync(
                c.UserEmail, subject, body, ct);

            notification.RecordAttempt(success, failureReason);

            if (success) { notification.MarkSent(DateTimeOffset.UtcNow); sent++; }
            else
            {
                notification.MarkFailed(DateTimeOffset.UtcNow);
                failed++;
                _logger.LogWarning("{AuditEvent} OverdueNotificationFailed {UserId} {Reason}",
                    "OverdueNotificationFailed", c.UserId, failureReason);
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
