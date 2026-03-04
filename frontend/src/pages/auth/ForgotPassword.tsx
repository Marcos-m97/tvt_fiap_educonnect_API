import { Box, Paper, TextField, Button, Typography, Stack } from "@mui/material";
import { useState } from "react";
import { api } from "../../services/api";
import AuthHeader from "../../components/layout/AuthHeader";

export default function ForgotPassword() {
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError("");
    setMessage("");

    try {
      await api.post("/usuario/forgot-password", { email });
      setMessage("Enviamos um código de verificação para seu e-mail.");
    } catch {
      setError("Erro ao enviar e-mail. Verifique o endereço informado.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box
      minHeight="100vh"
      display="flex"
      alignItems="center"
      justifyContent="center"
      sx={{
        backgroundColor: "primary.main",
        px: 2,
        py: 6
      }}
    >
      <Paper
        elevation={12}
        sx={{
          width: 420,
          minHeight: 320,
          p: 4,
          borderRadius: 3
        }}
      >
        <AuthHeader
          subtitle="Redefinir senha"
          showThemeToggle={false}
        />

        <form onSubmit={handleSubmit}>
          <Stack spacing={2}>
            <TextField
              label="Email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              fullWidth
            />

            {message && (
              <Typography color="success.main">
                {message}
              </Typography>
            )}

            {error && (
              <Typography color="error">
                {error}
              </Typography>
            )}

            <Button
              type="submit"
              variant="contained"
              fullWidth
              disabled={loading}
            >
              {loading ? "Enviando..." : "Enviar código"}
            </Button>
          </Stack>
        </form>
      </Paper>
    </Box>
  );
}