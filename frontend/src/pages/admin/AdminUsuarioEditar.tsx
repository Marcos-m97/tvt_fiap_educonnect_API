import {
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  CircularProgress
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SaveIcon from "@mui/icons-material/Save";
import AppLayout from "../../components/layout/AppLayout";
import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../services/api";

export default function AdminUsuarioEditar() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [usuario, setUsuario] = useState<any>(null);
  const [perfil, setPerfil] = useState<any>(null);

  async function carregarDados() {
    try {
      setLoading(true);

      const response = await api.get(`/usuario/${id}`);
      const user = response.data;
      setUsuario(user);

      if (user.tipo === 1) {
        const adm = await api.get(`/admin/${user.id}`);
        setPerfil(adm.data);
      }

      if (user.tipo === 2) {
        const prof = await api.get(`/professor/${user.id}`);
        setPerfil(prof.data);
      }

      if (user.tipo === 3) {
        const aluno = await api.get(`/aluno/${user.id}`);
        setPerfil(aluno.data);
      }

    } catch (error) {
      console.error("Erro ao carregar dados:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarDados();
  }, [id]);

  async function handleSubmit() {
    try {
      setLoading(true);

      // Atualiza apenas nome (email não é editável)
      await api.put(`/usuario/${id}`, {
        nome: usuario.nome,
        email: usuario.email, // mantemos o mesmo email
        tipo: usuario.tipo
      });

      // Atualiza perfil
      if (usuario.tipo === 1) {
        await api.put(`/admin/${perfil.id}`, {
          usuarioId: usuario.id,
          departamento: perfil.departamento,
          cargo: perfil.cargo
        });
      }

      if (usuario.tipo === 2) {
        await api.put(`/professor/${perfil.id}`, {
          usuarioId: usuario.id,
          especialidade: perfil.especialidade,
          formacao: perfil.formacao,
          curriculoLattes: perfil.curriculoLattes
        });
      }

      if (usuario.tipo === 3) {
        await api.put(`/aluno/${perfil.id}`, {
          usuarioId: usuario.id,
          cpf: perfil.cpf,
          dataNascimento: perfil.dataNascimento,
          endereco: perfil.endereco
        });
      }

      navigate(`/admin/usuarios/${id}`);

    } catch (error) {
      console.error("Erro ao atualizar:", error);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AppLayout>

      {/* HEADER */}
      <Box position="relative" mb={4}>
        <Typography variant="h4" textAlign="center" fontWeight={600}>
          Editar Usuário
        </Typography>

        <Box position="absolute" right={0} top={0}>
          <Button
            variant="outlined"
            startIcon={<ArrowBackIcon />}
            onClick={() => navigate(-1)}
          >
            Voltar
          </Button>
        </Box>
      </Box>

      {loading && (
        <Box display="flex" justifyContent="center">
          <CircularProgress />
        </Box>
      )}

      {!loading && usuario && (
        <Card sx={{ borderRadius: 3, boxShadow: 3 }}>
          <CardContent>
            <Box display="flex" flexDirection="column" gap={3}>

              <Typography fontWeight={600}>
                Dados do Usuário
              </Typography>

              {/* Nome editável */}
              <TextField
                label="Nome"
                value={usuario.nome}
                onChange={(e) =>
                  setUsuario({ ...usuario, nome: e.target.value })
                }
                fullWidth
              />

              {/* Email bloqueado */}
              <TextField
                label="Email"
                value={usuario.email}
                disabled
                fullWidth
              />

              {perfil && (
                <>
                  <Typography fontWeight={600} mt={3}>
                    Dados do Perfil
                  </Typography>

                  {usuario.tipo === 1 && (
                    <>
                      <TextField
                        label="Departamento"
                        value={perfil.departamento || ""}
                        onChange={(e) =>
                          setPerfil({
                            ...perfil,
                            departamento: e.target.value
                          })
                        }
                        fullWidth
                      />

                      <TextField
                        label="Cargo"
                        value={perfil.cargo || ""}
                        onChange={(e) =>
                          setPerfil({
                            ...perfil,
                            cargo: e.target.value
                          })
                        }
                        fullWidth
                      />
                    </>
                  )}

                  {usuario.tipo === 2 && (
                    <>
                      <TextField
                        label="Especialidade"
                        value={perfil.especialidade || ""}
                        onChange={(e) =>
                          setPerfil({
                            ...perfil,
                            especialidade: e.target.value
                          })
                        }
                        fullWidth
                      />

                      <TextField
                        label="Formação"
                        value={perfil.formacao || ""}
                        onChange={(e) =>
                          setPerfil({
                            ...perfil,
                            formacao: e.target.value
                          })
                        }
                        fullWidth
                      />

                      <TextField
                        label="Currículo Lattes"
                        multiline
                        rows={3}
                        value={perfil.curriculoLattes || ""}
                        onChange={(e) =>
                          setPerfil({
                            ...perfil,
                            curriculoLattes: e.target.value
                          })
                        }
                        fullWidth
                      />
                    </>
                  )}

                  {usuario.tipo === 3 && (
                    <>
                      <TextField
                        label="CPF"
                        value={perfil.cpf || ""}
                        onChange={(e) =>
                          setPerfil({
                            ...perfil,
                            cpf: e.target.value
                          })
                        }
                        fullWidth
                      />

                      <TextField
                        label="Data de Nascimento"
                        type="date"
                        value={perfil.dataNascimento?.split("T")[0] || ""}
                        onChange={(e) =>
                          setPerfil({
                            ...perfil,
                            dataNascimento: e.target.value
                          })
                        }
                        fullWidth
                        InputLabelProps={{ shrink: true }}
                      />

                      <TextField
                        label="Endereço"
                        value={perfil.endereco || ""}
                        onChange={(e) =>
                          setPerfil({
                            ...perfil,
                            endereco: e.target.value
                          })
                        }
                        fullWidth
                      />
                    </>
                  )}
                </>
              )}

              <Box display="flex" justifyContent="flex-end">
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={handleSubmit}
                >
                  Salvar Alterações
                </Button>
              </Box>

            </Box>
          </CardContent>
        </Card>
      )}

    </AppLayout>
  );
}
