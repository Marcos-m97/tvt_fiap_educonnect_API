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
      <Box mb={4}>
        <Typography
          variant="h3"
          fontWeight={550}
          textAlign="center"
          gutterBottom
        >
          Criar Usuário
        </Typography>

        <Typography
          variant="body1"
          color="text.secondary"
          textAlign="center"
        >
          O usuário receberá um e-mail para redefinir a senha.
        </Typography>
      </Box>

      <Box maxWidth="900px" mx="auto">

        <Box mb={2}>
          <Button
            variant="outlined"
            startIcon={<ArrowBackIcon />}
            onClick={() => navigate("/admin/usuarios")}
          >
            Voltar
          </Button>
        </Box>

        <Card
          sx={{
            borderRadius: 3,
            boxShadow: 3
          }}
        >
          <CardContent>

            {loading && (
              <Box display="flex" justifyContent="center" py={3}>
                <CircularProgress />
              </Box>
            )}

            {!loading && (
              <Box display="flex" flexDirection="column" gap={3}>

                {/* DADOS USUARIO */}
                <Typography
                  fontWeight={600}
                  textAlign="center"
                  variant="h6"
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

                {/* ADMIN */}
                {usuario.tipo === 1 && (
                  <>
                    <Typography
                      fontWeight={600}
                      textAlign="center"
                      variant="h6"
                      mt={2}
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
                  </>
                )}

                {/* PROFESSOR */}
                {usuario.tipo === 2 && (
                  <>
                    <Typography
                      fontWeight={600}
                      textAlign="center"
                      variant="h6"
                      mt={2}
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

      </Box>

    </AppLayout>
  );
}
