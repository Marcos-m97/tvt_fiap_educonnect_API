import { useState } from "react";
import {
  Box,
  Paper,
  Button,
  Stack,
} from "@mui/material";
import LoginForm from "../../components/login/LoginForm";
import AuthHeader from "../../components/layout/AuthHeader";

type Role = "ADMIN" | "PROFESSOR";

export default function LoginColaborador() {
  const [role, setRole] = useState<Role>("ADMIN");

  return (
    <Box
      minHeight="100vh"
      display="flex"
      alignItems="center"
      justifyContent="center"
    >
      <Paper
        elevation={10}
        sx={{
          width: 420,
          minHeight: 420,
          p: 4,
          display: "flex",
          flexDirection: "column",
        }}
      >
        <AuthHeader subtitle="Acesso para colaboradores" />

        {/* Escolha de papel */}
        <Stack direction="row" spacing={2} mb={3}>
          <Button
            fullWidth
            variant={role === "ADMIN" ? "contained" : "outlined"}
            onClick={() => setRole("ADMIN")}
          >
            Administrador
          </Button>

          <Button
            fullWidth
            variant={role === "PROFESSOR" ? "contained" : "outlined"}
            onClick={() => setRole("PROFESSOR")}
          >
            Professor
          </Button>
        </Stack>

        {/* Formulário com largura total (corrige o "espremido") */}
        <Box
          flexGrow={1}
          display="flex"
          alignItems="center"
          justifyContent="center"
          width="100%"
        >
          <Box width="100%">
            <LoginForm role={role} />
          </Box>
        </Box>
      </Paper>
    </Box>
  );
}