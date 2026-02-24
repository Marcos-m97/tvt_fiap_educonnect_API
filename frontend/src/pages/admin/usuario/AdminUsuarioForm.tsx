import {
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  MenuItem,
  CircularProgress,
  Divider
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SaveIcon from "@mui/icons-material/Save";
import AppLayout from "../../../components/layout/AppLayout";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../../../services/api";

export default function AdminUsuarioForm() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const [usuario, setUsuario] = useState({
    nome: "",
    email: "",
    tipo: 1
  });

  const [admin, setAdmin] = useState({
    departamento: "",
    cargo: ""
  });

  const [professor, setProfessor] = useState({
    especialidade: "",
    formacao: "",
    curriculoLattes: ""
  });

  async function handleSubmit() {
    try {
      setLoading(true);

      const response = await api.post("/usuario", {
        ...usuario,
        senha: "Ab123456!"
      });

      const usuarioId = response.data.usuario.id;

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

      {/* HEADER */}
      <Box mb={6} textAlign="center">
        <Typography variant="h4" fontWeight={600} gutterBottom>
          Criar Usuário
        </Typography>

        <Typography variant="body1" color="text.secondary">
          O usuário receberá um e-mail para redefinir a senha.
        </Typography>
      </Box>

      <Box maxWidth="1000px" mx="auto">

        <Card
          sx={{
            borderRadius: 4,
            boxShadow: 5,
            px: 6,
            py: 6
          }}
        >
          <CardContent sx={{ p: 0 }}>

            {loading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}

            {!loading && (
              <Box display="flex" flexDirection="column" gap={4}>

                {/* DADOS DO USUÁRIO */}
                <Typography
                  fontWeight={600}
                  variant="h6"
                  textAlign="center"
                >
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

                <Divider sx={{ my: 2 }} />

                {/* ADMIN */}
                {usuario.tipo === 1 && (
                  <Box display="flex" flexDirection="column" gap={4}>
                    <Typography
                      fontWeight={600}
                      variant="h6"
                      textAlign="center"
                    >
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
                  </Box>
                )}

                {/* PROFESSOR */}
                {usuario.tipo === 2 && (
                  <Box display="flex" flexDirection="column" gap={4}>
                    <Typography
                      fontWeight={600}
                      variant="h6"
                      textAlign="center"
                    >
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
                  </Box>
                )}

                {/* BOTÕES CENTRALIZADOS */}
                <Box
                  display="flex"
                  justifyContent="center"
                  gap={3}
                  mt={3}
                >
                  <Button
                    variant="outlined"
                    startIcon={<ArrowBackIcon />}
                    onClick={() => navigate("/admin/usuarios")}
                    sx={{
                      px: 5,
                      borderRadius: 3
                    }}
                  >
                    Voltar
                  </Button>

                  <Button
                    startIcon={<SaveIcon />}
                    onClick={handleSubmit}
                    sx={{
                      px: 5,
                      borderRadius: 3,
                      background: "linear-gradient(90deg, #1976d2, #26c6da)",
                      color: "#fff",
                      "&:hover": {
                        background: "linear-gradient(90deg, #1565c0, #00acc1)"
                      }
                    }}
                  >
                    Criar Usuário
                  </Button>
                </Box>

              </Box>
            )}

          </CardContent>
        </Card>

      </Box>

    </AppLayout>
  );
}