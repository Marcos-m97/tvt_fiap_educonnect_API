import { createContext, useContext, useMemo, useState } from "react";
import { ThemeProvider } from "@mui/material";
import { darkTheme } from "../app/theme";
import { lightTheme } from "../app/theme.light"; // criaremos já

type ThemeMode = "light" | "dark";

type ThemeCtx = {
  mode: ThemeMode;
  toggle: () => void;
};

const ThemeContext = createContext<ThemeCtx | null>(null);

export function ThemeProviderApp({ children }: { children: React.ReactNode }) {
  const [mode, setMode] = useState<ThemeMode>(
    (localStorage.getItem("theme") as ThemeMode) || "dark"
  );

  const toggle = () => {
    setMode((prev) => {
      const next = prev === "dark" ? "light" : "dark";
      localStorage.setItem("theme", next);
      return next;
    });
  };

  const theme = useMemo(
    () => (mode === "dark" ? darkTheme : lightTheme),
    [mode]
  );

  return (
    <ThemeContext.Provider value={{ mode, toggle }}>
      <ThemeProvider theme={theme}>{children}</ThemeProvider>
    </ThemeContext.Provider>
  );
}

export const useThemeApp = () => {
  const ctx = useContext(ThemeContext);
  if (!ctx) throw new Error("useThemeApp must be used within ThemeProviderApp");
  return ctx;
};
