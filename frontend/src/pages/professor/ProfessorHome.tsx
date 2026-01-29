import { Typography, Box } from "@mui/material";
import AppLayout from "../../components/layout/AppLayout";

export default function ProfessorHome() {
  return (
    <AppLayout>
      <Typography variant="h4" gutterBottom>
        Painel do Professor
      </Typography>

      <Box>
        {/* Aqui depois entram os botões do professor */}
      </Box>
    </AppLayout>
  );
}
