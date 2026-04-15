/**
 * EAP design tokens mirrored from docs/09-ux-ui/design-system-tokens.md.
 * CSS variables are the source of truth at runtime; this module provides
 * typed constants for logic-layer usage (charts, conditional styling, etc.).
 */

export const brandColors = {
  primary: "#2C3A82",
  dark: "#0F1822",
  white: "#FFFFFF",
  secondaryBlue: "#0051B1",
  green: "#C0CB6C",
  brown: "#A18E77",
  warmGrey: "#F1EEE8",
} as const;

export const statusColors = {
  pending: { bg: "#F8F9FB", text: "#2C3A82", border: "#C7D2FE" },
  completed: { bg: "#F3F7E7", text: "#5C6B1E", border: "#D7E2A0" },
  overdue: { bg: "#FFF4F2", text: "#9A3412", border: "#F5C2B8" },
  draft: { bg: "#F8F9FB", text: "#6B7280", border: "#D1D5DB" },
  published: { bg: "#EEF3FF", text: "#2C3A82", border: "#B8C5F2" },
  archived: { bg: "#F5F5F4", text: "#57534E", border: "#D6D3D1" },
  superseded: { bg: "#F9F7F3", text: "#7C6A58", border: "#D8C9B7" },
} as const;

export const spacing = {
  0: "0",
  1: "4px",
  2: "8px",
  3: "12px",
  4: "16px",
  5: "20px",
  6: "24px",
  8: "32px",
  10: "40px",
  12: "48px",
  16: "64px",
  20: "80px",
} as const;

export const radius = {
  none: "0",
  sm: "6px",
  md: "10px",
  lg: "14px",
  xl: "18px",
  full: "9999px",
} as const;

export const chartSeries = [
  "#2C3A82",
  "#0051B1",
  "#C0CB6C",
  "#A18E77",
  "#8DBCF4",
  "#D8C9B7",
] as const;

export type StatusKey = keyof typeof statusColors;
