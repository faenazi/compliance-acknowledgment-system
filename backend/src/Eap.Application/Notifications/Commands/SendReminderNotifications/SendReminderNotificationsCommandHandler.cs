using Eap.Application.Notifications.Abstractions;
using Eap.Application.Notifications.Models;
using Eap.Domain.Notifications;
using Eap.Domain.Requirements;
using Eap.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eap.Application.Notifications.Commands.SendReminderNotifications;

internal sealed class SendReminderNotificationsCommandHandler
    : IRequestHandler<SendReminderNotificationsCommand, NotificationResultDto>
{
    private readonly EapDbContext _db;
    private readonly IEmailSender _emailSender;
    private readonly INotificationRepository _notificationRepo;
    private readonly NotificationOptions _options;
    private readonly ILogger<SendReminderNotificationsCommandHandler> _logger;

    public SendReminderNotificationsCommandHandler(
        EapDbContext db,
        IEmailSender emailSender,
        INotificationRepository notificationRepo,
        IOptions<NotificationOptions> options,
        ILogger<SendReminderNotificationsCommandHandler> logger)
    {
        _db = db;
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

        var candidates = await (
            from r in _db.UserActionRequirements
            where r.IsCurrent
                && r.Status == UserActionRequirementStatus.Pending
                && r.DueDate.HasValue
                && r.DueDate.Value <= cutoffDate
            join u in _db.Users on r.UserId equals u.Id
            join av in _db.AcknowledgmentVersions on r.AcknowledgmentVersionId equals av.Id
            join ad in _db.AcknowledgmentDefinitions on av.AcknowledgmentDefinitionId equals ad.Id
            select new { Requirement = r, User = u, Definition = ad }
        ).ToListAsync(ct);

        int sent = 0, failed = 0, skipped = 0;

        foreach (var c in candidates)
        {
            if (string.IsNullOrWhiteSpace(c.User.Email))
            {
                skipped++;
                continue;
            }

            var alreadySent = await _notificationRepo.ExistsAsync(
                c.User.Id, NotificationType.Reminder, c.Requirement.Id, ct);
            if (alreadySent)
            {
                skipped++;
                continue;
            }

            var subject = $"تذكير: {c.Definition.Title}";
            var body = $"""
                <div dir="rtl" style="font-family: 'Segoe UI', sans-serif;">
                  <h2>تذكير بإجراء مطلوب</h2>
                  <p>لديك إجراء مطلوب: <strong>{c.Definition.Title}</strong></p>
                  <p>الموعد النهائي: {c.Requirement.DueDate:yyyy-MM-dd}</p>
                  <p>يرجى إتمام الإجراء قبل الموعد النهائي.</p>
                </div>
                """;

            var notification = new Notification(
                c.User.Id, c.User.Email, NotificationType.Reminder,
                "UserActionRequirement", c.Requirement.Id, subject, body);

            var (success, failureReason) = await _emailSender.SendAsync(
                c.User.Email, subject, body, ct);

            notification.RecordAttempt(success, failureReason);

            if (success) { notification.MarkSent(DateTimeOffset.UtcNow); sent++; }
            else
            {
                notification.MarkFailed(DateTimeOffset.UtcNow);
                failed++;
                _logger.LogWarning("{AuditEvent} ReminderNotificationFailed {UserId} {Reason}",
                    "ReminderNotificationFailed", c.User.Id, failureReason);
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
