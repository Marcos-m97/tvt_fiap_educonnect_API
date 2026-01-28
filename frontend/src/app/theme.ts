import { createTheme } from "@mui/material/styles";

/* =====================================================
   🎨 DESIGN TOKENS (DARK)
   ===================================================== */
const tokens = {
  bg: "#0b0f1a",
  bgSoft: "#11162a",
  card: "#1a2040",

  primary: "#5ea3ff",
  primaryHover: "#3b82f6",

  accent: "#22d3ee",

  text: "#e6e8f0",
  muted: "#9aa3c7",
};

/* =====================================================
   🌙 DARK THEME (ALINHADO AO LIGHT)
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
     ✍️ TYPOGRAPHY (IGUAL AO LIGHT)
     ===================================================== */
  typography: {
    fontFamily: "'Inter', system-ui, sans-serif",

    h4: {
      fontWeight: 700,
      letterSpacing: "0.06em",
    },
  },

  /* =====================================================
     🔲 SHAPE
     ===================================================== */
  shape: {
    borderRadius: 18,
  },

  /* =====================================================
     🧩 COMPONENT OVERRIDES (MÍNIMOS, COMO NO LIGHT)
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

          "& fieldset": {
            borderColor: "#2a3568",
          },

          "&:hover fieldset": {
            borderColor: tokens.primary,
          },

          "&.Mui-focused fieldset": {
            borderColor: tokens.accent,
          },
        },
      },
    },

    /* ===== BUTTONS (DEIXA DEFAULT DO MUI) ===== */
    MuiButton: {
      styleOverrides: {
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
          background: tokens.card,
        },
      },
    },
  },
});
