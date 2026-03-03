import {
  AppBar,
  Toolbar,
  Typography,
  IconButton,
  Box,
  Tooltip
} from "@mui/material";
import LogoutIcon from "@mui/icons-material/Logout";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import LightModeIcon from "@mui/icons-material/LightMode";
import HomeIcon from "@mui/icons-material/Home";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";

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

  function handleHome() {
    if (!user) return;

    switch (user.tipo) {
      case 0:
      case 1:
        navigate("/admin");
        break;
      case 2:
        navigate("/professor");
        break;
      case 3:
        navigate("/aluno");
        break;
      default:
        navigate("/");
    }
  }

  function handlePerfil() {
    navigate("/perfil");
  }

  return (
    <AppBar position="static">
      <Toolbar sx={{ justifyContent: "space-between" }}>
        <Typography
          variant="h6"
          sx={{ cursor: "pointer" }}
          onClick={handleHome}
        >
          EduConnect 🎓
        </Typography>

        <Box display="flex" alignItems="center" gap={1}>
          {/* 🏠 HOME */}
          <Tooltip title="Home">
            <IconButton color="inherit" onClick={handleHome}>
              <HomeIcon />
            </IconButton>
          </Tooltip>

          {/* 👤 PERFIL */}
          <Tooltip title="Meu Perfil">
            <IconButton color="inherit" onClick={handlePerfil}>
              <AccountCircleIcon />
            </IconButton>
          </Tooltip>

          {/* Email */}
          <Typography variant="body2">
            👩‍💻 {user?.email}
          </Typography>

          {/* Tema */}
          <Tooltip title="Alternar tema">
            <IconButton color="inherit" onClick={toggle}>
              {mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
            </IconButton>
          </Tooltip>

          {/* Logout */}
          <Tooltip title="Sair">
            <IconButton color="inherit" onClick={handleLogout}>
              <LogoutIcon />
            </IconButton>
          </Tooltip>
        </Box>
      </Toolbar>
    </AppBar>
  );
}