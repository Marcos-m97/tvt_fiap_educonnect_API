import { Typography, Box } from "@mui/material";
import AppLayout from "../../components/layout/AppLayout";

export default function AdminHome() {
  return (
    <AppLayout>
      <Typography variant="h4" gutterBottom>
        Painel do Administrador
      </Typography>

      <Box>
        {/* Aqui depois entram os botões do admin */}
      </Box>
    </AppLayout>
  );
}
