import {
  AppBar,
  Toolbar,
  Typography,
  IconButton,
  Box,
  Tooltip,
  Avatar
} from "@mui/material";
import SchoolIcon from "@mui/icons-material/School";
import LogoutIcon from "@mui/icons-material/Logout";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import LightModeIcon from "@mui/icons-material/LightMode";
import HomeIcon from "@mui/icons-material/Home";
import PersonIcon from "@mui/icons-material/Person";

import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import { useThemeApp } from "../../contexts/ThemeContext";
import { api } from "../../services/api";

export default function TopBar() {
  const { user, logout } = useAuth();
  const { mode, toggle } = useThemeApp();
  const navigate = useNavigate();

  const baseUrl = api.defaults.baseURL?.replace("/api", "");

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
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 1,
            cursor: "pointer",
            fontWeight: 600
          }}
          onClick={handleHome}
        >
          <SchoolIcon sx={{ fontSize: 22 }} />
          EduConnect
        </Typography>

        <Box display="flex" alignItems="center" gap={1}>

          {/* HOME */}
          <Tooltip title="Home">
            <IconButton color="inherit" onClick={handleHome}>
              <HomeIcon />
            </IconButton>
          </Tooltip>

          {/* PERFIL + EMAIL */}
          <Tooltip title="Meu Perfil">
            <Box
              display="flex"
              alignItems="center"
              gap={1}
              sx={{ cursor: "pointer" }}
              onClick={handlePerfil}
            >
              <Avatar
                src={
                  user?.fotoPerfilUrl
                    ? `${baseUrl}${user.fotoPerfilUrl}`
                    : undefined
                }
                sx={{
                  width: 32,
                  height: 32,
                  bgcolor: "grey.300"
                }}
              >
                {!user?.fotoPerfilUrl && <PersonIcon />}
              </Avatar>

              <Typography variant="body2">
                {user?.email}
              </Typography>
            </Box>
          </Tooltip>

          {/* TEMA */}
          <Tooltip title="Alternar tema">
            <IconButton color="inherit" onClick={toggle}>
              {mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
            </IconButton>
          </Tooltip>

          {/* LOGOUT */}
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