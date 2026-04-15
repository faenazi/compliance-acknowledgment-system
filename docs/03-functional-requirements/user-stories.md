# User Stories

## 1. Purpose

This document defines the key user stories for the Enterprise Acknowledgment Platform (EAP).

It translates the business and functional requirements into implementable stories that can be used for:
- sprint planning
- backlog creation
- development execution
- testing preparation

The stories in this document focus on Phase 1 (MVP).

---

## 2. Story Structure

Each story follows this format:

- As a [role]
- I want [goal]
- So that [business value]

---

## 3. Identity & User Management

### US-001 User Login
As an employee,  
I want to sign in using my corporate Active Directory account,  
so that I can securely access my assigned acknowledgments and disclosures.

### US-002 User Profile Creation
As the system,  
I want to create a local user profile when a user logs in for the first time,  
so that the platform can track actions and maintain user history.

### US-003 User Profile Synchronization
As the system,  
I want to retrieve user attributes from LDAP / Active Directory during login or synchronization,  
so that targeting, reporting, and scoped access remain accurate.

### US-004 Use AD Attributes
As the system,  
I want to use AD attributes such as department and group memberships,  
so that acknowledgments can be assigned to the correct users.

---

## 4. Policy Management

### US-010 Create Policy
As a Policy Manager,  
I want to create a policy record,  
so that the platform can manage policies in a structured way.

### US-011 Upload Policy Document
As a Policy Manager,  
I want to upload a PDF policy document,  
so that users can review the official policy version.

### US-012 Create Policy Version
As a Policy Manager,  
I want to create a new version of a policy,  
so that policy changes can be managed without losing history.

### US-013 Publish Policy Version
As a Publisher,  
I want to publish a policy version,  
so that it becomes available for linking to acknowledgments.

### US-014 View Historical Policy Versions
As an Auditor or authorized user,  
I want to view historical policy versions,  
so that I can verify what was in effect at a given point in time.

---

## 5. Acknowledgment Management

### US-020 Create Acknowledgment
As an Acknowledgment Manager,  
I want to create an acknowledgment definition,  
so that users can be assigned required actions against policies.

### US-021 Link Acknowledgment to Policy Version
As an Acknowledgment Manager,  
I want to link an acknowledgment to a specific policy version,  
so that the required action is tied to the correct document version.

### US-022 Define Action Type
As an Acknowledgment Manager,  
I want to define whether the action is a simple acknowledgment or a form-based disclosure,  
so that the system can present the correct user experience.

### US-023 Define Dates
As an Acknowledgment Manager,  
I want to define start and due dates,  
so that the action can be scheduled and tracked correctly.

### US-024 Publish Acknowledgment
As a Publisher,  
I want to publish an acknowledgment version,  
so that it becomes active for the intended audience.

### US-025 Archive Acknowledgment
As a Publisher,  
I want to archive an acknowledgment version,  
so that it is no longer used for future assignments while still preserving history.

---

## 6. Form-Based Disclosures

### US-030 Configure Form-Based Disclosure
As an Acknowledgment Manager,  
I want to define a structured set of fields for a disclosure form,  
so that business-specific declarations can be collected through the platform.

### US-031 Use Supported Field Types
As an Acknowledgment Manager,  
I want to configure supported field types such as text, date, select, and file upload,  
so that different disclosure scenarios can be supported.

### US-032 Define Required Fields
As an Acknowledgment Manager,  
I want to mark some fields as required,  
so that users must provide mandatory information before submission.

### US-033 Maintain Form by Version
As the system,  
I want each form definition to belong to a specific acknowledgment version,  
so that submitted records remain historically accurate.

### US-034 Submit Disclosure Form
As an employee,  
I want to fill in and submit a required disclosure form,  
so that I can complete my compliance obligation.

### US-035 Preserve Submitted Form Data
As the system,  
I want to store submitted form values with the related user acknowledgment record,  
so that they can be reviewed later for compliance and audit purposes.

---

## 7. Audience Targeting

### US-040 Assign to All Users
As an Acknowledgment Manager,  
I want to assign an action to all users,  
so that organization-wide acknowledgments can be managed efficiently.

### US-041 Assign by Department
As an Acknowledgment Manager,  
I want to assign an action to a department,  
so that only relevant employees receive it.

### US-042 Assign by AD Group
As an Acknowledgment Manager,  
I want to assign an action to users in a specific AD group,  
so that assignments follow the organization’s directory structure.

### US-043 Exclude Specific Users
As an Acknowledgment Manager,  
I want to exclude specific users from an otherwise targeted action,  
so that exceptions can be handled operationally without changing the general rule.

### US-044 Preview Target Audience
As an Acknowledgment Manager,  
I want to preview the target audience before publishing,  
so that I can confirm the action will reach the intended users.

---

## 8. Recurrence & Scheduling

### US-050 Onboarding Only Action
As a business owner,  
I want an action to be required only once at onboarding,  
so that new employees complete mandatory acknowledgments when they join.

### US-051 Annual Action
As a business owner,  
I want an action to recur annually,  
so that periodic acknowledgments and disclosures are renewed on schedule.

### US-052 Onboarding + Annual Action
As a business owner,  
I want an action to occur at onboarding and then recur annually,  
so that both initial and ongoing compliance needs are met.

### US-053 On Change Action
As a business owner,  
I want an action to be re-submitted when a user’s circumstances change,  
so that disclosures stay current.

### US-054 Event-Driven Action
As a business owner,  
I want an action to be submitted when a relevant event occurs,  
so that event-based disclosures such as gifts and hospitality are captured at the right time.

---

## 9. User Portal

### US-060 View Dashboard
As an employee,  
I want to see my required actions on a dashboard,  
so that I know what is pending, completed, or overdue.

### US-061 View Action Details
As an employee,  
I want to open the details of an assigned action,  
so that I can understand what is required.

### US-062 View Linked Policy
As an employee,  
I want to view the linked policy document from the action screen,  
so that I can review the official content before acknowledging or disclosing.

### US-063 Submit Simple Acknowledgment
As an employee,  
I want to submit a simple acknowledgment,  
so that I can confirm I have read and understood the policy.

### US-064 Submit Commitment Acknowledgment
As an employee,  
I want to submit an acknowledgment that includes commitment text,  
so that I can confirm both awareness and commitment.

### US-065 Submit Form-Based Disclosure
As an employee,  
I want to submit a form-based disclosure,  
so that I can provide the required structured information.

### US-066 View Personal History
As an employee,  
I want to view my past acknowledgments and disclosures,  
so that I can track what I have already completed.

### US-067 See Status Clearly
As an employee,  
I want to clearly see whether each assigned action is Pending, Completed, or Overdue,  
so that I can prioritize what I need to do.

---

## 10. Admin Portal

### US-070 Manage Policies
As a Policy Manager,  
I want to manage policy records and versions,  
so that policy content remains organized and version-controlled.

### US-071 Manage Acknowledgments
As an Acknowledgment Manager,  
I want to manage acknowledgment and disclosure definitions,  
so that business actions can be configured and published centrally.

### US-072 Configure Audience
As an Acknowledgment Manager,  
I want to define the target audience of an action,  
so that only relevant users receive it.

### US-073 Configure Recurrence
As an Acknowledgment Manager,  
I want to define recurrence behavior and dates,  
so that actions are triggered correctly.

### US-074 View Compliance Data
As a Compliance Viewer,  
I want to access compliance data and dashboards,  
so that I can monitor completion and follow up where needed.

### US-075 Access Historical Records
As an Auditor or authorized user,  
I want to access historical policies, actions, and submissions,  
so that I can perform governance and audit review.

---

## 11. Submission Processing

### US-080 Record Submission
As the system,  
I want to record every valid user submission with its version and timestamp,  
so that completion can be tracked accurately.

### US-081 Prevent Duplicate Submission
As the system,  
I want to prevent duplicate submissions for the same user and same action version,  
so that the record remains accurate and clean.

### US-082 Update Status to Completed
As the system,  
I want to mark a user requirement as Completed after a valid submission,  
so that compliance reporting remains current.

### US-083 Mark Overdue
As the system,  
I want to mark an action as Overdue when the due date passes without completion,  
so that non-compliance can be tracked.

---

## 12. Compliance Tracking

### US-090 View Compliance by User
As a Compliance Viewer,  
I want to see compliance status by user,  
so that I can identify who has or has not completed required actions.

### US-091 View Compliance by Department
As a Compliance Viewer,  
I want to see compliance status by department,  
so that I can identify trends and ownership areas.

### US-092 Identify Non-Compliant Users
As a Compliance Viewer,  
I want to identify users with Pending or Overdue actions,  
so that I can take follow-up action.

### US-093 Measure Completion
As a Compliance Viewer,  
I want completion metrics by policy and action,  
so that I can understand the level of compliance.

---

## 13. Notifications

### US-100 Send New Assignment Notification
As the system,  
I want to send an email when a new action is assigned,  
so that the user is aware of the requirement.

### US-101 Send Reminder Notification
As the system,  
I want to send reminders before due date,  
so that users have a chance to complete actions on time.

### US-102 Send Overdue Notification
As the system,  
I want to send overdue notifications when users miss deadlines,  
so that delayed actions are highlighted.

### US-103 Log Notification Outcome
As the system,  
I want to record whether notifications were sent successfully or failed,  
so that operational issues can be monitored.

---

## 14. Reporting

### US-110 Compliance Report
As a Compliance Viewer,  
I want a compliance report,  
so that I can assess completion across the organization.

### US-111 Non-Compliant Users Report
As a Compliance Viewer,  
I want a report of users who have not completed required actions,  
so that I can follow up efficiently.

### US-112 Completion by Action Report
As a Compliance Viewer,  
I want a report showing completion by action,  
so that I can evaluate which acknowledgments or disclosures are complete.

### US-113 Completion by Department Report
As a Compliance Viewer,  
I want a report showing completion by department,  
so that I can compare compliance across business units.

### US-114 Export Reports
As an authorized user,  
I want to export reports to Excel or PDF,  
so that I can share or archive them.

---

## 15. Audit Logging

### US-120 Audit Administrative Actions
As an Auditor,  
I want key administrative actions to be logged,  
so that system activity is traceable.

### US-121 Audit User Submissions
As an Auditor,  
I want user acknowledgment and disclosure submissions to be logged,  
so that there is evidence of completion.

### US-122 Preserve Historical Traceability
As an Auditor,  
I want policy versions, acknowledgment versions, and user submissions to remain historically linked,  
so that past states can be reconstructed accurately.

---

## 16. Access Control

### US-130 Role-Based Access
As a System Administrator,  
I want users to be assigned roles,  
so that access is managed consistently.

### US-131 Scoped Access
As a System Administrator,  
I want users to have access scopes such as Global, Department, or Owned Content,  
so that permissions can be controlled without duplicating roles by department.

### US-132 Restrict Access by Role and Scope
As the system,  
I want to enforce role and scope rules on features and data,  
so that users only access what they are allowed to see and manage.

---

## 17. Phase 1 Priority Stories

### US-140 Conflict of Interest Annual Disclosure
As an employee,  
I want to submit my conflict of interest disclosure on onboarding and annually,  
so that I remain compliant with governance requirements.

### US-141 Conflict of Interest Update on Change
As an employee,  
I want to submit an updated disclosure when my situation changes,  
so that the organization has current information.

### US-142 Gifts and Hospitality Event Disclosure
As an employee,  
I want to submit a disclosure when I receive a gift or hospitality offer,  
so that the situation is recorded according to policy.

### US-143 Acceptable Use Acknowledgment
As an employee,  
I want to acknowledge the acceptable use policy when I join,  
so that my awareness is recorded.

### US-144 HR Policy Acknowledgments
As an employee,  
I want to acknowledge HR-related policies during onboarding,  
so that required policies are completed as part of my joining process.
