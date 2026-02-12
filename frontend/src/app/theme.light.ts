import { createTheme } from "@mui/material/styles";

const tokens = {
  bg: "hsl(130, 60%, 84%)",        // verde água bem claro
  bgSoft: "#f3f3f3",
  card: "#ffffff",
  primary: "#3b82f6",
  primaryHover: "#2563eb",
  accent: "#2dd4bf",    // menta
  text: "#0f172a",
  muted: "#475569",
};

export const lightTheme = createTheme({
  palette: {
    mode: "light",
    background: { default: tokens.bg, paper: tokens.card },
    primary: { main: tokens.primary },
    text: { primary: tokens.text, secondary: tokens.muted },
  },
  typography: {
    fontFamily: "'Inter', system-ui, sans-serif",
    h4: { fontWeight: 700, letterSpacing: "0.06em" },
  },
  shape: { borderRadius: 18 },
  components: {
    MuiOutlinedInput: {
      styleOverrides: {
        root: {
          backgroundColor: tokens.bgSoft,
          borderRadius: 12,
        },
      },
    },
    MuiButton: {
      styleOverrides: {
        contained: {
          background: `linear-gradient(135deg, ${tokens.primary}, ${tokens.accent})`,
        },
      },
    },
  },
});