import { Typography, Box } from "@mui/material";
import AppLayout from "../../components/layout/AppLayout";

export default function AlunoHome() {
  return (
    <AppLayout>
      <Typography variant="h4" gutterBottom>
        Painel do Aluno
      </Typography>

      <Box>
        {/* Aqui depois entram os botões do aluno */}
      </Box>
    </AppLayout>
  );
}
