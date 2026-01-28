import { createTheme } from "@mui/material/styles";

/* =====================================================
   🎨 DESIGN TOKENS
   ===================================================== */
const tokens = {
  bg: "#0b0f1a",
  bgSoft: "#11162a",
  card: "#1a2040",

  primary: "#5ea3ff",
  primaryHover: "#3b82f6",

  accent: "#22d3ee",
  success: "#22c55e",

  text: "#e6e8f0",
  muted: "#9aa3c7",
};

/* =====================================================
   🌙 DARK THEME
   ===================================================== */
export const darkTheme = createTheme({
  palette: {
    mode: "dark",

    background: {
      default: tokens.bg,
      paper: tokens.card,
    },

    primary: {
      main: tokens.primary,
    },

    text: {
      primary: tokens.text,
      secondary: tokens.muted,
    },
  },

  /* =====================================================
     ✍️ TYPOGRAPHY
     ===================================================== */
  typography: {
    fontFamily: "'Inter', system-ui, sans-serif",

    h4: {
      fontWeight: 700,
      letterSpacing: "0.06em",
    },

    body2: {
      fontSize: "0.95rem",
    },
  },

  /* =====================================================
     🔲 SHAPE
     ===================================================== */
  shape: {
    borderRadius: 18,
  },

  /* =====================================================
     🧩 COMPONENT OVERRIDES
     ===================================================== */
  components: {
    /* ===== INPUTS ===== */
    MuiTextField: {
      defaultProps: {
        variant: "outlined",
      },
    },

    MuiOutlinedInput: {
      styleOverrides: {
        root: {
          backgroundColor: tokens.bgSoft,
          borderRadius: 12,
          transition: "0.25s",

          "& fieldset": {
            borderColor: "#2a3568",
          },

          "&:hover fieldset": {
            borderColor: tokens.primary,
          },

          "&.Mui-focused fieldset": {
            borderColor: tokens.accent,
            boxShadow: `0 0 0 2px rgba(34, 211, 238, 0.25)`,
          },
        },
      },
    },

    /* ===== BUTTONS ===== */
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 14,
          padding: "12px",
          fontWeight: 600,
          textTransform: "none",
          boxShadow: "0 8px 20px rgba(94, 163, 255, 0.25)",
        },

        contained: {
          background: `linear-gradient(135deg, ${tokens.primary}, ${tokens.accent})`,
          color: "#020617",

          "&:hover": {
            background: `linear-gradient(135deg, ${tokens.primaryHover}, ${tokens.accent})`,
          },
        },

        outlined: {
          borderColor: tokens.primary,
          color: tokens.primary,

          "&:hover": {
            backgroundColor: "rgba(94, 163, 255, 0.08)",
          },
        },
      },
    },

    /* ===== CARD / PAPER ===== */
    MuiPaper: {
      styleOverrides: {
        root: {
          background: `linear-gradient(180deg, ${tokens.card}, ${tokens.bgSoft})`,
          boxShadow: "0 20px 60px rgba(0, 0, 0, 0.6)",
        },
      },
    },
  },
});
