import { CssBaseline } from "@mui/material";
import { RouterProvider } from "react-router-dom";
import { router } from "./router";
import { ThemeProviderApp } from "../contexts/ThemeContext";
import { AuthProvider } from "../contexts/AuthContext";

export default function App() {
  return (
    <ThemeProviderApp>
      <AuthProvider>
        <CssBaseline />
        <RouterProvider router={router} />
      </AuthProvider>
    </ThemeProviderApp>
  );
}
