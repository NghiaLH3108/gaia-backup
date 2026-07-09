---
name: EcoTrace Synthesis
colors:
  surface: '#fff8f6'
  surface-dim: '#f8d2c4'
  surface-bright: '#fff8f6'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#fff1ec'
  surface-container: '#ffe9e2'
  surface-container-high: '#ffe2d9'
  surface-container-highest: '#ffdbce'
  on-surface: '#2b160f'
  on-surface-variant: '#40493d'
  inverse-surface: '#422b22'
  inverse-on-surface: '#ffede7'
  outline: '#707a6c'
  outline-variant: '#bfcaba'
  surface-tint: '#1b6d24'
  primary: '#0d631b'
  on-primary: '#ffffff'
  primary-container: '#2e7d32'
  on-primary-container: '#cbffc2'
  inverse-primary: '#88d982'
  secondary: '#006e1c'
  on-secondary: '#ffffff'
  secondary-container: '#91f78e'
  on-secondary-container: '#00731e'
  tertiary: '#00598f'
  on-tertiary: '#ffffff'
  tertiary-container: '#0072b6'
  on-tertiary-container: '#e9f2ff'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#a3f69c'
  primary-fixed-dim: '#88d982'
  on-primary-fixed: '#002204'
  on-primary-fixed-variant: '#005312'
  secondary-fixed: '#94f990'
  secondary-fixed-dim: '#78dc77'
  on-secondary-fixed: '#002204'
  on-secondary-fixed-variant: '#005313'
  tertiary-fixed: '#cfe5ff'
  tertiary-fixed-dim: '#99cbff'
  on-tertiary-fixed: '#001d34'
  on-tertiary-fixed-variant: '#004a78'
  background: '#fff8f6'
  on-background: '#2b160f'
  surface-variant: '#ffdbce'
  surface-light: '#FFFFFF'
  surface-accent: '#C8E6C9'
  warning-orange: '#FF9800'
  glass-stroke: rgba(255, 255, 255, 0.3)
  glass-fill: rgba(255, 255, 255, 0.7)
typography:
  display-hero:
    fontFamily: Inter
    fontSize: 48px
    fontWeight: '800'
    lineHeight: 56px
    letterSpacing: -0.02em
  display-hero-mobile:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '800'
    lineHeight: 40px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
  headline-lg-mobile:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '700'
    lineHeight: 32px
  headline-md:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  label-sm:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '600'
    lineHeight: 20px
    letterSpacing: 0.05em
  mono-data:
    fontFamily: Inter
    fontSize: 13px
    fontWeight: '500'
    lineHeight: 18px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 8px
  container-padding-mobile: 20px
  container-padding-desktop: 64px
  gutter: 24px
  card-gap: 16px
  section-margin: 80px
---

## Brand & Style
The design system embodies **Modern Sustainability + Technology + Transparency**. It bridges the gap between raw agricultural origins and high-tech traceability. The visual direction is "Tech-Organic," blending the precision of a data-driven SaaS platform with the warmth of environmental stewardship.

The chosen style is **Modern Corporate with Glassmorphism**. This approach uses high-quality white space and structured grids to establish trust, while translucent "glass" overlays and vibrant blurs represent the "Transparency" pillar of the brand. The UI should feel premium, cinematic, and storytelling-driven to elevate a recycled product into a luxury sustainable artifact.

**Emotional Response:**
- **Trust:** Through clean, systematic data presentation.
- **Connection:** Through immersive, parallax-driven storytelling.
- **Innovation:** Through smooth animations and modern Flutter-optimized components.

## Colors
The palette is rooted in the "Forest to Factory" concept. **Forest Green** serves as the primary anchor for institutional trust and deep nature. **Eco Green** provides a vibrant, active energy for primary actions and success states. **Sky Blue** is used sparingly for technology-focused elements like GPS, metadata, and digital tracking.

**Earth Brown** acts as a sophisticated neutral, replacing harsh grays to maintain an organic feel in text and borders. **Light Green** is utilized for large surface areas and background tints to reduce visual fatigue. Use the **Glass-fill** and **Glass-stroke** tokens for card overlays to create the required depth without losing the airy, minimal aesthetic.

## Typography
**Inter** is the sole typeface, chosen for its exceptional legibility in data-heavy dashboards and its clean, modern profile in storytelling. 

- **Display Hero:** Used for impactful narrative statements (e.g., "I Used To Be A Durian Shell").
- **Headline Levels:** Use for card titles and section headers. 
- **Label-sm:** Styled in All-Caps for metadata labels (GPS, ID, Timestamps) to provide a technical, "scanned" aesthetic.
- **Body-lg:** Optimized for the narrative "Story" sections to ensure a comfortable reading experience on mobile.

## Layout & Spacing
The design system utilizes a **Fluid Grid** for the Supplier Portal and a **Contextual/Centric** layout for the Product Storytelling Experience.

- **Supplier Portal:** 12-column grid on desktop, 4-column on mobile. Cards should span full width on mobile and tile dynamically on desktop.
- **Storytelling Experience:** Single-column centered content (max-width 800px) for readability, with full-bleed hero and video sections.
- **Rhythm:** Use an 8px base unit. All internal card padding should be 24px (3 units) to feel spacious and premium.

## Elevation & Depth
Depth is created through a mix of **Tonal Layers** and **Glassmorphism**, avoiding heavy black shadows.

- **Primary Surfaces:** Use a very soft, tinted ambient shadow (Hex: #2E7D32 at 8% opacity, 20px blur) to make cards feel like they are floating on a natural surface.
- **Glass Effects:** For the Product Storytelling site, use `BackdropFilter` with a blur of 15px and a semi-transparent white fill (70% opacity). Add a thin 1px border (`glass-stroke`) to define the edges.
- **Active State:** When a card is hovered or active, increase the elevation and add a subtle Eco Green glow rather than a dark shadow.

## Shapes
Following the UI principles, the design uses **Rounded** corners (0.5rem / 8px base) scaling up to **16px (rounded-lg)** for containers and cards. 

- **Standard Buttons/Inputs:** 8px roundedness.
- **Feature Cards:** 16px roundedness to evoke a "softer," more approachable eco-friendly feel.
- **Image Containers:** Always 16px rounded to match the card containers.
- **Pill Tags:** Use "Pill-shaped" (rounded-full) for status indicators like "Approved" or "In Transit."

## Components
- **Buttons:** 
  - *Primary:* Eco Green background, white text, 16px vertical padding. 
  - *Secondary:* Transparent with a Forest Green 2px border.
- **Cards:** White background with 16px radius. Dashboard cards feature a top-accent bar (4px) in the color corresponding to the KPI (e.g., Blue for logistics, Green for approvals).
- **Timeline:** A vertical line in the Portal; a horizontal, scroll-triggered line in the Storytelling app. Milestones are circles with icons (Material Symbols).
- **Input Fields:** Filled style using `surface-accent` at 20% opacity. Metadata fields (read-only) use a light gray-brown background with a "lock" icon.
- **Media Upload:** A dashed-border container using Forest Green. Include a "Leaf" loading animation during photo processing.
- **KPI Counters:** Large font weight (700) with a subtle count-up animation upon the section entering the viewport.
- **Navigation:** Top-bar for the portal with a glassmorphic background that sticks to the top during scroll.