import React from "react";
import ReactDOM from "react-dom/client";
import App from "./app/App";
import { ThemeProviderApp } from "./contexts/ThemeContext";
import { AuthProvider } from "./contexts/AuthContext";
import { CssBaseline } from "@mui/material";
import "./index.css";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <ThemeProviderApp>
      <CssBaseline />
      <AuthProvider>
        <App />
      </AuthProvider>
    </ThemeProviderApp>
  </React.StrictMode>
);