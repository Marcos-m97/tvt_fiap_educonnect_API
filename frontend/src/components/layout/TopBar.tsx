import { AppBar, Toolbar, Typography, IconButton, Box } from "@mui/material";
import LogoutIcon from "@mui/icons-material/Logout";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import LightModeIcon from "@mui/icons-material/LightMode";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import { useThemeApp } from "../../contexts/ThemeContext";

export default function TopBar() {
  const { user, logout } = useAuth();
  const { mode, toggle } = useThemeApp();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <AppBar position="static">
      <Toolbar sx={{ justifyContent: "space-between" }}>
        <Typography variant="h6">EduConnect</Typography>

        <Box display="flex" alignItems="center" gap={1}>
          <Typography variant="body2">
            {user?.email}
          </Typography>

          {/* 🌙 / ☀️ Toggle de tema */}
          <IconButton color="inherit" onClick={toggle}>
            {mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
          </IconButton>

          {/* 🚪 Logout */}
          <IconButton color="inherit" onClick={handleLogout}>
            <LogoutIcon />
          </IconButton>
        </Box>
      </Toolbar>
    </AppBar>
  );
}
