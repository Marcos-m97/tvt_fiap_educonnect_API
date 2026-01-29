import { Button, TextField, Stack, Typography, Link } from "@mui/material";
import { useState } from "react";
import { useNavigate, Link as RouterLink } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";

export default function LoginForm() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      const usuario = await login(email, senha);

      switch (usuario.tipo) {
        case 0:
          navigate("/admin");
          break;
        case 1:
          navigate("/admin");
          break;
        case 2:
          navigate("/professor");
          break;
        case 3:
          navigate("/aluno");
          break;
      }
    } catch {
      setError("Credenciais inválidas.");
    } finally {
      setLoading(false);
    }
  }

  return (
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

        <TextField
          label="Senha"
          type="password"
          value={senha}
          onChange={(e) => setSenha(e.target.value)}
          required
          fullWidth
        />

        {error && (
          <Typography color="error" fontSize="0.9rem">
            {error}
          </Typography>
        )}

        <Button
          type="submit"
          variant="contained"
          fullWidth
          disabled={loading}
        >
          {loading ? "Entrando..." : "Entrar"}
        </Button>

        <Link
          component={RouterLink}
          to="/forgot-password"
          underline="hover"
          textAlign="center"
          fontSize="0.9rem"
        >
          Esqueci minha senha
        </Link>
      </Stack>
    </form>
  );
}
