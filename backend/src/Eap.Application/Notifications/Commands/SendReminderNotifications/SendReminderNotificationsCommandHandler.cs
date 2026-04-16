using Eap.Application.Notifications.Abstractions;
using Eap.Application.Notifications.Models;
using Eap.Domain.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eap.Application.Notifications.Commands.SendReminderNotifications;

internal sealed class SendReminderNotificationsCommandHandler
    : IRequestHandler<SendReminderNotificationsCommand, NotificationResultDto>
{
    private readonly INotificationCandidateQuery _candidateQuery;
    private readonly IEmailSender _emailSender;
    private readonly INotificationRepository _notificationRepo;
    private readonly NotificationOptions _options;
    private readonly ILogger<SendReminderNotificationsCommandHandler> _logger;

    public SendReminderNotificationsCommandHandler(
        INotificationCandidateQuery candidateQuery,
        IEmailSender emailSender,
        INotificationRepository notificationRepo,
        IOptions<NotificationOptions> options,
        ILogger<SendReminderNotificationsCommandHandler> logger)
    {
        _candidateQuery = candidateQuery;
        _emailSender = emailSender;
        _notificationRepo = notificationRepo;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<NotificationResultDto> Handle(
        SendReminderNotificationsCommand request, CancellationToken ct)
    {
        var reminderDays = request.ReminderDaysBeforeDue ?? _options.ReminderDaysBeforeDue;
        var cutoffDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(reminderDays));

        var candidates = await _candidateQuery.GetReminderCandidatesAsync(cutoffDate, ct);

        int sent = 0, failed = 0, skipped = 0;

        foreach (var c in candidates)
        {
            if (string.IsNullOrWhiteSpace(c.UserEmail))
            {
                skipped++;
                continue;
            }

            var alreadySent = await _notificationRepo.ExistsAsync(
                c.UserId, NotificationType.Reminder, c.RequirementId, ct);
            if (alreadySent)
            {
                skipped++;
                continue;
            }

            var subject = $"تذكير: {c.DefinitionTitle}";
            var body = $"""
                <div dir="rtl" style="font-family: 'Segoe UI', sans-serif;">
                  <h2>تذكير بإجراء مطلوب</h2>
                  <p>لديك إجراء مطلوب: <strong>{c.DefinitionTitle}</strong></p>
                  <p>الموعد النهائي: {c.DueDate:yyyy-MM-dd}</p>
                  <p>يرجى إتمام الإجراء قبل الموعد النهائي.</p>
                </div>
                """;

            var notification = new Notification(
                c.UserId, c.UserEmail, NotificationType.Reminder,
                "UserActionRequirement", c.RequirementId, subject, body);

            var (success, failureReason) = await _emailSender.SendAsync(
                c.UserEmail, subject, body, ct);

            notification.RecordAttempt(success, failureReason);

            if (success) { notification.MarkSent(DateTimeOffset.UtcNow); sent++; }
            else
            {
                notification.MarkFailed(DateTimeOffset.UtcNow);
                failed++;
                _logger.LogWarning("{AuditEvent} ReminderNotificationFailed {UserId} {Reason}",
                    "ReminderNotificationFailed", c.UserId, failureReason);
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
