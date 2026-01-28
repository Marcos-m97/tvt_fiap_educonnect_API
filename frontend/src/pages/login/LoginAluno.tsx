import { Box, Paper } from "@mui/material";
import LoginForm from "../../components/login/LoginForm";
import AuthHeader from "../../components/layout/AuthHeader";

export default function LoginAluno() {
  return (
    <Box
      minHeight="100vh"
      display="flex"
      alignItems="center"
      justifyContent="center"
    >
      <Paper
        elevation={10}
        sx={{ width: 420, minHeight: 380, p: 4 }}
      >
        <AuthHeader subtitle="Conectando você à educação" />

        <LoginForm role="ALUNO" />
      </Paper>
    </Box>
  );
}
