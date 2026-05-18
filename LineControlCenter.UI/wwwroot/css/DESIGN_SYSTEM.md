# Line Control Center — Design System Documentation

## 🎨 Color Palette

### Base Backgrounds
| Variable | Hex | Usage |
|---|---|---|
| `--color-bg-base` | `#0a1628` | Main application background |
| `--color-bg-card` | `#0d1f3c` | Card/panel backgrounds |
| `--color-bg-panel` | `#112244` | Secondary panels |
| `--color-bg-elevated` | `#1a2d4a` | Elevated surfaces (hover states) |
| `--color-border` | `#1a3a5c` | Default borders |
| `--color-border-bright` | `#2d5278` | Focus/interactive borders |

### Accent Colors
| Variable | Hex | Usage |
|---|---|---|
| `--color-accent-cyan` | `#00b4d8` | Primary accent, links, interactive elements |
| `--color-accent-teal` | `#00e5c0` | Secondary accent |
| `--color-accent-green` | `#7ed957` | Success states, positive metrics |
| `--color-accent-blue` | `#2563eb` | Information, gradients |
| `--color-accent-purple` | `#a855f7` | Analytics, AI features |
| `--color-accent-orange` | `#ff6b35` | Safety, warnings |

### Status Colors
| Variable | Hex | Usage |
|---|---|---|
| `--color-status-red` | `#ff3b3b` | Errors, failures, critical alerts |
| `--color-status-amber` | `#f59e0b` | Warnings, attention needed |
| `--color-status-green` | `#22c55e` | Success, passing tests |
| `--color-status-blue` | `#00b4d8` | Information |

### Text Colors
| Variable | Hex | Usage |
|---|---|---|
| `--color-text-primary` | `#e8f4f8` | Primary content text |
| `--color-text-secondary` | `#7ab3cc` | Labels, secondary content |
| `--color-text-dim` | `#3d6b8a` | Disabled states, very low priority |
| `--color-text-link` | `#00b4d8` | Clickable links |

### Shift-Specific
| Variable | Hex | Usage |
|---|---|---|
| `--color-shift-morning` | `#f59e0b` | Morning shift indicator |
| `--color-shift-night` | `#00b4d8` | Night shift indicator |

---

## 📐 Spacing Scale

```css
--space-xs:  0.25rem;  /* 4px  */
--space-sm:  0.5rem;   /* 8px  */
--space-md:  1rem;     /* 16px */
--space-lg:  1.5rem;   /* 24px */
--space-xl:  2rem;     /* 32px */
--space-2xl: 3rem;     /* 48px */
```

**Usage:** Prefer using variables over hard-coded pixel values.

---

## 🔲 Border Radius

```css
--radius-sm:  0.5rem;   /* 8px  */
--radius-md:  0.75rem;  /* 12px */
--radius-lg:  1rem;     /* 16px */
--radius-xl:  1.25rem;  /* 20px */
```

---

## 🌑 Shadows

```css
--shadow-sm: 0 2px 8px rgba(0, 0, 0, 0.3);
--shadow-md: 0 4px 16px rgba(0, 0, 0, 0.4);
--shadow-lg: 0 8px 32px rgba(0, 0, 0, 0.5);
--shadow-glow-cyan: 0 0 20px rgba(0, 180, 216, 0.15);
--shadow-glow-green: 0 0 20px rgba(126, 217, 87, 0.15);
--shadow-glow-red: 0 0 20px rgba(255, 59, 59, 0.15);
```

---

## 🅰️ Typography

### Font Families
```css
--font-primary: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, ...;
--font-display: 'Orbitron', monospace;  /* For headings, metrics */
--font-mono: 'Roboto Mono', 'Courier New', monospace;
```

### Font Sizes
```css
--text-xs:   0.75rem;   /* 12px */
--text-sm:   0.875rem;  /* 14px */
--text-base: 1rem;      /* 16px */
--text-lg:   1.125rem;  /* 18px */
--text-xl:   1.25rem;   /* 20px */
--text-2xl:  1.5rem;    /* 24px */
--text-3xl:  1.875rem;  /* 30px */
```

---

## ⏱️ Transitions

```css
--transition-fast: 150ms ease;
--transition-base: 250ms ease;
--transition-slow: 350ms ease;
```

---

## 📚 Utility Classes

### Text Colors
```html
<div class="text-primary">Primary text</div>
<div class="text-secondary">Secondary text</div>
<div class="text-dim">Dimmed text</div>
<div class="text-success">Success message</div>
<div class="text-warning">Warning message</div>
<div class="text-danger">Error message</div>
```

### Backgrounds
```html
<div class="bg-base">Base background</div>
<div class="bg-card">Card background</div>
<div class="bg-panel">Panel background</div>
```

### Borders & Radius
```html
<div class="border-default rounded-lg">Card</div>
<div class="border-bright rounded-xl">Interactive element</div>
```

### Shadows
```html
<div class="shadow-sm">Subtle shadow</div>
<div class="shadow-md">Medium shadow</div>
<div class="shadow-lg">Large shadow</div>
```

---

## 🧩 Component Classes

### Card
```html
<div class="lcc-card">
    Card content
</div>
```

### Badge
```html
<span class="lcc-badge lcc-badge--success">ON TARGET</span>
<span class="lcc-badge lcc-badge--warning">MONITOR</span>
<span class="lcc-badge lcc-badge--danger">AT RISK</span>
<span class="lcc-badge lcc-badge--info">INFO</span>
```

### Button
```html
<button class="lcc-btn lcc-btn--primary">Launch</button>
<button class="lcc-btn lcc-btn--outlined">Cancel</button>
```

---

## 🎬 Animations

### Available Keyframes
- `lcc-spin` — 360° rotation (3s loop)
- `lcc-spin-rev` — reverse rotation (2s loop)
- `lcc-shimmer` — horizontal shimmer effect
- `lcc-pulse` — scale + opacity pulse
- `lcc-fade-in` — fade in from bottom
- `lcc-ping` — expanding ring effect
- `lcc-dot-pulse` — subtle opacity pulse

**Example:**
```css
.my-spinner {
    animation: lcc-spin 2s linear infinite;
}
```

---

## 🎯 Best Practices

### ✅ DO
- Use CSS variables for colors: `color: var(--color-accent-cyan);`
- Use spacing scale: `padding: var(--space-md);`
- Use semantic mappings: `var(--color-primary)`, `var(--color-success)`
- Apply transitions: `transition: all var(--transition-base);`
- Use `color-mix()` for transparency: `color-mix(in srgb, var(--color-accent-cyan) 15%, transparent)`

### ❌ DON'T
- Hard-code hex values: ~~`color: #00b4d8;`~~
- Hard-code pixel spacing: ~~`padding: 16px;`~~
- Use inline styles for repeating patterns
- Mix old color scheme with new variables

---

## 🔄 Migration Guide

### Before (Old Inline Styles)
```html
<div style="background:#0d1a2e; border:1px solid #0d2137; border-radius:16px; padding:24px;">
```

### After (Design System)
```html
<div class="bg-card border-default rounded-lg" style="padding:var(--space-lg);">
```

Or create a reusable class:
```css
.my-component {
    background: var(--color-bg-card);
    border: 1px solid var(--color-border);
    border-radius: var(--radius-lg);
    padding: var(--space-lg);
}
```

---

## 📦 File Structure

```
LineControlCenter.UI/
├── wwwroot/
│   └── css/
│       └── design-system.css    ← Global variables & utilities
└── Components/
    └── Pages/
        ├── Home.razor
        └── Home.razor.css        ← Component-scoped styles
```

---

## 🚀 Quick Start

1. **Include design system** in `App.razor`:
   ```html
   <link href="css/design-system.css" rel="stylesheet" />
   ```

2. **Use variables** in component styles:
   ```css
   .my-card {
       background: var(--color-bg-card);
       color: var(--color-text-primary);
       border-radius: var(--radius-lg);
   }
   ```

3. **Apply utility classes**:
   ```html
   <div class="lcc-card shadow-md">Content</div>
   ```

---

## 📞 Support

For questions about the design system, reference this documentation or check existing component implementations in `Home.razor.css` and `design-system.css`.

**Version:** 1.0  
**Last Updated:** 2025-01-20
