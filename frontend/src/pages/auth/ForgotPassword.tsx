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
    <Box minHeight="100vh" display="flex" alignItems="center" justifyContent="center">
      <Paper sx={{ width: 420, p: 4 }}>
        <AuthHeader subtitle="Redefinir senha" />

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

            {message && <Typography color="success.main">{message}</Typography>}
            {error && <Typography color="error">{error}</Typography>}

            <Button type="submit" variant="contained" disabled={loading}>
              {loading ? "Enviando..." : "Enviar código"}
            </Button>
          </Stack>
        </form>
      </Paper>
    </Box>
  );
}