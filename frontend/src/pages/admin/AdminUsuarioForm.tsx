import {
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  MenuItem,
  CircularProgress
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SaveIcon from "@mui/icons-material/Save";
import AppLayout from "../../components/layout/AppLayout";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../../services/api";

export default function AdminUsuarioForm() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(false);

  // ===== Usuario base =====
  const [usuario, setUsuario] = useState({
    nome: "",
    email: "",
    senha: "",
    tipo: 1
  });

  // ===== Admin =====
  const [admin, setAdmin] = useState({
    departamento: "",
    cargo: ""
  });

  // ===== Professor =====
  const [professor, setProfessor] = useState({
    especialidade: "",
    formacao: "",
    curriculoLattes: ""
  });

  async function handleSubmit() {
    try {
      setLoading(true);

      // 1️⃣ cria usuario
      const response = await api.post("/usuario", usuario);
      const usuarioId = response.data.usuario.id;

      // 2️⃣ cria perfil dependendo do tipo
      if (usuario.tipo === 1) {
        await api.post("/admin", {
          usuarioId,
          ...admin
        });
      }

      if (usuario.tipo === 2) {
        await api.post("/professor", {
          usuarioId,
          ...professor
        });
      }

      navigate("/admin/usuarios");
    } catch (error) {
      console.error("Erro ao criar usuário:", error);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AppLayout>
      {/* HEADER CENTRALIZADO */}
      <Box textAlign="center" mb={4}>
        <Typography variant="h4" gutterBottom>
          Criar Usuário
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Crie administradores ou professores.
        </Typography>
      </Box>

      {/* BOTÃO VOLTAR PADRÃO */}
      <Box mb={3}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate("/admin/usuarios")}
          variant="outlined"
        >
          Voltar
        </Button>
      </Box>

      <Card>
        <CardContent>
          {loading && (
            <Box display="flex" justifyContent="center" py={3}>
              <CircularProgress />
            </Box>
          )}

          {!loading && (
            <Box display="flex" flexDirection="column" gap={3}>
              {/* ===== DADOS USUARIO ===== */}
              <Typography fontWeight={600}>
                Dados do Usuário
              </Typography>

              <TextField
                label="Nome"
                value={usuario.nome}
                onChange={(e) =>
                  setUsuario({ ...usuario, nome: e.target.value })
                }
                fullWidth
              />

              <TextField
                label="Email"
                value={usuario.email}
                onChange={(e) =>
                  setUsuario({ ...usuario, email: e.target.value })
                }
                fullWidth
              />

              <TextField
                label="Senha"
                type="password"
                value={usuario.senha}
                onChange={(e) =>
                  setUsuario({ ...usuario, senha: e.target.value })
                }
                fullWidth
              />

              <TextField
                select
                label="Tipo"
                value={usuario.tipo}
                onChange={(e) =>
                  setUsuario({
                    ...usuario,
                    tipo: Number(e.target.value)
                  })
                }
                fullWidth
              >
                <MenuItem value={1}>Admin</MenuItem>
                <MenuItem value={2}>Professor</MenuItem>
              </TextField>

              {/* ===== FORM ADMIN ===== */}
              {usuario.tipo === 1 && (
                <>
                  <Typography fontWeight={600} mt={2}>
                    Dados do Administrador
                  </Typography>

                  <TextField
                    label="Departamento"
                    value={admin.departamento}
                    onChange={(e) =>
                      setAdmin({
                        ...admin,
                        departamento: e.target.value
                      })
                    }
                    fullWidth
                  />

                  <TextField
                    label="Cargo"
                    value={admin.cargo}
                    onChange={(e) =>
                      setAdmin({
                        ...admin,
                        cargo: e.target.value
                      })
                    }
                    fullWidth
                  />
                </>
              )}

              {/* ===== FORM PROFESSOR ===== */}
              {usuario.tipo === 2 && (
                <>
                  <Typography fontWeight={600} mt={2}>
                    Dados do Professor
                  </Typography>

                  <TextField
                    label="Especialidade"
                    value={professor.especialidade}
                    onChange={(e) =>
                      setProfessor({
                        ...professor,
                        especialidade: e.target.value
                      })
                    }
                    fullWidth
                  />

                  <TextField
                    label="Formação"
                    value={professor.formacao}
                    onChange={(e) =>
                      setProfessor({
                        ...professor,
                        formacao: e.target.value
                      })
                    }
                    fullWidth
                  />

                  <TextField
                    label="Currículo Lattes"
                    multiline
                    rows={3}
                    value={professor.curriculoLattes}
                    onChange={(e) =>
                      setProfessor({
                        ...professor,
                        curriculoLattes: e.target.value
                      })
                    }
                    fullWidth
                  />
                </>
              )}

              <Box display="flex" justifyContent="flex-end">
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={handleSubmit}
                >
                  Criar
                </Button>
              </Box>
            </Box>
          )}
        </CardContent>
      </Card>
    </AppLayout>
  );
}
