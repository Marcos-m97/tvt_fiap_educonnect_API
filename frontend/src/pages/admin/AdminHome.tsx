import { Typography, Box, Button } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";

export default function AdminHome() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <Box p={4}>
      <Typography variant="h4" gutterBottom>
        Bem-vindo, {user?.nome}
      </Typography>

      <Typography variant="body1" gutterBottom>
        Perfil: Administrador
      </Typography>

      <Button variant="contained" onClick={handleLogout}>
        Sair
      </Button>
    </Box>
  );
}
