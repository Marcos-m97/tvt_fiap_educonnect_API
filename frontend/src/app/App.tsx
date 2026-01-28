import { CssBaseline } from "@mui/material";
import { RouterProvider } from "react-router-dom";
import { router } from "./router";
import { ThemeProviderApp } from "../contexts/ThemeContext";

export default function App() {
  return (
    <ThemeProviderApp>
      <CssBaseline />
      <RouterProvider router={router} />
    </ThemeProviderApp>
  );
}
