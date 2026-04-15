# Lifecycle Models

## 1. Purpose

This document defines the lifecycle models and state transitions for the Enterprise Acknowledgment Platform (EAP).

It describes how the main business entities move through their states and what conditions trigger each transition.

The purpose of these lifecycle models is to ensure:

- consistent system behavior
- clear operational rules
- predictable implementation
- reliable auditability
- alignment between business and technical design

---

## 2. Lifecycle Scope

This document covers the lifecycle of:

- Policy
- Policy Version
- Acknowledgment Definition
- Acknowledgment Version
- User Action Requirement
- User Acknowledgment / Disclosure Submission
- Recurring Action Cycle

---

## 3. Policy Lifecycle

## 3.1 Policy Entity Lifecycle

A policy is the master business object that may contain multiple versions over time.

### States

- Draft
- Published
- Archived

### State Definitions

#### Draft
The policy record exists but is not yet active for business use.

#### Published
The policy has an active published version and is available for linking to acknowledgments.

#### Archived
The policy is no longer active for future use, but its historical versions remain available for reference.

### Allowed Transitions

| From | To | Condition |
|------|----|----------|
| Draft | Published | At least one valid version is published |
| Published | Archived | Policy is retired from active use |
| Archived | Draft | Allowed only if policy is reintroduced under controlled business process |

---

## 3.2 Policy Version Lifecycle

Each policy version is an immutable business snapshot.

### States

- Draft
- Published
- Superseded
- Archived

### State Definitions

#### Draft
The version is being prepared and may still be edited.

#### Published
The version is active and can be linked to acknowledgments.

#### Superseded
The version was previously published but has been replaced by a newer published version.

#### Archived
The version is retained only for historical reference.

### Allowed Transitions

| From | To | Condition |
|------|----|----------|
| Draft | Published | Version is complete and approved for use |
| Published | Superseded | A newer version is published |
| Superseded | Archived | Historical housekeeping or retention policy |
| Draft | Archived | Draft is cancelled or abandoned |

### Rules

- Only one Published version is allowed per policy at a time
- Published versions are immutable
- Any material change requires a new version

---

## 4. Acknowledgment Definition Lifecycle

## 4.1 Acknowledgment Definition Lifecycle

An acknowledgment definition is the master object for actions linked to policies.

### States

- Draft
- Published
- Archived

### State Definitions

#### Draft
The acknowledgment is being created or edited and is not yet active.

#### Published
The acknowledgment has an active version that can be assigned to users.

#### Archived
The acknowledgment is retired from future use.

### Allowed Transitions

| From | To | Condition |
|------|----|----------|
| Draft | Published | At least one valid version is published |
| Published | Archived | Business owner retires the acknowledgment |
| Archived | Draft | Allowed only through controlled reactivation process if needed |

---

## 4.2 Acknowledgment Version Lifecycle

Each acknowledgment version represents a specific business snapshot, including:
- linked policy version
- recurrence model
- audience targeting
- form definition if applicable

### States

- Draft
- Published
- Superseded
- Archived

### State Definitions

#### Draft
The version may still be edited and is not visible to users.

#### Published
The version is active and may generate user requirements.

#### Superseded
The version has been replaced by a newer published version.

#### Archived
The version is retained for history only.

### Allowed Transitions

| From | To | Condition |
|------|----|----------|
| Draft | Published | Business owner publishes the version |
| Published | Superseded | A newer version is published |
| Superseded | Archived | Historical lifecycle management |
| Draft | Archived | Draft cancelled |

### Rules

- Only one Published version is allowed per acknowledgment definition at a time
- Published versions must not be edited in place
- If the form changes materially, a new version is required
- If the linked policy version changes, a new acknowledgment version is typically required

---

## 5. User Action Requirement Lifecycle

A user action requirement represents the obligation or assignment for a specific user to complete a specific acknowledgment version in a specific cycle.

This is the business layer that answers:
- what is required from this user
- in what period
- in what state

### States

- Pending
- Completed
- Overdue
- Cancelled

### State Definitions

#### Pending
The action is assigned to the user and is awaiting submission.

#### Completed
The user has successfully submitted the required acknowledgment or disclosure.

#### Overdue
The due date has passed and the required submission has not been completed.

#### Cancelled
The requirement is no longer valid for the user because of cancellation, reassignment, or business change.

### Allowed Transitions

| From | To | Condition |
|------|----|----------|
| Pending | Completed | Valid submission recorded |
| Pending | Overdue | Due date passes without completion |
| Pending | Cancelled | Requirement withdrawn |
| Overdue | Completed | Late submission recorded |
| Overdue | Cancelled | Requirement withdrawn |
| Completed | Cancelled | Allowed only under controlled administrative reversal or invalidation |

### Rules

- Pending is the default state when a requirement is assigned
- Overdue only applies if a due date exists
- Completed requires a valid submission
- Cancelled must be auditable and restricted

---

## 6. User Submission Lifecycle

A user submission represents the actual completed user action, whether:
- simple acknowledgment
- acknowledgment with commitment
- form-based disclosure

### States

- Draft
- Submitted
- Recorded
- Invalidated

### State Definitions

#### Draft
Optional state if the platform allows saving progress for a form-based disclosure in a future phase.
Not required for Phase 1 unless explicitly implemented.

#### Submitted
The user submitted the action successfully through the user portal.

#### Recorded
The submission has been fully processed and stored as the official record.

#### Invalidated
The submission is marked invalid due to administrative correction, technical duplication resolution, or business reversal.

### Allowed Transitions

| From | To | Condition |
|------|----|----------|
| Draft | Submitted | User completes and submits |
| Submitted | Recorded | System finalizes and stores the submission |
| Recorded | Invalidated | Controlled administrative invalidation |
| Submitted | Invalidated | Processing failure resolution or duplicate handling |

### Phase 1 Guidance

For MVP, the platform may simplify this lifecycle to:

- Submitted
- Invalidated

or even treat a successful submission as directly Recorded if the implementation is synchronous.

---

## 7. Form-Based Disclosure Lifecycle

A form-based disclosure is a specialized submission lifecycle where the user must complete structured input.

### States

- Not Started
- In Progress
- Submitted
- Recorded
- Invalidated

### State Definitions

#### Not Started
The user has a required disclosure but has not started it.

#### In Progress
Optional state if draft saving is supported later.

#### Submitted
The user completed the form and submitted it.

#### Recorded
The form data has been accepted and stored as the official submission.

#### Invalidated
The disclosure is no longer considered valid due to administrative correction or business rule.

### Allowed Transitions

| From | To | Condition |
|------|----|----------|
| Not Started | In Progress | User begins entering data |
| In Progress | Submitted | User submits the form |
| Submitted | Recorded | System validates and records |
| Recorded | Invalidated | Administrative invalidation |
| Not Started | Submitted | If draft state is not implemented and user submits directly |

### Phase 1 Guidance

For MVP, if draft saving is not supported, the practical lifecycle may be:

- Not Started
- Submitted
- Recorded
- Invalidated

---

## 8. Recurring Action Cycle Lifecycle

The platform must support recurring actions such as:
- onboarding only
- annual
- onboarding + annual
- on change
- event-driven

This lifecycle defines how a recurring obligation is created and renewed.

### States

- Scheduled
- Active
- Fulfilled
- Expired
- Renewed

### State Definitions

#### Scheduled
The next cycle is defined but has not started yet.

#### Active
The cycle is currently active and requires user action.

#### Fulfilled
The user completed the action for that cycle.

#### Expired
The cycle ended without valid completion.

#### Renewed
A new cycle has been generated for the next recurrence window.

### Allowed Transitions

| From | To | Condition |
|------|----|----------|
| Scheduled | Active | Start date is reached |
| Active | Fulfilled | User completes action |
| Active | Expired | End date or due date passes without completion |
| Fulfilled | Renewed | New cycle created according to recurrence rule |
| Expired | Renewed | New cycle created according to recurrence rule |

### Recurrence-Specific Rules

#### Onboarding Only
- one cycle only
- no renewal unless explicitly re-triggered by administration

#### Annual
- one active cycle per year
- completion in one year does not satisfy the next year

#### Onboarding + Annual
- one onboarding cycle when user joins
- thereafter annual cycles are generated

#### On Change
- cycle becomes active when a declared change occurs
- previous completion does not remove need for a new change-triggered cycle

#### Event-Driven
- cycle becomes active only when a triggering event exists
- no event means no active requirement

---

## 9. Notification Lifecycle

Notification lifecycle is relevant for reminder and alert management.

### States

- Queued
- Sent
- Failed
- Retried
- Cancelled

### State Definitions

#### Queued
Notification is prepared and waiting for delivery.

#### Sent
Notification was successfully delivered to Exchange or SMTP transport.

#### Failed
Delivery failed.

#### Retried
Notification failed previously and is retried according to delivery logic.

#### Cancelled
Notification is no longer needed.

### Allowed Transitions

| From | To | Condition |
|------|----|----------|
| Queued | Sent | Delivery successful |
| Queued | Failed | Delivery failed |
| Failed | Retried | Retry policy applies |
| Retried | Sent | Retry successful |
| Retried | Failed | Retry failed again |
| Queued | Cancelled | No longer needed |

### Rules

- Notification failure must not change the submission state
- Notification delivery must be logged
- Retry logic is optional in Phase 1 but recommended for reminders

---

## 10. Audit Record Lifecycle

Audit records are immutable operational evidence.

### States

- Recorded
- Retained
- Archived

### State Definitions

#### Recorded
The audit event is captured and stored.

#### Retained
The audit event remains available within the active retention window.

#### Archived
The audit event is moved to historical storage per retention policy.

### Allowed Transitions

| From | To | Condition |
|------|----|----------|
| Recorded | Retained | Normal retention process |
| Retained | Archived | Archive process based on policy |

### Rules

- Audit records must not be edited
- Audit records must remain readable to authorized roles
- Audit records must preserve actor, action, timestamp, and target entity

---

## 11. Lifecycle Relationships

The following relationships must hold true:

### LR-001
A published acknowledgment version must reference a valid policy version.

### LR-002
A user requirement must reference a published acknowledgment version.

### LR-003
A user submission must reference the exact acknowledgment version used at submission time.

### LR-004
A form-based disclosure submission must remain historically linked to the form definition applicable at submission time.

### LR-005
A superseded version may still have historical user records linked to it.

### LR-006
Archiving a master object must not break historical records linked to older versions.

---

## 12. Phase 1 Simplification Rules

To support fast launch, the first release may simplify some lifecycle models while preserving business correctness.

### Phase 1 Simplifications

- no enforcement lifecycle
- no exception lifecycle
- no defer lifecycle
- no approval lifecycle
- no advanced review workflow lifecycle
- optional omission of draft-saving lifecycle for form submissions

### Minimum Required Lifecycles for Phase 1

The following lifecycles must be fully supported in Phase 1:

- Policy Version Lifecycle
- Acknowledgment Version Lifecycle
- User Action Requirement Lifecycle
- User Submission Lifecycle
- Recurring Action Cycle Lifecycle

---

## 13. Summary State Diagrams (Text Form)

## 13.1 Policy Version

Draft → Published → Superseded → Archived

## 13.2 Acknowledgment Version

Draft → Published → Superseded → Archived

## 13.3 User Action Requirement

Pending → Completed  
Pending → Overdue  
Pending → Cancelled  
Overdue → Completed  
Overdue → Cancelled

## 13.4 User Submission

Submitted → Recorded  
Recorded → Invalidated

## 13.5 Recurring Cycle

Scheduled → Active → Fulfilled  
Scheduled → Active → Expired  
Fulfilled → Renewed  
Expired → Renewed
