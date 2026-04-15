# EAP Frontend (Sprint 0)

Next.js foundation for the Enterprise Acknowledgment Platform user and admin portals.

## Stack

- next 16.2.3
- react 19.2.5 / react-dom 19.2.5
- typescript 6.0.2
- tailwindcss 4.2.2 + @tailwindcss/postcss 4.2.2 + postcss 8.5.9
- @tanstack/react-query 5.97.0
- axios 1.15.0
- react-hook-form 7.72.1
- zod 4.3.6
- clsx 2.1.1 + tailwind-merge 3.5.0
- lucide-react 1.8.0

All versions are pinned per `docs/04-solution-design/libraries-and-packages.md`.
No alternatives, no `latest`, no unapproved additions.

## Structure

```
src/
  app/
    layout.tsx            RTL-first root layout (lang=ar, dir=rtl)
    globals.css           Tailwind 4 + @theme token mapping
    providers.tsx         Client providers (TanStack Query)
    page.tsx              Landing placeholder
    (user)/dashboard/     User portal dashboard placeholder
    admin/dashboard/      Admin portal dashboard placeholder
    login/                LDAP/AD login placeholder (Sprint 1)
  components/
    layout/AppShell.tsx   Minimal app shell
    ui/                   Base components: button, card, badge
  lib/
    api/client.ts         Axios instance + ApiError shape
    query/QueryProvider   TanStack Query provider
    tokens/design-tokens  Typed mirror of design tokens
    utils/cn.ts           clsx + tailwind-merge helper
  styles/tokens.css       CSS custom properties from docs/09-ux-ui/design-system-tokens.md
```

## Run

```
npm install
npm run dev
```

Dev server: http://localhost:3000.

## Sprint 0 Scope

Foundation only — no business features. Feature screens land in Sprints 1-8 per `docs/10-delivery/sprint-plan.md`.
