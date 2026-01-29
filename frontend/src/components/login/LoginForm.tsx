import { Button, TextField, Stack } from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";

export default function LoginForm() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);

    try {
      const usuario = await login(email, senha);

      // 🔀 Redirecionamento automático por perfil
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
        default:
          navigate("/login");
      }
    } catch {
      alert("Usuário ou senha inválidos.");
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

        <Button
          type="submit"
          variant="contained"
          fullWidth
          disabled={loading}
        >
          {loading ? "Entrando..." : "Entrar"}
        </Button>
      </Stack>
    </form>
  );
}
