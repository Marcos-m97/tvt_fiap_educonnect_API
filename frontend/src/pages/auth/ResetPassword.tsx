import {
  Box,
  Paper,
  TextField,
  Button,
  Typography,
  Stack,
  InputAdornment,
  IconButton,
  Alert,
} from "@mui/material";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import RadioButtonUncheckedIcon from "@mui/icons-material/RadioButtonUnchecked";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
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
  const [confirmarSenha, setConfirmarSenha] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  // 🔥 Preencher automaticamente via link do e-mail
  useEffect(() => {
    const emailParam = searchParams.get("email");
    const codigoParam = searchParams.get("codigo");

    if (emailParam) setEmail(emailParam);
    if (codigoParam) setCodigo(codigoParam);
  }, [searchParams]);

  // 🔐 Regras de validação
  const validations = {
    length: novaSenha.length >= 8,
    uppercase: /[A-Z]/.test(novaSenha),
    lowercase: /[a-z]/.test(novaSenha),
    number: /[0-9]/.test(novaSenha),
    special: /[^A-Za-z0-9]/.test(novaSenha),
  };

  const senhaValida = Object.values(validations).every(Boolean);

  const senhasIguais =
    novaSenha.length > 0 && novaSenha === confirmarSenha;

  const podeEnviar =
    senhaValida &&
    senhasIguais &&
    codigo.length > 0 &&
    !success;

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    if (!podeEnviar) return;

    setLoading(true);

    try {
      await api.post("/usuario/reset-password", {
        email,
        codigo,
        novaSenha,
      });

      setSuccess(true);

      // 🔄 Redireciona após 3 segundos
      setTimeout(() => {
        navigate("/login");
      }, 5000);

    } catch {
      setError("Código inválido ou expirado.");
    } finally {
      setLoading(false);
    }
  }

  function renderItem(valid: boolean, text: string) {
    return (
      <Box display="flex" alignItems="center" gap={1}>
        {valid ? (
          <CheckCircleIcon color="success" fontSize="small" />
        ) : (
          <RadioButtonUncheckedIcon color="disabled" fontSize="small" />
        )}
        <Typography
          variant="body2"
          color={valid ? "success.main" : "text.secondary"}
        >
          {text}
        </Typography>
      </Box>
    );
  }

  return (
    <Box
      minHeight="100vh"
      display="flex"
      alignItems="center"
      justifyContent="center"
    >
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
              disabled={!!searchParams.get("email") || success}
            />

            <TextField
              label="Código de verificação"
              value={codigo}
              onChange={(e) => setCodigo(e.target.value)}
              required
              disabled={!!searchParams.get("codigo") || success}
            />

            {/* 🔐 Nova senha */}
            <TextField
              label="Nova senha"
              type={showPassword ? "text" : "password"}
              value={novaSenha}
              onChange={(e) => setNovaSenha(e.target.value)}
              required
              disabled={success}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowPassword(!showPassword)}
                      edge="end"
                    >
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />

            {/* 🔎 Checklist */}
            <Box>
              {renderItem(validations.length, "Mínimo 8 caracteres")}
              {renderItem(validations.uppercase, "1 letra maiúscula")}
              {renderItem(validations.lowercase, "1 letra minúscula")}
              {renderItem(validations.number, "1 número")}
              {renderItem(validations.special, "1 caractere especial")}
            </Box>

            {/* 🔁 Confirmar senha */}
            <TextField
              label="Confirmar nova senha"
              type={showPassword ? "text" : "password"}
              value={confirmarSenha}
              onChange={(e) => setConfirmarSenha(e.target.value)}
              required
              disabled={success}
              error={confirmarSenha.length > 0 && !senhasIguais}
              helperText={
                confirmarSenha.length > 0 && !senhasIguais
                  ? "As senhas não coincidem"
                  : ""
              }
            />

            {error && <Typography color="error">{error}</Typography>}

            <Button
              type="submit"
              variant="contained"
              disabled={!podeEnviar || loading}
            >
              {loading ? "Salvando..." : "Redefinir senha"}
            </Button>

            {/* ✅ Mensagem de sucesso */}
            {success && (
              <Alert severity="success" sx={{ mt: 2 }}>
                Senha alterada com sucesso!
                <br />
                Um e-mail de confirmação será enviado.
                Você será redirecionado para o login.
              </Alert>
            )}
          </Stack>
        </form>
      </Paper>
    </Box>
  );
}
