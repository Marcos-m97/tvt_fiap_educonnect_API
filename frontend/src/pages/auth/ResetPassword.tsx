import { Box, Paper, TextField, Button, Typography, Stack } from "@mui/material";
import { useState, useEffect } from "react";
import { api } from "../../services/api";
import AuthHeader from "../../components/layout/AuthHeader";
import { useNavigate, useSearchParams } from "react-router-dom";

export default function ResetPassword() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const [email, setEmail] = useState("");
  const [codigo, setCodigo] = useState("");
  const [novaSenha, setNovaSenha] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  // 🔥 PREENCHE AUTOMATICAMENTE VINDO DO E-MAIL
  useEffect(() => {
    const emailParam = searchParams.get("email");
    const codigoParam = searchParams.get("codigo");

    if (emailParam) setEmail(emailParam);
    if (codigoParam) setCodigo(codigoParam);
  }, [searchParams]);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      await api.post("/usuario/reset-password", {
        email,
        codigo,
        novaSenha,
      });

      navigate("/login");
    } catch {
      setError("Código inválido ou expirado.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box minHeight="100vh" display="flex" alignItems="center" justifyContent="center">
      <Paper sx={{ width: 420, p: 4 }}>
        <AuthHeader subtitle="Criar nova senha" />

        <form onSubmit={handleSubmit}>
          <Stack spacing={2}>
            <TextField
              label="Email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              disabled={!!searchParams.get("email")} // opcional
            />

            <TextField
              label="Código de verificação"
              value={codigo}
              onChange={(e) => setCodigo(e.target.value)}
              required
              disabled={!!searchParams.get("codigo")} // opcional
            />

            <TextField
              label="Nova senha"
              type="password"
              value={novaSenha}
              onChange={(e) => setNovaSenha(e.target.value)}
              required
            />

            {error && <Typography color="error">{error}</Typography>}

            <Button type="submit" variant="contained" disabled={loading}>
              {loading ? "Salvando..." : "Redefinir senha"}
            </Button>
          </Stack>
        </form>
      </Paper>
    </Box>
  );
}
