# Design System Tokens

## 1. Purpose

This document defines the core design tokens for the Enterprise Acknowledgment Platform (EAP).

It provides a consistent visual foundation for:

- frontend implementation
- reusable UI components
- page templates
- dashboard layouts
- form styling
- table styling
- status indicators
- reporting UI

This token system is based on:
- The Environment Fund visual identity
- the approved UX principles
- Arabic-first enterprise usage
- a light, clean, institutional design approach

---

## 2. Design Token Principles

The token system must follow these principles:

- brand-aligned
- simple
- reusable
- consistent
- scalable
- implementation-friendly

The token model must support:
- User Portal
- Admin Portal
- dashboards
- reports
- forms
- data-heavy pages

---

## 3. Brand Color Tokens

## 3.1 Primary Brand Colors

### `color.brand.primary`
- Value: `#2C3A82`
- Usage:
  - primary buttons
  - active navigation
  - major section headers
  - selected states
  - brand emphasis

### `color.brand.dark`
- Value: `#0F1822`
- Usage:
  - primary text
  - strong contrast areas
  - dark backgrounds
  - high-emphasis content

### `color.brand.white`
- Value: `#FFFFFF`
- Usage:
  - base surfaces
  - cards
  - page backgrounds
  - text on dark backgrounds

---

## 3.2 Secondary Brand Colors

### `color.brand.secondaryBlue`
- Value: `#0051B1`
- Usage:
  - links
  - secondary highlights
  - chart accents
  - info emphasis

### `color.brand.green`
- Value: `#C0CB6C`
- Usage:
  - environmental accent
  - positive highlight
  - chart segments
  - light success-like emphasis where brand-appropriate

### `color.brand.brown`
- Value: `#A18E77`
- Usage:
  - tertiary accents
  - neutral badges
  - chart diversity
  - subtle emphasis

### `color.brand.warmGrey`
- Value: `#F1EEE8`
- Usage:
  - soft neutral backgrounds
  - subtle containers
  - separators
  - low-emphasis cards

---

## 4. Semantic Color Tokens

## 4.1 Surface Tokens

### `color.surface.page`
- Value: `#FFFFFF`

### `color.surface.card`
- Value: `#FFFFFF`

### `color.surface.subtle`
- Value: `#F8F9FB`

### `color.surface.soft`
- Value: `#F1EEE8`

### `color.surface.dark`
- Value: `#0F1822`

### `color.surface.brand`
- Value: `#2C3A82`

---

## 4.2 Text Tokens

### `color.text.primary`
- Value: `#0F1822`

### `color.text.secondary`
- Value: `#4B5563`

### `color.text.tertiary`
- Value: `#6B7280`

### `color.text.inverse`
- Value: `#FFFFFF`

### `color.text.link`
- Value: `#0051B1`

### `color.text.brand`
- Value: `#2C3A82`

### `color.text.disabled`
- Value: `#9CA3AF`

---

## 4.3 Border Tokens

### `color.border.default`
- Value: `#E5E7EB`

### `color.border.soft`
- Value: `#EEF1F4`

### `color.border.strong`
- Value: `#D1D5DB`

### `color.border.brand`
- Value: `#2C3A82`

---

## 4.4 Status Tokens

### Pending
- `color.status.pending.bg`: `#F8F9FB`
- `color.status.pending.text`: `#2C3A82`
- `color.status.pending.border`: `#C7D2FE`

### Completed
- `color.status.completed.bg`: `#F3F7E7`
- `color.status.completed.text`: `#5C6B1E`
- `color.status.completed.border`: `#D7E2A0`

### Overdue
- `color.status.overdue.bg`: `#FFF4F2`
- `color.status.overdue.text`: `#9A3412`
- `color.status.overdue.border`: `#F5C2B8`

### Draft
- `color.status.draft.bg`: `#F8F9FB`
- `color.status.draft.text`: `#6B7280`
- `color.status.draft.border`: `#D1D5DB`

### Published
- `color.status.published.bg`: `#EEF3FF`
- `color.status.published.text`: `#2C3A82`
- `color.status.published.border`: `#B8C5F2`

### Archived
- `color.status.archived.bg`: `#F5F5F4`
- `color.status.archived.text`: `#57534E`
- `color.status.archived.border`: `#D6D3D1`

### Superseded
- `color.status.superseded.bg`: `#F9F7F3`
- `color.status.superseded.text`: `#7C6A58`
- `color.status.superseded.border`: `#D8C9B7`

---

## 4.5 Feedback Tokens

### `color.feedback.info.bg`
- Value: `#EEF3FF`

### `color.feedback.info.text`
- Value: `#0051B1`

### `color.feedback.info.border`
- Value: `#BBD2F5`

### `color.feedback.success.bg`
- Value: `#F3F7E7`

### `color.feedback.success.text`
- Value: `#5C6B1E`

### `color.feedback.success.border`
- Value: `#D7E2A0`

### `color.feedback.warning.bg`
- Value: `#FFF8E8`

### `color.feedback.warning.text`
- Value: `#8A5A14`

### `color.feedback.warning.border`
- Value: `#F1DCA8`

### `color.feedback.error.bg`
- Value: `#FFF4F2`

### `color.feedback.error.text`
- Value: `#9A3412`

### `color.feedback.error.border`
- Value: `#F5C2B8`

---

## 5. Typography Tokens

## 5.1 Typeface Tokens

### `font.family.primary`
- Value: `"Aktiv Grotesk", "IBM Plex Sans Arabic", "Noto Sans Arabic", "Segoe UI", sans-serif`

### `font.family.english`
- Value: `"Aktiv Grotesk", "Inter", "Segoe UI", sans-serif`

### `font.family.mono`
- Value: `"SFMono-Regular", "Consolas", "Liberation Mono", monospace`

### Notes
- Aktiv Grotesk is the intended visual direction
- fallback fonts should remain clean and enterprise-appropriate
- Arabic readability takes priority over stylistic experimentation

---

## 5.2 Font Weight Tokens

### `font.weight.light`
- Value: `300`

### `font.weight.regular`
- Value: `400`

### `font.weight.medium`
- Value: `500`

### `font.weight.semibold`
- Value: `600`

### `font.weight.bold`
- Value: `700`

### `font.weight.black`
- Value: `800`

---

## 5.3 Font Size Tokens

### `font.size.xs`
- Value: `12px`

### `font.size.sm`
- Value: `14px`

### `font.size.md`
- Value: `16px`

### `font.size.lg`
- Value: `18px`

### `font.size.xl`
- Value: `20px`

### `font.size.2xl`
- Value: `24px`

### `font.size.3xl`
- Value: `30px`

### `font.size.4xl`
- Value: `36px`

---

## 5.4 Line Height Tokens

### `line.height.tight`
- Value: `1.25`

### `line.height.normal`
- Value: `1.5`

### `line.height.relaxed`
- Value: `1.75`

---

## 5.5 Typography Role Tokens

### `font.role.pageTitle`
- Size: `30px`
- Weight: `700`
- Line Height: `1.25`

### `font.role.sectionTitle`
- Size: `24px`
- Weight: `600`
- Line Height: `1.3`

### `font.role.cardTitle`
- Size: `18px`
- Weight: `600`
- Line Height: `1.4`

### `font.role.body`
- Size: `16px`
- Weight: `400`
- Line Height: `1.6`

### `font.role.label`
- Size: `14px`
- Weight: `500`
- Line Height: `1.4`

### `font.role.helper`
- Size: `12px`
- Weight: `400`
- Line Height: `1.5`

### `font.role.caption`
- Size: `12px`
- Weight: `400`
- Line Height: `1.4`

---

## 6. Spacing Tokens

Use a 4px spacing base.

### `space.0`
- Value: `0`

### `space.1`
- Value: `4px`

### `space.2`
- Value: `8px`

### `space.3`
- Value: `12px`

### `space.4`
- Value: `16px`

### `space.5`
- Value: `20px`

### `space.6`
- Value: `24px`

### `space.8`
- Value: `32px`

### `space.10`
- Value: `40px`

### `space.12`
- Value: `48px`

### `space.16`
- Value: `64px`

### `space.20`
- Value: `80px`

### Usage Guidance
- `8px–16px` for tight component spacing
- `16px–24px` for card internal spacing
- `24px–32px` for section spacing
- `32px–48px` for page-level spacing

---

## 7. Radius Tokens

### `radius.none`
- Value: `0`

### `radius.sm`
- Value: `6px`

### `radius.md`
- Value: `10px`

### `radius.lg`
- Value: `14px`

### `radius.xl`
- Value: `18px`

### `radius.full`
- Value: `9999px`

### Usage Guidance
- inputs/buttons: `10px`
- cards: `14px`
- pill badges: `9999px`

---

## 8. Shadow Tokens

### `shadow.none`
- Value: `none`

### `shadow.sm`
- Value: `0 1px 2px rgba(15, 24, 34, 0.06)`

### `shadow.md`
- Value: `0 6px 16px rgba(15, 24, 34, 0.08)`

### `shadow.lg`
- Value: `0 12px 30px rgba(15, 24, 34, 0.10)`

### Usage Guidance
- cards: `shadow.sm` or none
- modals: `shadow.md`
- large overlay panels: `shadow.lg`

The design should remain light and not overly elevated.

---

## 9. Layout Tokens

## 9.1 Container Widths

### `layout.container.sm`
- Value: `640px`

### `layout.container.md`
- Value: `768px`

### `layout.container.lg`
- Value: `1024px`

### `layout.container.xl`
- Value: `1280px`

### `layout.container.2xl`
- Value: `1440px`

---

## 9.2 Content Widths

### `layout.content.reading`
- Value: `760px`

### `layout.content.form`
- Value: `880px`

### `layout.content.dashboard`
- Value: `1280px`

### Usage Guidance
- policy reading pages: reading width
- forms: form width
- dashboards/admin lists: dashboard width

---

## 10. Button Tokens

## 10.1 Heights

### `button.height.sm`
- Value: `36px`

### `button.height.md`
- Value: `44px`

### `button.height.lg`
- Value: `52px`

## 10.2 Primary Button

### `button.primary.bg`
- Value: `#2C3A82`

### `button.primary.text`
- Value: `#FFFFFF`

### `button.primary.border`
- Value: `#2C3A82`

### `button.primary.hover`
- Value: `#24306A`

### `button.primary.disabledBg`
- Value: `#C7CEE8`

### `button.primary.disabledText`
- Value: `#FFFFFF`

## 10.3 Secondary Button

### `button.secondary.bg`
- Value: `#FFFFFF`

### `button.secondary.text`
- Value: `#2C3A82`

### `button.secondary.border`
- Value: `#C7D2FE`

### `button.secondary.hover`
- Value: `#F8F9FB`

## 10.4 Ghost Button

### `button.ghost.bg`
- Value: `transparent`

### `button.ghost.text`
- Value: `#2C3A82`

### `button.ghost.hover`
- Value: `rgba(44, 58, 130, 0.06)`

---

## 11. Input Tokens

## 11.1 Input Base

### `input.height`
- Value: `44px`

### `input.bg`
- Value: `#FFFFFF`

### `input.text`
- Value: `#0F1822`

### `input.border`
- Value: `#D1D5DB`

### `input.placeholder`
- Value: `#9CA3AF`

### `input.focus`
- Value: `#2C3A82`

### `input.disabledBg`
- Value: `#F8F9FB`

### `input.disabledText`
- Value: `#9CA3AF`

### `input.radius`
- Value: `10px`

## 11.2 Textarea

### `textarea.minHeight`
- Value: `120px`

## 11.3 Validation States

### `input.success.border`
- Value: `#C0CB6C`

### `input.error.border`
- Value: `#D97757`

### `input.warning.border`
- Value: `#D9B562`

---

## 12. Card Tokens

### `card.bg`
- Value: `#FFFFFF`

### `card.border`
- Value: `#E5E7EB`

### `card.radius`
- Value: `14px`

### `card.padding`
- Value: `24px`

### `card.shadow`
- Value: `0 1px 2px rgba(15, 24, 34, 0.06)`

### `card.headerGap`
- Value: `16px`

### `card.sectionGap`
- Value: `24px`

---

## 13. Table Tokens

### `table.header.bg`
- Value: `#F8F9FB`

### `table.header.text`
- Value: `#0F1822`

### `table.row.bg`
- Value: `#FFFFFF`

### `table.row.hover`
- Value: `#FAFBFD`

### `table.border`
- Value: `#E5E7EB`

### `table.cell.paddingY`
- Value: `14px`

### `table.cell.paddingX`
- Value: `16px`

### `table.radius`
- Value: `14px`

### Usage Guidance
- admin tables should prioritize readability
- avoid dense compact rows unless absolutely necessary
- status columns should use badge tokens

---

## 14. Badge Tokens

### `badge.height`
- Value: `28px`

### `badge.paddingX`
- Value: `10px`

### `badge.radius`
- Value: `9999px`

### `badge.fontSize`
- Value: `12px`

### `badge.fontWeight`
- Value: `600`

---

## 15. Icon Tokens

### `icon.xs`
- Value: `14px`

### `icon.sm`
- Value: `16px`

### `icon.md`
- Value: `20px`

### `icon.lg`
- Value: `24px`

### `icon.xl`
- Value: `32px`

### Usage Guidance
- use icons as support, not decoration
- action icons should always be paired with clear labels when ambiguity is possible

---

## 16. Z-Index Tokens

### `z.base`
- Value: `0`

### `z.dropdown`
- Value: `1000`

### `z.sticky`
- Value: `1100`

### `z.overlay`
- Value: `1200`

### `z.modal`
- Value: `1300`

### `z.toast`
- Value: `1400`

---

## 17. Motion Tokens

Motion must remain subtle and enterprise-appropriate.

### `motion.fast`
- Value: `120ms`

### `motion.normal`
- Value: `180ms`

### `motion.slow`
- Value: `240ms`

### `motion.easing.standard`
- Value: `ease`

### Usage Guidance
- use subtle transitions for hover, focus, open, and close
- avoid exaggerated animation
- do not use motion that slows task completion

---

## 18. Chart Tokens

Charts must use approved brand colors only.

### `chart.series.1`
- Value: `#2C3A82`

### `chart.series.2`
- Value: `#0051B1`

### `chart.series.3`
- Value: `#C0CB6C`

### `chart.series.4`
- Value: `#A18E77`

### `chart.series.5`
- Value: `#8DBCF4`

### `chart.series.6`
- Value: `#D8C9B7`

### `chart.grid`
- Value: `#E5E7EB`

### `chart.axis`
- Value: `#6B7280`

### `chart.label`
- Value: `#0F1822`

### Usage Guidance
- use `series.1–4` as primary palette
- use `series.5–6` only when additional differentiation is needed
- do not use non-brand chart colors without explicit approval

---

## 19. Graphic Tokens

## 19.1 Pattern Usage Tokens

### `graphic.pattern.primary`
- Value: brand pattern using `#2C3A82`

### `graphic.pattern.blue`
- Value: pattern using `#0051B1`

### `graphic.pattern.green`
- Value: pattern using `#C0CB6C`

### `graphic.pattern.brown`
- Value: pattern using `#A18E77`

### Usage Guidance
- use only in:
  - login
  - hero sections
  - empty states
  - report covers
  - soft decorative bands
- do not place behind dense form or table content

---

## 19.2 Logo Tokens

### `logo.variant.light`
- Use on dark backgrounds

### `logo.variant.dark`
- Use on light backgrounds

### `logo.variant.brand`
- Use only where approved by the visual identity rules

### Usage Guidance
- logo usage must preserve contrast
- logo should not be used as repeating decoration

---

## 20. Component Usage Guidance

## 20.1 User Portal
Prioritize:
- larger whitespace
- clear status labels
- simplified card layouts
- strong primary actions

## 20.2 Admin Portal
Prioritize:
- structured tables
- filters
- hierarchy
- dense but readable information display

## 20.3 Forms
Prioritize:
- field grouping
- readable labels
- generous spacing
- inline validation
- visible section separation

---

## 21. RTL Tokens and Direction Rules

### `direction.primary`
- Value: `rtl`

### `text.align.default`
- Value: `right`

### Rules
- all layout tokens must assume RTL-first placement
- left/right-specific tokens should be abstracted where possible into start/end semantics
- components must not assume LTR-only spacing logic

---

## 22. Anti-Pattern Rules

Do not:
- create new colors outside the token system without approval
- use multiple border radii inconsistently
- create one-off spacing values repeatedly
- use shadows too heavily
- create status styles that ignore semantic tokens
- use decorative graphics in dense operational screens
- mix unrelated typography scales across components

---

## 23. Token Implementation Guidance

These tokens should be implemented in the frontend as:

- Tailwind theme extension
- CSS custom properties
- design token constants
- shared UI utility layer

Recommended naming alignment:
- keep token names stable
- separate semantic tokens from raw brand values
- prefer reusable role-based names over one-off page-specific names

---

## 24. Summary

The EAP design system must be based on:

- brand navy
- dark readable text
- white/light surfaces
- controlled secondary accents
- strong Arabic-first typography hierarchy
- minimal and consistent component styling
- restrained brand graphics
- predictable spacing and layout

This token system is intended to make the UI:
- consistent
- scalable
- implementation-ready
- aligned with The Environment Fund identity
