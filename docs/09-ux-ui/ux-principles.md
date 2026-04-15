# UX Principles

## 1. Purpose

This document defines the UX and visual design principles for the Enterprise Acknowledgment Platform (EAP).

It ensures that the platform:

- aligns with The Environment Fund visual identity
- provides a clear and professional user experience
- supports Arabic-first enterprise usage
- remains consistent across all screens
- avoids unnecessary design experimentation

This document should guide:
- UI design
- frontend implementation
- design system decisions
- component styling
- page layout rules

---

## 2. UX Direction

The platform must follow a UX direction that is:

- institutional
- clear
- calm
- modern
- minimal
- highly readable
- Arabic-first
- task-oriented

The design must support internal enterprise usage, not marketing-style presentation.

The user experience must prioritize:

- clarity
- speed of completion
- low cognitive load
- trust
- consistency
- accessibility

---

## 3. Core UX Principles

## 3.1 Clarity First
Every screen must make it immediately clear:
- what the user is seeing
- what is required
- what action is available
- what the current status is

## 3.2 Action-Oriented Design
The interface must help users complete required actions quickly, especially:
- reading policies
- submitting acknowledgments
- submitting disclosures
- tracking status

## 3.3 Minimal Cognitive Load
The system must avoid:
- crowded layouts
- decorative overload
- excessive visual noise
- unclear hierarchy
- too many competing actions on the same screen

## 3.4 Consistency
The platform must maintain consistency across:
- layout structure
- navigation behavior
- button styles
- status indicators
- forms
- typography
- spacing
- color usage

## 3.5 Readability
Text-heavy screens must remain easy to read in Arabic and English.

This is especially important for:
- policy-related pages
- form instructions
- disclosure sections
- compliance and admin screens

## 3.6 Enterprise Simplicity
The system must feel reliable and professional.
It must not feel playful, experimental, or visually excessive.

---

## 4. Brand Alignment Principles

The UX and UI must align with The Environment Fund visual identity.

### 4.1 Brand Character
The visual experience should communicate:

- professionalism
- credibility
- environmental relevance
- calm authority
- institutional trust

### 4.2 Visual Tone
The visual tone must be:
- clean
- spacious
- light
- refined
- understated

### 4.3 Avoid
The UI must avoid:
- flashy gradients
- random colors
- overly saturated UI
- decorative illustrations unrelated to the brand
- trendy visual effects that weaken institutional tone

---

## 5. Color Principles

The UI color system must follow the official brand palette.

### 5.1 Primary Colors

Use the following primary colors as the main visual foundation:

- Brand Navy / Primary Blue: `#2C3A82`
- Brand Dark / Black: `#0F1822`
- White: `#FFFFFF`

### 5.2 Secondary Colors

Use the following secondary colors as support accents:

- Secondary Blue: `#0051B1`
- Environmental Green: `#C0CB6C`
- Natural Brown: `#A18E77`
- Warm Grey: `#F1EEE8`

### 5.3 General Usage Rules

The default interface must be light and white-based.

Recommended visual balance:

- white is the dominant surface color
- brand navy is the main action and identity color
- dark brand color is used for strong text and contrast
- secondary colors are used as accents only

### 5.4 Color Roles

#### Primary Blue `#2C3A82`
Use for:
- primary buttons
- active states
- selected navigation items
- section headers
- identity emphasis

#### Brand Dark `#0F1822`
Use for:
- primary text
- strong contrast areas
- dark backgrounds when needed
- major headings

#### Secondary Blue `#0051B1`
Use for:
- secondary emphasis
- links
- charts
- informative highlights

#### Environmental Green `#C0CB6C`
Use for:
- environmental accents
- positive states where appropriate
- selected infographic/chart segments
- secondary visual indicators

#### Natural Brown `#A18E77`
Use for:
- tertiary accents
- chart diversity
- subtle badge colors
- neutral emphasis areas

#### Warm Grey `#F1EEE8`
Use for:
- soft backgrounds
- neutral panels
- section separators
- subtle cards or containers

### 5.5 Forbidden Color Behavior

Do not:
- use random colors outside the approved palette
- overuse green or brown in the interface
- make the entire interface blue-heavy
- use bright error/success colors that clash with the brand palette unless functionally necessary

---

## 6. Color Balance Rules

The interface should visually follow the spirit of the brand balance:

### Light UI Priority
The default application mode must be light UI.

The recommended balance is:

- 50%+ white and light neutral space
- primary blue for major actions and identity anchors
- dark text for readability
- small controlled percentages of secondary colors

### Accent Color Discipline
Green, brown, and secondary blue should behave as accents, not primary surfaces.

### Charts and Infographics
Charts must use only the approved palette family unless a functional exception is documented.

---

## 7. Typography Principles

### 7.1 Typography Direction
The official visual direction is based on **Aktiv Grotesk**.

The UI typography must reflect:
- clarity
- professionalism
- strong hierarchy
- Arabic/English consistency

### 7.2 Practical Implementation Principle
If the exact brand font is not technically or legally feasible in the product runtime, the fallback font must preserve the same design direction:
- clean
- modern
- readable
- enterprise-appropriate

### 7.3 Hierarchy
The platform must use a clear text hierarchy:

- Page Title
- Section Title
- Card Title
- Body Text
- Field Label
- Helper Text
- Caption / Metadata

### 7.4 Weight Usage
Recommended weight behavior:

- Bold / SemiBold for main headings
- Medium / SemiBold for section titles
- Regular for body text
- Regular / Light for supporting metadata

### 7.5 Text Readability
Avoid:
- very small body text
- low-contrast text
- long dense paragraphs without spacing
- overly tight line spacing

---

## 8. Layout Principles

### 8.1 Arabic-First / RTL-First
The platform must be designed primarily for Arabic and RTL layout.

### 8.2 Spacious Layout
The design should use:
- clear spacing
- comfortable margins
- predictable alignment
- visual breathing room

### 8.3 Page Structure
Each page should generally follow this structure:

1. Page Title / Context Header
2. Status / Summary Area if relevant
3. Main Content Area
4. Supporting actions / side content if needed

### 8.4 Grid Discipline
Use a structured grid.
Avoid arbitrary placement of content blocks.

### 8.5 Scannability
Important information should be scannable through:
- clear section titles
- grouping
- card structure
- status labels
- spacing
- icon support where useful

---

## 9. Navigation Principles

### 9.1 Predictable Navigation
Navigation must be simple and stable.

Users should always understand:
- where they are
- what section they are in
- how to return
- what actions are available

### 9.2 Role-Aware Navigation
Navigation should adapt to the user role and scope.

### 9.3 Minimal Depth
Avoid deep and confusing navigation levels in MVP.

### 9.4 Dashboard as Primary Entry
The dashboard should act as the main entry point for:
- end users
- compliance viewers
- operational admins

---

## 10. Form UX Principles

### 10.1 Forms Must Be Easy to Complete
The platform includes important form-based disclosures.
These forms must feel:
- structured
- guided
- readable
- low-friction

### 10.2 Sectioning
Long forms must be divided into clear sections.

### 10.3 Labels and Help Text
Every field must have:
- clear label
- optional helper text where needed

### 10.4 Required Fields
Required fields must be clearly marked.

### 10.5 Validation Behavior
Validation messages must be:
- visible
- specific
- placed near the field
- easy to understand

### 10.6 Long Form Support
For disclosure forms such as conflict of interest, the UX must support:
- section headers
- logical field grouping
- enough spacing between questions
- ability to review before submission if needed

---

## 11. Status & Feedback Principles

### 11.1 Status Visibility
Users must always be able to understand the current state of an action.

Key statuses in Phase 1:
- Pending
- Completed
- Overdue

### 11.2 Visual Status Behavior
Statuses must be visually distinct but still brand-consistent.

### 11.3 Success Feedback
After a successful submission, the system must clearly confirm:
- that the action was submitted
- when it was submitted
- what happens next, if anything

### 11.4 Error Feedback
Errors must:
- explain the issue clearly
- indicate what the user should fix
- avoid technical jargon

---

## 12. Card and Table Principles

### 12.1 Cards
Cards should be used for:
- dashboard summaries
- acknowledgment items
- grouped content sections
- report summaries

Cards must be:
- clean
- lightly elevated or bordered
- not visually heavy

### 12.2 Tables
Tables should be used for:
- admin lists
- reports
- history
- compliance lists

Tables must support:
- readability
- sorting where needed
- filtering where needed
- clear column naming
- clean row spacing

---

## 13. Graphic Elements Principles

The brand guideline includes environmental/graphic identity elements and pattern systems.

### 13.1 Allowed Usage
Brand graphic elements may be used in:
- login page
- welcome/dashboard hero section
- empty states
- report covers
- section banners
- onboarding/intro areas

### 13.2 Restricted Usage
Graphic elements should not interfere with:
- tables
- form-heavy screens
- dense operational pages
- policy reading screens

### 13.3 Pattern Usage
Patterns may be used:
- subtly in headers
- as background bands
- in non-intrusive decorative areas

Patterns must never reduce readability.

---

## 14. Logo Usage Principles

### 14.1 Logo Contrast
The correct logo version must be used based on background contrast.

### 14.2 Logo Restraint
The logo should be used in controlled places only, such as:
- main app header
- login page
- official exports
- report cover pages

### 14.3 No Decorative Overuse
Do not use the logo as repetitive decorative content.

---

## 15. Dashboard Principles

### 15.1 End User Dashboard
The end user dashboard must focus on:
- what is required
- what is overdue
- what is completed
- what needs immediate action

### 15.2 Admin Dashboard
The admin/compliance dashboard must focus on:
- counts
- completion rates
- trends
- non-compliant users
- policy/action summaries

### 15.3 Visual Priority
Important information must appear above the fold.

---

## 16. Accessibility Principles

The platform should follow practical accessibility principles, including:

- readable contrast
- clear focus states
- logical keyboard order
- understandable labels
- visible validation messages
- understandable icon usage
- no color-only meaning when possible

---

## 17. Page Complexity Rules

### 17.1 One Primary Purpose per Screen
Each screen should have one primary purpose.

### 17.2 Avoid Multi-Purpose Clutter
Do not overload one screen with too many unrelated actions.

### 17.3 Separate Read vs Manage Flows
Reading a policy, completing an action, and managing an action should remain distinct experiences.

---

## 18. Design System Direction

The platform UI should be built on a reusable design system that includes:

- color tokens
- typography scale
- spacing scale
- button styles
- input styles
- card styles
- table styles
- badge/status styles
- modal/drawer behavior
- empty state patterns

This must be implemented consistently across User Portal and Admin Portal.

---

## 19. UX Anti-Patterns to Avoid

The platform must avoid:

- overly dense dashboards
- too many colors on one screen
- unclear primary buttons
- hidden key actions
- large decorative graphics inside operational screens
- inconsistent spacing
- mixed typography logic
- different form patterns across pages
- unclear state feedback
- crowded admin pages with weak hierarchy

---

## 20. Summary

The UX of the Enterprise Acknowledgment Platform must be:

- Arabic-first
- white-based
- brand-aligned
- calm
- highly readable
- operationally efficient
- suitable for internal enterprise usage

The visual system must be anchored in:
- brand navy
- dark contrast text
- controlled secondary accents
- clear typography hierarchy
- restrained graphic usage
- consistent component behavior
