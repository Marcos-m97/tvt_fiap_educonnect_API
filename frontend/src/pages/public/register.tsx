import {
  Box,
  Typography,
  TextField,
  Button,
  Stack,
  CircularProgress,
  InputAdornment,
  IconButton,
  Paper,
  Alert,
} from "@mui/material";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import RadioButtonUncheckedIcon from "@mui/icons-material/RadioButtonUnchecked";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../../services/api";

export default function Register() {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    nome: "",
    email: "",
    senha: "",
    confirmarSenha: "",
    cpf: "",
    dataNascimento: "",
    endereco: ""
  });

  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [success, setSuccess] = useState(false);

  function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  // 🔐 Regras de senha
  const validations = {
    length: form.senha.length >= 8,
    uppercase: /[A-Z]/.test(form.senha),
    lowercase: /[a-z]/.test(form.senha),
    number: /[0-9]/.test(form.senha),
    special: /[^A-Za-z0-9]/.test(form.senha),
  };

  const senhaValida = Object.values(validations).every(Boolean);
  const senhasIguais =
    form.senha.length > 0 && form.senha === form.confirmarSenha;

  const podeEnviar =
    senhaValida &&
    senhasIguais &&
    form.nome &&
    form.email &&
    form.cpf &&
    form.dataNascimento &&
    form.endereco &&
    !success;

  async function handleSubmit() {
    if (!podeEnviar) return;

    try {
      setLoading(true);

      // 1️⃣ Criar usuário
      const registerResponse = await api.post("/usuario/register", {
        nome: form.nome,
        email: form.email,
        senha: form.senha
      });

      const usuarioId = registerResponse.data.usuario.id;

      // 2️⃣ Criar perfil aluno
      await api.post("/aluno", {
        usuarioId,
        cpf: form.cpf,
        dataNascimento: form.dataNascimento,
        endereco: form.endereco
      });

      setSuccess(true);

      // 🔄 Redireciona após 4 segundos
      setTimeout(() => {
        navigate("/login");
      }, 4000);

    } catch (error) {
      alert("Erro ao criar conta.");
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
    <Box display="flex" minHeight="100vh">
      {/* LADO ESQUERDO */}
      <Box
        sx={{
          flex: 1,
          display: { xs: "none", md: "flex" },
          flexDirection: "column",
          justifyContent: "center",
          alignItems: "center",
          background: "linear-gradient(135deg, #1e3c72 0%, #2a5298 100%)",
          color: "white",
          p: 6,
        }}
      >
        <Typography variant="h3" fontWeight={800} mb={2}>
        🎓 EduConnect
        </Typography>
        <Typography variant="h6" sx={{ opacity: 0.9, maxWidth: 400, textAlign: "center" }}>
          Comece sua jornada acadêmica conosco.
          Crie sua conta e inicie sua matrícula agora mesmo.
        </Typography>
      </Box>

      {/* LADO DIREITO */}
      <Box
        sx={{
          flex: 1,
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          p: 4,
        }}
      >
        <Paper sx={{ width: "100%", maxWidth: 520, p: 5, borderRadius: 3 }}>
          <Typography variant="h4" fontWeight={700} mb={3} textAlign="center">
            Criar Conta
          </Typography>

          <Stack spacing={3}>
            <TextField
              label="Nome completo"
              name="nome"
              fullWidth
              value={form.nome}
              onChange={handleChange}
              disabled={success}
            />

            <TextField
              label="Email"
              name="email"
              type="email"
              fullWidth
              value={form.email}
              onChange={handleChange}
              disabled={success}
            />

            {/* 🔐 Senha */}
            <TextField
              label="Senha"
              name="senha"
              type={showPassword ? "text" : "password"}
              fullWidth
              value={form.senha}
              onChange={handleChange}
              disabled={success}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={() => setShowPassword(!showPassword)}>
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />

            {/* Checklist */}
            <Box>
              {renderItem(validations.length, "Mínimo 8 caracteres")}
              {renderItem(validations.uppercase, "1 letra maiúscula")}
              {renderItem(validations.lowercase, "1 letra minúscula")}
              {renderItem(validations.number, "1 número")}
              {renderItem(validations.special, "1 caractere especial")}
            </Box>

            {/* Confirmar senha */}
            <TextField
              label="Confirmar senha"
              name="confirmarSenha"
              type={showPassword ? "text" : "password"}
              fullWidth
              value={form.confirmarSenha}
              onChange={handleChange}
              disabled={success}
              error={form.confirmarSenha.length > 0 && !senhasIguais}
              helperText={
                form.confirmarSenha.length > 0 && !senhasIguais
                  ? "As senhas não coincidem"
                  : ""
              }
            />

            <TextField
              label="CPF"
              name="cpf"
              fullWidth
              value={form.cpf}
              onChange={handleChange}
              disabled={success}
            />

            <TextField
              label="Data de Nascimento"
              name="dataNascimento"
              type="date"
              fullWidth
              InputLabelProps={{ shrink: true }}
              value={form.dataNascimento}
              onChange={handleChange}
              disabled={success}
            />

            <TextField
              label="Endereço"
              name="endereco"
              fullWidth
              value={form.endereco}
              onChange={handleChange}
              disabled={success}
            />

            <Button
              variant="contained"
              size="large"
              disabled={!podeEnviar || loading}
              onClick={handleSubmit}
            >
              {loading ? <CircularProgress size={24} /> : "Criar Conta"}
            </Button>

            {/* ✅ Mensagem de sucesso */}
            {success && (
              <Alert severity="success" sx={{ mt: 2 }}>
                Conta criada com sucesso!
                <br />
                Verifique seu e-mail para confirmar sua conta.
                Você será redirecionado para o login.
              </Alert>
            )}

            <Button
              variant="text"
              onClick={() => navigate("/login")}
              disabled={success}
            >
              Já tenho conta
            </Button>
          </Stack>
        </Paper>
      </Box>
    </Box>
  );
}